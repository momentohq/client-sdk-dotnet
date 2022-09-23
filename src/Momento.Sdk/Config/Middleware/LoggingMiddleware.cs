using System;
using Grpc.Core;
using System.Threading.Tasks;
using Grpc.Core.Interceptors;

namespace Momento.Sdk.Config.Middleware
{
    public class LoggingMiddleware : IMiddleware
    {

        public async Task<MiddlewareResponseState<TResponse>> WrapRequest<TRequest, TResponse>(
            TRequest request,
            CallOptions callOptions,
            Func<TRequest, CallOptions, Task<MiddlewareResponseState<TResponse>>> continuation
        )
        {
            Console.WriteLine($"LOGGING MIDDLEWARE WRAPPING REQUEST: {request.GetType()}");
            var nextState = await continuation(request, callOptions);
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

