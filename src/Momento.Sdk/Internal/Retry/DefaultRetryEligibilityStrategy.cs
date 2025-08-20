#pragma warning disable 1591
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Protos.CacheClient;
using Momento.Protos.CacheClient.Pubsub;
using Momento.Sdk.Config.Retry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Momento.Sdk.Internal.Retry
{
    /// <summary>
    /// A retry eligibility strategy that returns true for status codes that
    /// are commonly understood to be retry-able (Unavailable, Internal), and
    /// for idempotent request types.
    /// </summary>
    public class DefaultRetryEligibilityStrategy : IRetryEligibilityStrategy
    {
        private readonly HashSet<StatusCode> _retryableStatusCodes = new()
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

        private readonly HashSet<Type> _retryableRequestTypes = new()
        {
            typeof(_GetRequest),
            typeof(_GetBatchRequest),
            typeof(_SetRequest),
            typeof(_SetBatchRequest),
            // Not retryable: typeof(_SetIfRequest),
            // SetIfNotExists is deprecated
            // Not retryable: typeof(_SetIfNotExistsRequest),
            typeof(_DeleteRequest),
            typeof(_KeysExistRequest),
            // Not retryable: typeof(_IncrementRequest),
            // Not retryable: typeof(_UpdateTtlRequest),
            typeof(_ItemGetTtlRequest),
            typeof(_ItemGetTypeRequest),

            typeof(_DictionaryGetRequest),
            typeof(_DictionaryFetchRequest),
            typeof(_DictionarySetRequest),
            // Not retryable: typeof(_DictionaryIncrementRequest),
            typeof(_DictionaryDeleteRequest),
            typeof(_DictionaryLengthRequest),

            typeof(_SetFetchRequest),
            typeof(_SetSampleRequest),
            typeof(_SetUnionRequest),
            typeof(_SetDifferenceRequest),
            typeof(_SetContainsRequest),
            typeof(_SetLengthRequest),
            // Not retryable: typeof(_SetPopRequest),

            // Not retryable: typeof(_ListPushFrontRequest),
            // Not retryable: typeof(_ListPushBackRequest),
            // Not retryable: typeof(_ListPopFrontRequest),
            // Not retryable: typeof(_ListPopBackRequest),
            // Not used: typeof(_ListEraseRequest),
            typeof(_ListRemoveRequest),
            typeof(_ListFetchRequest),
            typeof(_ListLengthRequest),
            // Not retryable: typeof(_ListConcatenateFrontRequest),
            // Not retryable: typeof(_ListConcatenateBackRequest),
            // Not retryable: typeof(_ListRetainRequest),

            typeof(_SortedSetPutRequest),
            typeof(_SortedSetFetchRequest),
            typeof(_SortedSetGetScoreRequest),
            typeof(_SortedSetRemoveRequest),
            // Not retryable: typeof(_SortedSetIncrementRequest),
            typeof(_SortedSetGetRankRequest),
            typeof(_SortedSetLengthRequest),
            typeof(_SortedSetLengthByScoreRequest),

            typeof(_SubscriptionRequest)
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
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (obj is null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (DefaultRetryEligibilityStrategy)obj;
            return _retryableRequestTypes.SetEquals(other._retryableRequestTypes) &&
                   _retryableStatusCodes.SetEquals(other._retryableStatusCodes);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = _retryableRequestTypes.Aggregate(17,
                    (current, type) => current * 31 + (type?.GetHashCode() ?? 0));
                return _retryableStatusCodes.Aggregate(hash,
                    (current, code) => current * 31 + code.GetHashCode());
            }
        }
    }
}
