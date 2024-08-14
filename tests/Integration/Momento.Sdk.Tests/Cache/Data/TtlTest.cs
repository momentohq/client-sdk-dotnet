using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Momento.Sdk.Internal.ExtensionMethods;

namespace Momento.Sdk.Tests.Integration.Cache.Data;

[Collection("CacheClient")]
public class TtlTest : TestBase
{
    // Test initialization
    public TtlTest(CacheClientFixture fixture) : base(fixture)
    {
    }


    [Theory]
    [InlineData(null, new byte[] { 0x00 }, 60)]
    [InlineData("cache", null, 60)]
    [InlineData(null, new byte[] { 0x00 }, -1)]
    public async Task UpdateTtlAsync_NullChecksByteArray_IsError(string cacheName, byte[] key, int ttlSeconds)
    {
        var ttl = TimeSpan.FromSeconds(ttlSeconds);
        var response = await client.UpdateTtlAsync(cacheName, key, ttl);
        Assert.True(response is CacheUpdateTtlResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheUpdateTtlResponse.Error)response).ErrorCode);
    }

    [Theory]
    [InlineData(null, "key", 60)]
    [InlineData("cache", null, 60)]
    [InlineData(null, "key", -1)]
    public async Task UpdateTtlAsync_NullChecksString_IsError(string cacheName, string key, int ttlSeconds)
    {
        var ttl = TimeSpan.FromSeconds(ttlSeconds);
        var response = await client.UpdateTtlAsync(cacheName, key, ttl);
        Assert.True(response is CacheUpdateTtlResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheUpdateTtlResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task UpdateTtlAsync_KeyIsByteArrayAndMissing_IsMiss()
    {
        byte[] key = Utils.NewGuidByteArray();
        var ttl = TimeSpan.FromSeconds(60);
        var response = await client.UpdateTtlAsync(cacheName, key, ttl);
        Assert.True(response is CacheUpdateTtlResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task UpdateTtlAsync_KeyIsStringAndMissing_IsMiss()
    {
        string key = Utils.NewGuidString();
        var ttl = TimeSpan.FromSeconds(60);
        var response = await client.UpdateTtlAsync(cacheName, key, ttl);
        Assert.True(response is CacheUpdateTtlResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task UpdateTtlAsync_KeyIsByteArrayAndExists_IsSet()
    {
        // Add an item with a minute ttl
        byte[] key = Utils.NewGuidByteArray();
        var ttl = TimeSpan.FromSeconds(60);
        var response = await client.SetAsync(cacheName, key, Utils.NewGuidByteArray(), ttl);
        Assert.True(response is CacheSetResponse.Success, $"Unexpected response: {response}");

        // Check it is there
        var existsResponse = await client.KeyExistsAsync(cacheName, key);
        Assert.True(existsResponse is CacheKeyExistsResponse.Success, $"Unexpected response: {response}");
        Assert.True(((CacheKeyExistsResponse.Success)existsResponse).Exists, "Key should exist");

        // Let's make the TTL really small.
        var updateTtlResponse = await client.UpdateTtlAsync(cacheName, key, TimeSpan.FromSeconds(1));
        Assert.True(updateTtlResponse is CacheUpdateTtlResponse.Set, $"UpdateTtl call should have been Set but was: {response}");

        // Wait for the TTL to expire
        await Task.Delay(2000);

        // Check it is gone
        existsResponse = await client.KeyExistsAsync(cacheName, key);
        Assert.True(existsResponse is CacheKeyExistsResponse.Success, $"Unexpected response: {response}");
        Assert.False(((CacheKeyExistsResponse.Success)existsResponse).Exists, $"Key {key} should not exist but it does");
    }

    [Fact]
    public async Task UpdateTtlAsync_KeyIsStringAndExists_IsSet()
    {
        // Add an item with a minute ttl
        string key = Utils.NewGuidString();
        var ttl = TimeSpan.FromSeconds(60);
        var response = await client.SetAsync(cacheName, key, Utils.NewGuidString(), ttl);
        Assert.True(response is CacheSetResponse.Success, $"Unexpected response: {response}");

        // Check it is there
        var existsResponse = await client.KeyExistsAsync(cacheName, key);
        Assert.True(existsResponse is CacheKeyExistsResponse.Success, $"Unexpected response: {response}");
        Assert.True(((CacheKeyExistsResponse.Success)existsResponse).Exists, $"Key {key} should exist but does not");

        // Let's make the TTL really small.
        var updateTtlResponse = await client.UpdateTtlAsync(cacheName, key, TimeSpan.FromSeconds(1));
        Assert.True(updateTtlResponse is CacheUpdateTtlResponse.Set, $"UpdateTtl call should have been Set but was: {response}");

        // Wait for the TTL to expire
        await Task.Delay(2000);

        // Check it is gone
        existsResponse = await client.KeyExistsAsync(cacheName, key);
        Assert.True(existsResponse is CacheKeyExistsResponse.Success, $"Unexpected response: {response}");
        Assert.False(((CacheKeyExistsResponse.Success)existsResponse).Exists, $"Key {key} should not exist but it does");
    }

    [Fact]
    public async Task ItemGetTtl_HappyPath()
    {
        // Add an item with a minute ttl
        string key = Utils.NewGuidString();
        var ttl = TimeSpan.FromSeconds(60);
        var response = await client.SetAsync(cacheName, key, Utils.NewGuidString(), ttl);
        Assert.True(response is CacheSetResponse.Success, $"Unexpected response: {response}");

        var ttlResponse = await client.ItemGetTtlAsync(cacheName, key);
        Assert.True(ttlResponse is CacheItemGetTtlResponse.Hit, $"Unexpected response: {ttlResponse}");
        var theTtl = ((CacheItemGetTtlResponse.Hit)ttlResponse).Value;
        Assert.True(theTtl.TotalMilliseconds < 60000,
            $"TTL should be less than 60 seconds but was {theTtl.TotalMilliseconds}");

        await Task.Delay(1000);

        var ttlResponse2 = await client.ItemGetTtlAsync(cacheName, key);
        Assert.True(ttlResponse2 is CacheItemGetTtlResponse.Hit, $"Unexpected response: {ttlResponse}");
        var theTtl2 = ((CacheItemGetTtlResponse.Hit)ttlResponse2).Value;

        Assert.True(theTtl2.TotalMilliseconds < theTtl.TotalMilliseconds,
            $"TTL should be less than the previous TTL {theTtl.TotalMilliseconds} but was {theTtl2.TotalMilliseconds}");
    }

    [Fact]
    public async Task ItemGetTtl_Miss()
    {
        var ttlResponse = await client.ItemGetTtlAsync(cacheName, Utils.NewGuidString());
        Assert.True(ttlResponse is CacheItemGetTtlResponse.Miss, $"Unexpected response: {ttlResponse}");
    }

    [Fact]
    public async Task ItemGetTtl_NonexistentCacheError()
    {
        var ttlResponse = await client.ItemGetTtlAsync(Utils.NewGuidString(), Utils.NewGuidString());
        Assert.True(ttlResponse is CacheItemGetTtlResponse.Error, $"Unexpected response: {ttlResponse}");
        Assert.Equal(MomentoErrorCode.NOT_FOUND_ERROR, ((CacheItemGetTtlResponse.Error)ttlResponse).ErrorCode);
    }

    [Fact]
    public async Task ItemGetTtl_EmptyCacheNameError()
    {
        var ttlResponse = await client.ItemGetTtlAsync(null!, Utils.NewGuidString());
        Assert.True(ttlResponse is CacheItemGetTtlResponse.Error, $"Unexpected response: {ttlResponse}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheItemGetTtlResponse.Error)ttlResponse).ErrorCode);
    }
}
