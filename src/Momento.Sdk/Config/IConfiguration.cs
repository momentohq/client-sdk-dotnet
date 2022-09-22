using System.Collections.Generic;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Retry;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Config;


/// <summary>
/// Contract for SDK configurables. A configuration must have a retry strategy, middlewares, and transport strategy.
/// </summary>
public interface IConfiguration
{
    public IRetryStrategy RetryStrategy { get; }
    public IList<IMiddleware> Middlewares { get; }
    public ITransportStrategy TransportStrategy { get; }
}
