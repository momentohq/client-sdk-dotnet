using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Momento.Sdk.Config.Middleware;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

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
    public class MaxConcurrentRequestsMiddleware : IMiddleware
    {
        private readonly FairAsyncSemaphore _semaphore;

        public MaxConcurrentRequestsMiddleware(int maxConcurrentRequests)
        {
            _semaphore = new FairAsyncSemaphore(maxConcurrentRequests);
        }

        public async Task<MiddlewareResponseState<TResponse>> WrapRequest<TRequest, TResponse>(TRequest request, CallOptions callOptions, Func<TRequest, CallOptions, Task<MiddlewareResponseState<TResponse>>> continuation)
        {
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
    }
}

