using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Protos.CacheClient;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Retry;
using System.Collections.Generic;

namespace Momento.Sdk.Tests.Integration.Retry;

[Collection("Retry")]
public class ExponentialBackoffRetryStrategyTests
{
    private readonly ICredentialProvider _authProvider;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IConfiguration _cacheConfig;
    private readonly TimeSpan CLIENT_TIMEOUT = TimeSpan.FromMilliseconds(3000);

    public ExponentialBackoffRetryStrategyTests()
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
        _cacheConfig = Configurations.Laptop.Latest(_loggerFactory).WithClientTimeout(CLIENT_TIMEOUT);
    }

    [Fact]
    public void ExponentialBackoffRetryStrategy_IneligibleRpc()
    {
        var retryStrategy = new ExponentialBackoffRetryStrategy(_loggerFactory);
        var testProps = new MomentoLocalCacheAndCacheClient(_authProvider, _loggerFactory, _cacheConfig, null, retryStrategy);
        testProps.CacheClient.IncrementAsync(testProps.CacheName, "key").Wait();
        Assert.Equal(0, testProps.TestMetricsCollector.GetTotalRetryCount(testProps.CacheName, MomentoRpcMethod.Increment));
    }

    [Fact]
    public void ExponentialBackoffRetryStrategy_FirstAttemptReturnsInitialDelayWithJitter()
    {
        var retryStrategy = new ExponentialBackoffRetryStrategy(
            _loggerFactory,
            initialDelay: TimeSpan.FromMilliseconds(100),
            maxBackoff: TimeSpan.FromMilliseconds(1000)
        );
        var overallDeadline = DateTime.UtcNow.AddMilliseconds(CLIENT_TIMEOUT.TotalMilliseconds);
        for (int i = 0; i < 100; i++)
        {
            var retryDelay = retryStrategy.DetermineWhenToRetryRequest(new Status(StatusCode.Unavailable, "unavailable"), new _GetRequest(), 0, overallDeadline);
            if (retryDelay == null)
            {
                Assert.Fail("Retry delay should not be null");
            }
            else
            {
                Assert.InRange(retryDelay.Value, 100, 300);
            }
        }
    }

    [Fact]
    public void ExponentialBackoffRetryStrategy_SecondAttemptShouldDoubleBaseDelayWithJitter()
    {
        var retryStrategy = new ExponentialBackoffRetryStrategy(
            _loggerFactory,
            initialDelay: TimeSpan.FromMilliseconds(100),
            maxBackoff: TimeSpan.FromMilliseconds(1000)
        );
        var overallDeadline = DateTime.UtcNow.AddMilliseconds(CLIENT_TIMEOUT.TotalMilliseconds);
        for (int i = 0; i < 100; i++)
        {
            var retryDelay = retryStrategy.DetermineWhenToRetryRequest(new Status(StatusCode.Unavailable, "unavailable"), new _GetRequest(), 1, overallDeadline);
            if (retryDelay == null)
            {
                Assert.Fail("Retry delay should not be null");
            }
            else
            {
                Assert.InRange(retryDelay.Value, 200, 600);
            }
        }
    }

    [Fact]
    public void ExponentialBackoffRetryStrategy_ShouldNotExceedLimitWhenMaxBackoffReached()
    {
        var retryStrategy = new ExponentialBackoffRetryStrategy(
            _loggerFactory,
            initialDelay: TimeSpan.FromMilliseconds(100),
            maxBackoff: TimeSpan.FromMilliseconds(500)
        );
        var overallDeadline = DateTime.UtcNow.AddMilliseconds(CLIENT_TIMEOUT.TotalMilliseconds);    
        var retryDelay = retryStrategy.DetermineWhenToRetryRequest(new Status(StatusCode.Unavailable, "unavailable"), new _GetRequest(), 100, overallDeadline);
        if (retryDelay == null)
        {
            Assert.Fail("Retry delay should not be null");
        }
        else
        {
            Assert.InRange(retryDelay.Value, 500, 1500);
        }
    }

    [Fact]
    public void ExponentialBackoffRetryStrategy_FullOutage_ShouldRetryMultipleTimesUntilClientTimeout()
    {
        var retryStrategy = new ExponentialBackoffRetryStrategy(
            _loggerFactory,
            initialDelay: TimeSpan.FromMilliseconds(100),
            maxBackoff: TimeSpan.FromMilliseconds(2000)
        );
        var middlewareArgs = new MomentoLocalMiddlewareArgs
        {
            ReturnError = MomentoErrorCode.SERVER_UNAVAILABLE.ToStringValue(),
            ErrorRpcList = new List<string> { MomentoRpcMethod.Get.ToMomentoLocalMetadataString() },
        };
        var testProps = new MomentoLocalCacheAndCacheClient(_authProvider, _loggerFactory, _cacheConfig, middlewareArgs, retryStrategy);
        testProps.CacheClient.GetAsync(testProps.CacheName, "key").Wait();
        var retryCount = testProps.TestMetricsCollector.GetTotalRetryCount(testProps.CacheName, MomentoRpcMethod.Get);
        Assert.InRange(retryCount, 2, 5);
        var averageRetryDelay = testProps.TestMetricsCollector.GetAverageTimeBetweenRetries(testProps.CacheName, MomentoRpcMethod.Get);
        Assert.InRange(averageRetryDelay, 100, 2000);
    }

    [Fact]
    public void ExponentialBackoffRetryStrategy_TemporaryOutage()
    {
        var retryStrategy = new ExponentialBackoffRetryStrategy(
            _loggerFactory,
            initialDelay: TimeSpan.FromMilliseconds(100),
            maxBackoff: TimeSpan.FromMilliseconds(2000)
        );
        var middlewareArgs = new MomentoLocalMiddlewareArgs
        {
            ReturnError = MomentoErrorCode.SERVER_UNAVAILABLE.ToStringValue(),
            ErrorRpcList = new List<string> { MomentoRpcMethod.Get.ToMomentoLocalMetadataString() },
            ErrorCount = 2,
        };
        var testProps = new MomentoLocalCacheAndCacheClient(_authProvider, _loggerFactory, _cacheConfig, middlewareArgs, retryStrategy);
        testProps.CacheClient.GetAsync(testProps.CacheName, "key").Wait();
        Assert.Equal(2, testProps.TestMetricsCollector.GetTotalRetryCount(testProps.CacheName, MomentoRpcMethod.Get));
        var averageRetryDelay = testProps.TestMetricsCollector.GetAverageTimeBetweenRetries(testProps.CacheName, MomentoRpcMethod.Get);
        Assert.InRange(averageRetryDelay, 100, 600);
    }
}
