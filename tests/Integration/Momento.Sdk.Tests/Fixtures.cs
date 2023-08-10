using System;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;

namespace Momento.Sdk.Tests;

/// <summary>
/// A cache client fixture.
/// Use this when not testing client-building edge cases:
/// re-using the client drops overall integration test time down ~5X.
/// </summary>
public class CacheClientFixture : IDisposable
{
    public ICacheClient Client { get; private set; }
    public ICredentialProvider AuthProvider { get; private set; }
    public string CacheName { get; private set; }

    public TimeSpan DefaultTtl { get; private set; } = TimeSpan.FromSeconds(10);

    public CacheClientFixture()
    {
        AuthProvider = new EnvMomentoTokenProvider("TEST_AUTH_TOKEN");
        CacheName = $"dotnet-integration-{Utils.NewGuidString()}";
        Client = new TestCacheClient(Configurations.Laptop.Latest(LoggerFactory.Create(builder =>
                {
                    builder.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = true;
                        options.SingleLine = true;
                        options.TimestampFormat = "hh:mm:ss ";
                    });
                    builder.AddFilter("Grpc.Net.Client", LogLevel.Error);
                    builder.SetMinimumLevel(LogLevel.Information);
                })),
                AuthProvider, defaultTtl: DefaultTtl);
        Utils.CreateCacheForTest(Client, CacheName);
    }

    public void Dispose()
    {
        var result = Client.DeleteCacheAsync(CacheName).Result;
        Client.Dispose();
    }
}

/// <summary>
/// Register the fixture in xUnit.
/// </summary>
[CollectionDefinition("CacheClient")]
public class CacheClientCollection : ICollectionFixture<CacheClientFixture>
{

}
