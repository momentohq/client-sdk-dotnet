using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;

namespace Momento.Sdk.Tests;

[Collection("SimpleCacheClient")]
public class SimpleCacheDataTest
{
    private readonly ICredentialProvider authProvider;
    private readonly string cacheName;
    private const uint DefaultTtlSeconds = SimpleCacheClientFixture.DefaultTtlSeconds;
    private SimpleCacheClient client;

    // Test initialization
    public SimpleCacheDataTest(SimpleCacheClientFixture fixture)
    {
        client = fixture.Client;
        authProvider = fixture.AuthProvider;
        cacheName = fixture.CacheName;
    }

    [Theory]
    [InlineData(null, new byte[] { 0x00 }, new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", new byte[] { 0x00 }, null)]
    public async Task SetAsync_NullChecksByteArrayByteArray_ThrowsException(string cacheName, byte[] key, byte[] value)
    {
        CacheSetResponse response = await client.SetAsync(cacheName, key, value);
        var errorResponse = (CacheSetResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);
        response = await client.SetAsync(cacheName, key, value, DefaultTtlSeconds);
        errorResponse = (CacheSetResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);
    }

    // Tests SetAsyc(cacheName, byte[], byte[]) as well as GetAsync(cacheName, byte[])
    [Fact]
    public async Task SetAsync_KeyIsByteArrayValueIsByteArray_HappyPath()
    {
        byte[] key = Utils.NewGuidByteArray();
        byte[] value = Utils.NewGuidByteArray();
        await client.SetAsync(cacheName, key, value);
        CacheGetResponse response = await client.GetAsync(cacheName, key);
        var goodResponse = (CacheGetResponse.Hit)response;
        byte[]? setValue = goodResponse.ByteArray;
        Assert.Equal(value, setValue);

        key = Utils.NewGuidByteArray();
        value = Utils.NewGuidByteArray();
        await client.SetAsync(cacheName, key, value, ttlSeconds: 15);
        response = await client.GetAsync(cacheName, key);
        goodResponse = (CacheGetResponse.Hit)response;
        setValue = goodResponse.ByteArray;
        Assert.Equal(value, setValue);
    }

    [Theory]
    [InlineData(null, new byte[] { 0x00 })]
    [InlineData("cache", null)]
    public async Task GetAsync_NullChecksByteArray_ThrowsException(string cacheName, byte[] key)
    {
        CacheGetResponse response = await client.GetAsync(cacheName, key);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheGetResponse.Error)response).ErrorCode);
    }

    [Theory]
    [InlineData(null, "key", "value")]
    [InlineData("cache", null, "value")]
    [InlineData("cache", "key", null)]
    public async Task SetAsync_NullChecksStringString_ThrowsException(string cacheName, string key, string value)
    {
        CacheSetResponse response = await client.SetAsync(cacheName, key, value);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetResponse.Error)response).ErrorCode);
        response = await client.SetAsync(cacheName, key, value, DefaultTtlSeconds);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetResponse.Error)response).ErrorCode);
    }

    // Also tests GetAsync(cacheName, string)
    [Fact]
    public async Task SetAsync_KeyIsStringValueIsString_HappyPath()
    {
        string key = Utils.NewGuidString();
        string value = Utils.NewGuidString();
        await client.SetAsync(cacheName, key, value);
        CacheGetResponse response = await client.GetAsync(cacheName, key);
        var goodResponse = (CacheGetResponse.Hit)response;
        string? setValue = goodResponse.String();
        Assert.Equal(value, setValue);

        key = Utils.NewGuidString();
        value = Utils.NewGuidString();
        await client.SetAsync(cacheName, key, value, ttlSeconds: 15);
        response = await client.GetAsync(cacheName, key);
        goodResponse = (CacheGetResponse.Hit)response;
        setValue = goodResponse.String();
        Assert.Equal(value, setValue);
    }

    [Theory]
    [InlineData(null, "key")]
    [InlineData("cache", null)]
    public async Task GetAsync_NullChecksString_ThrowsException(string cacheName, string key)
    {
        CacheGetResponse response = await client.GetAsync(cacheName, key);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheGetResponse.Error)response).ErrorCode);
    }

    [Theory]
    [InlineData(null, "key", new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", "key", null)]
    public async Task SetAsync_NullChecksStringByteArray_ThrowsException(string cacheName, string key, byte[] value)
    {
        CacheSetResponse response = await client.SetAsync(cacheName, key, value);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetResponse.Error)response).ErrorCode);
        response = await client.SetAsync(cacheName, key, value, DefaultTtlSeconds);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetResponse.Error)response).ErrorCode);
    }

    // Also tests GetAsync(cacheName, string)
    [Fact]
    public async Task SetAsync_KeyIsStringValueIsByteArray_HappyPath()
    {
        string key = Utils.NewGuidString();
        byte[] value = Utils.NewGuidByteArray();
        await client.SetAsync(cacheName, key, value);
        CacheGetResponse response = await client.GetAsync(cacheName, key);
        var goodResponse = (CacheGetResponse.Hit)response;
        byte[]? setValue = goodResponse.ByteArray;
        Assert.Equal(value, setValue);

        key = Utils.NewGuidString();
        value = Utils.NewGuidByteArray();
        await client.SetAsync(cacheName, key, value, ttlSeconds: 15);
        response = await client.GetAsync(cacheName, key);
        var anotherGoodResponse = (CacheGetResponse.Hit)response;
        byte[]? anotherSetValue = anotherGoodResponse.ByteArray;
        Assert.Equal(value, anotherSetValue);
    }

    [Fact]
    public async Task GetAsync_ExpiredTtl_HappyPath()
    {
        string key = Utils.NewGuidString();
        string value = Utils.NewGuidString();
        await client.SetAsync(cacheName, key, value, 1);
        await Task.Delay(3000);
        CacheGetResponse result = await client.GetAsync(cacheName, key);
        Assert.True(result is CacheGetResponse.Miss);
    }

    [Theory]
    [InlineData(null, new byte[] { 0x00 })]
    [InlineData("cache", null)]
    public async Task DeleteAsync_NullChecksByte_ThrowsException(string cacheName, byte[] key)
    {
        CacheDeleteResponse result = await client.DeleteAsync(cacheName, key);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDeleteResponse.Error)result).ErrorCode);
    }

    [Fact]
    public async Task DeleteAsync_KeyIsByteArray_HappyPath()
    {
        // Set a key to then delete
        byte[] key = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        byte[] value = new byte[] { 0x05, 0x06, 0x07, 0x08 };
        await client.SetAsync(cacheName, key, value, ttlSeconds: 60);
        CacheGetResponse getResponse = await client.GetAsync(cacheName, key);
        Assert.True(getResponse is CacheGetResponse.Hit);

        // Delete
        await client.DeleteAsync(cacheName, key);

        // Check deleted
        getResponse = await client.GetAsync(cacheName, key);
        Assert.True(getResponse is CacheGetResponse.Miss);
    }

    [Theory]
    [InlineData(null, "key")]
    [InlineData("cache", null)]
    public async Task DeleteAsync_NullChecksString_ThrowsException(string cacheName, string key)
    {
        CacheDeleteResponse response = await client.DeleteAsync(cacheName, key);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDeleteResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task DeleteAsync_KeyIsString_HappyPath()
    {
        // Set a key to then delete
        string key = Utils.NewGuidString();
        string value = Utils.NewGuidString();
        await client.SetAsync(cacheName, key, value, ttlSeconds: 60);
        CacheGetResponse getResponse = await client.GetAsync(cacheName, key);
        Assert.True(getResponse is CacheGetResponse.Hit);

        // Delete
        await client.DeleteAsync(cacheName, key);

        // Check deleted
        getResponse = await client.GetAsync(cacheName, key);
        Assert.True(getResponse is CacheGetResponse.Miss);
    }
}
