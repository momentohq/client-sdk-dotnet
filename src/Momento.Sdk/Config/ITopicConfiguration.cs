using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Transport;
using System;
using System.Collections.Generic;

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
    /// <inheritdoc cref="Momento.Sdk.Config.Middleware.ITopicMiddleware" />
    public IList<ITopicMiddleware> Middlewares { get; }

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
    /// Replace the middlewares on an existing instance of TopicConfiguration object with
    /// the provided middlewares.
    /// </summary>
    /// <param name="middlewares">The list of middlewares to be used.</param>
    public ITopicConfiguration WithMiddlewares(IEnumerable<ITopicMiddleware> middlewares);

    /// <summary>
    /// Add the specified middleware to an existing instance of TopicConfiguration object as an addition to
    /// the existing middlewares.
    /// </summary>
    /// <param name="middleware">The middleware to be used.</param>
    /// <returns>TopicConfiguration object with middleware provided</returns>
    public ITopicConfiguration AddMiddleware(ITopicMiddleware middleware);
}
