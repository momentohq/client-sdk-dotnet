using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Momento.Sdk.Internal.ExtensionMethods;

namespace Momento.Sdk.Tests;

[Collection("CacheClient")]
public class CacheDataTest : TestBase
{
    // Test initialization
    public CacheDataTest(CacheClientFixture fixture) : base(fixture)
    {
    }


    [Theory]
    [InlineData(null, new byte[] { 0x00 })]
    [InlineData("cache", null)]
    public async Task KeyExistsAsync_NullChecksByteArray_IsError(string cacheName, byte[] key)
    {
        CacheKeyExistsResponse response = await client.KeyExistsAsync(cacheName, key);
        Assert.True(response is CacheKeyExistsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheKeyExistsResponse.Error)response).ErrorCode);
    }


    [Theory]
    [InlineData(null, "foo")]
    [InlineData("cache", null)]
    public async Task KeyExistsAsync_NullChecksString_IsError(string cacheName, string key)
    {
        CacheKeyExistsResponse response = await client.KeyExistsAsync(cacheName, key);
        Assert.True(response is CacheKeyExistsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheKeyExistsResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task KeyExistsAsync_ByteArray()
    {
        byte[] key = Utils.NewGuidByteArray();
        byte[] value = Utils.NewGuidByteArray();
        await client.SetAsync(cacheName, key, value);
        CacheKeyExistsResponse response = await client.KeyExistsAsync(cacheName, key);
        Assert.True(response is CacheKeyExistsResponse.Success, $"Unexpected response: {response}");
        var goodResponse = (CacheKeyExistsResponse.Success)response;
        bool exists = goodResponse.Exists;
        Assert.True(exists);

        key = Utils.NewGuidByteArray();
        response = await client.KeyExistsAsync(cacheName, key);
        Assert.True(response is CacheKeyExistsResponse.Success, $"Unexpected response: {response}");
        goodResponse = (CacheKeyExistsResponse.Success)response;
        exists = goodResponse.Exists;
        Assert.False(exists);
    }

    [Fact]
    public async Task KeyExistsAsync_String()
    {
        string key = Utils.NewGuidString();
        string value = Utils.NewGuidString();
        await client.SetAsync(cacheName, key, value);
        CacheKeyExistsResponse response = await client.KeyExistsAsync(cacheName, key);
        Assert.True(response is CacheKeyExistsResponse.Success, $"Unexpected response: {response}");
        var goodResponse = (CacheKeyExistsResponse.Success)response;
        bool exists = goodResponse.Exists;
        Assert.True(exists);

        key = Utils.NewGuidString();
        response = await client.KeyExistsAsync(cacheName, key);
        Assert.True(response is CacheKeyExistsResponse.Success, $"Unexpected response: {response}");
        goodResponse = (CacheKeyExistsResponse.Success)response;
        exists = goodResponse.Exists;
        Assert.False(exists);
    }

    [Fact]
    public async Task KeysExistAsync_NullChecksByteArray_IsError()
    {
        var validKeys = new List<byte[]> { new byte[] { 0x00 } };
        List<byte[]> nullKeys = null!;
        var oneNullKey = new List<byte[]> { null! };

        CacheKeysExistResponse response = await client.KeysExistAsync(null!, validKeys);
        Assert.True(response is CacheKeysExistResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheKeysExistResponse.Error)response).ErrorCode);
        response = await client.KeysExistAsync(cacheName, nullKeys);
        Assert.True(response is CacheKeysExistResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheKeysExistResponse.Error)response).ErrorCode);
        response = await client.KeysExistAsync(cacheName, oneNullKey);
        Assert.True(response is CacheKeysExistResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheKeysExistResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task KeysExistAsync_NullChecksString_IsError()
    {
        var validKeys = new List<string> { "foo" };
        List<byte[]> nullKeys = null!;
        var oneNullKey = new List<string> { null! };

        CacheKeysExistResponse response = await client.KeysExistAsync(null!, validKeys);
        Assert.True(response is CacheKeysExistResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheKeysExistResponse.Error)response).ErrorCode);
        response = await client.KeysExistAsync(cacheName, nullKeys);
        Assert.True(response is CacheKeysExistResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheKeysExistResponse.Error)response).ErrorCode);
        response = await client.KeysExistAsync(cacheName, oneNullKey);
        Assert.True(response is CacheKeysExistResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheKeysExistResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task KeysExistAsync_ByteArray()
    {
        byte[] key1 = Utils.NewGuidByteArray();
        byte[] key2 = Utils.NewGuidByteArray();
        byte[] key3 = Utils.NewGuidByteArray();

        await client.SetAsync(cacheName, key1, key1);
        await client.SetAsync(cacheName, key3, key3);

        var keys = new byte[][] { key1, key2, key3 };
        CacheKeysExistResponse response = await client.KeysExistAsync(cacheName, keys);
        Assert.True(response is CacheKeysExistResponse.Success, $"Unexpected response: {response}");
        var goodResponse = (CacheKeysExistResponse.Success)response;
        Assert.Equal(new List<bool> { true, false, true }, goodResponse.ExistsEnumerable.ToList());

        var keysInADifferentOrder = new byte[][] { key2, key3, key1 };
        response = await client.KeysExistAsync(cacheName, keysInADifferentOrder);
        Assert.True(response is CacheKeysExistResponse.Success, $"Unexpected response: {response}");
        goodResponse = (CacheKeysExistResponse.Success)response;
        Assert.Equal(new List<bool> { false, true, true }, goodResponse.ExistsEnumerable.ToList());
    }

    [Fact]
    public async Task KeysExistAsync_String()
    {
        var key1 = Utils.NewGuidString();
        var key2 = Utils.NewGuidString();
        var key3 = Utils.NewGuidString();

        await client.SetAsync(cacheName, key1, key1);
        await client.SetAsync(cacheName, key3, key3);

        var keys = new string[] { key1, key2, key3 };
        CacheKeysExistResponse response = await client.KeysExistAsync(cacheName, keys);
        Assert.True(response is CacheKeysExistResponse.Success, $"Unexpected response: {response}");
        var goodResponse = (CacheKeysExistResponse.Success)response;
        Assert.Equal(new List<bool> { true, false, true }, goodResponse.ExistsEnumerable.ToList());

        var expectedDict = new Dictionary<string, bool>
        {
            [key1] = true,
            [key2] = false,
            [key3] = true
        };
        Assert.Equal(expectedDict, goodResponse.ExistsDictionary);

        var keysInADifferentOrder = new string[] { key2, key3, key1 };
        response = await client.KeysExistAsync(cacheName, keysInADifferentOrder);
        Assert.True(response is CacheKeysExistResponse.Success, $"Unexpected response: {response}");
        goodResponse = (CacheKeysExistResponse.Success)response;
        Assert.Equal(new List<bool> { false, true, true }, goodResponse.ExistsEnumerable.ToList());
        Assert.Equal(expectedDict, goodResponse.ExistsDictionary);
    }

    [Theory]
    [InlineData(null, new byte[] { 0x00 }, new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", new byte[] { 0x00 }, null)]
    public async Task SetAsync_NullChecksByteArrayByteArray_IsError(string cacheName, byte[] key, byte[] value)
    {
        CacheSetResponse response = await client.SetAsync(cacheName, key, value);
        Assert.True(response is CacheSetResponse.Error, $"Unexpected response: {response}");
        var errorResponse = (CacheSetResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);

        response = await client.SetAsync(cacheName, key, value, defaultTtl);
        Assert.True(response is CacheSetResponse.Error, $"Unexpected response: {response}");
        errorResponse = (CacheSetResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task SetAsync_InvalidTTLByteArrayByteArray_IsError(int ttlSeconds)
    {
        CacheSetResponse response = await client.SetAsync(cacheName, new byte[] { 0x00 }, new byte[] { 0x00 }, TimeSpan.FromSeconds(ttlSeconds));
        Assert.True(response is CacheSetResponse.Error, $"Unexpected response: {response}");
        var errorResponse = (CacheSetResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);
    }

    // Tests SetAsync(cacheName, byte[], byte[]) as well as GetAsync(cacheName, byte[])
    [Fact]
    public async Task SetAsync_KeyIsByteArrayValueIsByteArray_HappyPath()
    {
        byte[] key = Utils.NewGuidByteArray();
        byte[] value = Utils.NewGuidByteArray();
        await client.SetAsync(cacheName, key, value);
        CacheGetResponse response = await client.GetAsync(cacheName, key);
        Assert.True(response is CacheGetResponse.Hit, $"Unexpected response: {response}");
        var goodResponse = (CacheGetResponse.Hit)response;
        byte[] setValue = goodResponse.ValueByteArray;
        Assert.Equal(value, setValue);

        key = Utils.NewGuidByteArray();
        value = Utils.NewGuidByteArray();
        await client.SetAsync(cacheName, key, value, ttl: TimeSpan.FromSeconds(15));
        response = await client.GetAsync(cacheName, key);
        Assert.True(response is CacheGetResponse.Hit, $"Unexpected response: {response}");
        goodResponse = (CacheGetResponse.Hit)response;
        setValue = goodResponse.ValueByteArray;
        Assert.Equal(value, setValue);
    }

    [Theory]
    [InlineData(null, new byte[] { 0x00 })]
    [InlineData("cache", null)]
    public async Task GetAsync_NullChecksByteArray_IsError(string cacheName, byte[] key)
    {
        CacheGetResponse response = await client.GetAsync(cacheName, key);
        Assert.True(response is CacheGetResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheGetResponse.Error)response).ErrorCode);
    }

    [Theory]
    [InlineData(null, "key", "value")]
    [InlineData("cache", null, "value")]
    [InlineData("cache", "key", null)]
    public async Task SetAsync_NullChecksStringString_IsError(string cacheName, string key, string value)
    {
        CacheSetResponse response = await client.SetAsync(cacheName, key, value);
        Assert.True(response is CacheSetResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetResponse.Error)response).ErrorCode);

        response = await client.SetAsync(cacheName, key, value, defaultTtl);
        Assert.True(response is CacheSetResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetResponse.Error)response).ErrorCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task SetAsync_InvalidTTLStringString_IsError(int ttlSeconds)
    {
        CacheSetResponse response = await client.SetAsync(cacheName, "hello", "world", TimeSpan.FromSeconds(ttlSeconds));
        Assert.True(response is CacheSetResponse.Error, $"Unexpected response: {response}");
        var errorResponse = (CacheSetResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);
    }

    // Also tests GetAsync(cacheName, string)
    [Fact]
    public async Task SetAsync_KeyIsStringValueIsString_HappyPath()
    {
        // Set without TTL
        string key = Utils.NewGuidString();
        string value = Utils.NewGuidString();
        var setResponse = await client.SetAsync(cacheName, key, value);
        Assert.True(setResponse is CacheSetResponse.Success, $"Unexpected response: {setResponse}");

        CacheGetResponse response = await client.GetAsync(cacheName, key);
        Assert.True(response is CacheGetResponse.Hit, $"Unexpected response: {response}");
        var goodResponse = (CacheGetResponse.Hit)response;
        string setValue = goodResponse.ValueString;
        Assert.Equal(value, setValue);

        // Set with TTL
        key = Utils.NewGuidString();
        value = Utils.NewGuidString();
        setResponse = await client.SetAsync(cacheName, key, value, ttl: TimeSpan.FromSeconds(15));
        Assert.True(setResponse is CacheSetResponse.Success, $"Unexpected response: {setResponse}");

        response = await client.GetAsync(cacheName, key);
        Assert.True(response is CacheGetResponse.Hit, $"Unexpected response: {response}");
        goodResponse = (CacheGetResponse.Hit)response;
        setValue = goodResponse.ValueString;
        Assert.Equal(value, setValue);
    }

    [Theory]
    [InlineData(null, "key")]
    [InlineData("cache", null)]
    public async Task GetAsync_NullChecksString_IsError(string cacheName, string key)
    {
        CacheGetResponse response = await client.GetAsync(cacheName, key);
        Assert.True(response is CacheGetResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheGetResponse.Error)response).ErrorCode);
    }

    [Theory]
    [InlineData(null, "key", new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", "key", null)]
    public async Task SetAsync_NullChecksStringByteArray_IsError(string cacheName, string key, byte[] value)
    {
        CacheSetResponse response = await client.SetAsync(cacheName, key, value);
        Assert.True(response is CacheSetResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetResponse.Error)response).ErrorCode);

        response = await client.SetAsync(cacheName, key, value, defaultTtl);
        Assert.True(response is CacheSetResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetResponse.Error)response).ErrorCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task SetAsync_InvalidTTLStringByteArray_IsError(int ttlSeconds)
    {
        CacheSetResponse response = await client.SetAsync(cacheName, "hello", new byte[] { 0x00 }, TimeSpan.FromSeconds(ttlSeconds));
        Assert.True(response is CacheSetResponse.Error, $"Unexpected response: {response}");
        var errorResponse = (CacheSetResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);
    }

    // Also tests GetAsync(cacheName, string)
    [Fact]
    public async Task SetAsync_KeyIsStringValueIsByteArray_HappyPath()
    {
        // Set without TTL
        string key = Utils.NewGuidString();
        byte[] value = Utils.NewGuidByteArray();
        var setResponse = await client.SetAsync(cacheName, key, value);
        Assert.True(setResponse is CacheSetResponse.Success, $"Unexpected response: {setResponse}");

        CacheGetResponse response = await client.GetAsync(cacheName, key);
        var goodResponse = (CacheGetResponse.Hit)response;
        byte[] setValue = goodResponse.ValueByteArray;
        Assert.Equal(value, setValue);

        // Set with TTL
        key = Utils.NewGuidString();
        value = Utils.NewGuidByteArray();
        setResponse = await client.SetAsync(cacheName, key, value, ttl: TimeSpan.FromSeconds(15));
        Assert.True(setResponse is CacheSetResponse.Success, $"Unexpected response: {setResponse}");

        response = await client.GetAsync(cacheName, key);
        var anotherGoodResponse = (CacheGetResponse.Hit)response;
        byte[] anotherSetValue = anotherGoodResponse.ValueByteArray;
        Assert.Equal(value, anotherSetValue);
    }

    [Fact]
    public async Task GetAsync_ExpiredTtl_HappyPath()
    {
        string key = Utils.NewGuidString();
        string value = Utils.NewGuidString();
        var setResponse = await client.SetAsync(cacheName, key, value, TimeSpan.FromSeconds(1));
        Assert.True(setResponse is CacheSetResponse.Success, $"Unexpected response: {setResponse}");
        await Task.Delay(3000);
        CacheGetResponse result = await client.GetAsync(cacheName, key);
        Assert.True(result is CacheGetResponse.Miss, $"Unexpected response: {result}");
    }

    [Theory]
    [InlineData(null, new byte[] { 0x00 })]
    [InlineData("cache", null)]
    public async Task DeleteAsync_NullChecksByte_IsError(string cacheName, byte[] key)
    {
        CacheDeleteResponse result = await client.DeleteAsync(cacheName, key);
        Assert.True(result is CacheDeleteResponse.Error, $"Unexpected response: {result}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDeleteResponse.Error)result).ErrorCode);
    }

    [Fact]
    public async Task DeleteAsync_KeyIsByteArray_HappyPath()
    {
        // Set a key to then delete
        byte[] key = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        byte[] value = new byte[] { 0x05, 0x06, 0x07, 0x08 };
        var setResponse = await client.SetAsync(cacheName, key, value, ttl: TimeSpan.FromMinutes(1));
        Assert.True(setResponse is CacheSetResponse.Success, $"Unexpected response: {setResponse}");
        CacheGetResponse getResponse = await client.GetAsync(cacheName, key);
        Assert.True(getResponse is CacheGetResponse.Hit, $"Unexpected response: {getResponse}");

        // Delete
        var deleteResponse = await client.DeleteAsync(cacheName, key);
        Assert.True(deleteResponse is CacheDeleteResponse.Success, $"Unexpected response: {deleteResponse}");

        // Check deleted
        getResponse = await client.GetAsync(cacheName, key);
        Assert.True(getResponse is CacheGetResponse.Miss, $"Unexpected response: {getResponse}");
    }

    [Theory]
    [InlineData(null, "key")]
    [InlineData("cache", null)]
    public async Task DeleteAsync_NullChecksString_IsError(string cacheName, string key)
    {
        CacheDeleteResponse response = await client.DeleteAsync(cacheName, key);
        Assert.True(response is CacheDeleteResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDeleteResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task DeleteAsync_KeyIsString_HappyPath()
    {
        // Set a key to then delete
        string key = Utils.NewGuidString();
        string value = Utils.NewGuidString();
        var setResponse = await client.SetAsync(cacheName, key, value, ttl: TimeSpan.FromMinutes(1));
        Assert.True(setResponse is CacheSetResponse.Success, $"Unexpected response: {setResponse}");
        CacheGetResponse getResponse = await client.GetAsync(cacheName, key);
        Assert.True(getResponse is CacheGetResponse.Hit, $"Unexpected response: {getResponse}");

        // Delete
        await client.DeleteAsync(cacheName, key);

        // Check deleted
        getResponse = await client.GetAsync(cacheName, key);
        Assert.True(getResponse is CacheGetResponse.Miss, $"Unexpected response: {getResponse}");
    }

    // Also tests GetAsync(cacheName, string)
    [Fact]
    public async Task SetIfNotExistsAsync_KeyIsStringValueIsString_HappyPath()
    {
        // Set without TTL
        string key = Utils.NewGuidString();
        string originalValue = Utils.NewGuidString();
        var setIfNotExistsResponse = await client.SetIfNotExistsAsync(cacheName, key, originalValue);
        Assert.True(setIfNotExistsResponse is CacheSetIfNotExistsResponse.Stored, $"Unexpected response: {setIfNotExistsResponse}");

        CacheGetResponse response = await client.GetAsync(cacheName, key);
        Assert.True(response is CacheGetResponse.Hit, $"Unexpected response: {response}");
        var goodResponse = (CacheGetResponse.Hit)response;
        string setValue = goodResponse.ValueString;
        Assert.Equal(originalValue, setValue);

        // Set an existing value without TTL
        string newValue = Utils.NewGuidString();
        setIfNotExistsResponse = await client.SetIfNotExistsAsync(cacheName, key, newValue);
        Assert.True(setIfNotExistsResponse is CacheSetIfNotExistsResponse.NotStored, $"Unexpected response: {setIfNotExistsResponse}");

        response = await client.GetAsync(cacheName, key);
        Assert.True(response is CacheGetResponse.Hit, $"Unexpected response: {response}");
        goodResponse = (CacheGetResponse.Hit)response;
        setValue = goodResponse.ValueString;
        Assert.Equal(originalValue, setValue);

        await client.DeleteAsync(cacheName, key);

        // Set with TTL
        key = Utils.NewGuidString();
        var value = Utils.NewGuidString();
        setIfNotExistsResponse = await client.SetIfNotExistsAsync(cacheName, key, value, ttl: TimeSpan.FromSeconds(15));
        Assert.True(setIfNotExistsResponse is CacheSetIfNotExistsResponse.Stored, $"Unexpected response: {setIfNotExistsResponse}");

        response = await client.GetAsync(cacheName, key);
        Assert.True(response is CacheGetResponse.Hit, $"Unexpected response: {response}");
        goodResponse = (CacheGetResponse.Hit)response;
        setValue = goodResponse.ValueString;
        Assert.Equal(value, setValue);

        await client.DeleteAsync(cacheName, key);

    }

    [Theory]
    [InlineData(null, "key", "value")]
    [InlineData("cache", null, "value")]
    [InlineData("cache", "key", null)]
    public async Task SetIfNotExistsAsync_NullChecksStringString_IsError(string cacheName, string key, string value)
    {
        CacheSetIfNotExistsResponse response = await client.SetIfNotExistsAsync(cacheName, key, value);
        Assert.True(response is CacheSetIfNotExistsResponse.Error, $"Unexpected response: {response}");
        var errorResponse = (CacheSetIfNotExistsResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);

        response = await client.SetIfNotExistsAsync(cacheName, key, value, defaultTtl);
        Assert.True(response is CacheSetIfNotExistsResponse.Error, $"Unexpected response: {response}");
        errorResponse = (CacheSetIfNotExistsResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task SetIfNotExistsAsync_InvalidTTLStringString_IsError(int ttlSeconds)
    {
        CacheSetIfNotExistsResponse response = await client.SetIfNotExistsAsync(cacheName, "key", "value", TimeSpan.FromSeconds(ttlSeconds));
        Assert.True(response is CacheSetIfNotExistsResponse.Error, $"Unexpected response: {response}");
        var errorResponse = (CacheSetIfNotExistsResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);
    }

    [Fact]
    public async Task SetIfNotExistsAsync_KeyIsByteArrayValueIsString_HappyPath()
    {
        // Set without TTL
        byte[] key = Utils.NewGuidByteArray();
        string originalValue = Utils.NewGuidString();
        var setIfNotExistsResponse = await client.SetIfNotExistsAsync(cacheName, key, originalValue);
        Assert.True(setIfNotExistsResponse is CacheSetIfNotExistsResponse.Stored, $"Unexpected response: {setIfNotExistsResponse}");

        CacheGetResponse response = await client.GetAsync(cacheName, key);
        Assert.True(response is CacheGetResponse.Hit, $"Unexpected response: {response}");
        var goodResponse = (CacheGetResponse.Hit)response;
        string setValue = goodResponse.ValueString;
        Assert.Equal(originalValue, setValue);

        // Set an existing value without TTL
        string newValue = Utils.NewGuidString();
        setIfNotExistsResponse = await client.SetIfNotExistsAsync(cacheName, key, newValue);
        Assert.True(setIfNotExistsResponse is CacheSetIfNotExistsResponse.NotStored, $"Unexpected response: {setIfNotExistsResponse}");

        response = await client.GetAsync(cacheName, key);
        Assert.True(response is CacheGetResponse.Hit, $"Unexpected response: {response}");
        goodResponse = (CacheGetResponse.Hit)response;
        setValue = goodResponse.ValueString;
        Assert.Equal(originalValue, setValue);

        await client.DeleteAsync(cacheName, key);

        // Set with TTL
        key = Utils.NewGuidByteArray();
        var value = Utils.NewGuidString();
        setIfNotExistsResponse = await client.SetIfNotExistsAsync(cacheName, key, value, ttl: TimeSpan.FromSeconds(15));
        Assert.True(setIfNotExistsResponse is CacheSetIfNotExistsResponse.Stored, $"Unexpected response: {setIfNotExistsResponse}");

        response = await client.GetAsync(cacheName, key);
        Assert.True(response is CacheGetResponse.Hit, $"Unexpected response: {response}");
        goodResponse = (CacheGetResponse.Hit)response;
        setValue = goodResponse.ValueString;
        Assert.Equal(value, setValue);

        await client.DeleteAsync(cacheName, key);

    }

    [Theory]
    [InlineData(null, new byte[] { 0x01 }, "value")]
    [InlineData("cache", null, "value")]
    [InlineData("cache", new byte[] { 0x01 }, null)]
    public async Task SetIfNotExistsAsync_NullChecksByteArrayString_IsError(string cacheName, byte[] key, string value)
    {
        CacheSetIfNotExistsResponse response = await client.SetIfNotExistsAsync(cacheName, key, value);
        Assert.True(response is CacheSetIfNotExistsResponse.Error, $"Unexpected response: {response}");
        var errorResponse = (CacheSetIfNotExistsResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);

        response = await client.SetIfNotExistsAsync(cacheName, key, value, defaultTtl);
        Assert.True(response is CacheSetIfNotExistsResponse.Error, $"Unexpected response: {response}");
        errorResponse = (CacheSetIfNotExistsResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task SetIfNotExistsAsync_InvalidTTLByteArrayString_IsError(int ttlSeconds)
    {
        CacheSetIfNotExistsResponse response = await client.SetIfNotExistsAsync(cacheName, new byte[] { 0x00 }, "value", TimeSpan.FromSeconds(ttlSeconds));
        Assert.True(response is CacheSetIfNotExistsResponse.Error, $"Unexpected response: {response}");
        var errorResponse = (CacheSetIfNotExistsResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);
    }

    [Fact]
    public async Task SetIfNotExistsAsync_KeyIsStringValueIsByteArray_HappyPath()
    {
        // Set without TTL
        string key = Utils.NewGuidString();
        byte[] originalValue = new byte[] { 0x00 };
        var setIfNotExistsResponse = await client.SetIfNotExistsAsync(cacheName, key, originalValue);
        Assert.True(setIfNotExistsResponse is CacheSetIfNotExistsResponse.Stored, $"Unexpected response: {setIfNotExistsResponse}");

        CacheGetResponse response = await client.GetAsync(cacheName, key);
        Assert.True(response is CacheGetResponse.Hit, $"Unexpected response: {response}");
        var goodResponse = (CacheGetResponse.Hit)response;
        byte[] setValue = goodResponse.ValueByteArray;
        Assert.Equal(originalValue, setValue);

        // Set an existing value without TTL
        byte[] newValue = new byte[] { 0x01 };
        setIfNotExistsResponse = await client.SetIfNotExistsAsync(cacheName, key, newValue);
        Assert.True(setIfNotExistsResponse is CacheSetIfNotExistsResponse.NotStored, $"Unexpected response: {setIfNotExistsResponse}");

        response = await client.GetAsync(cacheName, key);
        Assert.True(response is CacheGetResponse.Hit, $"Unexpected response: {response}");
        goodResponse = (CacheGetResponse.Hit)response;
        setValue = goodResponse.ValueByteArray;
        Assert.Equal(originalValue, setValue);

        await client.DeleteAsync(cacheName, key);

        // Set with TTL
        key = Utils.NewGuidString();
        byte[] value = new byte[] { 0x00 };
        setIfNotExistsResponse = await client.SetIfNotExistsAsync(cacheName, key, value, ttl: TimeSpan.FromSeconds(15));
        Assert.True(setIfNotExistsResponse is CacheSetIfNotExistsResponse.Stored, $"Unexpected response: {setIfNotExistsResponse}");

        response = await client.GetAsync(cacheName, key);
        Assert.True(response is CacheGetResponse.Hit, $"Unexpected response: {response}");
        goodResponse = (CacheGetResponse.Hit)response;
        setValue = goodResponse.ValueByteArray;
        Assert.Equal(value, setValue);

        await client.DeleteAsync(cacheName, key);

    }

    [Theory]
    [InlineData(null, "key", new byte[] { 0x01 })]
    [InlineData("cache", null, new byte[] { 0x01 })]
    [InlineData("cache", "key", null)]
    public async Task SetIfNotExistsAsync_NullChecksStringByteArray_IsError(string cacheName, string key, byte[] value)
    {
        CacheSetIfNotExistsResponse response = await client.SetIfNotExistsAsync(cacheName, key, value);
        Assert.True(response is CacheSetIfNotExistsResponse.Error, $"Unexpected response: {response}");
        var errorResponse = (CacheSetIfNotExistsResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);

        response = await client.SetIfNotExistsAsync(cacheName, key, value, defaultTtl);
        Assert.True(response is CacheSetIfNotExistsResponse.Error, $"Unexpected response: {response}");
        errorResponse = (CacheSetIfNotExistsResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task SetIfNotExistsAsync_InvalidTTLStringByteArray_IsError(int ttlSeconds)
    {
        CacheSetIfNotExistsResponse response = await client.SetIfNotExistsAsync(cacheName, "key", new byte[] { 0x00 }, TimeSpan.FromSeconds(ttlSeconds));
        Assert.True(response is CacheSetIfNotExistsResponse.Error, $"Unexpected response: {response}");
        var errorResponse = (CacheSetIfNotExistsResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);
    }

    [Fact]
    public async Task SetIfNotExistsAsync_KeyIsByteArrayValueIsByteArray_HappyPath()
    {
        // Set without TTL
        byte[] key = new byte[] { 0x00 };
        byte[] originalValue = new byte[] { 0x00 };
        var setIfNotExistsResponse = await client.SetIfNotExistsAsync(cacheName, key, originalValue);
        Assert.True(setIfNotExistsResponse is CacheSetIfNotExistsResponse.Stored, $"Unexpected response: {setIfNotExistsResponse}");

        CacheGetResponse response = await client.GetAsync(cacheName, key);
        Assert.True(response is CacheGetResponse.Hit, $"Unexpected response: {response}");
        var goodResponse = (CacheGetResponse.Hit)response;
        byte[] setValue = goodResponse.ValueByteArray;
        Assert.Equal(originalValue, setValue);

        // Set an existing value without TTL
        byte[] newValue = new byte[] { 0x01 };
        setIfNotExistsResponse = await client.SetIfNotExistsAsync(cacheName, key, newValue);
        Assert.True(setIfNotExistsResponse is CacheSetIfNotExistsResponse.NotStored, $"Unexpected response: {setIfNotExistsResponse}");

        response = await client.GetAsync(cacheName, key);
        Assert.True(response is CacheGetResponse.Hit, $"Unexpected response: {response}");
        goodResponse = (CacheGetResponse.Hit)response;
        setValue = goodResponse.ValueByteArray;
        Assert.Equal(originalValue, setValue);

        // Set with TTL
        key = new byte[] { 0x01 };
        var value = new byte[] { 0x01 };
        setIfNotExistsResponse = await client.SetIfNotExistsAsync(cacheName, key, value, ttl: TimeSpan.FromSeconds(15));
        Assert.True(setIfNotExistsResponse is CacheSetIfNotExistsResponse.Stored, $"Unexpected response: {setIfNotExistsResponse}");

        response = await client.GetAsync(cacheName, key);
        Assert.True(response is CacheGetResponse.Hit, $"Unexpected response: {response}");
        goodResponse = (CacheGetResponse.Hit)response;
        setValue = goodResponse.ValueByteArray;
        Assert.Equal(value, setValue);
    }


    [Theory]
    [InlineData(null, new byte[] { 0x01 }, new byte[] { 0x01 })]
    [InlineData("cache", null, new byte[] { 0x01 })]
    [InlineData("cache", new byte[] { 0x01 }, null)]
    public async Task SetIfNotExistsAsync_NullChecksByteArrayByteArray_IsError(string cacheName, byte[] key, byte[] value)
    {
        CacheSetIfNotExistsResponse response = await client.SetIfNotExistsAsync(cacheName, key, value);
        Assert.True(response is CacheSetIfNotExistsResponse.Error, $"Unexpected response: {response}");
        var errorResponse = (CacheSetIfNotExistsResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);

        response = await client.SetIfNotExistsAsync(cacheName, key, value, defaultTtl);
        Assert.True(response is CacheSetIfNotExistsResponse.Error, $"Unexpected response: {response}");
        errorResponse = (CacheSetIfNotExistsResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task SetIfNotExistsAsync_InvalidTTLByteArrayByteArray_IsError(int ttlSeconds)
    {
        CacheSetIfNotExistsResponse response = await client.SetIfNotExistsAsync(cacheName, new byte[] { 0x01 }, new byte[] { 0x00 }, TimeSpan.FromSeconds(ttlSeconds));
        Assert.True(response is CacheSetIfNotExistsResponse.Error, $"Unexpected response: {response}");
        var errorResponse = (CacheSetIfNotExistsResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);
    }

    [Fact]
    public async Task IncrementAsync_KeyIsString_HappyPath()
    {
        // Key does not exists; Set without TTL and default amount
        string key = Utils.NewGuidString();
        var incrementResponse = await client.IncrementAsync(cacheName, key);
        Assert.True(incrementResponse is CacheIncrementResponse.Success, $"Unexpected response: {incrementResponse}");
        var response = await client.GetAsync(cacheName, key);
        var goodResponse = (CacheGetResponse.Hit)response;
        string setValue = goodResponse.ValueString;
        string expectedValue = "1";
        Assert.Equal(setValue, expectedValue);

        // Key exists; Set without TTL and default amount
        incrementResponse = await client.IncrementAsync(cacheName, key);
        Assert.True(incrementResponse is CacheIncrementResponse.Success, $"Unexpected response: {incrementResponse}");
        response = await client.GetAsync(cacheName, key);
        goodResponse = (CacheGetResponse.Hit)response;
        setValue = goodResponse.ValueString;
        expectedValue = "2";
        Assert.Equal(setValue, expectedValue);

        // Key exists; Set without TTL and not default amount
        incrementResponse = await client.IncrementAsync(cacheName, key, 5);
        Assert.True(incrementResponse is CacheIncrementResponse.Success, $"Unexpected response: {incrementResponse}");
        response = await client.GetAsync(cacheName, key);
        goodResponse = (CacheGetResponse.Hit)response;
        setValue = goodResponse.ValueString;
        expectedValue = "7";
        Assert.Equal(setValue, expectedValue);

        // Set with TTL and not default amount
        key = Utils.NewGuidString();
        incrementResponse = await client.IncrementAsync(cacheName, key, 5, TimeSpan.FromSeconds(15));
        Assert.True(incrementResponse is CacheIncrementResponse.Success, $"Unexpected response: {incrementResponse}");
        response = await client.GetAsync(cacheName, key);
        goodResponse = (CacheGetResponse.Hit)response;
        setValue = goodResponse.ValueString;
        expectedValue = "5";
        Assert.Equal(setValue, expectedValue);
    }

    [Theory]
    [InlineData(null, "key")]
    [InlineData("cache", null)]
    public async Task IncrementAsync_NullChecksString_IsError(string cacheName, string key)
    {
        CacheIncrementResponse response = await client.IncrementAsync(cacheName, key);
        Assert.True(response is CacheIncrementResponse.Error, $"Unexpected response: {response}");
        var errorResponse = (CacheIncrementResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);

        response = await client.IncrementAsync(cacheName, key, 5, TimeSpan.FromSeconds(10));
        Assert.True(response is CacheIncrementResponse.Error, $"Unexpected response: {response}");
        errorResponse = (CacheIncrementResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task IncrementAsync_InvalidTTLString_IsError(int ttlSeconds)
    {
        CacheIncrementResponse response = await client.IncrementAsync(cacheName, "key", 1, TimeSpan.FromSeconds(ttlSeconds));
        Assert.True(response is CacheIncrementResponse.Error, $"Unexpected response: {response}");
        var errorResponse = (CacheIncrementResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);
    }

    [Fact]
    public async Task IncrementAsync_KeyIsByteArray_HappyPath()
    {
        // Key does not exists; Set without TTL and default amount
        byte[] key = new byte[] { 0x05 };
        var incrementResponse = await client.IncrementAsync(cacheName, key);
        Assert.True(incrementResponse is CacheIncrementResponse.Success, $"Unexpected response: {incrementResponse}");
        var response = await client.GetAsync(cacheName, key);
        var goodResponse = (CacheGetResponse.Hit)response;
        string setValue = goodResponse.ValueString;
        string expectedValue = "1";
        Assert.Equal(setValue, expectedValue);

        // Key exists; Set without TTL and default amount
        incrementResponse = await client.IncrementAsync(cacheName, key);
        Assert.True(incrementResponse is CacheIncrementResponse.Success, $"Unexpected response: {incrementResponse}");
        response = await client.GetAsync(cacheName, key);
        goodResponse = (CacheGetResponse.Hit)response;
        setValue = goodResponse.ValueString;
        expectedValue = "2";
        Assert.Equal(setValue, expectedValue);

        // Key exists; Set without TTL and not default amount
        incrementResponse = await client.IncrementAsync(cacheName, key, 5);
        Assert.True(incrementResponse is CacheIncrementResponse.Success, $"Unexpected response: {incrementResponse}");
        response = await client.GetAsync(cacheName, key);
        goodResponse = (CacheGetResponse.Hit)response;
        setValue = goodResponse.ValueString;
        expectedValue = "7";
        Assert.Equal(setValue, expectedValue);

        // Set with TTL and not default amount
        key = new byte[] { 0x06 };
        incrementResponse = await client.IncrementAsync(cacheName, key, 5, TimeSpan.FromSeconds(15));
        Assert.True(incrementResponse is CacheIncrementResponse.Success, $"Unexpected response: {incrementResponse}");
        response = await client.GetAsync(cacheName, key);
        goodResponse = (CacheGetResponse.Hit)response;
        setValue = goodResponse.ValueString;
        expectedValue = "5";
        Assert.Equal(setValue, expectedValue);
    }

    [Theory]
    [InlineData(null, new byte[] { 0x01 })]
    [InlineData("cache", null)]
    public async Task IncrementAsync_NullChecksByteArray_IsError(string cacheName, byte[] key)
    {
        CacheIncrementResponse response = await client.IncrementAsync(cacheName, key);
        Assert.True(response is CacheIncrementResponse.Error, $"Unexpected response: {response}");
        var errorResponse = (CacheIncrementResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);

        response = await client.IncrementAsync(cacheName, key, 5, TimeSpan.FromSeconds(10));
        Assert.True(response is CacheIncrementResponse.Error, $"Unexpected response: {response}");
        errorResponse = (CacheIncrementResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task IncrementAsync_InvalidTTLByteArray_IsError(int ttlSeconds)
    {
        CacheIncrementResponse response = await client.IncrementAsync(cacheName, new byte[] { 0x01 }, 1, TimeSpan.FromSeconds(ttlSeconds));
        Assert.True(response is CacheIncrementResponse.Error, $"Unexpected response: {response}");
        var errorResponse = (CacheIncrementResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, errorResponse.ErrorCode);
    }
}
