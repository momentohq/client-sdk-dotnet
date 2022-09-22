using System.Collections.Generic;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Retry;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Config;

public class Configuration : IConfiguration
{
    public IRetryStrategy RetryStrategy { get; set; }
    public IList<IMiddleware> Middlewares { get; set; }
    public ITransportStrategy TransportStrategy { get; set; }

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
}
