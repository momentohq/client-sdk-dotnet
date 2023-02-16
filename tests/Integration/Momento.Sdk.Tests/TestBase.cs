using Momento.Sdk.Auth;
using Momento.Sdk.Tests;

namespace Momento.Sdk.Tests;

public class TestBase
{
    protected readonly ICredentialProvider authProvider;
    protected readonly string cacheName;
    protected TimeSpan defaultTtl;
    protected SimpleCacheClient client;

    public TestBase(SimpleCacheClientFixture fixture)
    {
        this.client = fixture.Client;
        this.cacheName = fixture.CacheName;
        this.authProvider = fixture.AuthProvider;
        this.defaultTtl = fixture.DefaultTtl;
    }
}
