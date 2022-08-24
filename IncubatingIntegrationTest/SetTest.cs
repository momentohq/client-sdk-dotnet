using Momento.Sdk.Responses;

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
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidByteArray();

        await client.SetAddAsync(cacheName, setName, element, false);

        var fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.HIT, fetchResponse.Status);

        var set = fetchResponse.ByteArraySet;
        Assert.Single(set);
        Assert.Contains(element, set);
    }

    [Fact]
    public async Task SetAddFetch_ElementIsByteArray_NoRefreshTtl()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidByteArray();

        await client.SetAddAsync(cacheName, setName, element, false, ttlSeconds: 5);
        await Task.Delay(100);

        await client.SetAddAsync(cacheName, setName, element, false, ttlSeconds: 10);
        await Task.Delay(4900);

        var response = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task SetAddFetch_ElementIsByteArray_RefreshTtl()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidByteArray();

        await client.SetAddAsync(cacheName, setName, element, false, ttlSeconds: 1);
        await client.SetAddAsync(cacheName, setName, element, true, ttlSeconds: 10);
        await Task.Delay(1000);

        var response = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Single(response.ByteArraySet);
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
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidString();

        await client.SetAddAsync(cacheName, setName, element, false);

        var fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.HIT, fetchResponse.Status);

        var set = fetchResponse.StringSet();
        Assert.Single(set);
        Assert.Contains(element, set);
    }

    [Fact]
    public async Task SetAddFetch_ElementIsString_NoRefreshTtl()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidString();

        await client.SetAddAsync(cacheName, setName, element, false, ttlSeconds: 5);
        await Task.Delay(100);

        await client.SetAddAsync(cacheName, setName, element, false, ttlSeconds: 10);
        await Task.Delay(4900);

        var response = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task SetAddFetch_ElementIsString_RefreshTtl()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidString();

        await client.SetAddAsync(cacheName, setName, element, false, ttlSeconds: 1);
        await client.SetAddAsync(cacheName, setName, element, true, ttlSeconds: 10);
        await Task.Delay(1000);

        var response = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Single(response.StringSet());
    }

    [Fact]
    public async Task SetAddBatchAsync_NullChecksByteArray_ThrowsException()
    {
        var setName = Utils.NewGuidString();
        var set = new HashSet<byte[]>();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(null!, setName, set, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(cacheName, null!, set, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(cacheName, setName, (IEnumerable<byte[]>)null!, false));

        set.Add(null!);
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(cacheName, setName, set, false));
    }

    [Fact]
    public async Task SetAddBatchAsync_ElementsAreByteArrayEnumerable_HappyPath()
    {
        var setName = Utils.NewGuidString();
        var element1 = Utils.NewGuidByteArray();
        var element2 = Utils.NewGuidByteArray();
        var content = new List<byte[]>() { element1, element2 };

        await client.SetAddBatchAsync(cacheName, setName, content, false, 10);

        var fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.HIT, fetchResponse.Status);

        var set = fetchResponse.ByteArraySet;
        Assert.Equal(2, set!.Count);
        Assert.Contains(element1, set);
        Assert.Contains(element2, set);
    }

    [Fact]
    public async Task SetAddBatchAsync_ElementsAreByteArrayEnumerable_NoRefreshTtl()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidByteArray();
        var content = new List<byte[]>() { element };

        await client.SetAddBatchAsync(cacheName, setName, content, false, ttlSeconds: 5);
        await Task.Delay(100);

        await client.SetAddBatchAsync(cacheName, setName, content, false, ttlSeconds: 10);
        await Task.Delay(4900);

        var response = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task SetAddBatchAsync_ElementsAreByteArrayEnumerable_RefreshTtl()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidByteArray();
        var content = new List<byte[]>() { element };

        await client.SetAddBatchAsync(cacheName, setName, content, false, ttlSeconds: 1);
        await client.SetAddBatchAsync(cacheName, setName, content, true, ttlSeconds: 10);
        await Task.Delay(1000);

        var response = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.HIT, response.Status);

        var set = response.ByteArraySet;
        Assert.Single(set);
        Assert.Contains(element, set);
    }

    [Fact]
    public async Task SetAddBatchAsync_NullChecksString_ThrowsException()
    {
        var setName = Utils.NewGuidString();
        var set = new HashSet<string>();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(null!, setName, set, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(cacheName, null!, set, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(cacheName, setName, (IEnumerable<string>)null!, false));

        set.Add(null!);
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAddBatchAsync(cacheName, setName, set, false));
    }

    [Fact]
    public async Task SetAddBatchAsync_ElementsAreStringEnumerable_HappyPath()
    {
        var setName = Utils.NewGuidString();
        var element1 = Utils.NewGuidString();
        var element2 = Utils.NewGuidString();
        var content = new List<string>() { element1, element2 };

        await client.SetAddBatchAsync(cacheName, setName, content, false, 10);

        var fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.HIT, fetchResponse.Status);

        var set = fetchResponse.StringSet();
        Assert.Equal(2, set!.Count);
        Assert.Contains(element1, set);
        Assert.Contains(element2, set);
    }

    [Fact]
    public async Task SetAddBatchAsync_ElementsAreStringEnumerable_NoRefreshTtl()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidString();
        var content = new List<string>() { element };

        await client.SetAddBatchAsync(cacheName, setName, content, false, ttlSeconds: 5);
        await Task.Delay(100);

        await client.SetAddBatchAsync(cacheName, setName, content, false, ttlSeconds: 10);
        await Task.Delay(4900);

        var response = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task SetAddBatchAsync_ElementsAreStringEnumerable_RefreshTtl()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidString();
        var content = new List<string>() { element };

        await client.SetAddBatchAsync(cacheName, setName, content, false, ttlSeconds: 1);
        await client.SetAddBatchAsync(cacheName, setName, content, true, ttlSeconds: 10);
        await Task.Delay(1000);

        var response = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.HIT, response.Status);

        var set = response.StringSet();
        Assert.Single(set);
        Assert.Contains(element, set);
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
        var setName = Utils.NewGuidString();
        var response = await client.SetFetchAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
        Assert.Null(response.ByteArraySet);
        Assert.Null(response.StringSet());
    }

    [Fact]
    public async Task SetFetchAsync_UsesCachedByteArraySet_HappyPath()
    {
        var setName = Utils.NewGuidString();
        await client.SetAddBatchAsync(cacheName, setName, new string[] { Utils.NewGuidString(), Utils.NewGuidString() }, false);
        var response = await client.SetFetchAsync(cacheName, setName);

        var set1 = response.ByteArraySet;
        var set2 = response.ByteArraySet;
        Assert.Same(set1, set2);
    }

    [Fact]
    public async Task SetFetchAsync_UsesCachedStringSet_HappyPath()
    {
        var setName = Utils.NewGuidString();
        await client.SetAddBatchAsync(cacheName, setName, new string[] { Utils.NewGuidString(), Utils.NewGuidString() }, false);
        var response = await client.SetFetchAsync(cacheName, setName);

        var set1 = response.StringSet();
        var set2 = response.StringSet();
        Assert.Same(set1, set2);
    }

    [Theory]
    [InlineData(null, "my-set")]
    [InlineData("my-cache", null)]
    public async Task SetDeleteAsync_NullChecks_ThrowsException(string cacheName, string setName)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetDeleteAsync(cacheName, setName));
    }

    [Fact]
    public async Task SetDeleteAsync_SetDoesNotExist_Noop()
    {
        var setName = Utils.NewGuidString();
        Assert.Equal(CacheGetStatus.MISS, (await client.SetFetchAsync(cacheName, setName)).Status);
        await client.SetDeleteAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.MISS, (await client.SetFetchAsync(cacheName, setName)).Status);
    }

    [Fact]
    public async Task SetDeleteAsync_SetExists_HappyPath()
    {
        var setName = Utils.NewGuidString();
        await client.SetAddAsync(cacheName, setName, Utils.NewGuidString(), false);
        await client.SetAddAsync(cacheName, setName, Utils.NewGuidString(), false);
        await client.SetAddAsync(cacheName, setName, Utils.NewGuidString(), false);

        Assert.Equal(CacheGetStatus.HIT, (await client.SetFetchAsync(cacheName, setName)).Status);
        await client.SetDeleteAsync(cacheName, setName);
        Assert.Equal(CacheGetStatus.MISS, (await client.SetFetchAsync(cacheName, setName)).Status);
    }
}
