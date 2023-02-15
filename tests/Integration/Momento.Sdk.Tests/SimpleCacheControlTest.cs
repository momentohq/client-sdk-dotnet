using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Momento.Sdk.Auth;

namespace Momento.Sdk.Tests;

[Collection("SimpleCacheClient")]
public class SimpleCacheControlTest : TestBase
{
    public SimpleCacheControlTest(SimpleCacheClientFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void SimpleCacheClientConstructor_BadJWT_InvalidJwtException()
    {
        Environment.SetEnvironmentVariable("BAD_MOMENTO_AUTH_TOKEN", "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJpbnRlZ3JhdGlvbiJ9.ZOgkTs");
        Assert.Throws<InvalidArgumentException>(
            () => new EnvMomentoTokenProvider("BAD_MOMENTO_AUTH_TOKEN")
        );
        Environment.SetEnvironmentVariable("BAD_MOMENTO_AUTH_TOKEN", null);
    }

    [Fact]
    public void SimpleCacheClientConstructor_NullJWT_InvalidJwtException()
    {
        Environment.SetEnvironmentVariable("BAD_MOMENTO_AUTH_TOKEN", null);
        Assert.Throws<InvalidArgumentException>(
            () => new EnvMomentoTokenProvider("BAD_MOMENTO_AUTH_TOKEN")
        );
    }

    [Fact]
    public async Task DeleteCacheAsync_NullCache_InvalidArgumentError()
    {
        DeleteCacheResponse deleteResponse = await client.DeleteCacheAsync(null!);
        Assert.True(deleteResponse is DeleteCacheResponse.Error, $"Unexpected response: {deleteResponse}");
        DeleteCacheResponse.Error errorResponse = (DeleteCacheResponse.Error)deleteResponse;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.InnerException.ErrorCode);
    }

    [Fact]
    public async Task DeleteCacheAsync_CacheDoesntExist_NotFoundException()
    {
        DeleteCacheResponse response = await client.DeleteCacheAsync("non-existent cache");
        Assert.True(response is DeleteCacheResponse.Error, $"Unexpected response: {response}");
        var errorResponse = (DeleteCacheResponse.Error)response;
        Assert.Equal(MomentoErrorCode.NOT_FOUND_ERROR, errorResponse.InnerException.ErrorCode);
    }

    [Fact]
    public async Task CreateCacheAsync_NullCache_InvalidArgumentError()
    {
        CreateCacheResponse response = await client.CreateCacheAsync(null!);
        Assert.True(response is CreateCacheResponse.Error, $"Unexpected response: {response}");
        CreateCacheResponse.Error errorResponse = (CreateCacheResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.InnerException.ErrorCode);
    }

    [Fact]
    public async Task FlushCacheAsync_HappyPath()
    {
        string cacheName = Utils.NewGuidString();
        await client.CreateCacheAsync(cacheName);

        try
        {
            string key = Utils.NewGuidString();
            string value = Utils.NewGuidString();
            // Set with a long TTL
            await client.SetAsync(cacheName, key, value, TimeSpan.FromHours(1));
            CacheGetResponse response = await client.GetAsync(cacheName, key);
            Assert.True(response is CacheGetResponse.Hit, $"Unexpected response: {response}");

            // Flush
            FlushCacheResponse flushCacheResponse = await client.FlushCacheAsync(cacheName);

            // Verify
            Assert.True(flushCacheResponse is FlushCacheResponse.Success, $"Unexpected response: {flushCacheResponse}");
            response = await client.GetAsync(cacheName, key);
            Assert.True(response is CacheGetResponse.Miss, $"Unexpected response: {response}");
        }
        finally
        {
            DeleteCacheResponse deleteResp = await client.DeleteCacheAsync(cacheName);
            Assert.True(deleteResp is DeleteCacheResponse.Success, $"Unexpected response: {deleteResp} while deleting cache: {cacheName}.");
        }

    }

    [Fact]
    public async Task FlushCacheAsync_CacheNotFound()
    {
        FlushCacheResponse response = await client.FlushCacheAsync("non-existent cache");
        Assert.True(response is FlushCacheResponse.Error, $"Unexpected response: {response}");
        var errorResponse = (FlushCacheResponse.Error)response;
        Assert.Equal(MomentoErrorCode.NOT_FOUND_ERROR, errorResponse.InnerException.ErrorCode);
    }

    [Fact]
    public async Task FlushCacheAsync_NullCache_IllegalArgumentError()
    {
        FlushCacheResponse response = await client.FlushCacheAsync(null!);
        Assert.True(response is FlushCacheResponse.Error, $"Unexpected response: {response}");
        var errorResponse = (FlushCacheResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.InnerException.ErrorCode);
    }

    // Tests: creating a cache, listing a cache, and deleting a cache.
    [Fact]
    public async Task ListCachesAsync_OneCache_HappyPath()
    {
        // Create cache
        string cacheName = Utils.NewGuidString();
        await client.CreateCacheAsync(cacheName);

        // Test cache exists
        ListCachesResponse result = await client.ListCachesAsync();
        Assert.True(result is ListCachesResponse.Success, $"Unexpected response: {result}");
        var successResult = (ListCachesResponse.Success)result;
        List<CacheInfo> caches = successResult.Caches;
        Assert.Contains(new CacheInfo(cacheName), caches);

        // Test deleting cache
        await client.DeleteCacheAsync(cacheName);
        result = await client.ListCachesAsync();
        Assert.True(result is ListCachesResponse.Success, $"Unexpected response: {result}");
        var successResult2 = (ListCachesResponse.Success)result;
        var caches2 = successResult2.Caches;
        Assert.DoesNotContain(new CacheInfo(cacheName), caches2);
    }

    [Fact]
    public async Task ListCachesAsync_Iteration_HappyPath()
    {
        // Create caches
        List<String> cacheNames = new List<String>();

        // TODO: increase limit after pagination is enabled
        foreach (int val in Enumerable.Range(1, 3))
        {
            String cacheName = Utils.NewGuidString();
            cacheNames.Add(cacheName);
            await client.CreateCacheAsync(cacheName);
        }
        try
        {
            // List caches
            HashSet<String> retrievedCaches = new HashSet<string>();
            ListCachesResponse result = await client.ListCachesAsync();

            Assert.True(result is ListCachesResponse.Success, $"Unexpected response: {result}");
            var successResult = (ListCachesResponse.Success)result;
            foreach (CacheInfo cache in successResult.Caches)
            {
                retrievedCaches.Add(cache.Name);
            }

            foreach (String cache in cacheNames)
            {
                Assert.Contains(cache, retrievedCaches);
            }
        }
        finally
        {
            // Cleanup
            foreach (String cacheName in cacheNames)
            {
                await client.DeleteCacheAsync(cacheName);
            }
        }
    }
}
