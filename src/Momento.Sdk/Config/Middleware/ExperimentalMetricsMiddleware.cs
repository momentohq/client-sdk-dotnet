using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Momento.Sdk.Config.Middleware
{

    /// <summary>
    /// A struct that records metrics about a Momento request. These metrics include the number of requests active 
    /// at the start and finish of the request, the type of the request, the status code of the response, 
    /// timing information about the request (start, end, and duration times in milliseconds), 
    /// and the size of the request and response bodies in bytes.
    ///
    /// Metrics may be added to this struct in the future if they are useful.
    /// </summary>
    public record struct ExperimentalRequestMetrics(
        // The number of requests active at the start of the request.
        int NumActiveRequestsAtStart,
        // The number of requests active at the finish of the request (including the request itself).
        int NumActiveRequestsAtFinish,
        // The generated grpc object type of the request.
        string RequestType,
        // The grpc status code of the response.
        StatusCode Status,
        // The time the request started (millis since epoch).
        long StartTime,
        // The time the request completed (millis since epoch).
        long EndTime,
        // The duration of the request (in millis).
        long Duration,
        // The size of the request body in bytes.
        int RequestSize,
        // The size of the response body in bytes.
        int ResponseSize
    );

    /// <summary>
    /// This middleware enables per-request client-side metrics. This is an abstract
    /// class that does not route the metrics to a specific destination; concrete subclasses
    /// may store the metrics as they see fit.
    ///
    /// The metrics format is currently considered experimental. In a future release,
    /// once the format is considered stable, this class will be renamed to remove
    /// the Experimental prefix.
    ///
    /// WARNING: enabling this middleware may have minor performance implications,
    /// so enable with caution.
    /// </summary>
    public abstract class ExperimentalMetricsMiddleware : IMiddleware
    {
        private int activeRequestCount = 0;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor for the ExperimentalMetricsMiddleware class.
        /// </summary>
        /// <param name="loggerFactory">Used for logging in case of errors.</param>
        protected ExperimentalMetricsMiddleware(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ExperimentalMetricsMiddleware>();
        }

        /// <summary>
        /// Increments and returns the active request count in a thread-safe way.
        /// </summary>
        public void IncrementActiveRequestCount()
        {
            Interlocked.Increment(ref activeRequestCount);
        }

        /// <summary>
        /// Decrements and returns the active request count in a thread-safe way.
        /// </summary>
        public void DecrementActiveRequestCount()
        {
            Interlocked.Decrement(ref activeRequestCount);
        }

        /// <summary>
        /// Output metrics for a Momento request to a destination decided by the implementing class.
        /// </summary>
        public abstract Task EmitMetrics(ExperimentalRequestMetrics metrics);

        /// <inheritdoc/>
        public async Task<MiddlewareResponseState<TResponse>> WrapRequest<TRequest, TResponse>(
        TRequest request,
        CallOptions callOptions,
        Func<TRequest, CallOptions, Task<MiddlewareResponseState<TResponse>>> continuation
        ) where TRequest : class where TResponse : class
        {
            if (!(request is Google.Protobuf.IMessage requestMessage))
            {
                _logger.LogError("Expected request to be type Google.Protobuf.IMessage. Found {}", request.GetType());
                return await continuation(request, callOptions);
            }
            var requestSize = requestMessage.CalculateSize();

            var startActiveRequestCount = activeRequestCount;
            IncrementActiveRequestCount();

            var startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var nextState = await continuation(request, callOptions);

            var endActiveRequestCount = activeRequestCount;
            var endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            if (!(nextState.ResponseAsync.Result is Google.Protobuf.IMessage response))
            {
                _logger.LogError("Expected response to be type Google.Protobuf.IMessage. Found {}", nextState.ResponseAsync.Result.GetType());
                DecrementActiveRequestCount();
                return nextState;
            }
            var responseSize = response.CalculateSize();

            var status = nextState.GetStatus();

            var metrics = new ExperimentalRequestMetrics(
                startActiveRequestCount,
                endActiveRequestCount,
                request.GetType().Name,
                status.StatusCode,
                startTime,
                endTime,
                endTime - startTime,
                requestSize,
                responseSize
            );

            await EmitMetrics(metrics);

            DecrementActiveRequestCount();

            return nextState;
        }

    }
}
