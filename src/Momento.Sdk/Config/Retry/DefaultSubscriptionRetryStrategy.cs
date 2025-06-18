using Microsoft.Extensions.Logging;
using Momento.Sdk.Exceptions;
using System;

namespace Momento.Sdk.Config.Retry
{
    /// <summary>
    /// Default implementation of <see cref="ISubscriptionRetryStrategy"/> that determines whether and when to resubscribe to a topic after an error occurs. Defaults to resubscribing after a delay of 500 milliseconds if the error is eligible for resubscription.
    /// Uses <see cref="DefaultSubscriptionRetryEligibilityStrategy"/> to determine if the error is eligible for resubscription.
    /// </summary>
    public class DefaultSubscriptionRetryStrategy : ISubscriptionRetryStrategy
    {
        private readonly ILogger _logger;
        /// <inheritdoc />
        public ISubscriptionRetryEligibilityStrategy SubscriptionRetryEligibilityStrategy { get; }
        /// <inheritdoc />
        public TimeSpan RetryDelayInterval { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSubscriptionRetryStrategy"/> class.
        /// </summary>
        /// <param name="loggerFactory">Logger factory for logging debug messages.</param>
        /// <param name="subscriptionRetryEligibilityStrategy">Strategy to determine if the error is eligible for resubscription. Defaults to <see cref="DefaultSubscriptionRetryEligibilityStrategy"/>.</param>
        /// <param name="retryDelayInterval">The interval after which resubscribing to a topic should be attempted. Defaults to 500 milliseconds.</param>
        public DefaultSubscriptionRetryStrategy(ILoggerFactory loggerFactory, ISubscriptionRetryEligibilityStrategy? subscriptionRetryEligibilityStrategy = null, TimeSpan? retryDelayInterval = null)
        {
            _logger = loggerFactory.CreateLogger<DefaultSubscriptionRetryStrategy>();
            SubscriptionRetryEligibilityStrategy = subscriptionRetryEligibilityStrategy ?? new DefaultSubscriptionRetryEligibilityStrategy(_logger);
            RetryDelayInterval = retryDelayInterval ?? TimeSpan.FromMilliseconds(500);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSubscriptionRetryStrategy"/> class.
        /// </summary>
        /// <param name="logger">Logger for logging debug messages.</param>
        /// <param name="subscriptionRetryEligibilityStrategy">Strategy to determine if the error is eligible for resubscription. Defaults to <see cref="DefaultSubscriptionRetryEligibilityStrategy"/>.</param>
        /// <param name="retryDelayInterval">The interval after which resubscribing to a topic should be attempted. Defaults to 500 milliseconds.</param>
        public DefaultSubscriptionRetryStrategy(ILogger logger, ISubscriptionRetryEligibilityStrategy? subscriptionRetryEligibilityStrategy = null, TimeSpan? retryDelayInterval = null)
        {
            _logger = logger;
            SubscriptionRetryEligibilityStrategy = subscriptionRetryEligibilityStrategy ?? new DefaultSubscriptionRetryEligibilityStrategy(logger);
            RetryDelayInterval = retryDelayInterval ?? TimeSpan.FromMilliseconds(500);
        }

        /// <inheritdoc />
        public int? DetermineWhenToResubscribe(SdkException exception)
        {
            if (!SubscriptionRetryEligibilityStrategy.IsEligibleForResubscribe(exception))
            {
                _logger.LogDebug("Subscription retry eligibility check: {ErrorCode} is not eligible for resubscribe.", exception.ErrorCode);
                return null; // Null indicates no resubscribe.
            }

            return (int)RetryDelayInterval.TotalMilliseconds;
        }
    }
}
