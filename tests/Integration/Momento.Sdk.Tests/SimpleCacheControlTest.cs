using System.Collections.Generic;
using System.Linq;
using Momento.Sdk.Config;

namespace Momento.Sdk.Tests;

[Collection("SimpleCacheClient")]
public class SimpleCacheControlTest
{
    private SimpleCacheClient client;
    private string authToken;

    public SimpleCacheControlTest(SimpleCacheClientFixture fixture)
    {
        client = fixture.Client;
        authToken = fixture.AuthToken;
    }

    [Fact]
    public void SimpleCacheClientConstructor_BadJWT_InvalidJwtException()
    {
        Assert.Throws<InvalidArgumentException>(() => new SimpleCacheClient(Configurations.Laptop.Latest, "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJpbnRlZ3JhdGlvbiJ9.ZOgkTs", defaultTtlSeconds: 10));
    }

    [Fact]
    public void SimpleCacheClientConstructor_NullJWT_InvalidJwtException()
    {
        Assert.Throws<InvalidArgumentException>(() => new SimpleCacheClient(Configurations.Laptop.Latest, null!, defaultTtlSeconds: 10));
    }

    [Fact]
    public void DeleteCache_NullCache_InvalidArgumentError()
    {
        DeleteCacheResponse resp = client.DeleteCache(null!);
        DeleteCacheResponse.Error errResp = (DeleteCacheResponse.Error)resp;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errResp.Exception.ErrorCode);
    }

    [Fact]
    public void DeleteCache_CacheDoesntExist_NotFoundException()
    {
        // Assert.Throws<NotFoundException>(() => client.DeleteCache("non-existent cache"));
        DeleteCacheResponse resp = client.DeleteCache("non-existent cache");
        Assert.True(resp is DeleteCacheResponse.Error);
        DeleteCacheResponse.Error errResp = (DeleteCacheResponse.Error)resp;
        Assert.Equal(MomentoErrorCode.NOT_FOUND_ERROR, errResp.Exception.ErrorCode);
    }

    [Fact]
    public void CreateCache_NullCache_InvalidArgumentError()
    {
        CreateCacheResponse resp = client.CreateCache(null!);
        CreateCacheResponse.Error errResp = (CreateCacheResponse.Error)resp;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errResp.Exception.ErrorCode);
    }

    // Tests: creating a cache, listing a cache, and deleting a cache.
    [Fact]
    public void ListCaches_OneCache_HappyPath()
    {
        // Create cache
        string cacheName = Utils.NewGuidString();
        client.CreateCache(cacheName);

        // Test cache exists
        ListCachesResponse result = client.ListCaches();
        if (result is ListCachesResponse.Success successResult) {
            List<CacheInfo> caches = successResult.Caches;
            Assert.Contains(new CacheInfo(cacheName), caches);
        }

        // Test deleting cache
        client.DeleteCache(cacheName);
        result = client.ListCaches();
        if (result is ListCachesResponse.Success successResult2) {
            var caches = successResult2.Caches;
            Assert.DoesNotContain(new CacheInfo(cacheName), caches);
        }
    }

    [Fact]
    public void ListCaches_Iteration_HappyPath()
    {
        // Create caches
        List<String> cacheNames = new List<String>();

        // TODO: increase limit after pagination is enabled
        foreach (int val in Enumerable.Range(1, 5))
        {
            String cacheName = Utils.NewGuidString();
            cacheNames.Add(cacheName);
            client.CreateCache(cacheName);
        }

        // List caches
        HashSet<String> retrievedCaches = new HashSet<string>();
        ListCachesResponse result = client.ListCaches();
        while (true)
        {
            if (result is ListCachesResponse.Success successResult) {
                foreach (CacheInfo cache in successResult.Caches)
                {
                    retrievedCaches.Add(cache.Name);
                }
                if (successResult.NextPageToken == null)
                {
                    break;
                }
                result = client.ListCaches(successResult.NextPageToken);
            }
        }


        int sizeOverlap = cacheNames.Intersect(retrievedCaches).Count();

        // Cleanup
        foreach (String cacheName in cacheNames)
        {
            client.DeleteCache(cacheName);
        }

        Assert.True(sizeOverlap == cacheNames.Count);
    }

    [Fact]
    public void ListCaches_BadNextToken_NoException()
    {
        // A bad next token does not throw an exception
        client.ListCaches(nextPageToken: "hello world");
    }
}
