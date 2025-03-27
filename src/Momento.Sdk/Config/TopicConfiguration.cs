using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Config;

/// <inheritdoc cref="Momento.Sdk.Config.ITopicConfiguration" />
public class TopicConfiguration : ITopicConfiguration
{
    /// <inheritdoc />
    public ILoggerFactory LoggerFactory { get; }

    /// <inheritdoc />
    public ITopicTransportStrategy TransportStrategy { get; }
    /// <inheritdoc />
    public IList<IMiddleware> Middlewares { get; }
    /// <inheritdoc />
    public IList<Tuple<string, string>> SubscribeHeaders { get; }

    /// <summary>
    /// Create a new instance of a Topic Configuration object with provided arguments: <see cref="Momento.Sdk.Config.ITopicConfiguration.TransportStrategy"/>, <see cref="Momento.Sdk.Config.ITopicConfiguration.LoggerFactory"/>, and <see cref="Momento.Sdk.Config.ITopicConfiguration.Middlewares"/>
    /// </summary>
    /// <param name="transportStrategy">This is responsible for configuring network tunables.</param>
    /// <param name="loggerFactory">This is responsible for configuring logging.</param>
    /// <param name="middlewares">The Middleware interface allows the TopicConfiguration to provide a higher-order function that wraps all requests.</param>
    /// <param name="headers">The key-value pairs to add to the request headers.</param>
    /// <returns>TopicConfiguration object with custom transport strategy, logger factory, and middlewares provided</returns>
    public TopicConfiguration(ILoggerFactory loggerFactory, ITopicTransportStrategy transportStrategy, IList<IMiddleware> middlewares, IList<Tuple<string, string>> headers)
    {
        LoggerFactory = loggerFactory;
        TransportStrategy = transportStrategy;
        Middlewares = middlewares;
        SubscribeHeaders = headers;
    }

    /// <inheritdoc cref="Momento.Sdk.Config.IConfiguration" />
    public TopicConfiguration(ILoggerFactory loggerFactory, ITopicTransportStrategy transportStrategy): this(loggerFactory, transportStrategy, new List<IMiddleware>(), new List<Tuple<string, string>>())
    {
    }

    /// <inheritdoc />
    public ITopicConfiguration WithTransportStrategy(ITopicTransportStrategy transportStrategy)
    {
        return new TopicConfiguration(LoggerFactory, transportStrategy, Middlewares, SubscribeHeaders);
    }

    /// <inheritdoc/>
    public ITopicConfiguration WithClientTimeout(TimeSpan clientTimeout)
    {
        return new TopicConfiguration(
            loggerFactory: LoggerFactory,
            transportStrategy: TransportStrategy.WithClientTimeout(clientTimeout),
            middlewares: Middlewares,
            headers: SubscribeHeaders
        );
    }

    /// <inheritdoc />
    public ITopicConfiguration WithMiddlewares(IList<IMiddleware> middlewares)
    {
        return new TopicConfiguration(LoggerFactory, TransportStrategy, middlewares, SubscribeHeaders);
    }

    /// <inheritdoc />
    public TopicConfiguration WithAdditionalMiddlewares(IList<IMiddleware> additionalMiddlewares)
    {
        return new TopicConfiguration(LoggerFactory, TransportStrategy, Middlewares.Concat(additionalMiddlewares).ToList(), SubscribeHeaders);
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