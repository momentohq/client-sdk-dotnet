using System;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Internal.Retry;

namespace Momento.Sdk.Config.Retry;

/// <summary>
/// The most basic retry strategy; simply retries an eligible failed request the specified number of times.
/// </summary>
public class FixedCountRetryStrategy : IRetryStrategy
{
    private ILoggerFactory _loggerFactory;
    private ILogger _logger;
    private readonly IRetryEligibilityStrategy _eligibilityStrategy;

    /// <summary>
    /// The maximum number of attempts that should be made before giving up on an individual request.
    /// </summary>
    public int MaxAttempts { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="maxAttempts">The maximum number of attempts that should be made before giving up on an individual request.</param>
    /// <param name="eligibilityStrategy">The strategy used to determine whether a particular failed request is eligible for retry.</param>
    public FixedCountRetryStrategy(ILoggerFactory loggerFactory, int maxAttempts, IRetryEligibilityStrategy? eligibilityStrategy = null)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<FixedCountRetryStrategy>();
        _eligibilityStrategy = eligibilityStrategy ?? new DefaultRetryEligibilityStrategy(loggerFactory);
        MaxAttempts = maxAttempts;
    }

    /// <summary>
    /// Copy constructor that updates maxAttempts
    /// </summary>
    /// <param name="maxAttempts"></param>
    /// <returns>A new FixedCountRetryStrategy instance with updated maximum number of attempts</returns>
    public FixedCountRetryStrategy WithMaxAttempts(int maxAttempts)
    {
        return new(_loggerFactory, maxAttempts, _eligibilityStrategy);
    }

    /// <inheritdoc/>
    public int? DetermineWhenToRetryRequest<TRequest>(Status grpcStatus, TRequest grpcRequest, int attemptNumber, DateTime overallDeadline) where TRequest : class
    {
        _logger.LogDebug($"Determining whether request is eligible for retry; status code: {grpcStatus.StatusCode}, request type: {grpcRequest.GetType()}, attemptNumber: {attemptNumber}, maxAttempts: {MaxAttempts}");
        if (!_eligibilityStrategy.IsEligibleForRetry(grpcStatus, grpcRequest))
        {
            return null;
        }
        if (attemptNumber > MaxAttempts)
        {
            _logger.LogDebug($"Exceeded max retry count ({MaxAttempts})");
            return null;
        }
        _logger.LogDebug($"Request is eligible for retry (attempt {attemptNumber} of {MaxAttempts}, retrying immediately.");
        return 0;
    }

    /// <summary>
    /// Test equality by value.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }

        var other = (FixedCountRetryStrategy)obj;
        return MaxAttempts.Equals(other.MaxAttempts) &&
            _loggerFactory.Equals(other._loggerFactory) &&
            _eligibilityStrategy.Equals(other._eligibilityStrategy);
    }

    /// <summary>
    /// Trivial hash code implementation.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
