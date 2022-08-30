namespace Momento.Sdk.Incubating;

/// <summary>
/// Factory class used to instantiate the incubating Simple Cache Client.
///
/// Use this to enable preview features in the cache client.
/// </summary>
public class SimpleCacheClientFactory
{
    /// <summary>
    /// Instantiate an instance of the incubating Simple Cache Client.
    /// </summary>
    /// <param name="authToken">Momento JWT.</param>
    /// <param name="defaultTtlSeconds">Default time to live for the item in cache.</param>
    /// <param name="dataClientOperationTimeoutMilliseconds">Deadline (timeout) for communicating to the server. Defaults to 5 seconds.</param>
    /// <returns>An instance of the incubating Simple Cache Client.</returns>
    public static SimpleCacheClient CreateClient(string authToken, uint defaultTtlSeconds, uint? dataClientOperationTimeoutMilliseconds = null)
    {
        var simpleCacheClient = new Momento.Sdk.SimpleCacheClient(authToken, defaultTtlSeconds, dataClientOperationTimeoutMilliseconds);
        return new SimpleCacheClient(simpleCacheClient, authToken, defaultTtlSeconds, dataClientOperationTimeoutMilliseconds);
    }
}
