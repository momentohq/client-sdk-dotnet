using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Internal.Retry;
using System;

namespace Momento.Sdk.Config.Retry;

/// <summary>
/// Retry failed requests up until the client's timeout has been reached.
/// Retries are attempted at fixed intervals (retryDelayIntervalMillis) +/- jitter.
/// Retry attempts time out after responseDataReceivedTimeoutMillis to be able to 
/// allow multiple retries before the client's timeout is reached.
/// </summary>
public class FixedTimeoutRetryStrategy : IRetryStrategy
{
    /// <summary>
    /// The default retry delay interval. Schedules retry attempts to be 100ms later +/- jitter.
    /// </summary>
    public static readonly TimeSpan DEFAULT_RETRY_DELAY_INTERVAL = TimeSpan.FromMilliseconds(100);
    /// <summary>
    /// The default timeout for retry attempts.
    /// </summary>
    public static readonly TimeSpan DEFAULT_RESPONSE_DATA_RECEIVED_TIMEOUT = TimeSpan.FromMilliseconds(1000);

    private readonly ILogger _logger;
    private readonly IRetryEligibilityStrategy _eligibilityStrategy;
    private readonly TimeSpan _retryDelayInterval;
    private readonly TimeSpan _responseDataReceivedTimeout;

    /// <summary>
    /// Constructor for the FixedTimeoutRetryStrategy.
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="eligibilityStrategy"></param>
    /// <param name="retryDelayInterval">Amount of time between retry attempts.</param>
    /// <param name="responseDataReceivedTimeout">How long to wait for a retry attempt to succeed or timeout.</param>
    public FixedTimeoutRetryStrategy(ILoggerFactory loggerFactory, IRetryEligibilityStrategy? eligibilityStrategy = null, TimeSpan? retryDelayInterval = null, TimeSpan? responseDataReceivedTimeout = null)
    {
        _logger = loggerFactory.CreateLogger<FixedTimeoutRetryStrategy>();
        _eligibilityStrategy = eligibilityStrategy ?? new DefaultRetryEligibilityStrategy(loggerFactory);
        _retryDelayInterval = retryDelayInterval ?? DEFAULT_RETRY_DELAY_INTERVAL;
        _responseDataReceivedTimeout = responseDataReceivedTimeout ?? DEFAULT_RESPONSE_DATA_RECEIVED_TIMEOUT;
    }

    /// <inheritdoc/>
    public int? DetermineWhenToRetryRequest<TRequest>(Status grpcStatus, TRequest grpcRequest, int attemptNumber) where TRequest : class
    {
        _logger.LogDebug($"Determining whether request is eligible for retry; status code: {grpcStatus.StatusCode}, request type: {grpcRequest.GetType()}, attemptNumber: {attemptNumber}");
        if (!_eligibilityStrategy.IsEligibleForRetry(grpcStatus, grpcRequest))
        {
            return null;
        }
        _logger.LogDebug($"Request is eligible for retry (attempt {attemptNumber}), retrying after {_retryDelayInterval.TotalMilliseconds}ms +/- jitter.");
        return AddJitter((int)_retryDelayInterval.TotalMilliseconds);
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
        return _retryDelayInterval.Equals(other._retryDelayInterval) &&
            _responseDataReceivedTimeout.Equals(other._responseDataReceivedTimeout) &&
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
        return _responseDataReceivedTimeout.TotalMilliseconds;
    }
}
