namespace MomentoIntegrationTest;

/// <summary>
/// A cache client fixture.
/// Use this when not testing client-building edge cases:
/// re-using the client drops overall integration test time down ~5X.
/// </summary>
public class SimpleCacheClientFixture : IDisposable
{
    public SimpleCacheClient Client { get; private set; }
    public string AuthToken { get; private set; }
    public string CacheName { get; private set; }

    public const uint DefaultTtlSeconds = 10;

    public SimpleCacheClientFixture()
    {
        AuthToken = Environment.GetEnvironmentVariable("TEST_AUTH_TOKEN") ??
            throw new NullReferenceException("TEST_AUTH_TOKEN environment variable must be set.");
        CacheName = Environment.GetEnvironmentVariable("TEST_CACHE_NAME") ??
            throw new NullReferenceException("TEST_CACHE_NAME environment variable must be set.");
        Client = new(AuthToken, defaultTtlSeconds: DefaultTtlSeconds);

        try
        {
            Client.CreateCache(CacheName);
        }
        catch (AlreadyExistsException)
        {
        }
    }

    public void Dispose()
    {
        Client.DeleteCache(CacheName);
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
