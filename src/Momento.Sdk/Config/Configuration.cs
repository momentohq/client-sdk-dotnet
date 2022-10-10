using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Retry;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Config;

/// <inheritdoc cref="Momento.Sdk.Config.IConfiguration" />
public class Configuration : IConfiguration
{
    /// <inheritdoc cref="Microsoft.Extensions.Logging.ILoggerFactory" />
    public ILoggerFactory LoggerFactory { get; }
    /// <inheritdoc cref="Momento.Sdk.Config.Retry.IRetryStrategy" />
    public IRetryStrategy RetryStrategy { get; }
    /// <inheritdoc cref="Momento.Sdk.Config.Middleware.IMiddleware" />
    public IList<IMiddleware> Middlewares { get; }
    /// <inheritdoc cref="Momento.Sdk.Config.Transport.ITransportStrategy" />
    public ITransportStrategy TransportStrategy { get; }

    /// <summary>
    /// <inheritdoc cref="Momento.Sdk.Config.IConfiguration" />
    /// </summary>
    /// <param name="retryStrategy">Defines a contract for how and when to retry a request</param>
    /// <param name="transportStrategy">This is responsible for configuring network tunables.</param>
    /// <param name="loggerFactory">This is responsible for configuraing logging.</param>
    public Configuration(ILoggerFactory loggerFactory, IRetryStrategy retryStrategy, ITransportStrategy transportStrategy)
        : this(loggerFactory, retryStrategy, new List<IMiddleware>(), transportStrategy)
    {

    }

    /// <summary>
    /// <inheritdoc cref="Momento.Sdk.Config.IConfiguration" />
    /// </summary>
    /// <param name="retryStrategy">Defines a contract for how and when to retry a request</param>
    /// <param name="middlewares">The Middleware interface allows the Configuration to provide a higher-order function that wraps all requests.</param>
    /// <param name="transportStrategy">This is responsible for configuring network tunables.</param>
    /// <param name="loggerFactory">This is responsible for configuraing logging.</param>
    public Configuration(ILoggerFactory loggerFactory, IRetryStrategy retryStrategy, IList<IMiddleware> middlewares, ITransportStrategy transportStrategy)
    {
        this.LoggerFactory = loggerFactory;
        this.RetryStrategy = retryStrategy;
        this.Middlewares = middlewares;
        this.TransportStrategy = transportStrategy;
    }

    /// <summary>
    ///  Configures retry strategy
    /// </summary>
    /// <param name="retryStrategy">Defines a contract for how and when to retry a request</param>
    /// <returns>Configuration object with custom retry strategy provided</returns>
    public IConfiguration WithRetryStrategy(IRetryStrategy retryStrategy)
    {
        return new Configuration(LoggerFactory, retryStrategy, Middlewares, TransportStrategy);
    }

    /// <summary>
    ///  Configures middlewares
    /// </summary>
    /// <param name="middlewares">The Middleware interface allows the Configuration to provide a higher-order function that wraps all requests.</param>
    /// <returns>Configuration object with custom middlewares provided</returns>
    public IConfiguration WithMiddlewares(IList<IMiddleware> middlewares)
    {
        return new Configuration(LoggerFactory, RetryStrategy, middlewares, TransportStrategy);
    }

    /// <summary>
    ///  Configures transport trategy
    /// </summary>
    /// <param name="transportStrategy">This is responsible for configuring network tunables.</param>
    /// <returns>Configuration object with custom transport strategy provided</returns>
    public IConfiguration WithTransportStrategy(ITransportStrategy transportStrategy)
    {
        return new Configuration(LoggerFactory, RetryStrategy, Middlewares, transportStrategy);
    }

    /// <summary>
    ///  Configures middlewares
    /// </summary>
    /// <param name="additionalMiddlewares">The Middleware interface allows the Configuration to provide a higher-order function that wraps all requests.</param>
    /// <returns>Configuration object with custom middlewares provided</returns>
    public Configuration WithAdditionalMiddlewares(IList<IMiddleware> additionalMiddlewares)
    {
        return new(
            retryStrategy: RetryStrategy,
            middlewares: Middlewares.Concat(additionalMiddlewares).ToList(),
            transportStrategy: TransportStrategy,
            loggerFactory: LoggerFactory
        );
    }

    /// <summary>
    ///  Configures client timeout for transport strategy
    /// </summary>
    /// <param name="clientTimeoutMillis">Client timeout in milliseconds.</param>
    /// <returns>Configuration object with client timeout provided</returns>
    public Configuration WithClientTimeoutMillis(uint clientTimeoutMillis)
    {
        return new(
            retryStrategy: RetryStrategy,
            middlewares: Middlewares,
            transportStrategy: TransportStrategy.WithClientTimeoutMillis(clientTimeoutMillis),
            loggerFactory: LoggerFactory
        );
    }

    IConfiguration IConfiguration.WithClientTimeoutMillis(uint clientTimeoutMillis)
    {
        return WithClientTimeoutMillis(clientTimeoutMillis);
    }
}
