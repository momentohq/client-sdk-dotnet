using Microsoft.Extensions.Logging;
using Momento.Sdk.Exceptions;
using System.Collections.Generic;

namespace Momento.Sdk.Config.Retry
{
    /// <summary>
    /// Default implementation of <see cref="ISubscriptionRetryEligibilityStrategy"/> that determines whether a subscription error is eligible for resubscription.
    /// It considers certain error codes as non-retryable, such as NOT_FOUND_ERROR, PERMISSION_ERROR, AUTHENTICATION_ERROR, and CANCELLED_ERROR.
    /// </summary>
    public class DefaultSubscriptionRetryEligibilityStrategy : ISubscriptionRetryEligibilityStrategy
    {
        private readonly ILogger _logger;

        private readonly HashSet<MomentoErrorCode> _nonRetryableExceptions = new HashSet<MomentoErrorCode>
        {
            MomentoErrorCode.NOT_FOUND_ERROR,
            MomentoErrorCode.PERMISSION_ERROR,
            MomentoErrorCode.AUTHENTICATION_ERROR,
            MomentoErrorCode.CANCELLED_ERROR
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSubscriptionRetryEligibilityStrategy"/> class.
        /// </summary>
        /// <param name="logger">Logger for logging debug messages.</param>
        public DefaultSubscriptionRetryEligibilityStrategy(ILogger logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public bool IsEligibleForResubscribe(SdkException exception)
        {
            if (_nonRetryableExceptions.Contains(exception.ErrorCode))
            {
                _logger.LogDebug("Subscription retry eligibility check: {ErrorCode} is not eligible for resubscribe.", exception.ErrorCode);
                return false;
            }
            else
            {
                _logger.LogDebug("Subscription retry eligibility check: {ErrorCode} is eligible for resubscribe.", exception.ErrorCode);
                return true;
            }
        }
    }
}
