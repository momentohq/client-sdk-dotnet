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
    /// <inheritdoc />
    public ILoggerFactory LoggerFactory { get; }
    /// <inheritdoc />
    public IRetryStrategy RetryStrategy { get; }
    /// <inheritdoc />
    public IList<IMiddleware> Middlewares { get; }
    /// <inheritdoc />
    public ITransportStrategy TransportStrategy { get; }

    /// <inheritdoc cref="Momento.Sdk.Config.IConfiguration" />
    public Configuration(IRetryStrategy retryStrategy, ITransportStrategy transportStrategy, ILoggerFactory? loggerFactory = null)
        : this(retryStrategy, new List<IMiddleware>(), transportStrategy, loggerFactory)
    {

    }

    /// <summary>
    /// Create a new instance of Configuration obejct with provided arguments: <see cref="Momento.Sdk.Config.IConfiguration.RetryStrategy">, <see cref="Momento.Sdk.Config.IConfiguration.Middlewares">, <see cref="Momento.Sdk.Config.IConfiguration.TransportStrategy"/>, and <see cref="Momento.Sdk.Config.IConfiguration.LoggerFactory"/>
    /// </summary>
    /// <param name="retryStrategy">Defines a contract for how and when to retry a request</param>
    /// <param name="middlewares">The Middleware interface allows the Configuration to provide a higher-order function that wraps all requests.</param>
    /// <param name="transportStrategy">This is responsible for configuring network tunables.</param>
    /// <param name="loggerFactory">This is responsible for configuraing logging.</param>
    public Configuration(IRetryStrategy retryStrategy, IList<IMiddleware> middlewares, ITransportStrategy transportStrategy, ILoggerFactory? loggerFactory = null)

    {
        this.LoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;

        var retryStrategyWithLogger = retryStrategy.LoggerFactory != null ? retryStrategy : retryStrategy.WithLoggerFactory(loggerFactory!);
        var middlewaresWithLogger = middlewares.Select(m => m.LoggerFactory != null ? m : m.WithLoggerFactory(loggerFactory!)).ToList();

        this.RetryStrategy = retryStrategyWithLogger;
        this.Middlewares = middlewaresWithLogger;
        this.TransportStrategy = transportStrategy;
    }

    /// <inheritdoc />
    public IConfiguration WithLoggerFactory(ILoggerFactory loggerFactory)
    {
        return new Configuration(RetryStrategy, Middlewares, TransportStrategy, loggerFactory);
    }

    /// <inheritdoc />
    public IConfiguration WithRetryStrategy(IRetryStrategy retryStrategy)
    {
        return new Configuration(retryStrategy, Middlewares, TransportStrategy, LoggerFactory);
    }

    /// <inheritdoc />
    public IConfiguration WithMiddlewares(IList<IMiddleware> middlewares)
    {
        return new Configuration(RetryStrategy, middlewares, TransportStrategy, LoggerFactory);
    }

    /// <inheritdoc />
    public IConfiguration WithTransportStrategy(ITransportStrategy transportStrategy)
    {
        return new Configuration(RetryStrategy, Middlewares, transportStrategy, LoggerFactory);
    }

    /// <summary>
    /// Add the specified middlewares to an existing instance of Configuration object in addition to already specified middlewares.
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
    /// Add the specified client timeout to an existing instance of Configuration object as an addiion to the existing transport strategy.
    /// </summary>
    /// <param name="clientTimeoutMillis">The amount of time to wait before cancelling the request.</param>
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
