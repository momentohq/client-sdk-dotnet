namespace Momento.Sdk.Incubating.Tests.Integration;

public class TestBase
{
    protected readonly SimpleCacheClient client;
    protected readonly string cacheName;
    protected const uint defaultTtlSeconds = SimpleCacheClientFixture.DefaultTtlSeconds;

    public TestBase(SimpleCacheClientFixture fixture)
    {
        this.client = fixture.Client;
        this.cacheName = fixture.CacheName;
    }
}
