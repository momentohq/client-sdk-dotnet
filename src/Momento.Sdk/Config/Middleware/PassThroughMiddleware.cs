using System;
using System.Threading.Tasks;
using Grpc.Core.Interceptors;
using Momento.Sdk.Config.Retry;

namespace Momento.Sdk.Config.Middleware;

public class PassThroughMiddleware : IMiddleware
{
    public MiddlewareResponseState<TResponse> WrapRequest<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        Func<TRequest, ClientInterceptorContext<TRequest, TResponse>, MiddlewareResponseState<TResponse>> continuation
        )
        where TRequest : class
        where TResponse : class
    {
        return continuation(request, context);
    }
}
