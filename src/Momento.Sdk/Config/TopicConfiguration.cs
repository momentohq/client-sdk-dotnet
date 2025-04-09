using Microsoft.Extensions.Logging;
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

    /// <summary>
    /// Create a new instance of a Topic Configuration object with provided arguments: <see cref="Momento.Sdk.Config.ITopicConfiguration.TransportStrategy"/>, and <see cref="Momento.Sdk.Config.ITopicConfiguration.LoggerFactory"/>
    /// </summary>
    /// <param name="transportStrategy">This is responsible for configuring network tunables.</param>
    /// <param name="loggerFactory">This is responsible for configuring logging.</param>
    public TopicConfiguration(ILoggerFactory loggerFactory, ITopicTransportStrategy transportStrategy)
    {
        LoggerFactory = loggerFactory;
        TransportStrategy = transportStrategy;
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
