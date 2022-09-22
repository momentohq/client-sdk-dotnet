namespace Momento.Sdk.Config.Retry;

public class FixedCountRetryStrategy : IRetryStrategy
{
    public int MaxAttempts { get; set; }
    //FixedCountRetryStrategy(retryableStatusCodes = DEFAULT_RETRYABLE_STATUS_CODES, maxAttempts = 3),
    public FixedCountRetryStrategy(int maxAttempts)
    {
        MaxAttempts = maxAttempts;
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
