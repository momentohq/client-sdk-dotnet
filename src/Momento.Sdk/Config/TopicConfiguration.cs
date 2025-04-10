using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Transport;
using System;
using System.Collections.Generic;

namespace Momento.Sdk.Config;

/// <inheritdoc cref="Momento.Sdk.Config.ITopicConfiguration" />
public class TopicConfiguration : ITopicConfiguration
{
    /// <inheritdoc />
    public ILoggerFactory LoggerFactory { get; }

    /// <inheritdoc />
    public ITopicTransportStrategy TransportStrategy { get; }

    /// <inheritdoc />
    public IList<ITopicMiddleware> Middlewares { get; }

    /// <summary>
    /// Create a new instance of a Topic Configuration object with provided arguments: <see cref="Momento.Sdk.Config.ITopicConfiguration.TransportStrategy"/>, and <see cref="Momento.Sdk.Config.ITopicConfiguration.LoggerFactory"/>
    /// </summary>
    /// <param name="transportStrategy">This is responsible for configuring network tunables.</param>
    /// <param name="loggerFactory">This is responsible for configuring logging.</param>
    /// <param name="middlewares">This is responsible for configuring middleware for the topic client.</param>
    public TopicConfiguration(ILoggerFactory loggerFactory, ITopicTransportStrategy transportStrategy, IList<ITopicMiddleware>? middlewares = null)
    {
        LoggerFactory = loggerFactory;
        TransportStrategy = transportStrategy;
        Middlewares = middlewares ?? new List<ITopicMiddleware>();
    }

    /// <inheritdoc />
    public ITopicConfiguration WithTransportStrategy(ITopicTransportStrategy transportStrategy)
    {
        return new TopicConfiguration(LoggerFactory, transportStrategy, Middlewares);
    }

    /// <inheritdoc/>
    public ITopicConfiguration WithClientTimeout(TimeSpan clientTimeout)
    {
        return new TopicConfiguration(
            loggerFactory: LoggerFactory,
            transportStrategy: TransportStrategy.WithClientTimeout(clientTimeout),
            middlewares: Middlewares
        );
    }

    /// <summary>
    /// Replace the middlewares on an existing instance of TopicConfiguration object with
    /// the provided middlewares.
    /// </summary>
    /// <param name="middlewares"></param>
    /// <returns></returns>
    public ITopicConfiguration WithMiddlewares(IList<ITopicMiddleware> middlewares)
    {
        return new TopicConfiguration(
            loggerFactory: LoggerFactory,
            transportStrategy: TransportStrategy,
            middlewares: middlewares
        );
    }

    /// <summary>
    /// Add the specified middleware to an existing instance of TopicConfiguration object.
    /// </summary>
    /// <param name="middleware"></param>
    /// <returns></returns>
    public ITopicConfiguration AddMiddleware(ITopicMiddleware middleware)
    {
        Middlewares.Add(middleware);
        return new TopicConfiguration(
            loggerFactory: LoggerFactory,
            transportStrategy: TransportStrategy,
            middlewares: Middlewares
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
