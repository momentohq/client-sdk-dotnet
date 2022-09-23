using System.Collections.Generic;
using System.Linq;
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

    public Configuration WithAdditionalMiddlewares(IList<IMiddleware> additionalMiddlewares)
    {
        return new(
            retryStrategy: RetryStrategy,
            middlewares: Middlewares.Concat(additionalMiddlewares).ToList(),
            transportStrategy: TransportStrategy
        );
    }
}
