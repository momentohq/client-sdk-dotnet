using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Retry;

namespace Momento.Sdk.Config.Middleware;

public class PassThroughMiddleware : IMiddleware
{
    public ILoggerFactory LoggerFactory { get; }

    public PassThroughMiddleware(ILoggerFactory loggerFactory)
    {
        LoggerFactory = loggerFactory;
    }

    public PassThroughMiddleware WithLoggerFactory(ILoggerFactory loggerFactory)
    {
        return new(loggerFactory);
    }

    IMiddleware IMiddleware.WithLoggerFactory(ILoggerFactory loggerFactory)
    {
        return WithLoggerFactory(loggerFactory);
    }

    public Task<MiddlewareResponseState<TResponse>> WrapRequest<TRequest, TResponse>(
        TRequest request,
        CallOptions callOptions,
        Func<TRequest, CallOptions, Task<MiddlewareResponseState<TResponse>>> continuation
    ) where TRequest : class where TResponse : class
    {
        return continuation(request, callOptions);
    }

}
