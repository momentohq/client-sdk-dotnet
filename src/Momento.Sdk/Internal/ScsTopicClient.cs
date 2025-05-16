#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Protos.CacheClient.Pubsub;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal.ExtensionMethods;
using Momento.Sdk.Responses;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Momento.Sdk.Internal;

public class ScsTopicClientBase : IDisposable
{
    // TODO: make number of channels configurable using WithMaxSubscriptions config option
    private const int DEFAULT_STREAM_CHANNELS = 4;
    private const int DEFAULT_UNARY_CHANNELS = 4;
    private readonly List<TopicGrpcManager> unaryGrpcManagers;
    private readonly List<TopicGrpcManager> streamGrpcManagers;
    private int unaryClientIndex = 0;
    private int streamClientIndex = 0;

    protected readonly TimeSpan topicClientOperationTimeout = TimeSpan.FromSeconds(60);
    private readonly ILogger _logger;
    private bool hasSentOnetimeHeaders = false;
    protected readonly CacheExceptionMapper _exceptionMapper;

    public ScsTopicClientBase(ITopicConfiguration config, ICredentialProvider authProvider)
    {
        this._logger = config.LoggerFactory.CreateLogger<ScsTopicClient>();
        this._exceptionMapper = new CacheExceptionMapper(config.LoggerFactory);
        this.topicClientOperationTimeout = config.TransportStrategy.GrpcConfig.Deadline;

        unaryGrpcManagers = new List<TopicGrpcManager>();
        for (var i = 0; i < DEFAULT_UNARY_CHANNELS; i++)
        {
            unaryGrpcManagers.Add(new TopicGrpcManager(config, authProvider));
        }

        streamGrpcManagers = new List<TopicGrpcManager>();
        for (var i = 0; i < DEFAULT_STREAM_CHANNELS; i++)
        {
            streamGrpcManagers.Add(new TopicGrpcManager(config, authProvider));
        }
    }

    private Metadata MetadataWithCache(string cacheName)
    {
        if (this.hasSentOnetimeHeaders)
        {
            return new Metadata() { { "cache", cacheName } };
        }
        this.hasSentOnetimeHeaders = true;
        string sdkVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        string runtimeVer = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
        return new Metadata() { { "cache", cacheName }, { "agent", $"dotnet:topic:{sdkVersion}" }, { "runtime-v]ersion", runtimeVer } };
    }

    public TopicGrpcManager GetNextUnaryClient()
    {
        var index = Interlocked.Increment(ref unaryClientIndex);
        return this.unaryGrpcManagers[index % this.unaryGrpcManagers.Count];
    }

    public TopicGrpcManager GetNextStreamClient()
    {
        // TODO: active subscriptions bookkeeping
        var index = Interlocked.Increment(ref streamClientIndex);
        return this.streamGrpcManagers[index % this.streamGrpcManagers.Count];
    }

    public void Dispose()
    {
        for (var i = 0; i < DEFAULT_UNARY_CHANNELS; i++)
        {
            this.unaryGrpcManagers[i].Dispose();
        }
        for (var i = 0; i < DEFAULT_STREAM_CHANNELS; i++)
        {
            this.streamGrpcManagers[i].Dispose();
        }
        this.unaryGrpcManagers.Clear();
        this.streamGrpcManagers.Clear();
        this.unaryClientIndex = 0;
        this.streamClientIndex = 0;
    }
}

internal sealed class ScsTopicClient : ScsTopicClientBase
{
    private readonly ILogger _logger;

    public ScsTopicClient(ITopicConfiguration config, ICredentialProvider authProvider)
        : base(config, authProvider)
    {
        this._logger = config.LoggerFactory.CreateLogger<ScsTopicClient>();
    }

    public async Task<TopicPublishResponse> Publish(string cacheName, string topicName, byte[] value)
    {
        var topicValue = new _TopicValue
        {
            Binary = value.ToByteString()
        };
        return await SendPublish(cacheName, topicName, topicValue);
    }

    public async Task<TopicPublishResponse> Publish(string cacheName, string topicName, string value)
    {
        var topicValue = new _TopicValue
        {
            Text = value
        };
        return await SendPublish(cacheName, topicName, topicValue);
    }

