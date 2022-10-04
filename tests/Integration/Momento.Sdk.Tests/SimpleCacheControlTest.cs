using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;

namespace Momento.Sdk.Tests;

[Collection("SimpleCacheClient")]
public class SimpleCacheControlTest
{
    private SimpleCacheClient client;
    private ICredentialProvider authProvider;

    public SimpleCacheControlTest(SimpleCacheClientFixture fixture)
    {
        client = fixture.Client;
        authProvider = fixture.AuthProvider;
    }

    [Fact]
    public void SimpleCacheClientConstructor_BadJWT_InvalidJwtException()
    {
        Environment.SetEnvironmentVariable("BAD_MOMENTO_AUTH_TOKEN", "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJpbnRlZ3JhdGlvbiJ9.ZOgkTs");
        Assert.Throws<InvalidArgumentException>(
            () => new EnvironmentTokenProvider("BAD_MOMENTO_AUTH_TOKEN")
        );
    }

    [Fact]
    public void SimpleCacheClientConstructor_NullJWT_InvalidJwtException()
    {
        Environment.SetEnvironmentVariable("BAD_MOMENTO_AUTH_TOKEN", null);
        Assert.Throws<InvalidArgumentException>(
            () => new EnvironmentTokenProvider("BAD_MOMENTO_AUTH_TOKEN")
        );
    }

    [Fact]
    public async Task DeleteCacheAsync_NullCache_InvalidArgumentError()
    {
        DeleteCacheResponse resp = await client.DeleteCacheAsync(null!);
        DeleteCacheResponse.Error errResp = (DeleteCacheResponse.Error)resp;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errResp.Exception.ErrorCode);
    }

    [Fact]
    public async Task DeleteCacheAsync_CacheDoesntExist_NotFoundException()
    {
        // Assert.Throws<NotFoundException>(() => client.DeleteCache("non-existent cache"));
        DeleteCacheResponse resp = await client.DeleteCacheAsync("non-existent cache");
        Assert.True(resp is DeleteCacheResponse.Error);
        DeleteCacheResponse.Error errResp = (DeleteCacheResponse.Error)resp;
        Assert.Equal(MomentoErrorCode.NOT_FOUND_ERROR, errResp.Exception.ErrorCode);
    }

    [Fact]
    public async Task CreateCacheAsync_NullCache_InvalidArgumentError()
    {
        CreateCacheResponse resp = await client.CreateCacheAsync(null!);
        CreateCacheResponse.Error errResp = (CreateCacheResponse.Error)resp;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errResp.Exception.ErrorCode);
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
        if (result is ListCachesResponse.Success successResult)
        {
            List<CacheInfo> caches = successResult.Caches;
            Assert.Contains(new CacheInfo(cacheName), caches);
        }

        // Test deleting cache
        await client.DeleteCacheAsync(cacheName);
        result = await client.ListCachesAsync();
        if (result is ListCachesResponse.Success successResult2)
        {
            var caches = successResult2.Caches;
            Assert.DoesNotContain(new CacheInfo(cacheName), caches);
        }
    }

    [Fact]
    public async Task ListCachesAsync_Iteration_HappyPath()
    {
        // Create caches
        List<String> cacheNames = new List<String>();

        // TODO: increase limit after pagination is enabled
        foreach (int val in Enumerable.Range(1, 5))
        {
            String cacheName = Utils.NewGuidString();
            cacheNames.Add(cacheName);
            await client.CreateCacheAsync(cacheName);
        }

        // List caches
        HashSet<String> retrievedCaches = new HashSet<string>();
        ListCachesResponse result = await client.ListCachesAsync();
        while (true)
        {
            if (result is ListCachesResponse.Success successResult)
            {
                foreach (CacheInfo cache in successResult.Caches)
                {
                    retrievedCaches.Add(cache.Name);
                }
                if (successResult.NextPageToken == null)
                {
                    break;
                }
                result = await client.ListCachesAsync(successResult.NextPageToken);
            }
        }


        int sizeOverlap = cacheNames.Intersect(retrievedCaches).Count();

        // Cleanup
        foreach (String cacheName in cacheNames)
        {
            await client.DeleteCacheAsync(cacheName);
        }

        Assert.True(sizeOverlap == cacheNames.Count);
    }

    [Fact]
    public async Task ListCachesAsync_BadNextToken_NoException()
    {
        // A bad next token does not throw an exception
        await client.ListCachesAsync(nextPageToken: "hello world");
    }
}
