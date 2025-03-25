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
    private readonly TimeSpan RETRY_DELAY_MILLIS = TimeSpan.FromMilliseconds(100);
    private readonly TimeSpan RESPONSE_DATA_RECEIVED_TIMEOUT_MILLIS = TimeSpan.FromMilliseconds(1000);

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
    public void FixedTimeoutRetryStrategy_EligibleRpc_FullOutage() 
    {
        var maxAttempts = Convert.ToInt32(
            CLIENT_TIMEOUT_MILLIS.TotalMilliseconds / RESPONSE_DATA_RECEIVED_TIMEOUT_MILLIS.TotalMilliseconds
        );
        var middlewareArgs = new MomentoLocalMiddlewareArgs {
            ReturnError = MomentoErrorCode.SERVER_UNAVAILABLE,
            ErrorRpcList = new List<MomentoRpcMethod> { MomentoRpcMethod.Get },
            ErrorCount = maxAttempts
        };
        var testProps = new MomentoLocalCacheAndCacheClient(
            _authProvider, 
            _loggerFactory, 
            _cacheConfig, 
            middlewareArgs, 
            new FixedTimeoutRetryStrategy(
                _loggerFactory, 
                retryDelayIntervalMillis: RETRY_DELAY_MILLIS, 
                responseDataReceivedTimeoutMillis: RESPONSE_DATA_RECEIVED_TIMEOUT_MILLIS
            )
        );
        testProps.CacheClient.GetAsync(testProps.CacheName, "key").Wait();
        Assert.Equal(maxAttempts, testProps.TestMetricsCollector.GetTotalRetryCount(testProps.CacheName, MomentoRpcMethod.Get));
    }

    [Fact]
    public void FixedTimeoutRetryStrategy_EligibleRpc_TemporaryOutage() 
    {
        var middlewareArgs = new MomentoLocalMiddlewareArgs {
            ReturnError = MomentoErrorCode.SERVER_UNAVAILABLE,
            ErrorRpcList = new List<MomentoRpcMethod> { MomentoRpcMethod.Get },
            ErrorCount = 2
        };
        var testProps = new MomentoLocalCacheAndCacheClient(
            _authProvider, 
            _loggerFactory, 
            _cacheConfig, 
            middlewareArgs, 
            new FixedTimeoutRetryStrategy(
                _loggerFactory, 
                retryDelayIntervalMillis: RETRY_DELAY_MILLIS, 
                responseDataReceivedTimeoutMillis: RESPONSE_DATA_RECEIVED_TIMEOUT_MILLIS
            )
        );
        testProps.CacheClient.GetAsync(testProps.CacheName, "key").Wait();
        var maxAttempts = Convert.ToInt32(
            CLIENT_TIMEOUT_MILLIS.TotalMilliseconds / RESPONSE_DATA_RECEIVED_TIMEOUT_MILLIS.TotalMilliseconds
        );
        Assert.InRange(testProps.TestMetricsCollector.GetTotalRetryCount(testProps.CacheName, MomentoRpcMethod.Get), 1, maxAttempts);
    }
}