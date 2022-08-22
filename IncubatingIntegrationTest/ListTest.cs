using Momento.Sdk.Responses;

namespace IncubatingIntegrationTest;

[Collection("SimpleCacheClient")]
public class ListTest : TestBase
{
    public ListTest(SimpleCacheClientFixture fixture) : base(fixture)
    {
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
