using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Middleware;

namespace Momento.Sdk.Internal.Middleware
{
    //
    // During tuning, I discovered that the.NET grpc library behaves differently
    // than the others I'd previously tested.  Specifically, it will detect whenever
    // your number of concurrent requests exceeds a multiple of 100 and open a new
    // connection to the server when it does. Thus, in my loadgen scenario where I
    // was executing 5000 concurrent requests, the library would end up opening
    // on the order of 50 connections to the server.
    //
    // This behavior is undesirable for both the client and server, depending on
    // the network bandwidth available between the two.  Specifically, in a laptop
    // environment, throughput and latency both decrease when you exceed about 200
    // concurrent requests, so there is no value in opening so many additional
    // connections to the server and this could cause server-side throttling/load shedding.
    //
    // In later versions of .NET it appears that this behavior may be configurable:
    // https://learn.microsoft.com/en-us/aspnet/core/grpc/performance?view=aspnetcore-6.0#connection-concurrency
    // But in the older version that we are targeting it is not.
    //
    // Therefore we use an async semaphore to restrict the maximum number of
    // concurrent requests.Setting this to 200 in the laptop environment results
    // in far fewer connections, and throughput and latency for the happy path
    // cases are unaffected. For the degenerate case (5000+ concurrent requests),
    // this protects the server and actually seems to improve client-side p999
    // latencies by quite a bit.
    internal class MaxConcurrentRequestsMiddleware : IMiddleware
    {
        private readonly int _maxConcurrentRequests;
        private readonly FairAsyncSemaphore _semaphore;
        private readonly ILogger _logger;

        public MaxConcurrentRequestsMiddleware(ILoggerFactory loggerFactory, int maxConcurrentRequests)
        {
            _maxConcurrentRequests = maxConcurrentRequests;
            _semaphore = new FairAsyncSemaphore(maxConcurrentRequests);
            _logger = loggerFactory.CreateLogger<MaxConcurrentRequestsMiddleware>();
        }

        public async Task<MiddlewareResponseState<TResponse>> WrapRequest<TRequest, TResponse>(
            TRequest request,
            CallOptions callOptions,
            Func<TRequest, CallOptions, Task<MiddlewareResponseState<TResponse>>> continuation
        ) where TRequest : class where TResponse : class
        {
            if (_semaphore.GetCurrentTicketCount() == 0)
            {
                _logger.LogDebug("Max concurrent requests reached. The client will wait until one or more requests " +
                                 " have completed.");
            }

            await _semaphore.WaitOne();
            
            try
            {
                var result = await continuation(request, callOptions);
                // ensure that we don't return (and release the semaphore) until the response task is complete
                await result.ResponseAsync;
                return result;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            var other = (MaxConcurrentRequestsMiddleware)obj;
            return _maxConcurrentRequests == other._maxConcurrentRequests;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <inheritdoc/>
        public IList<Tuple<string, string>> AddStreamRequestHeaders()
        {
            return new List<Tuple<string, string>>();
        }
    }
}

