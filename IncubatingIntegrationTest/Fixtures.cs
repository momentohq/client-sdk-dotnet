namespace IncubatingIntegrationTest;

/// <summary>
/// A cache client fixture.
/// Use this when not testing client-building edge cases:
/// re-using the client drops overall integration test time down ~5X.
/// </summary>
public class SimpleCacheClientFixture : IDisposable
{
    public SimpleCacheClient Client { get; private set; }
    public string AuthToken { get; private set; }

    // TODO: this cache was specially created for this.
    // We will not programmatically create integration test cache for now.
    public const string CacheName = "client-sdk-csharp";
    public const uint DefaultTtlSeconds = 10;

    public SimpleCacheClientFixture()
    {
        AuthToken = Environment.GetEnvironmentVariable("TEST_AUTH_TOKEN") ??
            throw new NullReferenceException("TEST_AUTH_TOKEN environment variable must be set.");
        Client = SimpleCacheClientFactory.Get(AuthToken, defaultTtlSeconds: DefaultTtlSeconds);

        /*
        try
        {
            Client.CreateCache(CacheName);
        }
        catch (AlreadyExistsException)
        {
        }*/

        if (!Client.ListCaches().Caches.Contains(new MomentoSdk.Responses.CacheInfo(CacheName)))
        {
            throw new NotFoundException($"Cache {CacheName} not found. This is assumed to exist and have a BlobDb partition.");
        }
    }

    public void Dispose()
    {
        // TODO cleanup
        //Client.DeleteCache(CacheName);
        Client.Dispose();
    }
}

/// <summary>
/// Register the fixture in xUnit.
/// </summary>
[CollectionDefinition("SimpleCacheClient")]
public class SimpleCacheClientCollection : ICollectionFixture<SimpleCacheClientFixture>
{

}
