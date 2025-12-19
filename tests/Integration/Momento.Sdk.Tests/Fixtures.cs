using Microsoft.Extensions.Logging;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;

namespace Momento.Sdk.Tests.Integration;

/// <summary>
/// A cache client fixture.
/// Use this when not testing client-building edge cases:
/// re-using the client drops overall integration test time down ~5X.
/// </summary>
public class CacheClientFixture : IDisposable
{
    public ICacheClient Client { get; }
    public ICacheClient ClientWithConsistentReads { get; }
    public ICacheClient ClientWithBalancedReads { get; }
    public ICredentialProvider AuthProvider { get; }
    public string CacheName { get; }

    public TimeSpan DefaultTtl { get; } = TimeSpan.FromSeconds(10);

    public CacheClientFixture()
    {
        AuthProvider = new EnvMomentoTokenProvider("V1_API_KEY");

        // Enable consistent reads if the CONSISTENT_READS env var is set to anything
        var consistentReads = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CONSISTENT_READS"));

        var config = Configurations.Laptop.Latest(LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "hh:mm:ss ";
            });
            builder.AddFilter("Grpc.Net.Client", LogLevel.Error);
            builder.SetMinimumLevel(LogLevel.Information);
        }));
        var configWithConsistentReads = config.WithReadConcern(ReadConcern.Consistent);

        ClientWithBalancedReads = new CacheClient(config, AuthProvider, defaultTtl: DefaultTtl);
        ClientWithConsistentReads = new CacheClient(configWithConsistentReads, AuthProvider, defaultTtl: DefaultTtl);
        Client = consistentReads ? ClientWithConsistentReads : ClientWithBalancedReads;

        CacheName = $"dotnet-integration-{Utils.NewGuidString()}";

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
        AuthProvider = new EnvMomentoTokenProvider("V1_API_KEY");
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
        AuthProvider = new EnvMomentoTokenProvider("V1_API_KEY");
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

/// <summary>
/// A cache client fixture using api key v2.
/// Use this when not testing client-building edge cases:
/// re-using the client drops overall integration test time down ~5X.
/// </summary>
public class CacheClientApiKeyV2Fixture : IDisposable
{
    public ICacheClient Client { get; }
    public ICredentialProvider AuthProvider { get; }
    public string CacheName { get; }

    public TimeSpan DefaultTtl { get; } = TimeSpan.FromSeconds(10);

    public CacheClientApiKeyV2Fixture()
    {
        AuthProvider = new EnvMomentoV2TokenProvider();

        var config = Configurations.Laptop.Latest(LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "hh:mm:ss ";
            });
            builder.AddFilter("Grpc.Net.Client", LogLevel.Error);
            builder.SetMinimumLevel(LogLevel.Information);
        }));

        Client = new CacheClient(config, AuthProvider, defaultTtl: DefaultTtl);
        CacheName = $"dotnet-integration-v2-{Utils.NewGuidString()}";

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
[CollectionDefinition("CacheClientApiKeyV2")]
public class CacheClientApiKeyV2Collection : ICollectionFixture<CacheClientApiKeyV2Fixture>
{

}

public class TopicClientApiKeyV2Fixture : IDisposable
{
    public ITopicClient Client { get; private set; }
    public ICredentialProvider AuthProvider { get; private set; }


    public TopicClientApiKeyV2Fixture()
    {
        AuthProvider = new EnvMomentoV2TokenProvider();
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
[CollectionDefinition("TopicClientApiKeyV2")]
public class TopicClientApiKeyV2Collection : ICollectionFixture<TopicClientApiKeyV2Fixture>
{

}
