﻿using System.Collections.Generic;
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
        Assert.True(deleteResponse is DeleteCacheResponse.Error);
        DeleteCacheResponse.Error errorResponse = (DeleteCacheResponse.Error)deleteResponse;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.InnerException.ErrorCode);
    }

    [Fact]
    public async Task DeleteCacheAsync_CacheDoesntExist_NotFoundException()
    {
        DeleteCacheResponse response = await client.DeleteCacheAsync("non-existent cache");
        Assert.True(response is DeleteCacheResponse.Error);
        var errorResponse = (DeleteCacheResponse.Error)response;
        Assert.Equal(MomentoErrorCode.NOT_FOUND_ERROR, errorResponse.InnerException.ErrorCode);
    }

    [Fact]
    public async Task CreateCacheAsync_NullCache_InvalidArgumentError()
    {
        CreateCacheResponse response = await client.CreateCacheAsync(null!);
        Assert.True(response is CreateCacheResponse.Error);
        CreateCacheResponse.Error errorResponse = (CreateCacheResponse.Error)response;
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
        Assert.True(result is ListCachesResponse.Success);
        var successResult = (ListCachesResponse.Success)result;
        List<CacheInfo> caches = successResult.Caches;
        Assert.Contains(new CacheInfo(cacheName), caches);

        // Test deleting cache
        await client.DeleteCacheAsync(cacheName);
        result = await client.ListCachesAsync();
        Assert.True(result is ListCachesResponse.Success);
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
            Assert.True(result is ListCachesResponse.Success);
            var successResult = (ListCachesResponse.Success)result;
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
