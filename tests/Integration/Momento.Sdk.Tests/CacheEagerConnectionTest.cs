using Microsoft.Extensions.Logging;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Tests.Integration;

public class CacheEagerConnectionTest
{
    private readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
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
    private readonly TimeSpan defaultTtl = TimeSpan.FromMinutes(1);
    private readonly ICredentialProvider authProvider = new EnvMomentoTokenProvider("TEST_AUTH_TOKEN");

    [Fact]
    public void CacheClientConstructor_LazyConnection()
    {
        var config = Configurations.Laptop.Latest(loggerFactory);
        // just validating that we can construct the client
        var client = new CacheClient(config, authProvider, defaultTtl);
    }

    [Fact]
    public void CacheClientConstructor_EagerConnection_Success()
    {
        var config = Configurations.Laptop.Latest(loggerFactory);
        config = config.WithTransportStrategy(config.TransportStrategy.WithEagerConnectionTimeout(TimeSpan.FromSeconds(5)));
        // just validating that we can construct the client when the eager connection is successful
        var client = new CacheClient(config, authProvider, defaultTtl);
    }

    [Fact]
    public void CacheClientConstructor_WithChannelsAndMaxConn_Success()
    {
        var config = Configurations.Laptop.Latest(loggerFactory);
        IGrpcConfiguration grpcConfiguration = config.TransportStrategy.GrpcConfig;
        grpcConfiguration = grpcConfiguration.WithMinNumGrpcChannels(10);
        config = config.WithTransportStrategy(config.TransportStrategy
                                                    .WithGrpcConfig(grpcConfiguration)
                                                    .WithMaxConcurrentRequests(500));
        // still 500; clients shouldn't know we are doing 500/10 magic internally
        Assert.Equal(500, config.TransportStrategy.MaxConcurrentRequests);
        Assert.Equal(10, config.TransportStrategy.GrpcConfig.MinNumGrpcChannels);
        // just validating that we can construct the client wh
        var client = new CacheClient(config, authProvider, defaultTtl);
    }

    [Fact]
    public void CacheClientConstructor_EagerConnection_BadEndpoint()
    {
        var config = Configurations.Laptop.Latest(loggerFactory);
        config = config.WithTransportStrategy(config.TransportStrategy.WithEagerConnectionTimeout(TimeSpan.FromSeconds(2)));
        var authProviderWithBadCacheEndpoint = authProvider.WithCacheEndpoint("cache.cell-external-beta-1.prod.a.momentohq.com:65000");
        Console.WriteLine($"Hello developer!  We are about to run a test that verifies that the cache client is still operational even if our eager connection (ping) fails.  So you will see the test log a warning message about that.  It's expected, don't worry!");
        // validating that the constructor doesn't fail when the eager connection fails
        var client = new CacheClient(config, authProviderWithBadCacheEndpoint, defaultTtl);
    }
}
