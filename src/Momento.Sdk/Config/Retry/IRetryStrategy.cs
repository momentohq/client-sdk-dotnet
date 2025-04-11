using Grpc.Core;
using System;

namespace Momento.Sdk.Config.Retry;

/// <summary>
/// Defines a contract for how and when to retry a request
/// </summary>
public interface IRetryStrategy
{
    /// <summary>
    /// Calculates whether or not to retry a request based on the type of request and number of attempts.
    /// </summary>
    /// <param name="grpcStatus"></param>
    /// <param name="grpcRequest"></param>
    /// <param name="attemptNumber"></param>
    /// <param name="overallDeadline"></param>
    /// <returns>Returns number of milliseconds after which the request should be retried, or <see langword="null"/> if the request should not be retried.</returns>
    public int? DetermineWhenToRetryRequest<TRequest>(Status grpcStatus, TRequest grpcRequest, int attemptNumber, DateTime? overallDeadline = null) where TRequest : class;

    /// <summary>
    /// Calculates the deadline for a retry attempt, which may be different from the overall deadline.
    /// </summary>
    /// <param name="callOptions"></param>
    /// <param name="overallDeadline"></param>
    /// <returns></returns>
    public CallOptions CalculateRetryDeadline(CallOptions callOptions, DateTime overallDeadline);
}
