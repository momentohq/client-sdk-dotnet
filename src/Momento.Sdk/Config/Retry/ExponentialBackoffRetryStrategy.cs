using System;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Internal.Retry;

namespace Momento.Sdk.Config.Retry;

/// <summary>
/// TODO
/// </summary>
public class ExponentialBackoffRetryStrategy : IRetryStrategy
{
    /// <summary>
    /// TODO
    /// </summary>
    public static readonly double DEFAULT_INITIAL_DELAY_MS = 0.5;
    /// <summary>
    /// TODO
    /// </summary>
    public static readonly double DEFAULT_GROWTH_FACTOR = 2;
    /// <summary>
    /// TODO
    /// </summary>

    public static readonly double DEFAULT_MAX_BACKOFF_MS = 1000;
    private ILoggerFactory _loggerFactory;
    private ILogger _logger;
    private readonly IRetryEligibilityStrategy _eligibilityStrategy;
    
    private readonly double _initialDelayMillis;
    private readonly double _growthFactor;
    private readonly double _maxBackoffMillis;

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="eligibilityStrategy"></param>
    /// <param name="initialDelayMillis"></param>
    /// <param name="maxBackoffMillis"></param>
    public ExponentialBackoffRetryStrategy(ILoggerFactory loggerFactory, IRetryEligibilityStrategy? eligibilityStrategy = null, double? initialDelayMillis = null, double? maxBackoffMillis = null) 
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<ExponentialBackoffRetryStrategy>();
        _eligibilityStrategy = eligibilityStrategy ?? new DefaultRetryEligibilityStrategy(loggerFactory);
        _initialDelayMillis = initialDelayMillis ?? DEFAULT_INITIAL_DELAY_MS;
        _growthFactor = DEFAULT_GROWTH_FACTOR;
        _maxBackoffMillis = maxBackoffMillis ?? DEFAULT_MAX_BACKOFF_MS;
    }

    /// <inheritdoc/>
    public int? DetermineWhenToRetryRequest<TRequest>(Status grpcStatus, TRequest grpcRequest, int attemptNumber) where TRequest : class
    {
        _logger.LogDebug($"Determining whether request is eligible for retry; status code: {grpcStatus.StatusCode}, request type: {grpcRequest.GetType()}, attemptNumber: {attemptNumber}");
        if (!_eligibilityStrategy.IsEligibleForRetry(grpcStatus, grpcRequest))
        {
            return null;
        }

        double baseDelay = computeBaseDelay(attemptNumber);
        double previousBaseDelay = computePreviousBaseDelay(baseDelay);
        double maxDelay = previousBaseDelay * 3;
        double jitteredDelay = randomInRange(baseDelay, maxDelay);
        int jitteredDelayMs = Convert.ToInt32(jitteredDelay);

        _logger.LogDebug($"Request is eligible for retry (attempt {attemptNumber}), retrying after {jitteredDelayMs}ms.");
        return jitteredDelayMs;
    }

    private double computeBaseDelay(int attemptNumber) {
      if (attemptNumber <= 0) {
        return _initialDelayMillis;
      }

        double multiplier = Math.Pow(_growthFactor, attemptNumber);
        double baseDelay = _initialDelayMillis * multiplier;
        return Math.Min(baseDelay, _maxBackoffMillis);
    }

    private double computePreviousBaseDelay(double currentBaseDelay) {
      if (currentBaseDelay == _initialDelayMillis) {
        return _initialDelayMillis;
      }

      return currentBaseDelay / _growthFactor;
    }

    private double randomInRange(double min, double max) {
      if (min >= max) {
        return min;
      }
      return min + (new Random().NextDouble() * (max - min));
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
        return _initialDelayMillis.Equals(other._initialDelayMillis) &&
            _growthFactor.Equals(other._growthFactor) &&
            _maxBackoffMillis.Equals(other._maxBackoffMillis) &&
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

    /// <inheritdoc/>
    public double? GetResponseDataReceivedTimeoutMillis()
    {
        return null;
    }
}