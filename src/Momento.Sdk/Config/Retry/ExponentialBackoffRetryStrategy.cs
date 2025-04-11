using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Internal.Retry;
using System;

namespace Momento.Sdk.Config.Retry;

/// <summary>
/// Retry strategy that uses exponential backoff with decorrelated jitter.
/// - The first retry has a fixed delay of `initialDelayMillis`
/// - Backoff for subsequent retries is calculated as `initialDelayMillis * 2^attemptNumber`
/// - Subsequent retries have a delay that is a random value between 
/// the current backoff and 3 times the previous backoff, with the 
/// current backoff capped at `maxBackoffMillis`
/// </summary>
public class ExponentialBackoffRetryStrategy : IRetryStrategy
{
    /// <summary>
    /// Default initial delay for the first retry (in milliseconds)
    /// </summary>
    public static readonly TimeSpan DEFAULT_INITIAL_DELAY = TimeSpan.FromMilliseconds(0.5);
    /// <summary>
    /// Default growth factor for exponential backoff
    /// </summary>
    public static readonly double DEFAULT_GROWTH_FACTOR = 2;
    /// <summary>
    /// Default maximum delay to cap the exponential growth (in milliseconds)
    /// </summary>
    public static readonly TimeSpan DEFAULT_MAX_BACKOFF = TimeSpan.FromMilliseconds(1000);

    private ILogger _logger;
    private readonly IRetryEligibilityStrategy _eligibilityStrategy;

    private readonly TimeSpan _initialDelay;
    private readonly double _growthFactor;
    private readonly TimeSpan _maxBackoff;
    private readonly Random _random = new Random();

    /// <summary>
    /// Constructor for ExponentialBackoffRetryStrategy
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="eligibilityStrategy"></param>
    /// <param name="initialDelay"></param>
    /// <param name="maxBackoff"></param>
    public ExponentialBackoffRetryStrategy(ILoggerFactory loggerFactory, IRetryEligibilityStrategy? eligibilityStrategy = null, TimeSpan? initialDelay = null, TimeSpan? maxBackoff = null)
    {
        _logger = loggerFactory.CreateLogger<ExponentialBackoffRetryStrategy>();
        _eligibilityStrategy = eligibilityStrategy ?? new DefaultRetryEligibilityStrategy(loggerFactory);
        _initialDelay = initialDelay ?? DEFAULT_INITIAL_DELAY;
        _growthFactor = DEFAULT_GROWTH_FACTOR;
        _maxBackoff = maxBackoff ?? DEFAULT_MAX_BACKOFF;
    }

    /// <inheritdoc/>
    public int? DetermineWhenToRetryRequest<TRequest>(Status grpcStatus, TRequest grpcRequest, int attemptNumber) where TRequest : class
    {
        _logger.LogDebug($"Determining whether request is eligible for retry; status code: {grpcStatus.StatusCode}, request type: {grpcRequest.GetType()}, attemptNumber: {attemptNumber}");
        if (!_eligibilityStrategy.IsEligibleForRetry(grpcStatus, grpcRequest))
        {
            return null;
        }

        var baseDelay = ComputeBaseDelay(attemptNumber);
        var previousBaseDelay = ComputePreviousBaseDelay(baseDelay);
        var maxDelay = previousBaseDelay * 3;
        var jitteredDelay = RandomInRange(baseDelay, maxDelay);
        var jitteredDelayMs = Convert.ToInt32(jitteredDelay);

        _logger.LogDebug($"Request is eligible for retry (attempt {attemptNumber}), retrying after {jitteredDelayMs}ms.");
        return jitteredDelayMs;
    }

    private double ComputeBaseDelay(int attemptNumber)
    {
        if (attemptNumber <= 0)
        {
            return _initialDelay.TotalMilliseconds;
        }
        var multiplier = Math.Pow(_growthFactor, attemptNumber);
        var baseDelay = _initialDelay.TotalMilliseconds * multiplier;
        return Math.Min(baseDelay, _maxBackoff.TotalMilliseconds);
    }

    private double ComputePreviousBaseDelay(double currentBaseDelay)
    {
        if (currentBaseDelay == _initialDelay.TotalMilliseconds)
        {
            return _initialDelay.TotalMilliseconds;
        }
        return currentBaseDelay / _growthFactor;
    }

    private double RandomInRange(double min, double max)
    {
        if (min >= max)
        {
            return min;
        }
        return min + (_random.NextDouble() * (max - min));
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

        var other = (ExponentialBackoffRetryStrategy)obj;
        return _initialDelay.Equals(other._initialDelay) &&
            _growthFactor.Equals(other._growthFactor) &&
            _maxBackoff.Equals(other._maxBackoff) &&
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
