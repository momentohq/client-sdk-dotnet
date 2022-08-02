namespace IncubatingIntegrationTest;

public class TestBase
{
    protected readonly SimpleCacheClient client;
    protected readonly string cacheName = SimpleCacheClientFixture.CacheName;
    protected const uint defaultTtlSeconds = SimpleCacheClientFixture.DefaultTtlSeconds;

    public TestBase(SimpleCacheClientFixture fixture)
    {
        this.client = fixture.Client;
    }
}
