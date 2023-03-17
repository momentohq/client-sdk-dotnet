using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Momento.Sdk.Internal.ExtensionMethods;
using Momento.Sdk.Requests;
using Momento.Sdk.Responses;
using Momento.Sdk.Tests;

namespace Momento.Sdk.Tests;

[Collection("CacheClient")]
public class ListTest : TestBase
{
    public ListTest(CacheClientFixture fixture) : base(fixture)
    {
    }

    [Theory]
    [InlineData(null, "my-list", new string[] { "value" })]
    [InlineData("cache", null, new string[] { "value" })]
    [InlineData("cache", "my-list", null)]
    public async Task ListConcatenateFrontAsync_NullChecksStringArray_IsError(string cacheName, string listName, IEnumerable<string> values)
    {
        CacheListConcatenateFrontResponse response = await client.ListConcatenateFrontAsync(cacheName, listName, values);
        Assert.True(response is CacheListConcatenateFrontResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListConcatenateFrontResponse.Error)response).ErrorCode);
    }

    [Theory]
    [InlineData("cache", "my-list", 3, 1)]
    [InlineData("cache", "my-list", 3, 3)]
    [InlineData("cache", "my-list", -2, -3)]
    public async Task ListFetchAsync_InvalidIndex_IsError(string cacheName, string listName, int startIndex, int endIndex)
    {
        string[] values = new string[] { Utils.NewGuidString(), Utils.NewGuidString(), Utils.NewGuidString(), Utils.NewGuidString() };
        CacheListConcatenateFrontResponse response = await client.ListConcatenateFrontAsync(cacheName, listName, values);
        Assert.True(response is CacheListConcatenateFrontResponse.Success, $"Unexpected response: {response}");

        CacheListFetchResponse fetchResponse = await client.ListFetchAsync(cacheName, listName, startIndex, endIndex);
        Assert.True(fetchResponse is CacheListFetchResponse.Error, $"Unexpected response: {fetchResponse}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListFetchResponse.Error)fetchResponse).ErrorCode);
    }

    [Fact]
    public async Task ListFetchAsync_WithPositiveStartEndIndcies_HappyPath()
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
    public async Task ListFetchAsync_WithNegativeStartEndIndcies_HappyPath()
    {
        var listName = Utils.NewGuidString();
        string value1 = Utils.NewGuidString();
        string value2 = Utils.NewGuidString();
        string value3 = Utils.NewGuidString();
        string value4 = Utils.NewGuidString();
        string[] values = new string[] { value1, value2, value3, value4 };
        CacheListConcatenateFrontResponse response = await client.ListConcatenateFrontAsync(cacheName, listName, values);
        Assert.True(response is CacheListConcatenateFrontResponse.Success, $"Unexpected response: {response}");

        CacheListFetchResponse fetchResponse = await client.ListFetchAsync(cacheName, listName, -2, -1);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheListFetchResponse.Hit)fetchResponse;
        Assert.Equal(new string[] { value3 }, hitResponse.ValueListString);
    }

    [Theory]
    [InlineData("cache", "my-list", null, 1)]
    [InlineData("cache", "my-list", null, -3)]
    public async Task ListFetchAsync_WithNullStartIndex_HappyPath(string cacheName, string listName, int startIndex, int endIndex)
    {
        string value1 = Utils.NewGuidString();
        string value2 = Utils.NewGuidString();
        string value3 = Utils.NewGuidString();
        string value4 = Utils.NewGuidString();
        string[] values = new string[] { value1, value2, value3, value4 };
        CacheListConcatenateFrontResponse response = await client.ListConcatenateFrontAsync(cacheName, listName, values);
        Assert.True(response is CacheListConcatenateFrontResponse.Success, $"Unexpected response: {response}");

        CacheListFetchResponse fetchResponse = await client.ListFetchAsync(cacheName, listName, startIndex, endIndex);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheListFetchResponse.Hit)fetchResponse;
        Assert.Equal(new string[] { value1 }, hitResponse.ValueListString);
    }

    [Fact]
    public async Task ListFetchAsync_WithNullEndIndex_HappyPath()
    {
        var listName = Utils.NewGuidString();
        string value1 = Utils.NewGuidString();
        string value2 = Utils.NewGuidString();
        string value3 = Utils.NewGuidString();
        string value4 = Utils.NewGuidString();
        string[] values = new string[] { value1, value2, value3, value4 };
        CacheListConcatenateFrontResponse response = await client.ListConcatenateFrontAsync(cacheName, listName, values);
        Assert.True(response is CacheListConcatenateFrontResponse.Success, $"Unexpected response: {response}");

        CacheListFetchResponse fetchResponse = await client.ListFetchAsync(cacheName, listName, 0, null);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheListFetchResponse.Hit)fetchResponse;
        Assert.Equal(values, hitResponse.ValueListString);
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
    public async Task ListConcatenateFrontFetch_ValueIsByteArray_NoRefreshTtl()
    {
        var listName = Utils.NewGuidString();
        byte[][] values = new byte[][] { Utils.NewGuidByteArray() };

        await client.ListConcatenateFrontAsync(cacheName, listName, values, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(5)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(100);

        await client.ListConcatenateFrontAsync(cacheName, listName, values, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(5)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(4900);

        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task ListConcatenateFrontFetch_ValueIsByteArray_RefreshTtl()
    {
        var listName = Utils.NewGuidString();
        byte[][] values = new byte[][] { Utils.NewGuidByteArray() };

        await client.ListConcatenateFrontAsync(cacheName, listName, values, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(2)).WithNoRefreshTtlOnUpdates());
        await client.ListConcatenateFrontAsync(cacheName, listName, values, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(10)));
        await Task.Delay(2000);

        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheListFetchResponse.Hit)response;
        Assert.Equal(2, hitResponse.ValueListByteArray!.Count);
    }

    [Fact]
    public async Task ListConcatenateFrontAsync_ValueIsByteArrayTruncateBackToSizeIsZero_IsError()
    {
        byte[][] values = new byte[][] { };
        var response = await client.ListConcatenateFrontAsync("myCache", "listName", values, truncateBackToSize: 0);
        Assert.True(response is CacheListConcatenateFrontResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListConcatenateFrontResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task ListConcatenateFrontFetch_ValueIsStringArray_HappyPath()
    {
        var listName = Utils.NewGuidString();
        string[] values1 = new string[] { Utils.NewGuidString(), Utils.NewGuidString() };

        CacheListConcatenateFrontResponse concatenateResponse = await client.ListConcatenateFrontAsync(cacheName, listName, values1);
        Assert.True(concatenateResponse is CacheListConcatenateFrontResponse.Success, $"Unexpected response: {concatenateResponse}");
        var success = (CacheListConcatenateFrontResponse.Success)concatenateResponse;
        Assert.Equal(2, success.ListLength);
        Assert.Equal("Momento.Sdk.Responses.CacheListConcatenateFrontResponse+Success: ListLength: 2", success.ToString());

        CacheListFetchResponse fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheListFetchResponse.Hit)fetchResponse;

        var list = hitResponse.ValueListString!;
        Assert.NotEmpty(list);
        foreach (string value in values1)
        {
            Assert.Contains(value, list);
        }

        // Test adding at the front semantics
        string[] values2 = new string[] { Utils.NewGuidString(), Utils.NewGuidString() };
        concatenateResponse = await client.ListConcatenateFrontAsync(cacheName, listName, values2);
        Assert.True(concatenateResponse is CacheListConcatenateFrontResponse.Success, $"Unexpected response: {concatenateResponse}");
        var successResponse = (CacheListConcatenateFrontResponse.Success)concatenateResponse;
        Assert.Equal(4, successResponse.ListLength);

        fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        hitResponse = (CacheListFetchResponse.Hit)fetchResponse;
        list = hitResponse.ValueListString!;
        var values3 = new List<String>(values2);
        values3.AddRange(values1);
        Assert.Equal(values3, list);
    }

    [Fact]
    public async Task ListConcatenateFrontFetch_ValueIsStringArray_NoRefreshTtl()
    {
        var listName = Utils.NewGuidString();
        string[] values = new string[] { Utils.NewGuidString() };

        await client.ListConcatenateFrontAsync(cacheName, listName, values, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(5)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(100);

        await client.ListConcatenateFrontAsync(cacheName, listName, values, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(5)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(4900);

        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task ListConcatenateFrontFetch_ValueIsStringArray_RefreshTtl()
    {
        var listName = Utils.NewGuidString();
        string[] values = new string[] { Utils.NewGuidString() };

        await client.ListConcatenateFrontAsync(cacheName, listName, values, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(2)).WithNoRefreshTtlOnUpdates());
        await client.ListConcatenateFrontAsync(cacheName, listName, values, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(10)).WithRefreshTtlOnUpdates());
        await Task.Delay(2000);

        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheListFetchResponse.Hit)response;
        Assert.Equal(2, hitResponse.ValueListByteArray!.Count);
    }

    [Fact]
    public async Task ListConcatenateFrontAsync_ValueIsStringArrayTruncateBackToSizeIsZero_IsError()
    {
        string[] values = new string[] { };
        var response = await client.ListConcatenateFrontAsync("myCache", "listName", values, truncateBackToSize: 0);
        Assert.True(response is CacheListConcatenateFrontResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListConcatenateFrontResponse.Error)response).ErrorCode);
    }

    [Theory]
    [InlineData(null, "my-list", new string[] { "value" })]
    [InlineData("cache", null, new string[] { "value" })]
    [InlineData("cache", "my-list", null)]
    public async Task ListConcatenateBackAsync_NullChecksStringArray_IsError(string cacheName, string listName, IEnumerable<string> values)
    {
        CacheListConcatenateBackResponse response = await client.ListConcatenateBackAsync(cacheName, listName, values);
        Assert.True(response is CacheListConcatenateBackResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListConcatenateBackResponse.Error)response).ErrorCode);
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
    public async Task ListConcatenateBackFetch_ValueIsByteArray_NoRefreshTtl()
    {
        var listName = Utils.NewGuidString();
        byte[][] values = new byte[][] { Utils.NewGuidByteArray() };

        await client.ListConcatenateBackAsync(cacheName, listName, values, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(5)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(100);
        await client.ListConcatenateBackAsync(cacheName, listName, values, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(5)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(4900);

        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task ListConcatenateBackFetch_ValueIsByteArray_RefreshTtl()
    {
        var listName = Utils.NewGuidString();
        byte[][] values = new byte[][] { Utils.NewGuidByteArray() };

        await client.ListConcatenateBackAsync(cacheName, listName, values, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(5)).WithNoRefreshTtlOnUpdates());
        await client.ListConcatenateBackAsync(cacheName, listName, values, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(10)).WithRefreshTtlOnUpdates());
        await Task.Delay(2000);

        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheListFetchResponse.Hit)response;
        Assert.Equal(2, hitResponse.ValueListByteArray!.Count);
    }

    [Fact]
    public async Task ListConcatenateBackAsync_ValueIsByteArrayTruncateFrontoSizeIsZero_IsError()
    {
        byte[][] values = new byte[][] { };
        var response = await client.ListConcatenateBackAsync("myCache", "listName", values, truncateFrontToSize: 0);
        Assert.True(response is CacheListConcatenateBackResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListConcatenateBackResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task ListConcatenateBackFetch_ValueIsStringArray_HappyPath()
    {
        var listName = Utils.NewGuidString();
        string[] values1 = new string[] { Utils.NewGuidString(), Utils.NewGuidString() };

        CacheListConcatenateBackResponse concatenateResponse = await client.ListConcatenateBackAsync(cacheName, listName, values1);
        Assert.True(concatenateResponse is CacheListConcatenateBackResponse.Success, $"Unexpected response: {concatenateResponse}");
        var success = (CacheListConcatenateBackResponse.Success)concatenateResponse;
        Assert.Equal(2, success.ListLength);
        Assert.Equal("Momento.Sdk.Responses.CacheListConcatenateBackResponse+Success: ListLength: 2", success.ToString());

        CacheListFetchResponse fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheListFetchResponse.Hit)fetchResponse;

        var list = hitResponse.ValueListString!;
        Assert.NotEmpty(list);
        foreach (string value in values1)
        {
            Assert.Contains(value, list);
        }

        // Test adding at the front semantics
        string[] values2 = new string[] { Utils.NewGuidString(), Utils.NewGuidString() };
        concatenateResponse = await client.ListConcatenateBackAsync(cacheName, listName, values2);
        Assert.True(concatenateResponse is CacheListConcatenateBackResponse.Success, $"Unexpected response: {concatenateResponse}");
        var successResponse = (CacheListConcatenateBackResponse.Success)concatenateResponse;
        Assert.Equal(4, successResponse.ListLength);

        fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        hitResponse = (CacheListFetchResponse.Hit)fetchResponse;
        list = hitResponse.ValueListString!;
        var values3 = new List<String>(values1);
        values3.AddRange(values2);
        Assert.Equal(values3, list);
    }

    [Fact]
    public async Task ListConcatenateBackFetch_ValueIsStringArray_NoRefreshTtl()
    {
        var listName = Utils.NewGuidString();
        string[] values = new string[] { Utils.NewGuidString() };

        await client.ListConcatenateBackAsync(cacheName, listName, values, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(5)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(100);

        await client.ListConcatenateBackAsync(cacheName, listName, values, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(5)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(4900);

        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task ListConcatenateBackFetch_ValueIsStringArray_RefreshTtl()
    {
        var listName = Utils.NewGuidString();
        string[] values = new string[] { Utils.NewGuidString() };

        await client.ListConcatenateBackAsync(cacheName, listName, values, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(5)).WithNoRefreshTtlOnUpdates());
        await client.ListConcatenateBackAsync(cacheName, listName, values, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(10)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(2000);

        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheListFetchResponse.Hit)response;
        Assert.Equal(2, hitResponse.ValueListByteArray!.Count);
    }

    [Fact]
    public async Task ListConcatenateBackAsync_ValueIsStringArrayTruncateBackToSizeIsZero_IsError()
    {
        string[] values = new string[] { };
        var response = await client.ListConcatenateBackAsync("myCache", "listName", values, truncateFrontToSize: 0);
        Assert.True(response is CacheListConcatenateBackResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListConcatenateBackResponse.Error)response).ErrorCode);
    }

    ///////////

    [Theory]
    [InlineData(null, "my-list", new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-list", null)]
    public async Task ListPushFrontAsync_NullChecksByteArray_IsError(string cacheName, string listName, byte[] value)
    {
        CacheListPushFrontResponse response = await client.ListPushFrontAsync(cacheName, listName, value);
        Assert.True(response is CacheListPushFrontResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListPushFrontResponse.Error)response).ErrorCode);
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
    public async Task ListPushFrontFetch_ValueIsByteArray_NoRefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();

        await client.ListPushFrontAsync(cacheName, listName, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(5)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(100);

        await client.ListPushFrontAsync(cacheName, listName, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(10)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(4900);

        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task ListPushFrontFetch_ValueIsByteArray_RefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();

        await client.ListPushFrontAsync(cacheName, listName, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(2)).WithNoRefreshTtlOnUpdates());
        await client.ListPushFrontAsync(cacheName, listName, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(10)));
        await Task.Delay(2000);

        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheListFetchResponse.Hit)response;
        Assert.Equal(2, hitResponse.ValueListByteArray!.Count);
    }

    [Fact]
    public async Task ListPushFrontAsync_ValueIsByteArrayTruncateBackToSizeIsZero_IsError()
    {
        var response = await client.ListPushFrontAsync("myCache", "listName", new byte[] { 0x00 }, truncateBackToSize: 0);
        Assert.True(response is CacheListPushFrontResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListPushFrontResponse.Error)response).ErrorCode);
    }

    [Theory]
    [InlineData(null, "my-list", "my-value")]
    [InlineData("cache", null, "my-value")]
    [InlineData("cache", "my-list", null)]
    public async Task ListPushFrontAsync_NullChecksString_IsError(string cacheName, string listName, string value)
    {
        CacheListPushFrontResponse response = await client.ListPushFrontAsync(cacheName, listName, value);
        Assert.True(response is CacheListPushFrontResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListPushFrontResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task ListPushFrontFetch_ValueIsString_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();

        CacheListPushFrontResponse pushResponse = await client.ListPushFrontAsync(cacheName, listName, value1);
        Assert.True(pushResponse is CacheListPushFrontResponse.Success, $"Unexpected response: {pushResponse}");
        var successResponse = (CacheListPushFrontResponse.Success)pushResponse;
        Assert.Equal(1, successResponse.ListLength);
        Assert.Equal("Momento.Sdk.Responses.CacheListPushFrontResponse+Success: ListLength: 1", successResponse.ToString());
        successResponse = (CacheListPushFrontResponse.Success)pushResponse;

        CacheListFetchResponse fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheListFetchResponse.Hit)fetchResponse;

        var list = hitResponse.ValueListString;
        Assert.Single(list);
        Assert.Contains(value1, list);

        // Test push semantics
        var value2 = Utils.NewGuidString();
        pushResponse = await client.ListPushFrontAsync(cacheName, listName, value2);
        Assert.True(pushResponse is CacheListPushFrontResponse.Success, $"Unexpected response: {pushResponse}");
        successResponse = (CacheListPushFrontResponse.Success)pushResponse;
        Assert.Equal(2, successResponse.ListLength);

        fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        hitResponse = (CacheListFetchResponse.Hit)fetchResponse;
        list = hitResponse.ValueListString!;
        Assert.Equal(value2, list[0]);
        Assert.Equal(value1, list[1]);
    }

    [Fact]
    public async Task ListPushFrontFetch_ValueIsString_NoRefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidString();

        await client.ListPushFrontAsync(cacheName, listName, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(5)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(100);

        await client.ListPushFrontAsync(cacheName, listName, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(10)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(4900);

        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task ListPushFrontFetch_ValueIsString_RefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidString();

        await client.ListPushFrontAsync(cacheName, listName, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(2)).WithNoRefreshTtlOnUpdates());
        await client.ListPushFrontAsync(cacheName, listName, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(10)));
        await Task.Delay(2000);

        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheListFetchResponse.Hit)response;
        Assert.Equal(2, hitResponse.ValueListString!.Count);
    }

    [Fact]
    public async Task ListPushFrontAsync_ValueIsStringTruncateBackToSizeIsZero_IsError()
    {
        var response = await client.ListPushFrontAsync("myCache", "listName", "value", truncateBackToSize: 0);
        Assert.True(response is CacheListPushFrontResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListPushFrontResponse.Error)response).ErrorCode);
    }

    [Theory]
    [InlineData(null, "my-list", new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-list", null)]
    public async Task ListPushBackAsync_NullChecksByteArray_IsError(string cacheName, string listName, byte[] value)
    {
        CacheListPushBackResponse response = await client.ListPushBackAsync(cacheName, listName, value);
        Assert.True(response is CacheListPushBackResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListPushBackResponse.Error)response).ErrorCode);
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
    public async Task ListPushBackFetch_ValueIsByteArray_NoRefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();

        await client.ListPushBackAsync(cacheName, listName, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(5)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(100);

        await client.ListPushBackAsync(cacheName, listName, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(10)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(4900);

        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task ListPushBackFetch_ValueIsByteArray_RefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();

        await client.ListPushBackAsync(cacheName, listName, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(2)).WithNoRefreshTtlOnUpdates());
        await client.ListPushBackAsync(cacheName, listName, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(10)));
        await Task.Delay(2000);

        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheListFetchResponse.Hit)response;
        Assert.Equal(2, hitResponse.ValueListByteArray!.Count);
    }

    [Fact]
    public async Task ListPushBackAsync_ValueIsByteArrayTruncateFrontToSizeIsZero_IsError()
    {
        var response = await client.ListPushBackAsync("myCache", "listName", new byte[] { 0x00 }, truncateFrontToSize: 0);
        Assert.True(response is CacheListPushBackResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListPushBackResponse.Error)response).ErrorCode);
    }

    [Theory]
    [InlineData(null, "my-list", "my-value")]
    [InlineData("cache", null, "my-value")]
    [InlineData("cache", "my-list", null)]
    public async Task ListPushBackAsync_NullChecksString_IsError(string cacheName, string listName, string value)
    {
        CacheListPushBackResponse response = await client.ListPushBackAsync(cacheName, listName, value);
        Assert.True(response is CacheListPushBackResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListPushBackResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task ListPushBackTruncate_TruncatesList_String()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var value2 = Utils.NewGuidString();
        var value3 = Utils.NewGuidString();
        await client.ListPushBackAsync(cacheName, listName, value1);
        await client.ListPushBackAsync(cacheName, listName, value2);
        await client.ListPushBackAsync(cacheName, listName, value3, 2);
        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheListFetchResponse.Hit)response;
        Assert.Equal(2, hitResponse.ValueListString!.Count);
        Assert.Equal(value2, hitResponse.ValueListString![0]);
        Assert.Equal(value3, hitResponse.ValueListString![1]);
    }

    [Fact]
    public async Task ListPushBackTruncate_TruncatesList_Bytes()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidByteArray();
        var value2 = Utils.NewGuidByteArray();
        var value3 = Utils.NewGuidByteArray();
        await client.ListPushBackAsync(cacheName, listName, value1);
        await client.ListPushBackAsync(cacheName, listName, value2);
        await client.ListPushBackAsync(cacheName, listName, value3, 2);
        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheListFetchResponse.Hit)response;
        Assert.Equal(2, hitResponse.ValueListByteArray!.Count);
        Assert.Equal(value2, hitResponse.ValueListByteArray![0]);
        Assert.Equal(value3, hitResponse.ValueListByteArray![1]);
    }

    [Fact]
    public async Task ListPushFrontTruncate_TruncatesList_String()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var value2 = Utils.NewGuidString();
        var value3 = Utils.NewGuidString();
        await client.ListPushFrontAsync(cacheName, listName, value1);
        await client.ListPushFrontAsync(cacheName, listName, value2);
        await client.ListPushFrontAsync(cacheName, listName, value3, 2);
        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheListFetchResponse.Hit)response;
        Assert.Equal(2, hitResponse.ValueListString!.Count);
        Assert.Equal(value2, hitResponse.ValueListString![1]);
        Assert.Equal(value3, hitResponse.ValueListString![0]);
    }

    [Fact]
    public async Task ListPushFrontTruncate_TruncatesList_Bytes()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidByteArray();
        var value2 = Utils.NewGuidByteArray();
        var value3 = Utils.NewGuidByteArray();
        await client.ListPushFrontAsync(cacheName, listName, value1);
        await client.ListPushFrontAsync(cacheName, listName, value2);
        await client.ListPushFrontAsync(cacheName, listName, value3, 2);
        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheListFetchResponse.Hit)response;
        Assert.Equal(2, hitResponse.ValueListByteArray!.Count);
        Assert.Equal(value2, hitResponse.ValueListByteArray![1]);
        Assert.Equal(value3, hitResponse.ValueListByteArray![0]);
    }

    [Fact]
    public async Task ListPushBackFetch_ValueIsString_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();

        CacheListPushBackResponse pushResponse = await client.ListPushBackAsync(cacheName, listName, value1);
        Assert.True(pushResponse is CacheListPushBackResponse.Success, $"Unexpected response: {pushResponse}");
        var successResponse = (CacheListPushBackResponse.Success)pushResponse;
        Assert.Equal(1, successResponse.ListLength);
        Assert.Equal("Momento.Sdk.Responses.CacheListPushBackResponse+Success: ListLength: 1", successResponse.ToString());

        CacheListFetchResponse fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheListFetchResponse.Hit)fetchResponse;

        var list = hitResponse.ValueListString;
        Assert.Single(list);
        Assert.Contains(value1, list);

        // Test push semantics
        var value2 = Utils.NewGuidString();
        pushResponse = await client.ListPushBackAsync(cacheName, listName, value2);
        successResponse = (CacheListPushBackResponse.Success)pushResponse;
        successResponse = (CacheListPushBackResponse.Success)pushResponse;
        Assert.Equal(2, successResponse.ListLength);

        fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        hitResponse = (CacheListFetchResponse.Hit)fetchResponse;
        list = hitResponse.ValueListString!;
        Assert.Equal(value1, list[0]);
        Assert.Equal(value2, list[1]);
    }

    [Fact]
    public async Task ListPushBackFetch_ValueIsString_NoRefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidString();

        await client.ListPushBackAsync(cacheName, listName, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(2)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(100);

        await client.ListPushBackAsync(cacheName, listName, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(10)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(4900);

        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task ListPushBackFetch_ValueIsString_RefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidString();

        await client.ListPushBackAsync(cacheName, listName, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(2)));
        await client.ListPushBackAsync(cacheName, listName, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(10)));
        await Task.Delay(2000);

        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheListFetchResponse.Hit)response;
        Assert.Equal(2, hitResponse.ValueListString!.Count);
    }

    [Fact]
    public async Task ListPushBackAsync_ValueIsStringTruncateFrontToSizeIsZero_IsError()
    {
        var response = await client.ListPushBackAsync("myCache", "listName", "value", truncateFrontToSize: 0);
        Assert.True(response is CacheListPushBackResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListPushBackResponse.Error)response).ErrorCode);
    }

    [Theory]
    [InlineData(null, "my-list")]
    [InlineData("cache", null)]
    public async Task ListPopFrontAsync_NullChecks_IsError(string cacheName, string listName)
    {
        CacheListPopFrontResponse response = await client.ListPopFrontAsync(cacheName, listName);
        Assert.True(response is CacheListPopFrontResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListPopFrontResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task ListPopFrontAsync_ListIsMissing_HappyPath()
    {
        var listName = Utils.NewGuidString();
        CacheListPopFrontResponse response = await client.ListPopFrontAsync(cacheName, listName);
        Assert.True(response is CacheListPopFrontResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task ListPopFrontAsync_ValueIsByteArray_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidByteArray();
        var value2 = Utils.NewGuidByteArray();

        await client.ListPushFrontAsync(cacheName, listName, value1);
        await client.ListPushFrontAsync(cacheName, listName, value2);
        CacheListPopFrontResponse response = await client.ListPopFrontAsync(cacheName, listName);
        Assert.True(response is CacheListPopFrontResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheListPopFrontResponse.Hit)response;

        Assert.Equal(value2, hitResponse.ValueByteArray);
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
    }

    [Fact]
    public async Task CacheListPopFrontReponse_ToString_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value = "a";

        await client.ListPushFrontAsync(cacheName, listName, value);
        CacheListPopFrontResponse response = await client.ListPopFrontAsync(cacheName, listName);
        Assert.True(response is CacheListPopFrontResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheListPopFrontResponse.Hit)response;
        Assert.Equal("Momento.Sdk.Responses.CacheListPopFrontResponse+Hit: ValueString: \"a\" ValueByteArray: \"61\"", hitResponse.ToString());
    }

    [Theory]
    [InlineData(null, "my-list")]
    [InlineData("cache", null)]
    public async Task ListPopBackAsync_NullChecks_IsError(string cacheName, string listName)
    {
        CacheListPopBackResponse response = await client.ListPopBackAsync(cacheName, listName);
        Assert.True(response is CacheListPopBackResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListPopBackResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task ListPopBackAsync_ListIsMissing_HappyPath()
    {
        var listName = Utils.NewGuidString();
        CacheListPopBackResponse response = await client.ListPopBackAsync(cacheName, listName);
        Assert.True(response is CacheListPopBackResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task ListPopBackAsync_ValueIsByteArray_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidByteArray();
        var value2 = Utils.NewGuidByteArray();

        await client.ListPushBackAsync(cacheName, listName, value1);
        await client.ListPushBackAsync(cacheName, listName, value2);
        CacheListPopBackResponse response = await client.ListPopBackAsync(cacheName, listName);
        Assert.True(response is CacheListPopBackResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheListPopBackResponse.Hit)response;

        Assert.Equal(value2, hitResponse.ValueByteArray);
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
    }

    [Fact]
    public async Task CacheListPopBackReponse_ToString_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value = "a";

        await client.ListPushBackAsync(cacheName, listName, value);
        CacheListPopBackResponse response = await client.ListPopBackAsync(cacheName, listName);
        Assert.True(response is CacheListPopBackResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheListPopBackResponse.Hit)response;
        Assert.Equal("Momento.Sdk.Responses.CacheListPopBackResponse+Hit: ValueString: \"a\" ValueByteArray: \"61\"", hitResponse.ToString());
    }

    [Theory]
    [InlineData(null, "my-list")]
    [InlineData("cache", null)]
    public async Task ListFetchAsync_NullChecks_IsError(string cacheName, string listName)
    {
        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListFetchResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task ListFetchAsync_Missing_HappyPath()
    {
        var listName = Utils.NewGuidString();
        CacheListFetchResponse response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task ListFetchAsync_HasContentString_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var field1 = Utils.NewGuidString();
        var field2 = Utils.NewGuidString();
        var contentList = new List<string>() { field1, field2 };

        await client.ListPushFrontAsync(cacheName, listName, field2);
        await client.ListPushFrontAsync(cacheName, listName, field1);

        CacheListFetchResponse fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheListFetchResponse.Hit)fetchResponse;

        Assert.Equal(hitResponse.ValueListString, contentList);
    }

    [Fact]
    public async Task ListFetchAsync_HasContentByteArray_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var field1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidByteArray();
        var contentList = new List<byte[]> { field1, field2 };

        await client.ListPushFrontAsync(cacheName, listName, field2);
        await client.ListPushFrontAsync(cacheName, listName, field1);

        CacheListFetchResponse fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheListFetchResponse.Hit)fetchResponse;

        Assert.Contains(field1, hitResponse.ValueListByteArray!);
        Assert.Contains(field2, hitResponse.ValueListByteArray!);
        Assert.Equal(2, hitResponse.ValueListByteArray!.Count);
    }

    [Fact]
    public async Task CacheListFetchResponse_ToString_HappyPath()
    {
        var listName = Utils.NewGuidString();
        await client.ListConcatenateBackAsync(cacheName, listName, new string[] { "a", "b" });

        CacheListFetchResponse fetchResponse = await client.ListFetchAsync(cacheName, listName);

        Assert.True(fetchResponse is CacheListFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheListFetchResponse.Hit)fetchResponse;
        Assert.Equal("Momento.Sdk.Responses.CacheListFetchResponse+Hit: ValueListString: [\"a\", \"b\"] ValueListByteArray: [\"61\", \"62\"]", hitResponse.ToString());
    }

    [Theory]
    [InlineData(null, "my-list", new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-list", null)]
    public async Task ListRemoveValueAsync_NullChecksByteArray_IsError(string cacheName, string listName, byte[] value)
    {
        CacheListRemoveValueResponse response = await client.ListRemoveValueAsync(cacheName, listName, value);
        Assert.True(response is CacheListRemoveValueResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListRemoveValueResponse.Error)response).ErrorCode);
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
        await client.ListRemoveValueAsync(cacheName, listName, valueOfInterest);

        // Test not there
        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Hit, $"Unexpected response: {response}");
        var cachedList = ((CacheListFetchResponse.Hit)response).ValueListByteArray!;
        Assert.True(list.ListEquals(cachedList));
    }

    [Fact]
    public async Task ListRemoveValueAsync_ValueIsByteArray_ValueNotPresentNoop()
    {
        var listName = Utils.NewGuidString();
        var list = new List<byte[]>() { Utils.NewGuidByteArray(), Utils.NewGuidByteArray(), Utils.NewGuidByteArray() };

        foreach (var value in list)
        {
            await client.ListPushBackAsync(cacheName, listName, value);
        }

        await client.ListRemoveValueAsync(cacheName, listName, Utils.NewGuidByteArray());

        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Hit, $"Unexpected response: {response}");
        var cachedList = ((CacheListFetchResponse.Hit)response).ValueListByteArray!;
        Assert.True(list.ListEquals(cachedList));
    }

    [Fact]
    public async Task ListRemoveValueAsync_ValueIsByteArray_ListNotThereNoop()
    {
        var listName = Utils.NewGuidString();
        Assert.True(await client.ListFetchAsync(cacheName, listName) is CacheListFetchResponse.Miss);
        await client.ListRemoveValueAsync(cacheName, listName, Utils.NewGuidByteArray());
        Assert.True(await client.ListFetchAsync(cacheName, listName) is CacheListFetchResponse.Miss);
    }

    [Theory]
    [InlineData(null, "my-list", "")]
    [InlineData("cache", null, "")]
    [InlineData("cache", "my-list", null)]
    public async Task ListRemoveValueAsync_NullChecksString_IsError(string cacheName, string listName, string value)
    {
        CacheListRemoveValueResponse response = await client.ListRemoveValueAsync(cacheName, listName, value);
        Assert.True(response is CacheListRemoveValueResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListRemoveValueResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task ListRemoveValueAsync_ValueIsString_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var list = new List<string>() { Utils.NewGuidString(), Utils.NewGuidString(), Utils.NewGuidString() };
        var valueOfInterest = Utils.NewGuidString();

        // Add elements to the list
        foreach (var value in list)
        {
            await client.ListPushBackAsync(cacheName, listName, value);
        }

        await client.ListPushBackAsync(cacheName, listName, valueOfInterest);
        await client.ListPushBackAsync(cacheName, listName, valueOfInterest);

        // Remove value of interest
        await client.ListRemoveValueAsync(cacheName, listName, valueOfInterest);

        // Test not there
        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Hit, $"Unexpected response: {response}");
        var cachedList = ((CacheListFetchResponse.Hit)response).ValueListString!;
        Assert.True(list.SequenceEqual(cachedList));
    }

    [Fact]
    public async Task ListRemoveValueAsync_ValueIsByteString_ValueNotPresentNoop()
    {
        var listName = Utils.NewGuidString();
        var list = new List<string>() { Utils.NewGuidString(), Utils.NewGuidString(), Utils.NewGuidString() };

        foreach (var value in list)
        {
            await client.ListPushBackAsync(cacheName, listName, value);
        }

        await client.ListRemoveValueAsync(cacheName, listName, Utils.NewGuidString());

        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.True(response is CacheListFetchResponse.Hit, $"Unexpected response: {response}");
        var cachedList = ((CacheListFetchResponse.Hit)response).ValueListString!;
        Assert.True(list.SequenceEqual(cachedList));
    }

    [Fact]
    public async Task ListRemoveValueAsync_ValueIsString_ListNotThereNoop()
    {
        var listName = Utils.NewGuidString();
        Assert.True(await client.ListFetchAsync(cacheName, listName) is CacheListFetchResponse.Miss);
        await client.ListRemoveValueAsync(cacheName, listName, Utils.NewGuidString());
        Assert.True(await client.ListFetchAsync(cacheName, listName) is CacheListFetchResponse.Miss);
    }

    [Theory]
    [InlineData(null, "my-list")]
    [InlineData("cache", null)]
    public async Task ListLengthAsync_NullChecks_IsError(string cacheName, string listName)
    {
        CacheListLengthResponse response = await client.ListLengthAsync(cacheName, listName);
        Assert.True(response is CacheListLengthResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheListLengthResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task ListLengthAsync_ListIsMissing_HappyPath()
    {
        CacheListLengthResponse lengthResponse = await client.ListLengthAsync(cacheName, Utils.NewGuidString());
        Assert.True(lengthResponse is CacheListLengthResponse.Success, $"Unexpected response: {lengthResponse}");
        var successResponse = (CacheListLengthResponse.Success)lengthResponse;
        Assert.Equal(0, successResponse.Length);
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

    [Fact]
    public async Task ListDeleteAsync_ListDoesNotExist_Noop()
    {
        var listName = Utils.NewGuidString();
        Assert.True((await client.ListFetchAsync(cacheName, listName)) is CacheListFetchResponse.Miss);
        var deleteResponse = await client.DeleteAsync(cacheName, listName);
        Assert.True(deleteResponse is CacheDeleteResponse.Success, $"Unexpected response: {deleteResponse}");
        Assert.True((await client.ListFetchAsync(cacheName, listName)) is CacheListFetchResponse.Miss);
    }

    [Fact]
    public async Task ListDeleteAsync_ListExists_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var pushResponse = await client.ListPushFrontAsync(cacheName, listName, Utils.NewGuidString());
        Assert.True(pushResponse is CacheListPushFrontResponse.Success, $"Unexpected response: {pushResponse}");
        pushResponse = await client.ListPushFrontAsync(cacheName, listName, Utils.NewGuidString());
        Assert.True(pushResponse is CacheListPushFrontResponse.Success, $"Unexpected response: {pushResponse}");
        pushResponse = await client.ListPushFrontAsync(cacheName, listName, Utils.NewGuidString());
        Assert.True(pushResponse is CacheListPushFrontResponse.Success, $"Unexpected response: {pushResponse}");

        Assert.True((await client.ListFetchAsync(cacheName, listName)) is CacheListFetchResponse.Hit);
        var deleteResponse = await client.DeleteAsync(cacheName, listName);
        Assert.True(deleteResponse is CacheDeleteResponse.Success, $"Unexpected response: {deleteResponse}");

        var fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.True(fetchResponse is CacheListFetchResponse.Miss, $"Unexpected response: {fetchResponse}");
    }
}
