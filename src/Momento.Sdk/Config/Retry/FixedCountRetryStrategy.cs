using System.Net.NetworkInformation;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Momento.Sdk.Internal.Retry;

namespace Momento.Sdk.Config.Retry;

public class FixedCountRetryStrategy : IRetryStrategy
{
    public ILoggerFactory? LoggerFactory { get; }

    private ILogger _logger;
    private readonly IRetryEligibilityStrategy _eligibilityStrategy;

    public int MaxAttempts { get; }

    public FixedCountRetryStrategy(int maxAttempts, IRetryEligibilityStrategy? eligibilityStrategy = null, ILoggerFactory? loggerFactory = null)
    {
        LoggerFactory = loggerFactory;
        _logger = (loggerFactory ?? NullLoggerFactory.Instance).CreateLogger<FixedCountRetryStrategy>();
        _eligibilityStrategy = eligibilityStrategy ?? new DefaultRetryEligibilityStrategy(loggerFactory);
        MaxAttempts = maxAttempts;
    }

    public FixedCountRetryStrategy WithLoggerFactory(ILoggerFactory loggerFactory)
    {
        return new(MaxAttempts, _eligibilityStrategy.WithLoggerFactory(loggerFactory), loggerFactory);
    }

    IRetryStrategy IRetryStrategy.WithLoggerFactory(ILoggerFactory loggerFactory)
    {
        return WithLoggerFactory(loggerFactory);
    }

    public FixedCountRetryStrategy WithMaxAttempts(int maxAttempts)
    {
        return new(maxAttempts, _eligibilityStrategy, LoggerFactory);
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
