using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
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

    [Fact]
    public void FixedCountRetryStrategy_IneligibleRpc()
    {
        var testProps = new MomentoLocalCacheAndCacheClient(_authProvider, _loggerFactory, _cacheConfig, null, new FixedCountRetryStrategy(_loggerFactory, 3));
        testProps.CacheClient.IncrementAsync(testProps.CacheName, "key").Wait();
        Assert.Equal(0, testProps.TestMetricsCollector.GetTotalRetryCount(testProps.CacheName, MomentoRpcMethod.Increment));
    }

    [Fact]
    public void FixedCountRetryStrategy_EligibleRpc_FullOutage()
    {
        var maxAttempts = 3;
        var middlewareArgs = new MomentoLocalMiddlewareArgs
        {
            ReturnError = MomentoErrorCode.SERVER_UNAVAILABLE.ToStringValue(),
            ErrorRpcList = new List<string> { MomentoRpcMethod.Get.ToMomentoLocalMetadataString() },
        };
        var testProps = new MomentoLocalCacheAndCacheClient(_authProvider, _loggerFactory, _cacheConfig, middlewareArgs, new FixedCountRetryStrategy(_loggerFactory, maxAttempts));
        testProps.CacheClient.GetAsync(testProps.CacheName, "key").Wait();
        Assert.Equal(maxAttempts, testProps.TestMetricsCollector.GetTotalRetryCount(testProps.CacheName, MomentoRpcMethod.Get));
        var averageTimeBetweenRetries = testProps.TestMetricsCollector.GetAverageTimeBetweenRetries(testProps.CacheName, MomentoRpcMethod.Get);
        Assert.InRange(averageTimeBetweenRetries, 0, 10); // should be a negligible amount of time between retries
    }

    [Fact]
    public void FixedCountRetryStrategy_EligibleRpc_TemporaryOutage()
    {
        var middlewareArgs = new MomentoLocalMiddlewareArgs
        {
            ReturnError = MomentoErrorCode.SERVER_UNAVAILABLE.ToStringValue(),
            ErrorRpcList = new List<string> { MomentoRpcMethod.Get.ToMomentoLocalMetadataString() },
            ErrorCount = 2
        };
        var testProps = new MomentoLocalCacheAndCacheClient(_authProvider, _loggerFactory, _cacheConfig, middlewareArgs, new FixedCountRetryStrategy(_loggerFactory, 3));
        testProps.CacheClient.GetAsync(testProps.CacheName, "key").Wait();
        Assert.Equal(2, testProps.TestMetricsCollector.GetTotalRetryCount(testProps.CacheName, MomentoRpcMethod.Get));
        var averageTimeBetweenRetries = testProps.TestMetricsCollector.GetAverageTimeBetweenRetries(testProps.CacheName, MomentoRpcMethod.Get);
        Assert.InRange(averageTimeBetweenRetries, 0, 10); // should be a negligible amount of time between retries
    }
}