using System;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Momento.Sdk.Config.Retry
{
    /// <summary>
    /// Used to determine whether a failed request is eligible for retry.
    /// </summary>
    public interface IRetryEligibilityStrategy
    {
        /// <summary>
        /// Given the status code and request object for a failed request, returns
        /// <code>true</code> if the request is eligible for retry, <code>false</code>
        /// otherwise.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="status">The gRPC status of the failed request</param>
        /// <param name="request">The original gRPC request object</param>
        /// <returns></returns>
        public bool IsEligibleForRetry<TRequest>(Status status, TRequest request) where TRequest : class;
    }
}

