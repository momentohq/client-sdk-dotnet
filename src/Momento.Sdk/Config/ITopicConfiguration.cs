using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Retry;
using Momento.Sdk.Config.Transport;
using System;

namespace Momento.Sdk.Config;


/// <summary>
/// Contract for Topic SDK configurables.
/// </summary>
public interface ITopicConfiguration
{
    /// <inheritdoc cref="Microsoft.Extensions.Logging.ILoggerFactory" />
    public ILoggerFactory LoggerFactory { get; }
    /// <inheritdoc cref="Momento.Sdk.Config.Transport.ITransportStrategy" />
    public ITopicTransportStrategy TransportStrategy { get; }
    /// <inheritdoc cref="Momento.Sdk.Config.Retry.ISubscriptionRetryStrategy" />
    public ISubscriptionRetryStrategy SubscriptionRetryStrategy { get; }

    /// <summary>
    /// Creates a new instance of the Configuration object, updated to use the specified transport strategy.
    /// </summary>
    /// <param name="transportStrategy">This is responsible for configuring network tunables.</param>
    /// <returns>Configuration object with custom transport strategy provided</returns>
    public ITopicConfiguration WithTransportStrategy(ITopicTransportStrategy transportStrategy);

    /// <summary>
    /// Add the specified client timeout to an existing instance of TopicConfiguration object as an addiion to
    /// the existing transport strategy.
    /// </summary>
    /// <param name="clientTimeout">The amount of time to wait before cancelling the request.</param>
    /// <returns>TopicConfiguration object with client timeout provided</returns>
    public ITopicConfiguration WithClientTimeout(TimeSpan clientTimeout);

    /// <summary>
    /// Creates a new instance of the Configuration object, updated to use the specified subscription retry strategy.
    /// </summary>
    /// <param name="subscriptionRetryStrategy">Configures whether and when to resubscribe to a topic after an error occurs.</param>
    public ITopicConfiguration WithSubscriptionRetryStrategy(ISubscriptionRetryStrategy subscriptionRetryStrategy);
}
