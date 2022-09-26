﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Momento.Sdk.Config;

namespace Momento.Sdk.Tests;

[Collection("SimpleCacheClient")]
public class SimpleCacheDataTest
{
    private readonly string authToken;
    private readonly string cacheName;
    private const uint DefaultTtlSeconds = SimpleCacheClientFixture.DefaultTtlSeconds;
    private SimpleCacheClient client;

    // Test initialization
    public SimpleCacheDataTest(SimpleCacheClientFixture fixture)
    {
        client = fixture.Client;
        authToken = fixture.AuthToken;
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
    public async Task GetBatchAsync_NullCheckByteArray_ThrowsException()
    {
        CacheGetBatchResponse response = await client.GetBatchAsync(null!, new List<byte[]>());
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheGetBatchResponse.Error)response).ErrorCode);
        response = await client.GetBatchAsync("cache", (List<byte[]>)null!);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheGetBatchResponse.Error)response).ErrorCode);
        var badList = new List<byte[]>(new byte[][] { Utils.NewGuidByteArray(), null! });
        response = await client.GetBatchAsync("cache", badList);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheGetBatchResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task GetBatchAsync_KeysAreByteArray_HappyPath()
    {
        string key1 = Utils.NewGuidString();
        string value1 = Utils.NewGuidString();
        string key2 = Utils.NewGuidString();
        string value2 = Utils.NewGuidString();
        await client.SetAsync(cacheName, key1, value1);
        await client.SetAsync(cacheName, key2, value2);

        List<byte[]> keys = new() { Utils.Utf8ToByteArray(key1), Utils.Utf8ToByteArray(key2) };

        CacheGetBatchResponse result = await client.GetBatchAsync(cacheName, keys);
        var goodResult = (CacheGetBatchResponse.Success)result;
        string? stringResult1 = goodResult.Strings().ToList()[0];
        string? stringResult2 = goodResult.Strings().ToList()[1];
        Assert.Equal(value1, stringResult1);
        Assert.Equal(value2, stringResult2);
    }

    [Fact]
    public async Task GetBatchAsync_NullCheckString_ThrowsException()
    {
        CacheGetBatchResponse response = await client.GetBatchAsync(null!, new List<string>());
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheGetBatchResponse.Error)response).ErrorCode);
        response = await client.GetBatchAsync("cache", (List<string>)null!);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheGetBatchResponse.Error)response).ErrorCode);

        List<string> strings = new(new string[] { "key1", "key2", null! });
        response = await client.GetBatchAsync("cache", strings);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheGetBatchResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task GetBatchAsync_KeysAreString_HappyPath()
    {
        string key1 = Utils.NewGuidString();
        string value1 = Utils.NewGuidString();
        string key2 = Utils.NewGuidString();
        string value2 = Utils.NewGuidString();
        await client.SetAsync(cacheName, key1, value1);
        await client.SetAsync(cacheName, key2, value2);

        List<string> keys = new() { key1, key2, "key123123" };
        CacheGetBatchResponse result = await client.GetBatchAsync(cacheName, keys);

        var goodResult = (CacheGetBatchResponse.Success)result;
        Assert.Equal(goodResult.Strings(), new string[] { value1, value2, null! });
        Assert.True(goodResult.Responses[0] is CacheGetResponse.Hit);
        Assert.True(goodResult.Responses[1] is CacheGetResponse.Hit);
        Assert.True(goodResult.Responses[2] is CacheGetResponse.Miss);
    }

    [Fact]
    public async Task GetBatchAsync_Failure()
    {
        // Set very small timeout for dataClientOperationTimeoutMilliseconds
        IConfiguration config = Configurations.Laptop.Latest;
        config = config.WithTransportStrategy(
            config.TransportStrategy.WithGrpcConfig(
                config.TransportStrategy.GrpcConfig.WithDeadlineMilliseconds(1)));

        using SimpleCacheClient simpleCacheClient = new SimpleCacheClient(config, authToken, DefaultTtlSeconds);
        List<string> keys = new() { Utils.NewGuidString(), Utils.NewGuidString(), Utils.NewGuidString(), Utils.NewGuidString() };
        CacheGetBatchResponse response = await simpleCacheClient.GetBatchAsync(cacheName, keys);
        var badResponse = (CacheGetBatchResponse.Error)response;
        Assert.Equal(MomentoErrorCode.TIMEOUT_ERROR, badResponse.ErrorCode);
    }

    [Fact]
    public async Task SetBatchAsync_NullCheckByteArray_ThrowsException()
    {
        CacheSetBatchResponse response = await client.SetBatchAsync(null!, new Dictionary<byte[], byte[]>());
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetBatchResponse.Error)response).ErrorCode);
        response = await client.SetBatchAsync("cache", (Dictionary<byte[], byte[]>)null!);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetBatchResponse.Error)response).ErrorCode);

        var badDictionary = new Dictionary<byte[], byte[]>() { { Utils.Utf8ToByteArray("asdf"), null! } };
        response = await client.SetBatchAsync("cache", badDictionary);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetBatchResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task SetBatchAsync_ItemsAreByteArray_HappyPath()
    {
        var key1 = Utils.NewGuidByteArray();
        var key2 = Utils.NewGuidByteArray();
        var value1 = Utils.NewGuidByteArray();
        var value2 = Utils.NewGuidByteArray();

        var dictionary = new Dictionary<byte[], byte[]>() {
                { key1, value1 },
                { key2, value2 }
            };
        await client.SetBatchAsync(cacheName, dictionary);

        var getResponse = await client.GetAsync(cacheName, key1);
        var goodGetResponse = (CacheGetResponse.Hit)getResponse;
        Assert.Equal(value1, goodGetResponse.ByteArray);

        getResponse = await client.GetAsync(cacheName, key2);
        goodGetResponse = (CacheGetResponse.Hit)getResponse;
        Assert.Equal(value2, goodGetResponse.ByteArray);
    }

    [Fact]
    public async Task SetBatchAsync_NullCheckStrings_ThrowsException()
    {
        CacheSetBatchResponse response = await client.SetBatchAsync(null!, new Dictionary<string, string>());
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetBatchResponse.Error)response).ErrorCode);
        response = await client.SetBatchAsync("cache", (Dictionary<string, string>)null!);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetBatchResponse.Error)response).ErrorCode);

        var badDictionary = new Dictionary<string, string>() { { "asdf", null! } };
        response = await client.SetBatchAsync("cache", badDictionary);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetBatchResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task SetBatchAsync_KeysAreString_HappyPath()
    {
        var key1 = Utils.NewGuidString();
        var key2 = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var value2 = Utils.NewGuidString();

        var dictionary = new Dictionary<string, string>() {
                { key1, value1 },
                { key2, value2 }
            };
        await client.SetBatchAsync(cacheName, dictionary);

        var getResponse = await client.GetAsync(cacheName, key1);
        var goodGetResponse = (CacheGetResponse.Hit)getResponse;
        Assert.Equal(value1, goodGetResponse.String());

        getResponse = await client.GetAsync(cacheName, key2);
        goodGetResponse = (CacheGetResponse.Hit)getResponse;
        Assert.Equal(value2, goodGetResponse.String());
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
