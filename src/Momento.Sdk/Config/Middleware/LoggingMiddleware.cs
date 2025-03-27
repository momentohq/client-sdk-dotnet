using System;
using Grpc.Core;
using System.Threading.Tasks;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Momento.Sdk.Config.Middleware
{
    /// <summary>
    /// Basic middleware that logs at debug level for each request/response.  This
    /// is mostly provided as an example of how to implement middlewares.
    /// </summary>
    public class LoggingMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggerFactory"></param>
        public LoggingMiddleware(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<LoggingMiddleware>();
        }

        /// <inheritdoc/>
        public IList<Tuple<string, string>> AddStreamRequestHeaders()
        {
            return new List<Tuple<string, string>>();
        }

        /// <inheritdoc/>
        public async Task<MiddlewareResponseState<TResponse>> WrapRequest<TRequest, TResponse>(
            TRequest request,
            CallOptions callOptions,
            Func<TRequest, CallOptions, Task<MiddlewareResponseState<TResponse>>> continuation
        ) where TRequest : class where TResponse : class
        {
            _logger.LogDebug("Executing request of type: {}", request.GetType());
            var nextState = await continuation(request, callOptions);
            return new MiddlewareResponseState<TResponse>(
                ResponseAsync: nextState.ResponseAsync.ContinueWith(r =>
                {
                    _logger.LogDebug("Got response for request of type: {}", request.GetType());
                    return r.Result;
                }),
                ResponseHeadersAsync: nextState.ResponseHeadersAsync,
                GetStatus: nextState.GetStatus,
                GetTrailers: nextState.GetTrailers
            );
        }
    }
}

