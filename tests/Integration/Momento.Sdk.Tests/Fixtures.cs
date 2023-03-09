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
    public CacheClient Client { get; private set; }
    public ICredentialProvider AuthProvider { get; private set; }
    public string CacheName { get; private set; }

    public TimeSpan DefaultTtl { get; private set; } = TimeSpan.FromSeconds(10);

    public CacheClientFixture()
    {
        AuthProvider = new EnvMomentoTokenProvider("TEST_AUTH_TOKEN");
        CacheName = Environment.GetEnvironmentVariable("TEST_CACHE_NAME") ??
            throw new NullReferenceException("TEST_CACHE_NAME environment variable must be set.");
        Client = new(Configurations.Laptop.Latest(LoggerFactory.Create(builder =>
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

        var result = Client.CreateCacheAsync(CacheName).Result;
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
