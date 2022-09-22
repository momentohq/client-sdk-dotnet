using System.Collections.Generic;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Retry;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Config;


public class Configuration
{
    public IRetryStrategy RetryStrategy { get; private set; }
    public IList<IMiddleware> Middlewares { get; private set; }
    public ITransportStrategy TransportStrategy { get; private set; }

    public Configuration(IRetryStrategy retryStrategy, IList<IMiddleware> middlewares, ITransportStrategy transportStrategy)
    {
        this.RetryStrategy = retryStrategy;
        this.Middlewares = middlewares;
        this.TransportStrategy = transportStrategy;
    }
}