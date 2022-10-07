using System.Net.NetworkInformation;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Momento.Sdk.Internal.Retry;

namespace Momento.Sdk.Config.Retry;

public class FixedCountRetryStrategy : IRetryStrategy
{
    private ILoggerFactory _loggerFactory;
    private ILogger _logger;
    private readonly IRetryEligibilityStrategy _eligibilityStrategy;

    public int MaxAttempts { get; }

    public FixedCountRetryStrategy(ILoggerFactory loggerFactory, int maxAttempts, IRetryEligibilityStrategy? eligibilityStrategy = null)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<FixedCountRetryStrategy>();
        _eligibilityStrategy = eligibilityStrategy ?? new DefaultRetryEligibilityStrategy(loggerFactory);
        MaxAttempts = maxAttempts;
    }

    public FixedCountRetryStrategy WithMaxAttempts(int maxAttempts)
    {
        return new(_loggerFactory, maxAttempts, _eligibilityStrategy);
    }

    public int? DetermineWhenToRetryRequest<TRequest>(Status grpcStatus, TRequest grpcRequest, int attemptNumber) where TRequest : class
    {
        _logger.LogDebug($"Determining whether request is eligible for retry; status code: {grpcStatus.StatusCode}, request type: {grpcRequest.GetType()}, attemptNumber: {attemptNumber}, maxAttempts: {MaxAttempts}");
        if (! _eligibilityStrategy.IsEligibleForRetry(grpcStatus, grpcRequest))
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

}
