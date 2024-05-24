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
        AuthProvider = new EnvMomentoTokenProvider("MOMENTO_API_KEY");
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

public class TopicClientFixture : IDisposable
{
    public ITopicClient Client { get; private set; }
    public ICredentialProvider AuthProvider { get; private set; }


    public TopicClientFixture()
    {
        AuthProvider = new EnvMomentoTokenProvider("MOMENTO_API_KEY");
        Client = new TopicClient(TopicConfigurations.Laptop.latest(LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "hh:mm:ss ";
            });
            builder.AddFilter("Grpc.Net.Client", LogLevel.Error);
            builder.SetMinimumLevel(LogLevel.Information);
        })), AuthProvider);
    }

    public void Dispose()
    {
        Client.Dispose();
    }
}

/// <summary>
/// Register the fixture in xUnit.
/// </summary>
[CollectionDefinition("TopicClient")]
public class TopicClientCollection : ICollectionFixture<TopicClientFixture>
{

}

public class AuthClientFixture : IDisposable
{
    public IAuthClient Client { get; private set; }
    public ICredentialProvider AuthProvider { get; private set; }

    public AuthClientFixture()
    {
        AuthProvider = new EnvMomentoTokenProvider("MOMENTO_API_KEY");
        Client = new AuthClient(AuthConfigurations.Default.Latest(LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "hh:mm:ss ";
            });
            builder.AddFilter("Grpc.Net.Client", LogLevel.Error);
            builder.SetMinimumLevel(LogLevel.Information);
        })), AuthProvider);
    }

    public void Dispose()
    {
        Client.Dispose();
    }
}

/// <summary>
/// Register the fixture in xUnit.
/// </summary>
[CollectionDefinition("AuthClient")]
public class AuthClientCollection : ICollectionFixture<AuthClientFixture>
{

}
