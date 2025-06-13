using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Retry;
using Momento.Sdk.Config.Transport;
using System;

namespace Momento.Sdk.Config;

/// <inheritdoc cref="Momento.Sdk.Config.ITopicConfiguration" />
public class TopicConfiguration : ITopicConfiguration
{
    /// <inheritdoc />
    public ILoggerFactory LoggerFactory { get; }

    /// <inheritdoc />
    public ITopicTransportStrategy TransportStrategy { get; }

    /// <inheritdoc />
    public ISubscriptionRetryStrategy SubscriptionRetryStrategy { get; }

    /// <summary>
    /// Create a new instance of a Topic Configuration object with provided arguments: <see cref="Momento.Sdk.Config.ITopicConfiguration.TransportStrategy"/>, and <see cref="Momento.Sdk.Config.ITopicConfiguration.LoggerFactory"/>
    /// </summary>
    /// <param name="transportStrategy">This is responsible for configuring network tunables.</param>
    /// <param name="loggerFactory">This is responsible for configuring logging.</param>
    /// <param name="subscriptionRetryStrategy">Configures whether and when to resubscribe to a topic after an error occurs.</param>
    public TopicConfiguration(ILoggerFactory loggerFactory, ITopicTransportStrategy transportStrategy, ISubscriptionRetryStrategy? subscriptionRetryStrategy = null)
    {
        LoggerFactory = loggerFactory;
        TransportStrategy = transportStrategy;
        SubscriptionRetryStrategy = subscriptionRetryStrategy ?? new DefaultSubscriptionRetryStrategy(loggerFactory.CreateLogger<DefaultSubscriptionRetryStrategy>());
    }

    /// <inheritdoc />
    public ITopicConfiguration WithTransportStrategy(ITopicTransportStrategy transportStrategy)
    {
        return new TopicConfiguration(LoggerFactory, transportStrategy);
    }

    /// <inheritdoc/>
    public ITopicConfiguration WithClientTimeout(TimeSpan clientTimeout)
    {
        return new TopicConfiguration(
            loggerFactory: LoggerFactory,
            transportStrategy: TransportStrategy.WithClientTimeout(clientTimeout)
        );
    }

    /// <inheritdoc />
    public ITopicConfiguration WithSubscriptionRetryStrategy(ISubscriptionRetryStrategy subscriptionRetryStrategy)
    {
        return new TopicConfiguration(
            loggerFactory: LoggerFactory,
            transportStrategy: TransportStrategy,
            subscriptionRetryStrategy: subscriptionRetryStrategy
        );
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }

        var other = (TopicConfiguration)obj;
        return TransportStrategy.Equals(other.TransportStrategy) &&
               LoggerFactory.Equals(other.LoggerFactory);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
