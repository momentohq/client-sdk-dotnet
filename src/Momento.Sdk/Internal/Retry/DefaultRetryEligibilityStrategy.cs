#pragma warning disable 1591
using System;
using System.Collections.Generic;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Momento.Protos.CacheClient;
using Momento.Sdk.Config.Retry;

namespace Momento.Sdk.Internal.Retry
{
    /// <summary>
    /// A retry eligibility strategy that returns true for status codes that
    /// are commonly understood to be retry-able (Unavailable, Internal), and
    /// for idempotent request types.
    /// </summary>
    public class DefaultRetryEligibilityStrategy : IRetryEligibilityStrategy
    {
        private readonly HashSet<StatusCode> _retryableStatusCodes = new HashSet<StatusCode>
        {
            //StatusCode.OK,
            //StatusCode.Cancelled,
            //StatusCode.Unknown,
            //StatusCode.InvalidArgument,
            //StatusCode.DeadlineExceeded,
            //StatusCode.NotFound,
            //StatusCode.AlreadyExists,
            //StatusCode.PermissionDenied,
            //StatusCode.Unauthenticated,
            //StatusCode.ResourceExhausted,
            //StatusCode.FailedPrecondition,
            //StatusCode.Aborted,
            //StatusCode.OutOfRange,
            //StatusCode.Unimplemented,
            StatusCode.Internal,
            StatusCode.Unavailable,
            //StatusCode.DataLoss,
        };

        private readonly HashSet<Type> _retryableRequestTypes = new HashSet<Type>
        {
            typeof(_SetRequest),
            typeof(_GetRequest)
        };

        private readonly ILogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggerFactory"></param>
        public DefaultRetryEligibilityStrategy(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DefaultRetryEligibilityStrategy>();
        }

        /// <inheritdoc/>
        public bool IsEligibleForRetry<TRequest>(Status status, TRequest request)
            where TRequest : class
        {
            if (!_retryableStatusCodes.Contains(status.StatusCode))
            {
                _logger.LogDebug("Response with status code {} is not retryable.", status.StatusCode);
                return false;
            }

            if (!_retryableRequestTypes.Contains(request.GetType()))
            {
                _logger.LogDebug("Request with type {} is not retryable.", request.GetType());
                return false;
            }

            return true;
        }
    }
}

