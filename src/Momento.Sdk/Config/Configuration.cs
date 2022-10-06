using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Retry;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Config;

public class Configuration : IConfiguration
{
    public ILoggerFactory LoggerFactory { get; }
    public IRetryStrategy RetryStrategy { get; }
    public IList<IMiddleware> Middlewares { get; }
    public ITransportStrategy TransportStrategy { get; }

    public Configuration(IRetryStrategy retryStrategy, ITransportStrategy transportStrategy, ILoggerFactory? loggerFactory = null)
        : this(retryStrategy, new List<IMiddleware>(), transportStrategy, loggerFactory)
    {

    }

    public Configuration(IRetryStrategy retryStrategy, IList<IMiddleware> middlewares, ITransportStrategy transportStrategy, ILoggerFactory? loggerFactory = null)
    {
        this.LoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;

        var retryStrategyWithLogger = retryStrategy.LoggerFactory != null ? retryStrategy : retryStrategy.WithLoggerFactory(loggerFactory!);
        var middlewaresWithLogger = middlewares.Select(m => m.LoggerFactory != null ? m : m.WithLoggerFactory(loggerFactory!)).ToList();

        this.RetryStrategy = retryStrategyWithLogger;
        this.Middlewares = middlewaresWithLogger;
        this.TransportStrategy = transportStrategy;
    }

    public IConfiguration WithLoggerFactory(ILoggerFactory loggerFactory)
    {
        return new Configuration(RetryStrategy, Middlewares, TransportStrategy, loggerFactory);
    }

    public IConfiguration WithRetryStrategy(IRetryStrategy retryStrategy)
    {
        return new Configuration(retryStrategy, Middlewares, TransportStrategy, LoggerFactory);
    }

    public IConfiguration WithMiddlewares(IList<IMiddleware> middlewares)
    {
        return new Configuration(RetryStrategy, middlewares, TransportStrategy, LoggerFactory);
    }

    public IConfiguration WithTransportStrategy(ITransportStrategy transportStrategy)
    {
        return new Configuration(RetryStrategy, Middlewares, transportStrategy, LoggerFactory);
    }

    public Configuration WithAdditionalMiddlewares(IList<IMiddleware> additionalMiddlewares)
    {
        return new(
            retryStrategy: RetryStrategy,
            middlewares: Middlewares.Concat(additionalMiddlewares).ToList(),
            transportStrategy: TransportStrategy,
            loggerFactory: LoggerFactory
        );
    }

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
