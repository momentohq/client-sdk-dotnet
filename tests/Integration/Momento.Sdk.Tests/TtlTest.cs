using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Momento.Sdk.Internal.ExtensionMethods;

namespace Momento.Sdk.Tests;

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
        var response = await client.SetAsync(cacheName, key, Utils.NewGuidByteArray());
        Assert.True(response is CacheSetResponse.Success, $"Unexpected response: {response}");

        // Check it is there
        var existsResponse = await client.KeyExistsAsync(cacheName, key);
        Assert.True(existsResponse is CacheKeyExistsResponse.Success, "exists response should be success");
        Assert.True(((CacheKeyExistsResponse.Success)existsResponse).Exists, "Key should exist");

        // Let's make the TTL really small.
        var updateTtlResponse = await client.UpdateTtlAsync(cacheName, key, TimeSpan.FromSeconds(1));
        Assert.True(updateTtlResponse is CacheUpdateTtlResponse.Set, $"UpdateTtl call should have been Set but was: {response}");

        // Wait for the TTL to expire
        await Task.Delay(1000);

        // Check it is gone
        existsResponse = await client.KeyExistsAsync(cacheName, key);
        Assert.True(existsResponse is CacheKeyExistsResponse.Success, "exists response should be success");
        Assert.False(((CacheKeyExistsResponse.Success)existsResponse).Exists, "Key should not exist");
    }

    [Fact]
    public async Task UpdateTtlAsync_KeyIsStringAndExists_IsSet()
    {
        // Add an item with a minute ttl
        string key = Utils.NewGuidString();
        var ttl = TimeSpan.FromSeconds(60);
        var response = await client.SetAsync(cacheName, key, Utils.NewGuidString());
        Assert.True(response is CacheSetResponse.Success, $"Unexpected response: {response}");

        // Check it is there
        var existsResponse = await client.KeyExistsAsync(cacheName, key);
        Assert.True(existsResponse is CacheKeyExistsResponse.Success, "exists response should be success");
        Assert.True(((CacheKeyExistsResponse.Success)existsResponse).Exists, "Key should exist");

        // Let's make the TTL really small.
        var updateTtlResponse = await client.UpdateTtlAsync(cacheName, key, TimeSpan.FromSeconds(1));
        Assert.True(updateTtlResponse is CacheUpdateTtlResponse.Set, $"UpdateTtl call should have been Set but was: {response}");

        // Wait for the TTL to expire
        await Task.Delay(1000);

        // Check it is gone
        existsResponse = await client.KeyExistsAsync(cacheName, key);
        Assert.True(existsResponse is CacheKeyExistsResponse.Success, "exists response should be success");
        Assert.False(((CacheKeyExistsResponse.Success)existsResponse).Exists, "Key should not exist");
    }
}
