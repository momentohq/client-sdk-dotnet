using System;
using Grpc.Core.Interceptors;

namespace Momento.Sdk.Config.Middleware
{
    public class LoggingMiddleware : IMiddleware
    {
        public MiddlewareResponseState<TResponse> WrapRequest<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, Func<TRequest, ClientInterceptorContext<TRequest, TResponse>, MiddlewareResponseState<TResponse>> continuation)
            where TRequest : class
            where TResponse : class
        {
            Console.WriteLine($"LOGGING MIDDLEWARE WRAPPING REQUEST: {request.GetType()}");
            var nextState = continuation(request, context);
            Console.WriteLine($"LOGGING MIDDLEWARE WRAPPED REQUEST: {request.GetType()}");
            return new MiddlewareResponseState<TResponse>(
                ResponseAsync: nextState.ResponseAsync.ContinueWith(r =>
                {
                    Console.WriteLine($"LOGGING MIDDLEWARE RESPONSE CALLBACK: {request.GetType()}");
                    return r.Result;
                }),
                ResponseHeadersAsync: nextState.ResponseHeadersAsync,
                GetStatus: nextState.GetStatus,
                GetTrailers: nextState.GetTrailers
            );
        }
    }
}

