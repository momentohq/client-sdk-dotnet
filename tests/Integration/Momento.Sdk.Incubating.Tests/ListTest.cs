using Momento.Sdk.Internal.ExtensionMethods;
using Momento.Sdk.Responses;

namespace Momento.Sdk.Incubating.Tests;

[Collection("SimpleCacheClient")]
public class ListTest : TestBase
{
    public ListTest(SimpleCacheClientFixture fixture) : base(fixture)
    {
    }

    [Theory]
    [InlineData(null, "my-list", new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-list", null)]
    public async Task ListPushFrontAsync_NullChecksByteArray_ThrowsException(string cacheName, string listName, byte[] value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.ListPushFrontAsync(cacheName, listName, value, false));
    }

    [Fact]
    public async Task ListPushFrontFetch_ValueIsByteArray_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidByteArray();

        await client.ListPushFrontAsync(cacheName, listName, value1, false);

        var fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.HIT, fetchResponse.Status);

        var list = fetchResponse.ByteArrayList;
        Assert.Single(list);
        Assert.Contains(value1, list);

        // Test push semantics
        var value2 = Utils.NewGuidByteArray();
        await client.ListPushFrontAsync(cacheName, listName, value2, false);
        fetchResponse = await client.ListFetchAsync(cacheName, listName);

