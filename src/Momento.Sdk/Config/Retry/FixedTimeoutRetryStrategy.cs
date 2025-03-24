using System;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Internal.Retry;

namespace Momento.Sdk.Config.Retry;

/// <summary>
/// TODO
/// </summary>
public class FixedTimeoutRetryStrategy : IRetryStrategy
{
    private ILoggerFactory _loggerFactory;
    private ILogger _logger;
    private readonly IRetryEligibilityStrategy _eligibilityStrategy;
    private readonly TimeSpan _retryDelayIntervalMillis;
    private readonly TimeSpan _responseDataReceivedTimeoutMillis;

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="eligibilityStrategy"></param>
    /// <param name="retryDelayIntervalMillis"></param>
    /// <param name="responseDataReceivedTimeoutMillis"></param>
    public FixedTimeoutRetryStrategy(ILoggerFactory loggerFactory, IRetryEligibilityStrategy? eligibilityStrategy = null, TimeSpan? retryDelayIntervalMillis = null, TimeSpan? responseDataReceivedTimeoutMillis = null)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<FixedCountRetryStrategy>();
        _eligibilityStrategy = eligibilityStrategy ?? new DefaultRetryEligibilityStrategy(loggerFactory);
        _retryDelayIntervalMillis = retryDelayIntervalMillis ?? TimeSpan.FromMilliseconds(100);
        _responseDataReceivedTimeoutMillis = responseDataReceivedTimeoutMillis ?? TimeSpan.FromMilliseconds(1000);
    }

    /// <inheritdoc/>
    public int? DetermineWhenToRetryRequest<TRequest>(Status grpcStatus, TRequest grpcRequest, int attemptNumber) where TRequest : class
    {
        _logger.LogDebug($"Determining whether request is eligible for retry; status code: {grpcStatus.StatusCode}, request type: {grpcRequest.GetType()}, attemptNumber: {attemptNumber}");
        if (!_eligibilityStrategy.IsEligibleForRetry(grpcStatus, grpcRequest))
        {
            return null;
        }
        _logger.LogDebug($"Request is eligible for retry (attempt {attemptNumber}), retrying after {_retryDelayIntervalMillis} +/- jitter.");
        return AddJitter((int)_retryDelayIntervalMillis.TotalMilliseconds);
    }

    private int AddJitter(int whenToRetry)
    {
        return Convert.ToInt32((0.2 * new Random().NextDouble() + 0.9) * whenToRetry);
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

        var other = (FixedTimeoutRetryStrategy)obj;
        return _retryDelayIntervalMillis.Equals(other._retryDelayIntervalMillis) &&
            _responseDataReceivedTimeoutMillis.Equals(other._responseDataReceivedTimeoutMillis) &&
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
        return _responseDataReceivedTimeoutMillis.TotalMilliseconds;
    }
}