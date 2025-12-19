using Momento.Sdk.Auth;
using Momento.Sdk.Internal.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Momento.Sdk.Tests.Integration.Cache;

[Collection("CacheClientApiKeyV2")]
public class CacheTestApiKeyV2 : IClassFixture<CacheClientApiKeyV2Fixture>
{
    protected readonly ICredentialProvider authProvider;
    protected readonly string cacheName;
    protected TimeSpan defaultTtl;
    protected ICacheClient client;
    public CacheTestApiKeyV2(CacheClientApiKeyV2Fixture fixture)
    {
        this.client = fixture.Client;
        this.cacheName = fixture.CacheName;
        this.authProvider = fixture.AuthProvider;
        this.defaultTtl = fixture.DefaultTtl;
    }

    // control plane

    [Fact]
    public async Task FlushCacheAsync_HappyPath()
    {
        string cacheName = Utils.TestCacheName();
        Utils.CreateCacheForTest(client, cacheName);

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
    public async Task CreateListDeleteACache_HappyPath()
    {
        // Create cache
        string cacheName = Utils.TestCacheName();
        Utils.CreateCacheForTest(client, cacheName);

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

    // get set delete

    [Fact]
    public async Task SetAndGetAsync_Bytes_HappyPath()
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

    // other cache methods

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

    // dictionary

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsByteArrayValueIsByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();

        await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value);

        CacheDictionaryGetFieldResponse getResponse = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(getResponse is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {getResponse}");
        var hitResponse = (CacheDictionaryGetFieldResponse.Hit)getResponse;
        Assert.Equal(field, hitResponse.FieldByteArray);
    }

    [Fact]
    public async Task DictionaryIncrementAsync_IncrementFromZero_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var fieldName = Utils.NewGuidString();

        CacheDictionaryIncrementResponse incrementResponse = await client.DictionaryIncrementAsync(cacheName, dictionaryName, fieldName, 1);
        Assert.True(incrementResponse is CacheDictionaryIncrementResponse.Success, $"Unexpected response: {incrementResponse}");
        var successResponse = (CacheDictionaryIncrementResponse.Success)incrementResponse;
        Assert.Equal(1, successResponse.Value);

        incrementResponse = await client.DictionaryIncrementAsync(cacheName, dictionaryName, fieldName, 41);
        Assert.True(incrementResponse is CacheDictionaryIncrementResponse.Success, $"Unexpected response: {incrementResponse}");
        successResponse = (CacheDictionaryIncrementResponse.Success)incrementResponse;
        Assert.Equal(42, successResponse.Value);
        Assert.Equal("Momento.Sdk.Responses.CacheDictionaryIncrementResponse+Success: Value: 42", successResponse.ToString());

        incrementResponse = await client.DictionaryIncrementAsync(cacheName, dictionaryName, fieldName, -1042);
        Assert.True(incrementResponse is CacheDictionaryIncrementResponse.Success, $"Unexpected response: {incrementResponse}");
        successResponse = (CacheDictionaryIncrementResponse.Success)incrementResponse;
        Assert.Equal(-1000, successResponse.Value);

        CacheDictionaryGetFieldResponse getResponse = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, fieldName);
        Assert.True(getResponse is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {getResponse}");
        var hitResponse = (CacheDictionaryGetFieldResponse.Hit)getResponse;
        Assert.Equal("-1000", hitResponse.ValueString);
    }

