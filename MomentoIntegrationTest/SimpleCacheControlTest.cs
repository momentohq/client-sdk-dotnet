using System.Linq;
using System.Collections.Generic;

namespace MomentoIntegrationTest;

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
    public void SimpleCacheClientConstructor_BadRequestTimeout_ThrowsException()
    {
        Assert.Throws<InvalidArgumentException>(() => new SimpleCacheClient(authToken, defaultTtlSeconds: 10, dataClientOperationTimeoutMilliseconds: 0));
    }

    [Fact]
    public void SimpleCacheClientConstructor_BadJWT_InvalidJwtException()
    {
        Assert.Throws<InvalidArgumentException>(() => new SimpleCacheClient("eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJpbnRlZ3JhdGlvbiJ9.ZOgkTs", defaultTtlSeconds: 10));
    }

    [Fact]
    public void SimpleCacheClientConstructor_NullJWT_InvalidJwtException()
    {
        Assert.Throws<InvalidArgumentException>(() => new SimpleCacheClient(null!, defaultTtlSeconds: 10));
    }

    [Fact]
    public void DeleteCache_NullCache_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => client.DeleteCache(null!));
    }

    [Fact]
    public void DeleteCache_CacheDoesntExist_NotFoundException()
    {
        Assert.Throws<NotFoundException>(() => client.DeleteCache("non-existent cache"));
    }

    [Fact]
    public void CreateCache_NullCache_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => client.CreateCache(null!));
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
        List<CacheInfo> caches = result.Caches;
        Assert.Contains(new CacheInfo(cacheName), caches);

        // Test deleting cache
        client.DeleteCache(cacheName);
        result = client.ListCaches();
        caches = result.Caches;
        Assert.DoesNotContain(new CacheInfo(cacheName), caches);
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
            foreach (CacheInfo cache in result.Caches)
            {
                retrievedCaches.Add(cache.Name);
            }
            if (result.NextPageToken == null)
            {
                break;
            }
            result = client.ListCaches(result.NextPageToken);
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
