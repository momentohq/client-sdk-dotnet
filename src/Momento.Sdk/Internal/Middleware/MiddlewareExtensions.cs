using System;
using Grpc.Core;
using Momento.Sdk.Config.Middleware;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Momento.Sdk.Internal.Middleware
{

    public static class MiddlewareExtensions
    {
        public static async Task<MiddlewareResponseState<TResponse>> WrapRequest<TRequest, TResponse>(
            this IList<IMiddleware> middlewares,
            TRequest request,
            CallOptions callOptions,
            Func<TRequest, CallOptions, AsyncUnaryCall<TResponse>> continuation
        ) where TRequest : class where TResponse : class
        {
            Func<TRequest, CallOptions, Task<MiddlewareResponseState<TResponse>>> continuationWithMiddlewareResponseState = (r, o) =>
            {
                var result = continuation(r, o);
                return Task.FromResult(new MiddlewareResponseState<TResponse>(
                    ResponseAsync: result.ResponseAsync,
                    ResponseHeadersAsync: result.ResponseHeadersAsync,
                    GetStatus: result.GetStatus,
                    GetTrailers: result.GetTrailers
                ));
            };

            var wrapped = middlewares.Aggregate(continuationWithMiddlewareResponseState, (acc, middleware) =>
            {
                return (r, o) => middleware.WrapRequest(r, o, acc);
            });
            return await wrapped(request, callOptions);
        }

    }

}

