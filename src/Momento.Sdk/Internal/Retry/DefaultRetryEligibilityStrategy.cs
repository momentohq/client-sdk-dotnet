using System;
using System.Collections.Generic;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Momento.Protos.CacheClient;
using Momento.Sdk.Config.Retry;

namespace Momento.Sdk.Internal.Retry
{
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

        public ILoggerFactory? LoggerFactory { get; }

        private readonly ILogger _logger;

        public DefaultRetryEligibilityStrategy(ILoggerFactory? loggerFactory)
        {
            _logger = (loggerFactory ?? NullLoggerFactory.Instance).CreateLogger<DefaultRetryEligibilityStrategy>();
        }

        public DefaultRetryEligibilityStrategy WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            return new(loggerFactory);
        }

        IRetryEligibilityStrategy IRetryEligibilityStrategy.WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            return WithLoggerFactory(loggerFactory);
        }

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

