using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Retry;

namespace Momento.Sdk.Config.Middleware;

public class PassThroughMiddleware : IMiddleware
{
    private readonly ILogger _logger;

    public PassThroughMiddleware(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<PassThroughMiddleware>();
    }

    public Task<MiddlewareResponseState<TResponse>> WrapRequest<TRequest, TResponse>(
        TRequest request,
        CallOptions callOptions,
        Func<TRequest, CallOptions, Task<MiddlewareResponseState<TResponse>>> continuation
    ) where TRequest : class where TResponse : class
    {
        _logger.LogDebug("Hello from PassThroughMiddleware");
        return continuation(request, callOptions);
    }

}
