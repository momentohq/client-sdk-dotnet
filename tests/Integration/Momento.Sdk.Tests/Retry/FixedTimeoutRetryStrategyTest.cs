using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Retry;

namespace Momento.Sdk.Tests.Integration.Retry;

[Collection("Retry")]
public class FixedTimeoutRetryStrategyTests
{
    private readonly ICredentialProvider _authProvider;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IConfiguration _cacheConfig;
    private readonly TimeSpan CLIENT_TIMEOUT_MILLIS = TimeSpan.FromMilliseconds(3000);
    private readonly TimeSpan RETRY_DELAY = TimeSpan.FromMilliseconds(100);
    private readonly TimeSpan RESPONSE_DATA_RECEIVED_TIMEOUT = TimeSpan.FromMilliseconds(1000);

    public FixedTimeoutRetryStrategyTests()
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
        _cacheConfig = Configurations.Laptop.Latest(_loggerFactory).WithClientTimeout(CLIENT_TIMEOUT_MILLIS);
    }

    [Fact]
    public void FixedTimeoutRetryStrategy_IneligibleRpc() 
    {
        var testProps = new MomentoLocalCacheAndCacheClient(_authProvider, _loggerFactory, _cacheConfig, null, new FixedTimeoutRetryStrategy(_loggerFactory));
        testProps.CacheClient.IncrementAsync(testProps.CacheName, "key").Wait();
        Assert.Equal(0, testProps.TestMetricsCollector.GetTotalRetryCount(testProps.CacheName, MomentoRpcMethod.Increment));
    }

    [Fact]
    public void FixedTimeoutRetryStrategy_EligibleRpc_FullOutage_ShortDelays() 
    {
        var shortDelay = RETRY_DELAY.TotalMilliseconds + 100;
        var middlewareArgs = new MomentoLocalMiddlewareArgs {
            ReturnError = MomentoErrorCode.SERVER_UNAVAILABLE.ToStringValue(),
            ErrorRpcList = new List<string> { MomentoRpcMethod.Get.ToMomentoLocalMetadataString() },
            DelayMillis = Convert.ToInt32(shortDelay),
            DelayRpcList = new List<string> { MomentoRpcMethod.Get.ToMomentoLocalMetadataString() },
        };
        var testProps = new MomentoLocalCacheAndCacheClient(
            _authProvider, 
            _loggerFactory, 
            _cacheConfig, 
            middlewareArgs, 
            new FixedTimeoutRetryStrategy(
                _loggerFactory, 
                retryDelayInterval: RETRY_DELAY, 
                responseDataReceivedTimeout: RESPONSE_DATA_RECEIVED_TIMEOUT
            )
        );
        testProps.CacheClient.GetAsync(testProps.CacheName, "key").Wait();

        var delayBetweenAttempts = RETRY_DELAY.TotalMilliseconds + shortDelay;
        var maxAttempts = Convert.ToInt32(
            CLIENT_TIMEOUT_MILLIS.TotalMilliseconds / delayBetweenAttempts
        );
        Assert.InRange(testProps.TestMetricsCollector.GetTotalRetryCount(testProps.CacheName, MomentoRpcMethod.Get), 1, maxAttempts);

        // Jitter will be +/- 10% of the retry delay interval
        var minDelay = delayBetweenAttempts * 0.9;
        var maxDelay = delayBetweenAttempts * 1.1;
        Assert.InRange(testProps.TestMetricsCollector.GetAverageTimeBetweenRetries(testProps.CacheName, MomentoRpcMethod.Get), minDelay, maxDelay);
    }

    [Fact]
    public void FixedTimeoutRetryStrategy_EligibleRpc_FullOutage_LongDelays() 
    {
        var longDelay = RESPONSE_DATA_RECEIVED_TIMEOUT.TotalMilliseconds + 100;
        var middlewareArgs = new MomentoLocalMiddlewareArgs {
            ReturnError = MomentoErrorCode.SERVER_UNAVAILABLE.ToStringValue(),
            ErrorRpcList = new List<string> { MomentoRpcMethod.Get.ToMomentoLocalMetadataString() },
            DelayMillis = Convert.ToInt32(longDelay),
            DelayRpcList = new List<string> { MomentoRpcMethod.Get.ToMomentoLocalMetadataString() },
        };
        var testProps = new MomentoLocalCacheAndCacheClient(
            _authProvider, 
            _loggerFactory, 
            _cacheConfig, 
            middlewareArgs, 
            new FixedTimeoutRetryStrategy(
                _loggerFactory, 
                retryDelayInterval: RETRY_DELAY, 
                responseDataReceivedTimeout: RESPONSE_DATA_RECEIVED_TIMEOUT
            )
        );
        testProps.CacheClient.GetAsync(testProps.CacheName, "key").Wait();

        var delayBetweenAttempts = RETRY_DELAY.TotalMilliseconds + longDelay;
        var maxAttempts = Convert.ToInt32(
            CLIENT_TIMEOUT_MILLIS.TotalMilliseconds / delayBetweenAttempts
        );
        Assert.InRange(testProps.TestMetricsCollector.GetTotalRetryCount(testProps.CacheName, MomentoRpcMethod.Get), 1, maxAttempts);

        // Jitter will be +/- 10% of the retry delay interval
        var minDelay = delayBetweenAttempts * 0.9;
        var maxDelay = delayBetweenAttempts * 1.1;
        Assert.InRange(testProps.TestMetricsCollector.GetAverageTimeBetweenRetries(testProps.CacheName, MomentoRpcMethod.Get), minDelay, maxDelay);
    }

    [Fact]
    public void FixedTimeoutRetryStrategy_EligibleRpc_FullOutage_NoDelays() 
    {
        var middlewareArgs = new MomentoLocalMiddlewareArgs {
            ReturnError = MomentoErrorCode.SERVER_UNAVAILABLE.ToStringValue(),
            ErrorRpcList = new List<string> { MomentoRpcMethod.Get.ToMomentoLocalMetadataString() },
        };
        var testProps = new MomentoLocalCacheAndCacheClient(
            _authProvider, 
            _loggerFactory, 
            _cacheConfig, 
            middlewareArgs, 
            new FixedTimeoutRetryStrategy(
                _loggerFactory, 
                retryDelayInterval: RETRY_DELAY, 
                responseDataReceivedTimeout: RESPONSE_DATA_RECEIVED_TIMEOUT
            )
        );
        testProps.CacheClient.GetAsync(testProps.CacheName, "key").Wait();

        var maxAttempts = Convert.ToInt32(
            CLIENT_TIMEOUT_MILLIS.TotalMilliseconds / RETRY_DELAY.TotalMilliseconds
        ) + 1; // +1 to account for jitter on retry delay (can be 10% longer or shorter)
        Assert.InRange(testProps.TestMetricsCollector.GetTotalRetryCount(testProps.CacheName, MomentoRpcMethod.Get), 1, maxAttempts);

        // Jitter will be +/- 10% of the retry delay interval
        var minDelay = RETRY_DELAY.TotalMilliseconds * 0.9;
        var maxDelay = RETRY_DELAY.TotalMilliseconds * 1.1;
        Assert.InRange(testProps.TestMetricsCollector.GetAverageTimeBetweenRetries(testProps.CacheName, MomentoRpcMethod.Get), minDelay, maxDelay);
    }

    [Fact]
    public void FixedTimeoutRetryStrategy_EligibleRpc_TemporaryOutage() 
    {
        var middlewareArgs = new MomentoLocalMiddlewareArgs {
            ReturnError = MomentoErrorCode.SERVER_UNAVAILABLE.ToStringValue(),
            ErrorRpcList = new List<string> { MomentoRpcMethod.Get.ToMomentoLocalMetadataString() },
            ErrorCount = 2
        };
        var testProps = new MomentoLocalCacheAndCacheClient(
            _authProvider, 
            _loggerFactory, 
            _cacheConfig, 
            middlewareArgs, 
            new FixedTimeoutRetryStrategy(
                _loggerFactory, 
                retryDelayInterval: RETRY_DELAY, 
                responseDataReceivedTimeout: RESPONSE_DATA_RECEIVED_TIMEOUT
            )
        );
        testProps.CacheClient.GetAsync(testProps.CacheName, "key").Wait();
        Assert.Equal(2, testProps.TestMetricsCollector.GetTotalRetryCount(testProps.CacheName, MomentoRpcMethod.Get));

        // Jitter will be +/- 10% of the retry delay interval
        var minDelay = RETRY_DELAY.TotalMilliseconds * 0.9;
        var maxDelay = RETRY_DELAY.TotalMilliseconds * 1.1;
        Assert.InRange(testProps.TestMetricsCollector.GetAverageTimeBetweenRetries(testProps.CacheName, MomentoRpcMethod.Get), minDelay, maxDelay);
    }
}