        list = fetchResponse.ByteArrayList!;
        Assert.Equal(value2, list[0]);
        Assert.Equal(value1, list[1]);
    }

    [Fact]
    public async Task ListPushFrontFetch_ValueIsByteArray_NoRefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();

        await client.ListPushFrontAsync(cacheName, listName, value, false, ttlSeconds: 5);
        await Task.Delay(100);

        await client.ListPushFrontAsync(cacheName, listName, value, false, ttlSeconds: 10);
        await Task.Delay(4900);

        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task ListPushFrontFetch_ValueIsByteArray_RefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();

        await client.ListPushFrontAsync(cacheName, listName, value, false, ttlSeconds: 2);
        await client.ListPushFrontAsync(cacheName, listName, value, true, ttlSeconds: 10);
        await Task.Delay(2000);

        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(2, response.ByteArrayList!.Count);
    }

    [Fact]
    public async Task ListPushFrontAsync_ValueIsByteArrayTruncateBackToSizeIsZero_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await client.ListPushFrontAsync("myCache", "listName", new byte[] { 0x00 }, false, truncateBackToSize: 0));
    }

    [Theory]
    [InlineData(null, "my-list", "my-value")]
    [InlineData("cache", null, "my-value")]
    [InlineData("cache", "my-list", null)]
    public async Task ListPushFrontAsync_NullChecksString_ThrowsException(string cacheName, string listName, string value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.ListPushFrontAsync(cacheName, listName, value, false));
    }

    [Fact]
    public async Task ListPushFrontFetch_ValueIsString_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();

        await client.ListPushFrontAsync(cacheName, listName, value1, false);

        var fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.HIT, fetchResponse.Status);

        var list = fetchResponse.StringList();
        Assert.Single(list);
        Assert.Contains(value1, list);

        // Test push semantics
        var value2 = Utils.NewGuidString();
        await client.ListPushFrontAsync(cacheName, listName, value2, false);
        fetchResponse = await client.ListFetchAsync(cacheName, listName);

        list = fetchResponse.StringList()!;
        Assert.Equal(value2, list[0]);
        Assert.Equal(value1, list[1]);
    }

    [Fact]
    public async Task ListPushFrontFetch_ValueIsString_NoRefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidString();

        await client.ListPushFrontAsync(cacheName, listName, value, false, ttlSeconds: 5);
        await Task.Delay(100);

        await client.ListPushFrontAsync(cacheName, listName, value, false, ttlSeconds: 10);
        await Task.Delay(4900);

        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task ListPushFrontFetch_ValueIsString_RefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidString();

        await client.ListPushFrontAsync(cacheName, listName, value, false, ttlSeconds: 2);
        await client.ListPushFrontAsync(cacheName, listName, value, true, ttlSeconds: 10);
        await Task.Delay(2000);

        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(2, response.StringList()!.Count);
    }

    [Fact]
    public async Task ListPushFrontAsync_ValueIsStringTruncateBackToSizeIsZero_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await client.ListPushFrontAsync("myCache", "listName", "value", false, truncateBackToSize: 0));
    }

    [Theory]
    [InlineData(null, "my-list", new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-list", null)]
    public async Task ListPushBackAsync_NullChecksByteArray_ThrowsException(string cacheName, string listName, byte[] value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.ListPushBackAsync(cacheName, listName, value, false));
    }

    [Fact]
    public async Task ListPushBackFetch_ValueIsByteArray_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidByteArray();

        await client.ListPushBackAsync(cacheName, listName, value1, false);

        var fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.HIT, fetchResponse.Status);

        var list = fetchResponse.ByteArrayList;
        Assert.Single(list);
        Assert.Contains(value1, list);

        // Test push semantics
        var value2 = Utils.NewGuidByteArray();
        await client.ListPushBackAsync(cacheName, listName, value2, false);
        fetchResponse = await client.ListFetchAsync(cacheName, listName);

        list = fetchResponse.ByteArrayList!;
        Assert.Equal(value1, list[0]);
        Assert.Equal(value2, list[1]);
    }

    [Fact]
    public async Task ListPushBackFetch_ValueIsByteArray_NoRefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();

        await client.ListPushBackAsync(cacheName, listName, value, false, ttlSeconds: 5);
        await Task.Delay(100);

        await client.ListPushBackAsync(cacheName, listName, value, false, ttlSeconds: 10);
        await Task.Delay(4900);

        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task ListPushBackFetch_ValueIsByteArray_RefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();

        await client.ListPushBackAsync(cacheName, listName, value, false, ttlSeconds: 2);
        await client.ListPushBackAsync(cacheName, listName, value, true, ttlSeconds: 10);
        await Task.Delay(2000);

        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(2, response.ByteArrayList!.Count);
    }

    [Fact]
    public async Task ListPushBackAsync_ValueIsByteArrayTruncateFrontToSizeIsZero_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await client.ListPushBackAsync("myCache", "listName", new byte[] { 0x00 }, false, truncateFrontToSize: 0));
    }

    [Theory]
    [InlineData(null, "my-list", "my-value")]
    [InlineData("cache", null, "my-value")]
    [InlineData("cache", "my-list", null)]
    public async Task ListPushBackAsync_NullChecksString_ThrowsException(string cacheName, string listName, string value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.ListPushBackAsync(cacheName, listName, value, false));
    }

    [Fact]
    public async Task ListPushBackFetch_ValueIsString_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();

        await client.ListPushBackAsync(cacheName, listName, value1, false);

        var fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.HIT, fetchResponse.Status);

        var list = fetchResponse.StringList();
        Assert.Single(list);
        Assert.Contains(value1, list);

        // Test push semantics
        var value2 = Utils.NewGuidString();
        await client.ListPushBackAsync(cacheName, listName, value2, false);
        fetchResponse = await client.ListFetchAsync(cacheName, listName);

        list = fetchResponse.StringList()!;
        Assert.Equal(value1, list[0]);
        Assert.Equal(value2, list[1]);
    }

    [Fact]
    public async Task ListPushBackFetch_ValueIsString_NoRefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidString();

        await client.ListPushBackAsync(cacheName, listName, value, false, ttlSeconds: 5);
        await Task.Delay(100);

        await client.ListPushBackAsync(cacheName, listName, value, false, ttlSeconds: 10);
        await Task.Delay(4900);

        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task ListPushBackFetch_ValueIsString_RefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidString();

        await client.ListPushBackAsync(cacheName, listName, value, false, ttlSeconds: 2);
        await client.ListPushBackAsync(cacheName, listName, value, true, ttlSeconds: 10);
        await Task.Delay(2000);

        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(2, response.StringList()!.Count);
    }

    [Fact]
    public async Task ListPushBackAsync_ValueIsStringTruncateFrontToSizeIsZero_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await client.ListPushBackAsync("myCache", "listName", "value", false, truncateFrontToSize: 0));
    }

    [Theory]
    [InlineData(null, "my-list")]
    [InlineData("cache", null)]
    public async Task ListPopFrontAsync_NullChecks_ThrowsException(string cacheName, string listName)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.ListPopFrontAsync(cacheName, listName));
    }

    [Fact]
    public async Task ListPopFrontAsync_ListIsMissing_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var response = await client.ListPopFrontAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
        Assert.Null(response.ByteArray);
        Assert.Null(response.String());
    }

    [Fact]
    public async Task ListPopFrontAsync_ValueIsByteArray_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidByteArray();
        var value2 = Utils.NewGuidByteArray();

        await client.ListPushFrontAsync(cacheName, listName, value1, false);
        await client.ListPushFrontAsync(cacheName, listName, value2, false);
        var response = await client.ListPopFrontAsync(cacheName, listName);

        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value2, response.ByteArray);
    }

    [Fact]
    public async Task ListPopFrontAsync_ValueIsString_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var value2 = Utils.NewGuidString();

        await client.ListPushFrontAsync(cacheName, listName, value1, false);
        await client.ListPushFrontAsync(cacheName, listName, value2, false);
        var response = await client.ListPopFrontAsync(cacheName, listName);

        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value2, response.String());
    }

    [Theory]
    [InlineData(null, "my-list")]
    [InlineData("cache", null)]
    public async Task ListPopBackAsync_NullChecks_ThrowsException(string cacheName, string listName)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.ListPopBackAsync(cacheName, listName));
    }

    [Fact]
    public async Task ListPopBackAsync_ListIsMissing_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var response = await client.ListPopBackAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
        Assert.Null(response.ByteArray);
        Assert.Null(response.String());
    }

    [Fact]
    public async Task ListPopBackAsync_ValueIsByteArray_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidByteArray();
        var value2 = Utils.NewGuidByteArray();

        await client.ListPushBackAsync(cacheName, listName, value1, false);
        await client.ListPushBackAsync(cacheName, listName, value2, false);
        var response = await client.ListPopBackAsync(cacheName, listName);

        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value2, response.ByteArray);
    }

    [Fact]
    public async Task ListPopBackAsync_ValueIsString_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var value2 = Utils.NewGuidString();

        await client.ListPushBackAsync(cacheName, listName, value1, false);
        await client.ListPushBackAsync(cacheName, listName, value2, false);
        var response = await client.ListPopBackAsync(cacheName, listName);

        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value2, response.String());
    }

    [Theory]
    [InlineData(null, "my-list")]
    [InlineData("cache", null)]
    public async Task ListFetchAsync_NullChecks_ThrowsException(string cacheName, string listName)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.ListFetchAsync(cacheName, listName));
    }

    [Fact]
    public async Task ListFetchAsync_Missing_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
        Assert.Null(response.ByteArrayList);
        Assert.Null(response.StringList());
    }

    [Fact]
    public async Task ListFetchAsync_HasContentString_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var field1 = Utils.NewGuidString();
        var field2 = Utils.NewGuidString();
        var contentList = new List<string>() { field1, field2 };

        await client.ListPushFrontAsync(cacheName, listName, field2, true, ttlSeconds: 10);
        await client.ListPushFrontAsync(cacheName, listName, field1, true, ttlSeconds: 10);

        var fetchResponse = await client.ListFetchAsync(cacheName, listName);

        Assert.Equal(CacheGetStatus.HIT, fetchResponse.Status);
        Assert.Equal(fetchResponse.StringList(), contentList);
    }

    [Fact]
    public async Task ListFetchAsync_HasContentByteArray_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var field1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidByteArray();
        var contentList = new List<byte[]> { field1, field2 };

        await client.ListPushFrontAsync(cacheName, listName, field2, true, ttlSeconds: 10);
        await client.ListPushFrontAsync(cacheName, listName, field1, true, ttlSeconds: 10);

        var fetchResponse = await client.ListFetchAsync(cacheName, listName);

        Assert.Equal(CacheGetStatus.HIT, fetchResponse.Status);

        Assert.Contains(field1, fetchResponse.ByteArrayList!);
        Assert.Contains(field2, fetchResponse.ByteArrayList!);
        Assert.Equal(2, fetchResponse.ByteArrayList!.Count);
    }

    [Theory]
    [InlineData(null, "my-list", new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-list", null)]
    public async Task ListRemoveAllAsync_NullChecksByteArray_ThrowsException(string cacheName, string listName, byte[] value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.ListRemoveAllAsync(cacheName, listName, value));
    }

    [Fact]
    public async Task ListRemoveAllAsync_ValueIsByteArray_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var list = new List<byte[]>() { Utils.NewGuidByteArray(), Utils.NewGuidByteArray(), Utils.NewGuidByteArray() };
        var valueOfInterest = Utils.NewGuidByteArray();

        // Add elements to the list
        foreach (var value in list)
        {
            await client.ListPushBackAsync(cacheName, listName, value, true, ttlSeconds: 60);
        }

        await client.ListPushBackAsync(cacheName, listName, valueOfInterest, false);
        await client.ListPushBackAsync(cacheName, listName, valueOfInterest, false);

        // Remove value of interest
        await client.ListRemoveAllAsync(cacheName, listName, valueOfInterest);

        // Test not there
        var cachedList = (await client.ListFetchAsync(cacheName, listName)).ByteArrayList!;
        Assert.True(list.ListEquals(cachedList));
    }

    [Fact]
    public async Task ListRemoveAllAsync_ValueIsByteArray_ValueNotPresentNoop()
    {
        var listName = Utils.NewGuidString();
        var list = new List<byte[]>() { Utils.NewGuidByteArray(), Utils.NewGuidByteArray(), Utils.NewGuidByteArray() };

        foreach (var value in list)
        {
            await client.ListPushBackAsync(cacheName, listName, value, false);
        }

        await client.ListRemoveAllAsync(cacheName, listName, Utils.NewGuidByteArray());

        var cachedList = (await client.ListFetchAsync(cacheName, listName)).ByteArrayList!;
        Assert.True(list.ListEquals(cachedList));
    }

    [Fact]
    public async Task ListRemoveAllAsync_ValueIsByteArray_ListNotThereNoop()
    {
        var listName = Utils.NewGuidString();
        Assert.Equal(CacheGetStatus.MISS, (await client.ListFetchAsync(cacheName, listName)).Status);
        await client.ListRemoveAllAsync(cacheName, listName, Utils.NewGuidByteArray());
        Assert.Equal(CacheGetStatus.MISS, (await client.ListFetchAsync(cacheName, listName)).Status);
    }

    [Theory]
    [InlineData(null, "my-list", "")]
    [InlineData("cache", null, "")]
    [InlineData("cache", "my-list", null)]
    public async Task ListRemoveAllAsync_NullChecksString_ThrowsException(string cacheName, string listName, string value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.ListRemoveAllAsync(cacheName, listName, value));
    }

    [Fact]
    public async Task ListRemoveAllAsync_ValueIsString_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var list = new List<string>() { Utils.NewGuidString(), Utils.NewGuidString(), Utils.NewGuidString() };
        var valueOfInterest = Utils.NewGuidString();

        // Add elements to the list
        foreach (var value in list)
        {
            await client.ListPushBackAsync(cacheName, listName, value, true, ttlSeconds: 60);
        }

        await client.ListPushBackAsync(cacheName, listName, valueOfInterest, false);
        await client.ListPushBackAsync(cacheName, listName, valueOfInterest, false);

        // Remove value of interest
        await client.ListRemoveAllAsync(cacheName, listName, valueOfInterest);

        // Test not there
        var cachedList = (await client.ListFetchAsync(cacheName, listName)).StringList()!;
        Assert.True(list.SequenceEqual(cachedList));
    }

    [Fact]
    public async Task ListRemoveAllAsync_ValueIsByteString_ValueNotPresentNoop()
    {
        var listName = Utils.NewGuidString();
        var list = new List<string>() { Utils.NewGuidString(), Utils.NewGuidString(), Utils.NewGuidString() };

        foreach (var value in list)
        {
            await client.ListPushBackAsync(cacheName, listName, value, false);
        }

        await client.ListRemoveAllAsync(cacheName, listName, Utils.NewGuidString());

        var cachedList = (await client.ListFetchAsync(cacheName, listName)).StringList()!;
        Assert.True(list.SequenceEqual(cachedList));
    }

    [Fact]
    public async Task ListRemoveAllAsync_ValueIsString_ListNotThereNoop()
    {
        var listName = Utils.NewGuidString();
        Assert.Equal(CacheGetStatus.MISS, (await client.ListFetchAsync(cacheName, listName)).Status);
        await client.ListRemoveAllAsync(cacheName, listName, Utils.NewGuidString());
        Assert.Equal(CacheGetStatus.MISS, (await client.ListFetchAsync(cacheName, listName)).Status);
    }

    [Theory]
    [InlineData(null, "my-list")]
    [InlineData("cache", null)]
    public async Task ListLengthAsync_NullChecks_ThrowsException(string cacheName, string listName)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.ListLengthAsync(cacheName, listName));
    }

    [Fact]
    public async Task ListLengthAsync_ListIsMissing_HappyPath()
    {
        var lengthResponse = await client.ListLengthAsync(cacheName, Utils.NewGuidString());
        Assert.Equal(0, lengthResponse.Length);
    }

    [Fact]
    public async Task ListLengthAsync_ListIsFound_HappyPath()
    {
        var listName = Utils.NewGuidString();
        foreach (var i in Enumerable.Range(0, 10))
        {
            await client.ListPushBackAsync(cacheName, listName, Utils.NewGuidByteArray(), false);
        }

        var lengthResponse = await client.ListLengthAsync(cacheName, listName);
        Assert.Equal(10, lengthResponse.Length);
    }
}