    [Fact]
    public async Task CacheDictionaryFetchResponse_ToString_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, "a", "b");

        CacheDictionaryFetchResponse fetchResponse = await client.DictionaryFetchAsync(cacheName, dictionaryName);

        Assert.True(fetchResponse is CacheDictionaryFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheDictionaryFetchResponse.Hit)fetchResponse;
        Assert.Equal("Momento.Sdk.Responses.CacheDictionaryFetchResponse+Hit: ValueDictionaryStringString: {\"a\": \"b\"} ValueDictionaryByteArrayByteArray: {\"61\": \"62\"}", hitResponse.ToString());
    }

    [Fact]
    public async Task DictionaryDeleteAsync_DictionaryExists_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, Utils.NewGuidString(), Utils.NewGuidString());
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, Utils.NewGuidString(), Utils.NewGuidString());
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, Utils.NewGuidString(), Utils.NewGuidString());

        var fetchResponse = await client.DictionaryFetchAsync(cacheName, dictionaryName);
        Assert.True(fetchResponse is CacheDictionaryFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var deleteResponse = await client.DeleteAsync(cacheName, dictionaryName);
        Assert.True(deleteResponse is CacheDeleteResponse.Success, $"Unexpected response: {deleteResponse}");
        var response = await client.DictionaryFetchAsync(cacheName, dictionaryName);
        Assert.True(response is CacheDictionaryFetchResponse.Miss, $"Unexpected response: {fetchResponse}");
    }

    [Fact]
    public async Task DictionaryRemoveFieldAsync_FieldIsByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidByteArray();
        var value1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidByteArray();

        // Add a field then delete it
        var response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field1);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, field1, value1);
        response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field1);
        Assert.True(response is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {response}");

        await client.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field1);
        response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field1);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");

        // Test no-op
        response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field2);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
        await client.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field2);
        response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field2);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task DictionaryLengthAsync_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();

        await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value);

        CacheDictionaryLengthResponse lengthResponse = await client.DictionaryLengthAsync(cacheName, dictionaryName);
        Assert.True(lengthResponse is CacheDictionaryLengthResponse.Hit, $"Unexpected response: {lengthResponse}");
        var hitResponse = (CacheDictionaryLengthResponse.Hit)lengthResponse;
        Assert.Equal(1, hitResponse.Length);
    }

    // list

    [Fact]
    public async Task ListFetchAsync_WithPositiveStartEndIndices_HappyPath()
    {
        var listName = Utils.NewGuidString();
        string value1 = Utils.NewGuidString();
        string value2 = Utils.NewGuidString();
        string value3 = Utils.NewGuidString();
        string value4 = Utils.NewGuidString();
        string[] values = new string[] { value1, value2, value3, value4 };
        CacheListConcatenateFrontResponse response = await client.ListConcatenateFrontAsync(cacheName, listName, values);
        Assert.True(response is CacheListConcatenateFrontResponse.Success, $"Unexpected response: {response}");

        CacheListFetchResponse fetchResponse = await client.ListFetchAsync(cacheName, listName, 1, 3);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheListFetchResponse.Hit)fetchResponse;
        Assert.Equal(new string[] { value2, value3 }, hitResponse.ValueListString);
    }

    [Fact]
    public async Task ListConcatenateFrontFetch_ValueIsByteArray_HappyPath()
    {
        var listName = Utils.NewGuidString();
        byte[][] values1 = new byte[][] { Utils.NewGuidByteArray(), Utils.NewGuidByteArray() };

        CacheListConcatenateFrontResponse concatenateResponse = await client.ListConcatenateFrontAsync(cacheName, listName, values1);
        Assert.True(concatenateResponse is CacheListConcatenateFrontResponse.Success, $"Unexpected response: {concatenateResponse}");
        var success = (CacheListConcatenateFrontResponse.Success)concatenateResponse;
        Assert.Equal(2, success.ListLength);
        Assert.Equal("Momento.Sdk.Responses.CacheListConcatenateFrontResponse+Success: ListLength: 2", success.ToString());

        CacheListFetchResponse fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheListFetchResponse.Hit)fetchResponse;

        var list = hitResponse.ValueListByteArray;
        Assert.NotEmpty(list);
        foreach (byte[] value in values1)
        {
            Assert.Contains(value, list);
        }

        // Test adding at the front semantics
        byte[][] values2 = new byte[][] { Utils.NewGuidByteArray(), Utils.NewGuidByteArray() };
        concatenateResponse = await client.ListConcatenateFrontAsync(cacheName, listName, values2);
        Assert.True(concatenateResponse is CacheListConcatenateFrontResponse.Success, $"Unexpected response: {concatenateResponse}");
        var successResponse = (CacheListConcatenateFrontResponse.Success)concatenateResponse;
        Assert.Equal(4, successResponse.ListLength);

        fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        hitResponse = (CacheListFetchResponse.Hit)fetchResponse;
        list = hitResponse.ValueListByteArray!;
        for (int i = 0; i < values2.Length; i++)
        {
            Assert.Equal(values2[i], list[i]);
        }
        foreach (byte[] value in values1)
        {
            Assert.Contains(value, list);
        }
    }

    [Fact]
    public async Task ListRetainResponse_ToString_HappyPath()
    {
        var listName = Utils.NewGuidString();
        string value1 = Utils.NewGuidString();
        string value2 = Utils.NewGuidString();
        string value3 = Utils.NewGuidString();
        string value4 = Utils.NewGuidString();
        string value5 = Utils.NewGuidString();
        string value6 = Utils.NewGuidString();
        string[] values = new string[] { value1, value2, value3, value4, value5, value6 };

        var resetList = async () =>
        {
            await client.DeleteAsync(cacheName, listName);
            await client.ListConcatenateFrontAsync(cacheName, listName, values);
        };

        await resetList();
        CacheListRetainResponse response = await client.ListRetainAsync(cacheName, listName, 1, 4);
        Assert.True(response is CacheListRetainResponse.Success, $"Unexpected response: {response}");
        var successResponse = (CacheListRetainResponse.Success)response;
        Assert.Equal("Momento.Sdk.Responses.CacheListRetainResponse+Success: ListLength: 3", successResponse.ToString());
    }

    [Fact]
    public async Task ListConcatenateBackFetch_ValueIsByteArray_HappyPath()
    {
        var listName = Utils.NewGuidString();
        byte[][] values1 = new byte[][] { Utils.NewGuidByteArray(), Utils.NewGuidByteArray() };

        CacheListConcatenateBackResponse concatenateResponse = await client.ListConcatenateBackAsync(cacheName, listName, values1);
        Assert.True(concatenateResponse is CacheListConcatenateBackResponse.Success, $"Unexpected response: {concatenateResponse}");
        var success = (CacheListConcatenateBackResponse.Success)concatenateResponse;
        Assert.Equal(2, success.ListLength);
        Assert.Equal("Momento.Sdk.Responses.CacheListConcatenateBackResponse+Success: ListLength: 2", success.ToString());

        CacheListFetchResponse fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheListFetchResponse.Hit)fetchResponse;

        var list = hitResponse.ValueListByteArray;
        Assert.NotEmpty(list);
        foreach (byte[] value in values1)
        {
            Assert.Contains(value, list);
        }

        // Test adding at the front semantics
        byte[][] values2 = new byte[][] { Utils.NewGuidByteArray(), Utils.NewGuidByteArray() };
        concatenateResponse = await client.ListConcatenateBackAsync(cacheName, listName, values2);
        Assert.True(concatenateResponse is CacheListConcatenateBackResponse.Success, $"Unexpected response: {concatenateResponse}");
        var successResponse = (CacheListConcatenateBackResponse.Success)concatenateResponse;
        Assert.Equal(4, successResponse.ListLength);

        fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        hitResponse = (CacheListFetchResponse.Hit)fetchResponse;
        list = hitResponse.ValueListByteArray!;
        for (int i = 0; i < values2.Length; i++)
        {
            Assert.Equal(values1[i], list[i]);
        }
        foreach (byte[] value in values2)
        {
            Assert.Contains(value, list);
        }
    }

    [Fact]
    public async Task ListPushFrontFetch_ValueIsByteArray_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidByteArray();

        CacheListPushFrontResponse pushResponse = await client.ListPushFrontAsync(cacheName, listName, value1);
        Assert.True(pushResponse is CacheListPushFrontResponse.Success, $"Unexpected response: {pushResponse}");
        var successResponse = (CacheListPushFrontResponse.Success)pushResponse;
        Assert.Equal(1, successResponse.ListLength);
        Assert.Equal("Momento.Sdk.Responses.CacheListPushFrontResponse+Success: ListLength: 1", successResponse.ToString());

        CacheListFetchResponse fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheListFetchResponse.Hit)fetchResponse;

        var list = hitResponse.ValueListByteArray;
        Assert.Single(list);
        Assert.Contains(value1, list);

        // Test push semantics
        var value2 = Utils.NewGuidByteArray();
        pushResponse = await client.ListPushFrontAsync(cacheName, listName, value2);
        Assert.True(pushResponse is CacheListPushFrontResponse.Success, $"Unexpected response: {pushResponse}");
        successResponse = (CacheListPushFrontResponse.Success)pushResponse;
        Assert.Equal(2, successResponse.ListLength);

        fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        hitResponse = (CacheListFetchResponse.Hit)fetchResponse;
        list = hitResponse.ValueListByteArray!;
        Assert.Equal(value2, list[0]);
        Assert.Equal(value1, list[1]);
    }

    [Fact]
    public async Task ListPushBackFetch_ValueIsByteArray_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidByteArray();

        CacheListPushBackResponse pushResponse = await client.ListPushBackAsync(cacheName, listName, value1);
        Assert.True(pushResponse is CacheListPushBackResponse.Success, $"Unexpected response: {pushResponse}");
        var successResponse = (CacheListPushBackResponse.Success)pushResponse;
        Assert.Equal(1, successResponse.ListLength);
        Assert.Equal("Momento.Sdk.Responses.CacheListPushBackResponse+Success: ListLength: 1", successResponse.ToString());

        CacheListFetchResponse fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheListFetchResponse.Hit)fetchResponse;

        var list = hitResponse.ValueListByteArray;
        Assert.Single(list);
        Assert.Contains(value1, list);

        // Test push semantics
        var value2 = Utils.NewGuidByteArray();
        pushResponse = await client.ListPushBackAsync(cacheName, listName, value2);
        Assert.True(pushResponse is CacheListPushBackResponse.Success, $"Unexpected response: {pushResponse}");
        successResponse = (CacheListPushBackResponse.Success)pushResponse;
        Assert.Equal(2, successResponse.ListLength);

        fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        hitResponse = (CacheListFetchResponse.Hit)fetchResponse;
        list = hitResponse.ValueListByteArray!;
        Assert.Equal(value1, list[0]);
        Assert.Equal(value2, list[1]);
    }

    [Fact]
    public async Task ListPopFrontAsync_ValueIsString_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var value2 = Utils.NewGuidString();

        await client.ListPushFrontAsync(cacheName, listName, value1);
        await client.ListPushFrontAsync(cacheName, listName, value2);
        CacheListPopFrontResponse response = await client.ListPopFrontAsync(cacheName, listName);
        Assert.True(response is CacheListPopFrontResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheListPopFrontResponse.Hit)response;

        Assert.Equal(value2, hitResponse.ValueString);
        Assert.Equal(1, hitResponse.ListLength);
    }

    [Fact]
    public async Task ListPopBackAsync_ValueIsString_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var value2 = Utils.NewGuidString();

        await client.ListPushBackAsync(cacheName, listName, value1);
        await client.ListPushBackAsync(cacheName, listName, value2);
        CacheListPopBackResponse response = await client.ListPopBackAsync(cacheName, listName);
        Assert.True(response is CacheListPopBackResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheListPopBackResponse.Hit)response;

        Assert.Equal(value2, hitResponse.ValueString);
        Assert.Equal(1, hitResponse.ListLength);
    }

    [Fact]
    public async Task ListRemoveValueAsync_ValueIsByteArray_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var list = new List<byte[]>() { Utils.NewGuidByteArray(), Utils.NewGuidByteArray(), Utils.NewGuidByteArray() };
        var valueOfInterest = Utils.NewGuidByteArray();

        // Add elements to the list
        foreach (var value in list)
        {
            await client.ListPushBackAsync(cacheName, listName, value);
        }

        await client.ListPushBackAsync(cacheName, listName, valueOfInterest);
        await client.ListPushBackAsync(cacheName, listName, valueOfInterest);

        // Remove value of interest
        var removeResponse = await client.ListRemoveValueAsync(cacheName, listName, valueOfInterest);
        Assert.True(removeResponse is CacheListRemoveValueResponse.Success, $"Unexpected response: {removeResponse}");
        var successRemoveResponse = (CacheListRemoveValueResponse.Success)removeResponse;
        Assert.Equal(3, successRemoveResponse.ListLength);

        // Test not there
        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Hit, $"Unexpected response: {response}");
        var cachedList = ((CacheListFetchResponse.Hit)response).ValueListByteArray!;
        Assert.True(list.ListEquals(cachedList), $"lists did not match: {list} AND {cachedList}");
    }

    [Fact]
    public async Task ListLengthAsync_ListIsFound_HappyPath()
    {
        var listName = Utils.NewGuidString();
        foreach (var i in Enumerable.Range(0, 10))
        {
            await client.ListPushBackAsync(cacheName, listName, Utils.NewGuidByteArray());
        }

        CacheListLengthResponse lengthResponse = await client.ListLengthAsync(cacheName, listName);
        Assert.True(lengthResponse is CacheListLengthResponse.Success, $"Unexpected response: {lengthResponse}");
        var successResponse = (CacheListLengthResponse.Success)lengthResponse;
        Assert.Equal(10, successResponse.Length);
        Assert.Equal("Momento.Sdk.Responses.CacheListLengthResponse+Success: Length: 10", successResponse.ToString());
    }

    // set

    [Fact]
    public async Task SetAddFetch_ElementIsByteArray_HappyPath()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidByteArray();

        CacheSetAddElementResponse response = await client.SetAddElementAsync(cacheName, setName, element);
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");

        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Hit, $"Unexpected response: {fetchResponse}");

        var set = ((CacheSetFetchResponse.Hit)fetchResponse).ValueSetByteArray;
        Assert.Single(set);
        Assert.Contains(element, set);
    }

    [Fact]
    public async Task SetAddElementsAsync_ElementsAreByteArrayEnumerable_HappyPath()
    {
        var setName = Utils.NewGuidString();
        var element1 = Utils.NewGuidByteArray();
        var element2 = Utils.NewGuidByteArray();
        var content = new List<byte[]>() { element1, element2 };

        CacheSetAddElementsResponse response = await client.SetAddElementsAsync(cacheName, setName, content);
        Assert.True(response is CacheSetAddElementsResponse.Success, $"Unexpected response: {response}");

        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Hit, $"Unexpected response: {fetchResponse}");

        var set = ((CacheSetFetchResponse.Hit)fetchResponse).ValueSetByteArray;
        Assert.Equal(2, set!.Count);
        Assert.Contains(element1, set);
        Assert.Contains(element2, set);
    }

    [Fact]
    public async Task SetRemoveElementAsync_ElementIsByteArray_HappyPath()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidByteArray();

        CacheSetAddElementResponse response = await client.SetAddElementAsync(cacheName, setName, element);
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");

        // Remove element that is not there -- no-op
        CacheSetRemoveElementResponse removeResponse = await client.SetRemoveElementAsync(cacheName, setName, Utils.NewGuidByteArray());
        Assert.True(removeResponse is CacheSetRemoveElementResponse.Success, $"Unexpected response: {removeResponse}");
        // Fetch the whole set and make sure response has element we expect
        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var set = ((CacheSetFetchResponse.Hit)fetchResponse).ValueSetByteArray;
        Assert.Single(set);
        Assert.Contains(element, set);

        // Remove element
        removeResponse = await client.SetRemoveElementAsync(cacheName, setName, element);
        Assert.True(removeResponse is CacheSetRemoveElementResponse.Success, $"Unexpected response: {removeResponse}");
        fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Miss, $"Unexpected response: {fetchResponse}");
    }

    [Fact]
    public async Task SetSampleAsync_UsesCachedStringSet_HappyPath()
    {
        var setName = Utils.NewGuidString();
        var allValues = new HashSet<String> { "jalapeno", "habanero", "serrano", "poblano" };
        CacheSetAddElementsResponse setResponse = await client.SetAddElementsAsync(cacheName, setName, allValues);
        Assert.True(setResponse is CacheSetAddElementsResponse.Success, $"Unexpected response: {setResponse}");

        CacheSetSampleResponse allElementsResponse = await client.SetSampleAsync(cacheName, setName, allValues.Count);
        Assert.True(allElementsResponse is CacheSetSampleResponse.Hit, $"Unexpected response: {allElementsResponse}");
        var allElementsHitValues = ((CacheSetSampleResponse.Hit)allElementsResponse).ValueSetString;
        Assert.True(allValues.SetEquals(allElementsHitValues), $"Expected sample with with limit matching set size to return the entire set; expected ({String.Join(", ", allValues)}), got ({String.Join(", ", allElementsHitValues)})");

        CacheSetSampleResponse limitGreaterThanSetSizeResponse = await client.SetSampleAsync(cacheName, setName, 1000);
        Assert.True(limitGreaterThanSetSizeResponse is CacheSetSampleResponse.Hit, $"Unexpected response: {limitGreaterThanSetSizeResponse}");
        var limitGreaterThanSetSizeHitValues = ((CacheSetSampleResponse.Hit)limitGreaterThanSetSizeResponse).ValueSetString;
        Assert.True(allValues.SetEquals(limitGreaterThanSetSizeHitValues), $"Expected sample with with limit greater than set size to return the entire set; expected ({String.Join(", ", allValues)}), got ({String.Join(", ", limitGreaterThanSetSizeHitValues)})");

        CacheSetSampleResponse limitZeroResponse = await client.SetSampleAsync(cacheName, setName, 0);
        var emptySet = new HashSet<String>();
        var limitZeroHitValues = ((CacheSetSampleResponse.Hit)limitZeroResponse).ValueSetString;
        Assert.True(emptySet.SetEquals(limitZeroHitValues), $"Expected sample with with limit zero to return empty set; got ({limitZeroHitValues})");

        for (int i = 0; i < 10; i++)
        {
            CacheSetSampleResponse response = await client.SetSampleAsync(cacheName, setName, allValues.Count - 2);
            Assert.True(response is CacheSetSampleResponse.Hit, $"Unexpected response: {response}");
            var hitResponse = (CacheSetSampleResponse.Hit)response;
            var hitValues = hitResponse.ValueSetString;
            Assert.True(hitValues.IsSubsetOf(allValues),
                $"Expected hit values ({String.Join(", ", hitValues)}) to be subset of all values ({String.Join(", ", allValues)}), but it is not!");
        }
    }


    [Fact]
    public async Task CacheSetFetchResponse_ToString_HappyPath()
    {
        var setName = Utils.NewGuidString();
        await client.SetAddElementAsync(cacheName, setName, "a");

        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);

        Assert.True(fetchResponse is CacheSetFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheSetFetchResponse.Hit)fetchResponse;
        Assert.Equal("Momento.Sdk.Responses.CacheSetFetchResponse+Hit: ValueSetString: {\"a\"} ValueSetByteArray: {\"61\"}", hitResponse.ToString());
    }

    [Fact]
    public async Task SetLengthAsync_HappyPath()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidByteArray();

        await client.SetAddElementAsync(cacheName, setName, element);

        CacheSetLengthResponse lengthResponse = await client.SetLengthAsync(cacheName, setName);
        Assert.True(lengthResponse is CacheSetLengthResponse.Hit, $"Unexpected response: {lengthResponse}");
        var hitResponse = (CacheSetLengthResponse.Hit)lengthResponse;
        Assert.Equal(1, hitResponse.Length);
    }
}
