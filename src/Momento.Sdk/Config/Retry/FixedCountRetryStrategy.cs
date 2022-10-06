using Microsoft.Extensions.Logging;

namespace Momento.Sdk.Config.Retry;

public class FixedCountRetryStrategy : IRetryStrategy
{
    public ILoggerFactory? LoggerFactory { get; }
    public int MaxAttempts { get; }

    //FixedCountRetryStrategy(retryableStatusCodes = DEFAULT_RETRYABLE_STATUS_CODES, maxAttempts = 3),
    public FixedCountRetryStrategy(int maxAttempts, ILoggerFactory? loggerFactory = null)
    {
        LoggerFactory = loggerFactory;
        MaxAttempts = maxAttempts;
    }

    public FixedCountRetryStrategy WithLoggerFactory(ILoggerFactory loggerFactory)
    {
        return new(MaxAttempts, loggerFactory);
    }

    IRetryStrategy IRetryStrategy.WithLoggerFactory(ILoggerFactory loggerFactory)
    {
        return WithLoggerFactory(loggerFactory);
    }

    public FixedCountRetryStrategy WithMaxAttempts(int maxAttempts)
    {
        return new(maxAttempts, LoggerFactory);
    }

    public int? DetermineWhenToRetryRequest(IGrpcResponse grpcResponse, IGrpcRequest grpcRequest, int attemptNumber)
    {
        if (attemptNumber > MaxAttempts)
        {
            return null;
        }
        return 0;
    }
}
