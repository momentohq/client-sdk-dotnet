using Momento.Sdk.Config.Retry;

// namespace Momento.Sdk.Tests.Integration.Cache;
namespace Momento.Sdk.Tests.Integration.Retry;

[Collection("Retry")]
public class TestRetryMetricsCollectorTests
{
    [Fact]
    public void TestRetryMetricsCollector_NoData()
    {
        var collector = new TestRetryMetricsCollector();
        Assert.Empty(collector.AllMetrics);
        Assert.Equal(0, collector.GetTotalRetryCount("cache", MomentoRpcMethod.Get));
        Assert.Equal(0, collector.GetAverageTimeBetweenRetries("cache", MomentoRpcMethod.Get));
    }

    [Fact]
    public void TestRetryMetricsCollector_AddTimestamp()
    {
        var collector = new TestRetryMetricsCollector();
        collector.AddTimestamp("cache", MomentoRpcMethod.Get, 1);
        Assert.Single(collector.AllMetrics);
        Assert.Single(collector.AllMetrics["cache"]);
        Assert.Single(collector.AllMetrics["cache"][MomentoRpcMethod.Get]);
        Assert.Equal(1, collector.AllMetrics["cache"][MomentoRpcMethod.Get][0]);
    }

    [Fact]
    public void TestRetryMetricsCollector_GetAllMetrics()
    {
        var collector = new TestRetryMetricsCollector();
        collector.AddTimestamp("cache1", MomentoRpcMethod.Get, 1000);
        collector.AddTimestamp("cache1", MomentoRpcMethod.Set, 2000);
        collector.AddTimestamp("cache2", MomentoRpcMethod.Set, 3000);

        // Should have 2 keys in outer dictionary: cache1 and cache2.
        Assert.Equal(2, collector.AllMetrics.Count);

        // cache1 should have 2 entries: Get and Set. cache2 should have 1 entry: Set.
        Assert.Equal(2, collector.AllMetrics["cache1"].Count);
        Assert.Single(collector.AllMetrics["cache2"]);

        // Should have 1 timestamp for each method and cache that we added timestamps for.
        Assert.Single(collector.AllMetrics["cache1"][MomentoRpcMethod.Get]);
        Assert.Single(collector.AllMetrics["cache1"][MomentoRpcMethod.Set]);
        Assert.Single(collector.AllMetrics["cache2"][MomentoRpcMethod.Set]);
    }

    [Fact]
    public void TestRetryMetricsCollector_GetTotalRetryCount_NoRetries()
    {
        var collector = new TestRetryMetricsCollector();
        Assert.Equal(0, collector.GetTotalRetryCount("cache", MomentoRpcMethod.Get));

        collector.AddTimestamp("cache", MomentoRpcMethod.Get, 1000);
        Assert.Equal(0, collector.GetTotalRetryCount("cache", MomentoRpcMethod.Get));
    }

    [Fact]
    public void TestRetryMetricsCollector_GetTotalRetryCount_WithRetries()
    {
        var collector = new TestRetryMetricsCollector();
        collector.AddTimestamp("cache", MomentoRpcMethod.Get, 1000);
        collector.AddTimestamp("cache", MomentoRpcMethod.Get, 2000);
        collector.AddTimestamp("cache", MomentoRpcMethod.Get, 3000);
        Assert.Equal(2, collector.GetTotalRetryCount("cache", MomentoRpcMethod.Get));
    }

    [Fact]
    public void TestRetryMetricsCollector_GetAverageTimeBetweenRetries_NoRetries()
    {
        var collector = new TestRetryMetricsCollector();
        Assert.Equal(0, collector.GetAverageTimeBetweenRetries("cache", MomentoRpcMethod.Get));

        collector.AddTimestamp("cache", MomentoRpcMethod.Get, 1000);
        Assert.Equal(0, collector.GetAverageTimeBetweenRetries("cache", MomentoRpcMethod.Get));
    }

    [Fact]
    public void TestRetryMetricsCollector_GetAverageTimeBetweenRetries_WithRetries()
    {
        var collector = new TestRetryMetricsCollector();
        collector.AddTimestamp("cache", MomentoRpcMethod.Get, 1000);
        collector.AddTimestamp("cache", MomentoRpcMethod.Get, 2000);
        collector.AddTimestamp("cache", MomentoRpcMethod.Get, 4000);
        Assert.Equal(1500, collector.GetAverageTimeBetweenRetries("cache", MomentoRpcMethod.Get));
    }
}