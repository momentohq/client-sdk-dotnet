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
            typeof(_GetRequest),
            typeof(_DeleteRequest),
            typeof(_DictionarySetRequest),
            // not idempotent: typeof(_DictionaryIncrementRequest),
            typeof(_DictionaryGetRequest),
            typeof(_DictionaryFetchRequest),
            typeof(_DictionaryDeleteRequest),
            typeof(_DictionaryLengthRequest),
            typeof(_SetUnionRequest),
            typeof(_SetDifferenceRequest),
            typeof(_SetFetchRequest),
            // not idempotent: typeof(_ListPushFrontRequest),
            // not idempotent: typeof(_ListPushBackRequest),
            // not idempotent: typeof(_ListPopFrontRequest),
            // not idempotent: typeof(_ListPopBackRequest),
            typeof(_ListFetchRequest),
            /*
             *  Warning: in the future, this may not be idempotent
             *  Currently it supports removing all occurrences of a value.
             *  In the future, we may also add "the first/last N occurrences of a value".
             *  In the latter case it is not idempotent.
             */
            typeof(_ListRemoveRequest),
            typeof(_ListLengthRequest),
            // not idempotent: typeof(_ListConcatenateFrontRequest),
            // not idempotent: typeof(_ListConcatenateBackRequest)
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

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            var other = (DefaultRetryEligibilityStrategy)obj;
            return _retryableRequestTypes.SetEquals(other._retryableRequestTypes) &&
                _retryableStatusCodes.SetEquals(other._retryableStatusCodes);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

