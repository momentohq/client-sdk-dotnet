using System;
using Grpc.Core;
using System.Threading.Tasks;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Momento.Sdk.Config.Middleware
{
    /// <summary>
    /// Basic middleware that logs at debug level for each request/response.  This
    /// is mostly provided as an example of how to implement middlewares.
    /// </summary>
    public class LoggingMiddleware : IMiddleware
    {
        public ILoggerFactory LoggerFactory { get; }

        private readonly ILogger _logger;

        public LoggingMiddleware(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<LoggingMiddleware>();
        }

        public LoggingMiddleware WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            return new(loggerFactory);
        }

        IMiddleware IMiddleware.WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            return WithLoggerFactory(loggerFactory);
        }

        public async Task<MiddlewareResponseState<TResponse>> WrapRequest<TRequest, TResponse>(
            TRequest request,
            CallOptions callOptions,
            Func<TRequest, CallOptions, Task<MiddlewareResponseState<TResponse>>> continuation
        )
        {
            _logger.LogDebug("Executing request of type: {}", request?.GetType());
            var nextState = await continuation(request, callOptions);
            return new MiddlewareResponseState<TResponse>(
                ResponseAsync: nextState.ResponseAsync.ContinueWith(r =>
                {
                    _logger.LogDebug("Got response for request of type: {}", request?.GetType());
                    return r.Result;
                }),
                ResponseHeadersAsync: nextState.ResponseHeadersAsync,
                GetStatus: nextState.GetStatus,
                GetTrailers: nextState.GetTrailers
            );
        }
    }
}

