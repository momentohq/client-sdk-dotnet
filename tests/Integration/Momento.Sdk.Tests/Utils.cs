namespace Momento.Sdk.Tests.Integration;

/// <summary>
/// Ad-hoc utility methods.
/// </summary>
public static class Utils
{

    public static string TestCacheName() => "dotnet-integration-" + NewGuidString();

    public static string TestVectorIndexName() => "dotnet-integration-" + NewGuidString();

    public static string NewGuidString() => Guid.NewGuid().ToString();

    public static byte[] NewGuidByteArray() => Guid.NewGuid().ToByteArray();

    public static int InitialRefreshTtl { get; } = 4;

    public static int UpdatedRefreshTtl { get; } = 10;

    public static int WaitForItemToBeSet { get; } = 100;

    public static int WaitForInitialItemToExpire { get; } = 4900;

    public static void CreateCacheForTest(ICacheClient cacheClient, string cacheName)
    {
        var result = cacheClient.CreateCacheAsync(cacheName).Result;
        if (result is not (CreateCacheResponse.Success or CreateCacheResponse.CacheAlreadyExists))
        {
            throw new Exception($"Error when creating cache: {result}");
        }
    }
}
