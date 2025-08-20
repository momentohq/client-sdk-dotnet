using System.Collections.Generic;

namespace Momento.Sdk.Tests.Integration.Retry;

public class TestRetryMetricsCollector
{
    /// <summary>
    /// Data structure to store timestamps for all requests: cacheName -> requestName -> [timestamps]
    /// </summary>
    public Dictionary<string, Dictionary<MomentoRpcMethod, List<long>>> AllMetrics { get; private set; }

    public TestRetryMetricsCollector()
    {
        AllMetrics = new Dictionary<string, Dictionary<MomentoRpcMethod, List<long>>>();
    }

    public void AddTimestamp(string cacheName, MomentoRpcMethod requestName, long timestamp)
    {
        if (!AllMetrics.ContainsKey(cacheName))
        {
            AllMetrics[cacheName] = new Dictionary<MomentoRpcMethod, List<long>>();
        }
        if (!AllMetrics[cacheName].ContainsKey(requestName))
        {
            AllMetrics[cacheName][requestName] = new List<long>();
        }
        AllMetrics[cacheName][requestName].Add(timestamp);
    }

    public long GetTotalRetryCount(string cacheName, MomentoRpcMethod requestName)
    {
        if (!AllMetrics.ContainsKey(cacheName) || !AllMetrics[cacheName].ContainsKey(requestName))
        {
            return 0;
        }
        // The first timestamp is the initial request, not a retry, so we subtract 1.
        return Math.Max(0, AllMetrics[cacheName][requestName].Count - 1);
    }

    public long GetAverageTimeBetweenRetries(string cacheName, MomentoRpcMethod requestName)
    {
        if (!AllMetrics.ContainsKey(cacheName) || !AllMetrics[cacheName].ContainsKey(requestName))
        {
            // No retries occurred.
            return 0;
        }
        var timestamps = AllMetrics[cacheName][requestName];
        if (timestamps.Count < 2)
        {
            // No retries occurred.
            return 0;
        }

        long total = 0;
        for (int i = 1; i < timestamps.Count; i++)
        {
            total += timestamps[i] - timestamps[i - 1];
        }

        return total / (timestamps.Count - 1);
    }
}
