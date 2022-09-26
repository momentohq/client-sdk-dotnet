namespace Momento.Sdk.Config.Retry;

public class FixedCountRetryStrategy : IRetryStrategy
{
    public int MaxAttempts { get; }
    //FixedCountRetryStrategy(retryableStatusCodes = DEFAULT_RETRYABLE_STATUS_CODES, maxAttempts = 3),
    public FixedCountRetryStrategy(int maxAttempts)
    {
        MaxAttempts = maxAttempts;
    }

    public FixedCountRetryStrategy WithMaxAttempts(int maxAttempts)
    {
        return new(maxAttempts);
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
