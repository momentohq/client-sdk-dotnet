using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Retry;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Config;


/// <summary>
/// Contract for SDK configurables. A configuration must have a retry strategy, middlewares, and transport strategy.
/// </summary>
public interface IConfiguration
{
    /// <inheritdoc cref="Microsoft.Extensions.Logging.ILoggerFactory" />
    public ILoggerFactory LoggerFactory { get; }
    /// <inheritdoc cref="Momento.Sdk.Config.Retry.IRetryStrategy" />
    public IRetryStrategy RetryStrategy { get; }
    /// <inheritdoc cref="Momento.Sdk.Config.Middleware.IMiddleware" />
    public IList<IMiddleware> Middlewares { get; }
    /// <inheritdoc cref="Momento.Sdk.Config.Transport.ITransportStrategy" />
    public ITransportStrategy TransportStrategy { get; }
    /// <inheritdoc cref="Momento.Sdk.Config.ReadConcern" />
    public ReadConcern ReadConcern { get; }

    /// <summary>
    /// Creates a new instance of the Configuration object, updated to use the specified retry strategy.
    /// </summary>
    /// <param name="retryStrategy">Defines a contract for how and when to retry a request</param>
    /// <returns>Configuration object with custom retry strategy provided</returns>
    public IConfiguration WithRetryStrategy(IRetryStrategy retryStrategy);

    /// <summary>
    /// Creates a new instance of the Configuration object, updated to use the specified middlewares.
    /// </summary>
    /// <param name="middlewares">The Middleware interface allows the Configuration to provide a higher-order function that wraps all requests.</param>
    /// <returns>Configuration object with custom middlewares provided</returns>
    public IConfiguration WithMiddlewares(IList<IMiddleware> middlewares);

    /// <summary>
    /// Creates a new instance of the Configuration object, updated to use the specified transport strategy.
    /// </summary>
    /// <param name="transportStrategy">This is responsible for configuring network tunables.</param>
    /// <returns>Configuration object with custom transport strategy provided</returns>
    public IConfiguration WithTransportStrategy(ITransportStrategy transportStrategy);

    /// <summary>
    /// Creates a new instance of the Configuration object, updated to use the specified client timeout.
    /// </summary>
    /// <param name="clientTimeout">The amount of time to wait before cancelling the request.</param>
    /// <returns>Configuration object with custom client timeout provided</returns>
    public IConfiguration WithClientTimeout(TimeSpan clientTimeout);
    
    /// <summary>
    /// Creates a new instance of the Configuration object, updated to use the specified read concern.
    /// </summary>
    /// <param name="readConcern">The read concern setting.</param>
    /// <returns>Configuration object with custom read concern provided</returns>
    public IConfiguration WithReadConcern(ReadConcern readConcern);
}
