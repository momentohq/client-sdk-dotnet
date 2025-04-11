using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Internal;
using System;
using System.Threading.Tasks;

namespace Momento.Sdk.Config.Retry
{
    internal class RetryMiddleware : IMiddleware
    {
        private readonly ILogger _logger;
        private readonly IRetryStrategy _retryStrategy;
        private readonly TimeSpan _clientTimeout;

        public RetryMiddleware(ILoggerFactory loggerFactory, IRetryStrategy retryStrategy, TimeSpan clientTimeout)
        {
            _logger = loggerFactory.CreateLogger<RetryMiddleware>();
            _retryStrategy = retryStrategy;
            _clientTimeout = clientTimeout;
        }

        public async Task<MiddlewareResponseState<TResponse>> WrapRequest<TRequest, TResponse>(
            TRequest request,
            CallOptions callOptions,
            Func<TRequest, CallOptions, Task<MiddlewareResponseState<TResponse>>> continuation
        ) where TRequest : class where TResponse : class
        {
            // The first time we enter WrapRequest, we capture the overall deadline for the request
            // in case we're using the FixedTimeoutRetryStrategy
            DateTime _overallDeadline = callOptions.Deadline ?? Utils.CalculateDeadline(_clientTimeout);
            Console.WriteLine($"Overall deadline: {_overallDeadline}");

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

                    if (_retryStrategy is IDeadlineAwareRetryStrategy strategy)
                    {
                        retryAfterMillis = strategy.DetermineWhenToRetryRequest(status, request, attemptNumber, _overallDeadline);
                        callOptions = strategy.CalculateRetryDeadline(callOptions, _overallDeadline);
                        Console.WriteLine($"Retrying request with retry deadline {callOptions.Deadline}");
                    }
                    else
                    {
                        Console.WriteLine($"Retrying request without retry deadline");
                        retryAfterMillis = _retryStrategy.DetermineWhenToRetryRequest(status, request, attemptNumber);
                    }
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

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            var other = (RetryMiddleware)obj;
            return _logger.Equals(other._logger) && _retryStrategy.Equals(other._retryStrategy);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

