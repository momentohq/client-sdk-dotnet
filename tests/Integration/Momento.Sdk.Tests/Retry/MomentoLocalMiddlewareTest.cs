using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Retry;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Middleware;
using System.Threading.Tasks;
using Momento.Protos.CacheClient;
using Momento.Sdk.Internal.ExtensionMethods;
using Grpc.Core;

// namespace Momento.Sdk.Tests.Integration.Cache;
namespace Momento.Sdk.Tests.Integration.Retry;

[Collection("Retry")]
public class MomentoLocalMiddlewareTests
{
    private readonly ICredentialProvider _authProvider;

    public MomentoLocalMiddlewareTests()
    {
        _authProvider = new MomentoLocalProvider();
    }

    private ICacheClient CreateClientWithMiddleware(MomentoLocalMiddlewareArgs args)
    {
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
        var config = Configurations.Laptop.Latest(loggerFactory).WithMiddlewares([new MomentoLocalMiddleware(loggerFactory, args)]);
        var cacheClient = new CacheClient(config, _authProvider, TimeSpan.FromSeconds(10));
        return cacheClient;
    }

    private static MomentoLocalMiddleware CreateMiddleware(MomentoLocalMiddlewareArgs args)
    {
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
        return new MomentoLocalMiddleware(loggerFactory, args);
    }

    [Fact]
    public void MomentoLocalMiddleware_OneCache_OneRequest() {
      var testMetricsCollector = new TestRetryMetricsCollector();
        var args = new MomentoLocalMiddlewareArgs {
            TestMetricsCollector = testMetricsCollector,
            RequestId = Utils.NewGuidString(),
        };
        var cacheClient = CreateClientWithMiddleware(args);

        var cacheName = Utils.NewGuidString();
        cacheClient.CreateCacheAsync(cacheName).Wait();

        cacheClient.GetAsync(cacheName, "key").Wait();

        // Should contain only a single entry in the outer dictionary for the one cache
        Assert.Single(testMetricsCollector.AllMetrics);
        // Should contain only one entry for this cache for the Get method
        Assert.Single(testMetricsCollector.AllMetrics[cacheName]);
        // Should contain only one timestamp for this cache and method
        Assert.Single(testMetricsCollector.AllMetrics[cacheName][MomentoRpcMethod.Get]);
    }

    [Fact]
    public void MomentoLocalMiddleware_OneCache_MultipleRequests() {
        var testMetricsCollector = new TestRetryMetricsCollector();
        var args = new MomentoLocalMiddlewareArgs {
            TestMetricsCollector = testMetricsCollector,
            RequestId = Utils.NewGuidString(),
        };
        var cacheClient = CreateClientWithMiddleware(args);

        var cacheName = Utils.NewGuidString();
        cacheClient.CreateCacheAsync(cacheName).Wait();

        cacheClient.GetAsync(cacheName, "key").Wait();
        cacheClient.SetAsync(cacheName, "another-key", "value").Wait();
        cacheClient.GetAsync(cacheName, "another-key").Wait();

        // print all metrics
        foreach (var cache in testMetricsCollector.AllMetrics)
        {
            foreach (var method in cache.Value)
            {
                foreach (var timestamp in method.Value)
                {
                    Console.WriteLine($"Cache: {cache.Key}, Method: {method.Key}, Timestamp: {timestamp}");
                }
            }
        }

        // Should contain only a single entry in the outer dictionary for the one cache
        Assert.Single(testMetricsCollector.AllMetrics);
        // Should contain two entries for this cache for the Get and Set methods
        Assert.Equal(2, testMetricsCollector.AllMetrics[cacheName].Count);
        // Should contain two timestamps for this cache and Get method
        Assert.Equal(2, testMetricsCollector.AllMetrics[cacheName][MomentoRpcMethod.Get].Count);
        // Should contain a single timestamp for this cache and Set method
        Assert.Single(testMetricsCollector.AllMetrics[cacheName][MomentoRpcMethod.Set]);
    }

