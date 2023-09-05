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

        AsyncServerStreamingCall<_SubscriptionItem> subscription;
        try
        {
            _logger.LogTraceExecutingTopicRequest(RequestTypeTopicPublish, cacheName, topicName);
            subscription = grpcManager.Client.subscribe(request, new CallOptions());
        }
        catch (Exception e)
        {
            return _logger.LogTraceTopicRequestError(RequestTypeTopicPublish, cacheName, topicName,
                new TopicSubscribeResponse.Error(_exceptionMapper.Convert(e)));
        }

        var response = new TopicSubscribeResponse.Subscription(
            token => MoveNextAsync(subscription, token, cacheName, topicName),
            subscription.Dispose);
        return _logger.LogTraceTopicRequestSuccess(RequestTypeTopicPublish, cacheName, topicName,
            response);
    }

    private async ValueTask<TopicMessage?> MoveNextAsync(AsyncServerStreamingCall<_SubscriptionItem> subscription,
        CancellationToken cancellationToken, string cacheName, string topicName)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return null;
        }

        try
        {
            while (await subscription.ResponseStream.MoveNext(cancellationToken).ConfigureAwait(false))
            {
                var message = subscription.ResponseStream.Current;

                switch (message.KindCase)
                {
                    case _SubscriptionItem.KindOneofCase.Item:
                        switch (message.Item.Value.KindCase)
                        {
                            case _TopicValue.KindOneofCase.Text:
                                _logger.LogTraceTopicMessageReceived("text", cacheName, topicName);
                                return new TopicMessage.Text(message.Item);
                            case _TopicValue.KindOneofCase.Binary:
                                _logger.LogTraceTopicMessageReceived("binary", cacheName, topicName);
                                return new TopicMessage.Binary(message.Item);
                            case _TopicValue.KindOneofCase.None:
                            default:
                                _logger.LogTraceTopicMessageReceived("unknown", cacheName, topicName);
                                break;
                        }

                        break;
                    case _SubscriptionItem.KindOneofCase.Discontinuity:
                        _logger.LogTraceTopicMessageReceived("discontinuity", cacheName, topicName);
                        break;
                    case _SubscriptionItem.KindOneofCase.Heartbeat:
                        _logger.LogTraceTopicMessageReceived("heartbeat", cacheName, topicName);
                        break;
                    case _SubscriptionItem.KindOneofCase.None:
                        _logger.LogTraceTopicMessageReceived("none", cacheName, topicName);
                        break;
                    default:
                        _logger.LogTraceTopicMessageReceived("unknown", cacheName, topicName);
                        break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch (Exception e)
        {
            var sdkException = _exceptionMapper.Convert(e);
            return sdkException.ErrorCode == MomentoErrorCode.CANCELLED_ERROR
                ? null
                : new TopicMessage.Error(sdkException);
        }

        return null;
    }
}
#endif