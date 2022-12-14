using Microsoft.Extensions.Logging;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;

namespace Momento.Sdk.Tests.Integration;

public class SimpleCacheEagerConnectionTest
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
    public void SimpleCacheClientConstructor_LazyConnection()
    {
        var config = Configurations.Laptop.Latest(loggerFactory);
        // just validating that we can construct the client
        var client = new SimpleCacheClient(config, authProvider, defaultTtl);
    }
    
    [Fact]
    public void SimpleCacheClientConstructor_EagerConnection_Success()
    {
        var config = Configurations.Laptop.Latest(loggerFactory);
        config = config.WithTransportStrategy(config.TransportStrategy.WithEagerConnection());
        // just validating that we can construct the client when the eager connection is successful
        var client = new SimpleCacheClient(config, authProvider, defaultTtl);
    }
    
    
    [Fact]
    public void SimpleCacheClientConstructor_EagerConnection_BadEndpoint()
    {
        var config = Configurations.Laptop.Latest(loggerFactory);
        config = config.WithTransportStrategy(config.TransportStrategy.WithEagerConnection());
        var authProviderWithBadCacheEndpoint = authProvider.WithCacheEndpoint("localhost:65000");
        // validating that the constructor doesn't fail when the eager connection fails
        var client = new SimpleCacheClient(config, authProviderWithBadCacheEndpoint, defaultTtl);
    }
}