﻿using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Middleware;
using System.Linq;
using Momento.Protos.CacheClient;
using System.Collections.Generic;

namespace Momento.Sdk.Config.Retry
{
    internal class RetryMiddleware : IMiddleware
    {
        public ILoggerFactory? LoggerFactory { get; }

        private readonly ILogger _logger;
        private readonly IRetryStrategy _retryStrategy;

        public RetryMiddleware(ILoggerFactory loggerFactory, IRetryStrategy retryStrategy)
        {
            LoggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<RetryMiddleware>();
            _retryStrategy = retryStrategy;
        }

        public RetryMiddleware WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            return new(loggerFactory, _retryStrategy);
        }

        IMiddleware IMiddleware.WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            return WithLoggerFactory(loggerFactory);
        }

        public async Task<MiddlewareResponseState<TResponse>> WrapRequest<TRequest, TResponse>(
            TRequest request,
            CallOptions callOptions,
            Func<TRequest, CallOptions, Task<MiddlewareResponseState<TResponse>>> continuation
        ) where TRequest : class where TResponse : class
        {
            var foo = request.GetType();
            MiddlewareResponseState<TResponse> nextState;
            int attemptNumber = 0;
            int? retryAfterMillis = 0;
            do
            {
                var delay = retryAfterMillis ?? 0;
                if (delay > 0)
                {
                    await Task.Delay(delay);
                }
                attemptNumber++;
                nextState = await continuation(request, callOptions);

                // NOTE: we need a try/catch block here, because: (a) we cannot call
                // `nextState.GetStatus()` until after we `await` the response, or
                // it will throw an error.  and (b) if the status is anything other
                // than "ok", the `await` on the response will throw an exception.
                try
                {
                    await nextState.ResponseAsync;
                    
                    if (attemptNumber > 1)
                    {
                        _logger.LogDebug($"Retry succeeded (attempt {attemptNumber})");
                    }
                    break;
                }
                catch (Exception)
                {
                    var status = nextState.GetStatus();
                    _logger.LogDebug($"Request failed with status {status.StatusCode}, checking to see if we should retry; attempt Number: {attemptNumber}");
                    _logger.LogTrace($"Failed request status: {status}");
                    retryAfterMillis = _retryStrategy.DetermineWhenToRetryRequest(nextState.GetStatus(), request, attemptNumber);
                }
            }
            while (retryAfterMillis != null);

            return new MiddlewareResponseState<TResponse>(
                ResponseAsync: nextState.ResponseAsync,
                ResponseHeadersAsync: nextState.ResponseHeadersAsync,
                GetStatus: nextState.GetStatus,
                GetTrailers: nextState.GetTrailers
            );
        }
    }
}