    public async Task<TopicSubscribeResponse> Subscribe(string cacheName, string topicName,
        ulong? resumeAtTopicSequenceNumber = null, ulong? resumeAtTopicSequencePage = null)
    {
        return await SendSubscribe(cacheName, topicName, resumeAtTopicSequenceNumber, resumeAtTopicSequencePage);
    }

    private const string RequestTypeTopicPublish = "TOPIC_PUBLISH";
    private const string RequestTypeTopicSubscribe = "TOPIC_SUBSCRIBE";

    private async Task<TopicPublishResponse> SendPublish(string cacheName, string topicName, _TopicValue value)
    {
        var request = new _PublishRequest
        {
            CacheName = cacheName,
            Topic = topicName,
            Value = value
        };

        try
        {
            _logger.LogTraceExecutingTopicRequest(RequestTypeTopicPublish, cacheName, topicName);
            await this.GetNextUnaryClient().Client.publish(request, new CallOptions(deadline: Utils.CalculateDeadline(topicClientOperationTimeout)));
        }
        catch (Exception e)
        {
            return _logger.LogTraceTopicRequestError(RequestTypeTopicPublish, cacheName, topicName,
                new TopicPublishResponse.Error(_exceptionMapper.Convert(e)));
        }

        return _logger.LogTraceTopicRequestSuccess(RequestTypeTopicPublish, cacheName, topicName,
            new TopicPublishResponse.Success());
    }

    private async Task<TopicSubscribeResponse> SendSubscribe(string cacheName, string topicName,
        ulong? resumeAtTopicSequenceNumber, ulong? resumeAtTopicSequencePage)
    {
        SubscriptionWrapper subscriptionWrapper;
        try
        {
            var grpcManager = GetNextStreamClient();
            _logger.LogTraceExecutingTopicRequest(RequestTypeTopicSubscribe, cacheName, topicName);
            subscriptionWrapper = new SubscriptionWrapper(grpcManager, cacheName, topicName,
                resumeAtTopicSequenceNumber, resumeAtTopicSequencePage, _exceptionMapper, _logger);
            await subscriptionWrapper.Subscribe();
        }
        catch (Exception e)
        {
            return _logger.LogTraceTopicRequestError(RequestTypeTopicSubscribe, cacheName, topicName,
                new TopicSubscribeResponse.Error(_exceptionMapper.Convert(e)));
        }

        var response = new TopicSubscribeResponse.Subscription(
            cancellationToken => subscriptionWrapper.GetNextEventFromGrpcStreamAsync(cancellationToken),
            subscriptionWrapper.Dispose);
        return _logger.LogTraceTopicRequestSuccess(RequestTypeTopicSubscribe, cacheName, topicName,
            response);
    }

    private class SubscriptionWrapper : IDisposable
    {
        private readonly TopicGrpcManager _grpcManager;
        private readonly string _cacheName;
        private readonly string _topicName;
        private readonly CacheExceptionMapper _exceptionMapper;
        private readonly ILogger _logger;

        private AsyncServerStreamingCall<_SubscriptionItem>? _subscription;
        private ulong _lastSequenceNumber;
        private ulong _lastSequencePage;
        private bool _subscribed;

        public SubscriptionWrapper(TopicGrpcManager grpcManager, string cacheName,
            string topicName, ulong? resumeAtTopicSequenceNumber, ulong? resumeAtTopicSequencePage,
            CacheExceptionMapper exceptionMapper, ILogger logger)
        {
            _grpcManager = grpcManager;
            _cacheName = cacheName;
            _topicName = topicName;
            _lastSequenceNumber = resumeAtTopicSequenceNumber ?? 0;
            _lastSequencePage = resumeAtTopicSequencePage ?? 0;
            _exceptionMapper = exceptionMapper;
            _logger = logger;
        }

