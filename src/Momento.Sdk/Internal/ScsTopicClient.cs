#if NETSTANDARD2_0_OR_GREATER

using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Protos.CacheClient.Pubsub;
using Momento.Sdk.Config;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal.ExtensionMethods;
using Momento.Sdk.Responses;

namespace Momento.Sdk.Internal;

public class ScsTopicClientBase : IDisposable
{
    protected readonly TopicGrpcManager grpcManager;
    private readonly TimeSpan dataClientOperationTimeout;
    private readonly ILogger _logger;

    protected readonly CacheExceptionMapper _exceptionMapper;

    public ScsTopicClientBase(ITopicConfiguration config, string authToken, string endpoint)
    {
        this.grpcManager = new TopicGrpcManager(config, authToken, endpoint);
        this.dataClientOperationTimeout = config.TransportStrategy.GrpcConfig.Deadline;
        this._logger = config.LoggerFactory.CreateLogger<ScsDataClient>();
        this._exceptionMapper = new CacheExceptionMapper(config.LoggerFactory);
    }

    protected Metadata MetadataWithCache(string cacheName)
    {
        return new Metadata() { { "cache", cacheName } };
    }

    protected DateTime CalculateDeadline()
    {
        return DateTime.UtcNow.Add(dataClientOperationTimeout);
    }

    public void Dispose()
    {
        this.grpcManager.Dispose();
    }
}

internal sealed class ScsTopicClient : ScsTopicClientBase
{
    private readonly ILogger _logger;

    public ScsTopicClient(ITopicConfiguration config, string authToken, string endpoint)
        : base(config, authToken, endpoint)
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
        ulong? resumeAtTopicSequenceNumber = null)
    {
        return await SendSubscribe(cacheName, topicName, resumeAtTopicSequenceNumber);
    }

    private const string RequestTypeTopicPublish = "TOPIC_PUBLISH";
    private const string RequestTypeTopicSubscribe = "TOPIC_SUBSCRIBE";

    private async Task<TopicPublishResponse> SendPublish(string cacheName, string topicName, _TopicValue value)
    {
        _PublishRequest request = new _PublishRequest
        {
            CacheName = cacheName,
            Topic = topicName,
            Value = value
        };

        try
        {
            _logger.LogTraceExecutingTopicRequest(RequestTypeTopicPublish, cacheName, topicName);
            await grpcManager.Client.publish(request, new CallOptions(deadline: CalculateDeadline()));
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
        ulong? resumeAtTopicSequenceNumber)
    {
        var request = new _SubscriptionRequest
        {
            CacheName = cacheName,
            Topic = topicName
        };
        if (resumeAtTopicSequenceNumber != null)
        {
            request.ResumeAtTopicSequenceNumber = resumeAtTopicSequenceNumber.Value;
        }

        SubscriptionWrapper subscriptionWrapper;
        try
        {
            _logger.LogTraceExecutingTopicRequest(RequestTypeTopicSubscribe, cacheName, topicName);
            subscriptionWrapper = new SubscriptionWrapper(grpcManager, cacheName, topicName, _exceptionMapper, _logger);
        }
        catch (Exception e)
        {
            return _logger.LogTraceTopicRequestError(RequestTypeTopicSubscribe, cacheName, topicName,
                new TopicSubscribeResponse.Error(_exceptionMapper.Convert(e)));
        }

        var response = new TopicSubscribeResponse.Subscription(
            cancellationToken => subscriptionWrapper.GetNextRelevantMessageFromGrpcStreamAsync(cancellationToken),
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

        private AsyncServerStreamingCall<_SubscriptionItem> _subscription;
        private ulong? _lastSequenceNumber;

        public SubscriptionWrapper(TopicGrpcManager grpcManager, string cacheName,
            string topicName, CacheExceptionMapper exceptionMapper, ILogger logger)
        {
            _grpcManager = grpcManager;
            _cacheName = cacheName;
            _topicName = topicName;
            _exceptionMapper = exceptionMapper;
            _logger = logger;

            _subscription = Subscribe();
        }

        private AsyncServerStreamingCall<_SubscriptionItem> Subscribe()
        {
            var request = new _SubscriptionRequest
            {
                CacheName = _cacheName,
                Topic = _topicName
            };
            if (_lastSequenceNumber != null)
            {
                request.ResumeAtTopicSequenceNumber = _lastSequenceNumber.Value;
            }

            _logger.LogTraceExecutingTopicRequest(RequestTypeTopicSubscribe, _cacheName, _topicName);
            return _grpcManager.Client.subscribe(request, new CallOptions());
        }

        public async ValueTask<TopicMessage?> GetNextRelevantMessageFromGrpcStreamAsync(
            CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await _subscription.ResponseStream.MoveNext(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    var sdkException = _exceptionMapper.Convert(e);
                    if (sdkException.ErrorCode == MomentoErrorCode.CANCELLED_ERROR)
                    {
                        break;
                    }
                    //TODO: Are there other errors that we should break for?

                    _logger.LogTraceTopicSubscriptionError(_cacheName, _topicName, sdkException);

                    await Task.Delay(5_000, cancellationToken);
                    Dispose();
                    _subscription = Subscribe();
                    continue;
                }

                var message = _subscription.ResponseStream.Current;

                switch (message.KindCase)
                {
                    case _SubscriptionItem.KindOneofCase.Item:
                        _lastSequenceNumber = message.Item.TopicSequenceNumber;
                        switch (message.Item.Value.KindCase)
                        {
                            case _TopicValue.KindOneofCase.Text:
                                _logger.LogTraceTopicMessageReceived("text", _cacheName, _topicName);
                                return new TopicMessage.Text(message.Item.Value);
                            case _TopicValue.KindOneofCase.Binary:
                                _logger.LogTraceTopicMessageReceived("binary", _cacheName, _topicName);
                                return new TopicMessage.Binary(message.Item.Value);
                            case _TopicValue.KindOneofCase.None:
                            default:
                                _logger.LogTraceTopicMessageReceived("unknown", _cacheName, _topicName);
                                break;
                        }

                        break;
                    case _SubscriptionItem.KindOneofCase.Discontinuity:
                        _logger.LogTraceTopicDiscontinuityReceived(_cacheName, _topicName,
                            message.Discontinuity.LastTopicSequence, message.Discontinuity.NewTopicSequence);
                        break;
                    case _SubscriptionItem.KindOneofCase.Heartbeat:
                        _logger.LogTraceTopicMessageReceived("heartbeat", _cacheName, _topicName);
                        break;
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

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}
#endif