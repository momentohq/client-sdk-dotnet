using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Retry;

namespace Momento.Sdk.Tests.Integration.Retry;

[Collection("Retry")]
public class FixedCountRetryStrategyTests
{
    private readonly ICredentialProvider _authProvider;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IConfiguration _cacheConfig;

    public FixedCountRetryStrategyTests()
    {
        _authProvider = new MomentoLocalProvider();
        _loggerFactory = LoggerFactory.Create(builder =>
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
        _cacheConfig = Configurations.Laptop.Latest(_loggerFactory);
    }

    private ICacheClient CreateClientWithMiddleware(MomentoLocalMiddleware middleware)
    {
        var config = _cacheConfig.WithMiddlewares(new List<IMiddleware>() { middleware });
        var cacheClient = new CacheClient(config, _authProvider, TimeSpan.FromSeconds(60));
        return cacheClient;
    }

    [Fact]
    public void FixedCountRetryStrategy_IneligibleRpc() 
    {
        var middleware = new MomentoLocalMiddleware(_loggerFactory, null);
        var testMetricsCollector = middleware.TestMetricsCollector;
        var cacheClient = CreateClientWithMiddleware(middleware);

        var cacheName = Utils.NewGuidString();
        cacheClient.CreateCacheAsync(cacheName).Wait();

        cacheClient.IncrementAsync(cacheName, "key").Wait();
        Assert.Equal(0, testMetricsCollector.GetTotalRetryCount(cacheName, MomentoRpcMethod.Increment));
    }

    [Fact]
    public void FixedCountRetryStrategy_EligibleRpc_FullOutage() 
    {
        var maxAttempts = 3;
        var middleware = new MomentoLocalMiddleware(_loggerFactory, new MomentoLocalMiddlewareArgs
        {
            ReturnError = MomentoErrorCode.SERVER_UNAVAILABLE,
            ErrorRpcList = new List<MomentoRpcMethod> { MomentoRpcMethod.Get },
            ErrorCount = maxAttempts
        });
        var testMetricsCollector = middleware.TestMetricsCollector;
        var retryStrategy = new FixedCountRetryStrategy(_loggerFactory, maxAttempts);
        var config = _cacheConfig.WithAdditionalMiddlewares(new List<IMiddleware>() { middleware }).WithRetryStrategy(retryStrategy);
        var cacheClient = new CacheClient(config, _authProvider, TimeSpan.FromSeconds(60));

        var cacheName = Utils.NewGuidString();
        cacheClient.CreateCacheAsync(cacheName).Wait();

        cacheClient.GetAsync(cacheName, "key").Wait();
        Assert.Equal(maxAttempts, testMetricsCollector.GetTotalRetryCount(cacheName, MomentoRpcMethod.Get));
    }

    [Fact]
    public void FixedCountRetryStrategy_EligibleRpc_TemporaryOutage() 
    {
        var middleware = new MomentoLocalMiddleware(_loggerFactory, new MomentoLocalMiddlewareArgs
        {
            ReturnError = MomentoErrorCode.SERVER_UNAVAILABLE,
            ErrorRpcList = new List<MomentoRpcMethod> { MomentoRpcMethod.Get },
            ErrorCount = 2
        });
        var testMetricsCollector = middleware.TestMetricsCollector;
        var retryStrategy = new FixedCountRetryStrategy(_loggerFactory, 3);
        var config = _cacheConfig.WithAdditionalMiddlewares(new List<IMiddleware>() { middleware }).WithRetryStrategy(retryStrategy);
        var cacheClient = new CacheClient(config, _authProvider, TimeSpan.FromSeconds(60));

        var cacheName = Utils.NewGuidString();
        cacheClient.CreateCacheAsync(cacheName).Wait();

        cacheClient.GetAsync(cacheName, "key").Wait();
        Assert.Equal(2, testMetricsCollector.GetTotalRetryCount(cacheName, MomentoRpcMethod.Get));
    }
}