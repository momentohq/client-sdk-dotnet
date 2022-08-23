using Momento.Sdk.Responses;

namespace IncubatingIntegrationTest;

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
        var value = Utils.NewGuidByteArray();

        await client.ListPushFrontAsync(cacheName, listName, value, false);

        var fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.HIT, fetchResponse.Status);

        var list = fetchResponse.ByteArrayList;
        Assert.Single(list);
        Assert.Contains(value, list);
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

        await client.ListPushFrontAsync(cacheName, listName, value, false, ttlSeconds: 1);
        await Task.Delay(100);

        await client.ListPushFrontAsync(cacheName, listName, value, true, ttlSeconds: 10);
        await Task.Delay(1000);

        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(2, response.ByteArrayList!.Count);
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
        var value = Utils.NewGuidString();

        await client.ListPushFrontAsync(cacheName, listName, value, false);

        var fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.HIT, fetchResponse.Status);

        var list = fetchResponse.StringList();
        Assert.Single(list);
        Assert.Contains(value, list);
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

        await client.ListPushFrontAsync(cacheName, listName, value, false, ttlSeconds: 1);
        await Task.Delay(100);

        await client.ListPushFrontAsync(cacheName, listName, value, true, ttlSeconds: 10);
        await Task.Delay(1000);

        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(2, response.StringList()!.Count);
    }

    [Fact]
    public async Task ListPushFrontBatchAsync_NullChecksByteArray_ThrowsException()
    {
        var listName = Utils.NewGuidString();
        var list = new List<byte[]>();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.ListPushFrontBatchAsync(null!, listName, list, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.ListPushFrontBatchAsync(cacheName, null!, list, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.ListPushFrontBatchAsync(cacheName, listName, (IEnumerable<byte[]>)null!, false));

        list.Add(null!);
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.ListPushFrontBatchAsync(cacheName, listName, list, false));
    }

    [Fact]
    public async Task ListPushFrontBatchAsync_ValuesAreByteArrayEnumerable_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidByteArray();
        var value2 = Utils.NewGuidByteArray();
        var content = new List<byte[]>() { value1, value2 };

        // TODO: this does prepend instead of iterated push
        await client.ListPushFrontBatchAsync(cacheName, listName, content, false, 10);

        var fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.HIT, fetchResponse.Status);

        var list = fetchResponse.ByteArrayList;
        Assert.Equal(2, list!.Count);
        Assert.Equal(value1, list[0]);
        Assert.Equal(value2, list[1]);
    }

    [Fact]
    public async Task ListPushFrontBatchAsync_ValuesAreByteArrayEnumerable_NoRefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();
        var content = new List<byte[]>() { value };

        await client.ListPushFrontBatchAsync(cacheName, listName, content, false, ttlSeconds: 5);
        await Task.Delay(100);

        await client.ListPushFrontBatchAsync(cacheName, listName, content, false, ttlSeconds: 10);
        await Task.Delay(4900);

        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task ListPushFrontBatchAsync_ValuesAreByteArrayEnumerable_RefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();
        var content = new List<byte[]>() { value };

        await client.ListPushFrontBatchAsync(cacheName, listName, content, false, ttlSeconds: 1);
        await Task.Delay(100);

        await client.ListPushFrontBatchAsync(cacheName, listName, content, true, ttlSeconds: 10);
        await Task.Delay(1000);

        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.HIT, response.Status);

        var list = response.ByteArrayList;
        Assert.Equal(2, list!.Count);
        Assert.Equal(value, list[0]);
        Assert.Equal(value, list[1]);
    }

    [Fact]
    public async Task ListPushFrontBatchAsync_NullChecksString_ThrowsException()
    {
        var listName = Utils.NewGuidString();
        var list = new List<string>();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.ListPushFrontBatchAsync(null!, listName, list, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.ListPushFrontBatchAsync(cacheName, null!, list, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.ListPushFrontBatchAsync(cacheName, listName, (IEnumerable<string>)null!, false));

        list.Add(null!);
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.ListPushFrontBatchAsync(cacheName, listName, list, false));
    }

    [Fact]
    public async Task ListPushFrontBatchAsync_ValuesAreStringEnumerable_HappyPath()
    {
        var listName = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var value2 = Utils.NewGuidString();
        var content = new List<string>() { value1, value2 };

        await client.ListPushFrontBatchAsync(cacheName, listName, content, false, 10);

        var fetchResponse = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.HIT, fetchResponse.Status);

        // TODO: consider iterated push
        var list = fetchResponse.StringList();
        Assert.Equal(2, list!.Count);
        Assert.Equal(value1, list[0]);
        Assert.Equal(value2, list[1]);
    }

    [Fact]
    public async Task ListPushFrontBatchAsync_ValuesAreStringEnumerable_NoRefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidString();
        var content = new List<string>() { value };

        await client.ListPushFrontBatchAsync(cacheName, listName, content, false, ttlSeconds: 5);
        await Task.Delay(100);

        await client.ListPushFrontBatchAsync(cacheName, listName, content, false, ttlSeconds: 10);
        await Task.Delay(4900);

        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task ListPushFrontBatchAsync_ValuesAreStringEnumerable_RefreshTtl()
    {
        var listName = Utils.NewGuidString();
        var value = Utils.NewGuidString();
        var content = new List<string>() { value };

        await client.ListPushFrontBatchAsync(cacheName, listName, content, false, ttlSeconds: 1);
        await Task.Delay(100);

        await client.ListPushFrontBatchAsync(cacheName, listName, content, true, ttlSeconds: 10);
        await Task.Delay(1000);

        var response = await client.ListFetchAsync(cacheName, listName);
        Assert.Equal(CacheGetStatus.HIT, response.Status);

        var list = response.StringList();
        Assert.Equal(2, list!.Count);
        Assert.Equal(value, list[0]);
        Assert.Equal(value, list[1]);
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
}