        public async Task Subscribe()
        {
            var request = new _SubscriptionRequest
            {
                CacheName = _cacheName,
                Topic = _topicName
            };

            request.ResumeAtTopicSequenceNumber = _lastSequenceNumber;
            request.SequencePage = _lastSequencePage;

            _logger.LogTraceExecutingTopicRequest(RequestTypeTopicSubscribe, _cacheName, _topicName);
            var subscription = _grpcManager.Client.subscribe(request, new CallOptions());

            await subscription.ResponseStream.MoveNext();
            var firstMessage = subscription.ResponseStream.Current;
            // The first message to a new subscription will always be a heartbeat.
            if (firstMessage.KindCase is not _SubscriptionItem.KindOneofCase.Heartbeat)
            {
                throw new InternalServerException(
                    $"Expected heartbeat message for topic {_topicName} on cache {_cacheName}. Got: {firstMessage.KindCase}");
            }

            _subscription = subscription;
            _subscribed = true;
        }

        public async ValueTask<ITopicEvent?> GetNextEventFromGrpcStreamAsync(
            CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (!_subscribed)
                    {
                        await Subscribe();
                        _subscribed = true;
                    }

                    await _subscription!.ResponseStream.MoveNext(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    var sdkException = _exceptionMapper.Convert(e);
                    if (sdkException.ErrorCode is MomentoErrorCode.CANCELLED_ERROR)
                    {
                        break;
                    }

                    _logger.LogTraceTopicSubscriptionError(_cacheName, _topicName, sdkException);

                    // Certain errors can never be recovered
                    if (!IsErrorRecoverable(sdkException))
                    {
                        return new TopicMessage.Error(sdkException);
                    }

                    // If the error is recoverable, wait and attempt to resubscribe
                    Dispose();
                    await Task.Delay(5_000, cancellationToken);

                    _subscribed = false;
                    continue;
                }

                var message = _subscription.ResponseStream.Current;

                switch (message.KindCase)
                {
                    case _SubscriptionItem.KindOneofCase.Item:
                        _lastSequenceNumber = message.Item.TopicSequenceNumber;
                        _lastSequencePage = message.Item.SequencePage;

                        switch (message.Item.Value.KindCase)
                        {
                            case _TopicValue.KindOneofCase.Text:
                                _logger.LogTraceTopicMessageReceived("text", _cacheName, _topicName);
                                return new TopicMessage.Text(message.Item.Value, _lastSequenceNumber, _lastSequencePage, message.Item.PublisherId == "" ? null : message.Item.PublisherId);
                            case _TopicValue.KindOneofCase.Binary:
                                _logger.LogTraceTopicMessageReceived("binary", _cacheName, _topicName);
                                return new TopicMessage.Binary(message.Item.Value, _lastSequenceNumber, _lastSequencePage, message.Item.PublisherId == "" ? null : message.Item.PublisherId);
                            case _TopicValue.KindOneofCase.None:
                            default:
                                _logger.LogTraceTopicMessageReceived("unknown", _cacheName, _topicName);
                                break;
                        }

                        break;
                    case _SubscriptionItem.KindOneofCase.Discontinuity:
                        _logger.LogTraceTopicDiscontinuityReceived(_cacheName, _topicName,
                            message.Discontinuity.LastTopicSequence, message.Discontinuity.NewTopicSequence, message.Discontinuity.NewSequencePage);
                        _lastSequenceNumber = message.Discontinuity.NewTopicSequence;
                        _lastSequencePage = message.Discontinuity.NewSequencePage;
                        return new TopicSystemEvent.Discontinuity(message.Discontinuity.LastTopicSequence,
                            message.Discontinuity.NewTopicSequence,
                            message.Discontinuity.NewSequencePage);
                    case _SubscriptionItem.KindOneofCase.Heartbeat:
                        _logger.LogTraceTopicMessageReceived("heartbeat", _cacheName, _topicName);
                        return new TopicSystemEvent.Heartbeat();
                    case _SubscriptionItem.KindOneofCase.None:
                        _logger.LogTraceTopicMessageReceived("none", _cacheName, _topicName);
                        break;
                    default:
                        _logger.LogTraceTopicMessageReceived("unknown", _cacheName, _topicName);
                        break;
                }
            }

            return null;
        }

        private static bool IsErrorRecoverable(SdkException exception)
        {
            return exception.ErrorCode is not (MomentoErrorCode.PERMISSION_ERROR
                or MomentoErrorCode.AUTHENTICATION_ERROR);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
