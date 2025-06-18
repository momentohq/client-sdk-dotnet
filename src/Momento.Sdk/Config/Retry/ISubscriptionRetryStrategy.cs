using Momento.Sdk.Exceptions;
using System;

namespace Momento.Sdk.Config.Retry;

/// <summary>
/// Defines a contract for whether and when to resubscribe to a topic after an error occurs.
/// </summary>
public interface ISubscriptionRetryStrategy
{
    /// <summary>
    /// The strategy used to determine whether to resubscribe to a topic after an error occurs.
    /// </summary>
    ISubscriptionRetryEligibilityStrategy SubscriptionRetryEligibilityStrategy { get; }

    /// <summary>
    /// The interval after which resubscribing to a topic should be attempted.
    /// </summary>
    TimeSpan RetryDelayInterval { get; }

    /// <summary>
    /// Calculates when to resubscribe to a topic based on the type of sdk exception.
    /// </summary>
    /// <param name="exception"></param>
    /// <returns>Returns number of milliseconds after which the resubscribe should be attempted, 
    /// or <see langword="null"/> if the request should not be retried.</returns>
    public int? DetermineWhenToResubscribe(SdkException exception);
}
