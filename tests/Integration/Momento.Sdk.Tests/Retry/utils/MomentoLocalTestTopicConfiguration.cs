using Microsoft.Extensions.Logging;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Retry;
using Momento.Sdk.Config.Transport;
using Momento.Sdk.Internal;
using System.Collections.Generic;

namespace Momento.Sdk.Tests.Integration.Retry;

public class MomentoLocalTopicConfiguration : ITopicConfiguration, ITopicSubscriptionConnectionStateCallbacks, ITopicConfigWithHeaders
{
    public string RequestId { get; set; } = Utils.NewGuidString();
    private readonly MomentoLocalMiddlewareArgs? _args;
    public IList<KeyValuePair<string, string>> Headers { get; }
    public int StreamDisconnectedCounter = 0;
    public int StreamEstablishedCounter = 0;
    public ILoggerFactory LoggerFactory { get; }
    public ITopicTransportStrategy TransportStrategy { get; }

    public ISubscriptionRetryStrategy SubscriptionRetryStrategy { get; }

    public MomentoLocalTopicConfiguration(ILoggerFactory loggerFactory, ITopicTransportStrategy transportStrategy, ISubscriptionRetryStrategy subscriptionRetryStrategy, MomentoLocalMiddlewareArgs? args)
    {
        _args = args;
        Headers = this.CreateHeaders();
        LoggerFactory = loggerFactory;
        TransportStrategy = transportStrategy;
        SubscriptionRetryStrategy = subscriptionRetryStrategy;
    }

    private IList<KeyValuePair<string, string>> CreateHeaders()
    {
        var headerMappings = new Dictionary<string, string?>
        {
            { "request-id", RequestId },
            { "return-error", _args?.ReturnError },
            { "error-rpcs", ConvertToMetadataList(_args?.ErrorRpcList) },
            { "error-count", _args?.ErrorCount?.ToString() },
            { "delay-rpcs", ConvertToMetadataList(_args?.DelayRpcList) },
            { "delay-ms", _args?.DelayMillis?.ToString() },
            { "delay-count", _args?.DelayCount?.ToString() },
            { "stream-error-rpcs", ConvertToMetadataList(_args?.StreamErrorRpcList) },
            { "stream-error", _args?.StreamError },
            { "stream-error-message-limit", _args?.StreamErrorMessageLimit?.ToString() }
        };

        // Instantiate the KeyValuePair list item only if the value is not null.
        var headers = new List<KeyValuePair<string, string>>();
        foreach (var pair in headerMappings)
        {
            string key = pair.Key;
            string? value = pair.Value;
            {
                if (value != null && value.Length > 0)
                {
                    headers.Add(new KeyValuePair<string, string>(key, value.ToString()));
                }
            }
        }
        return headers;
    }

    private string? ConvertToMetadataList(IList<string>? values)
    {
        if (values == null || values.Count == 0)
        {
            return null;
        }
        return string.Join(" ", values);
    }

    public void OnStreamDisconnected()
    {
        StreamDisconnectedCounter++;
    }
    public void onStreamEstablished()
    {
        StreamEstablishedCounter++;
    }

    public ITopicConfiguration WithTransportStrategy(ITopicTransportStrategy transportStrategy)
    {
        return new MomentoLocalTopicConfiguration(LoggerFactory, transportStrategy, SubscriptionRetryStrategy, _args);
    }

    public ITopicConfiguration WithClientTimeout(TimeSpan clientTimeout)
    {
        return new MomentoLocalTopicConfiguration(
            loggerFactory: LoggerFactory,
            transportStrategy: TransportStrategy.WithClientTimeout(clientTimeout),
            args: _args,
            subscriptionRetryStrategy: SubscriptionRetryStrategy
        );
    }

    public ITopicConfiguration WithSubscriptionRetryStrategy(ISubscriptionRetryStrategy subscriptionRetryStrategy)
    {
        return new MomentoLocalTopicConfiguration(
            loggerFactory: LoggerFactory,
            transportStrategy: TransportStrategy,
            args: _args,
            subscriptionRetryStrategy: subscriptionRetryStrategy
        );
    }
}


