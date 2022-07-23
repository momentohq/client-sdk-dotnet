namespace MomentoSdk.Incubating;

public class SimpleCacheClientFactory
{
    public static SimpleCacheClient Get(string authToken, uint defaultTtlSeconds, uint? dataClientOperationTimeoutMilliseconds = null)
    {
        var simpleCacheClient = new MomentoSdk.SimpleCacheClient(authToken, defaultTtlSeconds, dataClientOperationTimeoutMilliseconds);
        return new SimpleCacheClient(simpleCacheClient);
    }
}
