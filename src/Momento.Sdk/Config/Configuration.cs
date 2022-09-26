using System.Collections.Generic;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Retry;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Config;

public class Configuration : IConfiguration
{
    public IRetryStrategy RetryStrategy { get; }
    public IList<IMiddleware> Middlewares { get; }
    public ITransportStrategy TransportStrategy { get; }

    public Configuration(IRetryStrategy retryStrategy, ITransportStrategy transportStrategy)
        : this(retryStrategy, new List<IMiddleware>(), transportStrategy)
    {

    }

    public Configuration(IRetryStrategy retryStrategy, IList<IMiddleware> middlewares, ITransportStrategy transportStrategy)
    {
        this.RetryStrategy = retryStrategy;
        this.Middlewares = middlewares;
        this.TransportStrategy = transportStrategy;
    }

    public IConfiguration WithRetryStrategy(IRetryStrategy retryStrategy)
    {
        return new Configuration(retryStrategy, Middlewares, TransportStrategy);
    }

    public IConfiguration WithMiddlewares(IList<IMiddleware> middlewares)
    {
        return new Configuration(RetryStrategy, middlewares, TransportStrategy);
    }

    public IConfiguration WithTransportStrategy(ITransportStrategy transportStrategy)
    {
        return new Configuration(RetryStrategy, Middlewares, transportStrategy);
    }
}
