using Microsoft.Extensions.Logging;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Transport;

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

#if NET6_0_OR_GREATER
public class TopicClientFixture : IDisposable
{
    public ITopicClient Client { get; private set; }
    public ICredentialProvider AuthProvider { get; private set; }

    
    public TopicClientFixture()
    {
        AuthProvider = new EnvMomentoTokenProvider("TEST_AUTH_TOKEN");
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "hh:mm:ss ";
            });
            builder.AddFilter("Grpc.Net.Client", LogLevel.Error);
            builder.SetMinimumLevel(LogLevel.Information);
        });
        var transportStrategy = new StaticTransportStrategy(
            loggerFactory: loggerFactory,
            maxConcurrentRequests: 200, // max of 2 connections https://github.com/momentohq/client-sdk-dotnet/issues/460
            grpcConfig: new StaticGrpcConfiguration(deadline: TimeSpan.FromMilliseconds(15000))
        );
        var config = new TopicConfiguration(loggerFactory, transportStrategy);
        Client = new TopicClient(config, AuthProvider);
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
#endif