    [Fact]
    public void MomentoLocalMiddleware_MultipleCaches_MultipleRequests() {
        var testMetricsCollector = new TestRetryMetricsCollector();
        var args = new MomentoLocalMiddlewareArgs {
            TestMetricsCollector = testMetricsCollector,
            RequestId = Utils.NewGuidString(),
        };
        var cacheClient = CreateClientWithMiddleware(args);

        var cacheName1 = Utils.NewGuidString();
        var cacheName2 = Utils.NewGuidString();
        cacheClient.CreateCacheAsync(cacheName1).Wait();
        cacheClient.CreateCacheAsync(cacheName2).Wait();

        cacheClient.SetAsync(cacheName1, "key", "value").Wait();
        cacheClient.GetAsync(cacheName1, "key").Wait();
        cacheClient.SetAsync(cacheName2, "another-key", "value").Wait();
        cacheClient.GetAsync(cacheName2, "another-key").Wait();

        // Should contain two entries in the outer dictionary for the two caches
        Assert.Equal(2, testMetricsCollector.AllMetrics.Count);

        // Should contain two entries for cache1 for the Get and Set methods
        Assert.Equal(2, testMetricsCollector.AllMetrics[cacheName1].Count);
        // Should contain a single timestamp for cache1 and Get method
        Assert.Single(testMetricsCollector.AllMetrics[cacheName1][MomentoRpcMethod.Get]);
        // Should contain a single timestamp for cache1 and Set method
        Assert.Single(testMetricsCollector.AllMetrics[cacheName1][MomentoRpcMethod.Set]);

        // Should contain two entries for cache2 for the Get and Set methods
        Assert.Equal(2, testMetricsCollector.AllMetrics[cacheName2].Count);
        // Should contain a single timestamp for cache2 and Get method
        Assert.Single(testMetricsCollector.AllMetrics[cacheName2][MomentoRpcMethod.Get]);
        // Should contain a single timestamp for cache2 and Set method
        Assert.Single(testMetricsCollector.AllMetrics[cacheName2][MomentoRpcMethod.Set]);
    }

    [Fact]
    public async Task MomentoLocalMiddleware_SetsCorrectMomentoLocalMetadata() {
        var testMetricsCollector = new TestRetryMetricsCollector();
        var args = new MomentoLocalMiddlewareArgs {
            TestMetricsCollector = testMetricsCollector,
            RequestId = Utils.NewGuidString(),
            ReturnError = MomentoErrorCode.INTERNAL_SERVER_ERROR,
            ErrorRpcList = [MomentoRpcMethod.Get],
            ErrorCount = 1,
            DelayRpcList = [MomentoRpcMethod.Set],
            DelayMillis = 100,
            DelayCount = 1,
            StreamErrorRpcList = [MomentoRpcMethod.TopicSubscribe],
            StreamError = MomentoErrorCode.INTERNAL_SERVER_ERROR,
            StreamErrorMessageLimit = 1,
        };
        var mw = CreateMiddleware(args);
        _GetRequest request = new _GetRequest() { CacheKey = "key".ToByteString() };
        var callOptions = new CallOptions().WithHeaders(new Metadata());
        callOptions.Headers!.Add("cache", "cache");
        var wrapped = await mw.WrapRequest(request, callOptions, async (req, opts) => {
            await Task.Delay(100);
            return new MiddlewareResponseState<_GetResponse>(
                ResponseAsync: Task.FromResult(new _GetResponse()),
                ResponseHeadersAsync: Task.FromResult(callOptions.Headers!),
                GetStatus: () => new Status(StatusCode.OK, ""),
                GetTrailers: () => callOptions.Headers!
            );
        });

        var trailers = wrapped.GetTrailers();
        Assert.Equal(args.RequestId, trailers.Get("request-id")?.Value);
        Assert.Equal(MomentoErrorCodeMetadataConverter.ToStringValue(MomentoErrorCode.INTERNAL_SERVER_ERROR), trailers.Get("return-error")?.Value);
        Assert.Equal(MomentoRpcMethodExtensions.ToStringValue(args.ErrorRpcList[0]), trailers.Get("error-rpcs")?.Value);
        Assert.Equal("1", trailers.Get("error-count")?.Value);
        Assert.Equal(MomentoRpcMethodExtensions.ToStringValue(args.DelayRpcList[0]), trailers.Get("delay-rpcs")?.Value);
        Assert.Equal("100", trailers.Get("delay-ms")?.Value);
        Assert.Equal("1", trailers.Get("delay-count")?.Value);
        Assert.Equal(MomentoRpcMethodExtensions.ToStringValue(args.StreamErrorRpcList[0]), trailers.Get("stream-error-rpcs")?.Value);
        Assert.Equal(MomentoErrorCodeMetadataConverter.ToStringValue(MomentoErrorCode.INTERNAL_SERVER_ERROR), trailers.Get("stream-error")?.Value);
        Assert.Equal("1", trailers.Get("stream-error-message-limit")?.Value);
    }
}