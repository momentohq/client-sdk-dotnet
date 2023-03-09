using Microsoft.Extensions.Logging;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;

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
    public void CacheClientConstructor_EagerConnection_BadEndpoint()
    {
        var config = Configurations.Laptop.Latest(loggerFactory);
        config = config.WithTransportStrategy(config.TransportStrategy.WithEagerConnectionTimeout(TimeSpan.FromSeconds(2)));
        var authProviderWithBadCacheEndpoint = authProvider.WithCacheEndpoint("cache.cell-external-beta-1.prod.a.momentohq.com:65000");
        // validating that the constructor doesn't fail when the eager connection fails
        var client = new CacheClient(config, authProviderWithBadCacheEndpoint, defaultTtl);
    }
}