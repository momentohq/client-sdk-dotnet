using Microsoft.Extensions.Logging;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Tests.Integration.Cache;

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
    private readonly ICredentialProvider authProvider = new EnvMomentoTokenProvider("MOMENTO_API_KEY");

    [Fact]
    public void CacheClientConstructor_LazyConnection()
    {
        var config = Configurations.Laptop.Latest(loggerFactory);
        // just validating that we can construct the client
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
                                                    .WithMaxConcurrentRequests(2));

        // just validating that we can construct the client wh
        var client = new CacheClient(config, authProvider, defaultTtl);
        // still 2; clients shouldn't know we are doing 2/10 magic internally
        Assert.Equal(2, config.TransportStrategy.MaxConcurrentRequests);
        Assert.Equal(10, config.TransportStrategy.GrpcConfig.MinNumGrpcChannels);
    }

    [Fact]
    public async void CacheClientCreate_EagerConnection_BadEndpoint()
    {
        var config = Configurations.Laptop.Latest(loggerFactory);
        var authProviderWithBadCacheEndpoint = authProvider.WithCacheEndpoint("cache.cell-external-beta-1.prod.a.momentohq.com:65000");
        Console.WriteLine($"Hello developer!  We are about to run a test that verifies that the cache client is still operational even if our eager connection (ping) fails.  So you will see the test log a warning message about that.  It's expected, don't worry!");

        await Assert.ThrowsAsync<ConnectionException>(async () => await CacheClient.CreateAsync(config, authProviderWithBadCacheEndpoint, defaultTtl, TimeSpan.FromSeconds(2)));
    }
}
