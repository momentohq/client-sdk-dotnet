using MomentoSdk.Responses;

namespace IncubatingIntegrationTest;

[Collection("SimpleCacheClient")]
public class SetTest : TestBase
{
    public SetTest(SimpleCacheClientFixture fixture) : base(fixture)
    {
    }

    [Theory]
    [InlineData(null, "my-set", new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-set", null)]
    public async Task SetAddAsync_NullChecksByteArray_ThrowsException(string cacheName, string setName, byte[] element)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddAsync(cacheName, setName, element, false));
    }

    [Fact]
    public async Task SetAddFetch_ElementIsByteArray_HappyPath()
    {
        var setName = Utils.GuidString();
        var element = Utils.GuidBytes();

        await client.SetAddAsync(cacheName, setName, element, false);

        var fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.HIT, fetchResponse.Status);

        var set = fetchResponse.ByteArraySet;
        Assert.Equal(1, set!.Count);
        Assert.True(set.Contains(element));
    }

    [Fact]
    public async Task SetAddFetch_ElementIsByteArray_NoRefreshTtl()
    {
        var setName = Utils.GuidString();
        var element = Utils.GuidBytes();

        await client.SetAddAsync(cacheName, setName, element, false, ttlSeconds: 5);
        Thread.Sleep(100);

        await client.SetAddAsync(cacheName, setName, element, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task SetAddFetch_ElementIsByteArray_RefreshTtl()
    {
        var setName = Utils.GuidString();
        var element = Utils.GuidBytes();

        await client.SetAddAsync(cacheName, setName, element, false, ttlSeconds: 1);
        Thread.Sleep(100);

        await client.SetAddAsync(cacheName, setName, element, true, ttlSeconds: 10);
        Thread.Sleep(1000);

        var response = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(1, response.ByteArraySet!.Count);
    }

    [Theory]
    [InlineData(null, "my-set", "my-element")]
    [InlineData("cache", null, "my-element")]
    [InlineData("cache", "my-set", null)]
    public async Task SetAddAsync_NullChecksString_ThrowsException(string cacheName, string setName, string element)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddAsync(cacheName, setName, element, false));
    }

    [Fact]
    public async Task SetAddFetch_ElementIsString_HappyPath()
    {
        var setName = Utils.GuidString();
        var element = Utils.GuidString();

        await client.SetAddAsync(cacheName, setName, element, false);

        var fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.HIT, fetchResponse.Status);

        var set = fetchResponse.StringSet();
        Assert.Equal(1, set!.Count);
        Assert.True(set.Contains(element));
    }

    [Fact]
    public async Task SetAddFetch_ElementIsString_NoRefreshTtl()
    {
        var setName = Utils.GuidString();
        var element = Utils.GuidString();

        await client.SetAddAsync(cacheName, setName, element, false, ttlSeconds: 5);
        Thread.Sleep(100);

        await client.SetAddAsync(cacheName, setName, element, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task SetAddFetch_ElementIsString_RefreshTtl()
    {
        var setName = Utils.GuidString();
        var element = Utils.GuidString();

        await client.SetAddAsync(cacheName, setName, element, false, ttlSeconds: 1);
        Thread.Sleep(100);

        await client.SetAddAsync(cacheName, setName, element, true, ttlSeconds: 10);
        Thread.Sleep(1000);

        var response = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(1, response.StringSet()!.Count);
    }

    [Fact]
    public async Task SetAddBatchAsync_NullChecksByteArray_ThrowsException()
    {
        var setName = Utils.GuidString();
        var set = new HashSet<byte[]>();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(null!, setName, set, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(cacheName, null!, set, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(cacheName, setName, (IEnumerable<byte[]>)null!, false));

        set.Add(null!);
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(cacheName, setName, set, false));

        // Variadic args
        var byteArray = Utils.GuidBytes();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(null!, setName, false, ttlSeconds: null, byteArray));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(cacheName, null!, false, ttlSeconds: null, byteArray));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(cacheName, setName, false, ttlSeconds: null, (byte[])null!));
    }

    [Fact]
    public async Task SetAddBatchAsync_NullChecksString_ThrowsException()
    {
        var setName = Utils.GuidString();
        var set = new HashSet<string>();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(null!, setName, set, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(cacheName, null!, set, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(cacheName, setName, (IEnumerable<string>)null!, false));

        set.Add(null!);
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(cacheName, setName, set, false));

        // Variadic args
        var str = Utils.GuidString();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(null!, setName, false, ttlSeconds: null, str));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(cacheName, null!, false, ttlSeconds: null, str));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(cacheName, setName, false, ttlSeconds: null, (string)null!));
    }

    [Theory]
    [InlineData(null, "my-set")]
    [InlineData("cache", null)]
    public async Task SetFetchAsync_NullChecks_ThrowsException(string cacheName, string setName)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetFetchAsync(cacheName, setName));
    }

    [Fact]
    public async Task SetFetchAsync_Missing_HappyPath()
    {
        var setName = Utils.GuidString();
        var response = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
        Assert.Null(response.ByteArraySet);
        Assert.Null(response.StringSet());
    }

    [Theory]
    [InlineData(null, "my-set")]
    [InlineData("my-cache", null)]
    public async Task SetDeleteAsync_NullChecks_ThrowsException(string cacheName, string setName)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetDeleteAsync(cacheName, setName));
    }
}
