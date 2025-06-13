using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Config.Retry
{
    /// <summary>
    /// Used to determine whether to resubscribe to a topic after an error occurs.
    /// </summary>
    public interface ISubscriptionRetryEligibilityStrategy
    {
        /// <summary>
        /// Given an sdk exception, returns <code>true</code> if the request is eligible for
        /// resubscribing to a topic, <code>false</code> otherwise.
        /// </summary>
        /// <param name="exception">The exception that occurred</param>
        public bool IsEligibleForResubscribe(SdkException exception);
    }
}

