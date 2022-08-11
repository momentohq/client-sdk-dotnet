using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomentoIntegrationTest;

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
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAsync(cacheName, key, value));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAsync(cacheName, key, value, DefaultTtlSeconds));
    }

    // Tests SetAsyc(cacheName, byte[], byte[]) as well as GetAsync(cacheName, byte[])
    [Fact]
    public async Task SetAsync_KeyIsByteArrayValueIsByteArray_HappyPath()
    {
        byte[] key = Utils.NewGuidByteArray();
        byte[] value = Utils.NewGuidByteArray();
        await client.SetAsync(cacheName, key, value);
        byte[]? setValue = (await client.GetAsync(cacheName, key)).Bytes;
        Assert.Equal(value, setValue);

        key = Utils.NewGuidByteArray();
        value = Utils.NewGuidByteArray();
        await client.SetAsync(cacheName, key, value, ttlSeconds: 15);
        setValue = (await client.GetAsync(cacheName, key)).Bytes;
        Assert.Equal(value, setValue);
    }

    [Theory]
    [InlineData(null, new byte[] { 0x00 })]
    [InlineData("cache", null)]
    public async Task GetAsync_NullChecksByteArray_ThrowsException(string cacheName, byte[] key)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetAsync(cacheName, key));
    }

    [Theory]
    [InlineData(null, "key", "value")]
    [InlineData("cache", null, "value")]
    [InlineData("cache", "key", null)]
    public async Task SetAsync_NullChecksStringString_ThrowsException(string cacheName, string key, string value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAsync(cacheName, key, value));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAsync(cacheName, key, value, DefaultTtlSeconds));
    }

    // Also tests GetAsync(cacheName, string)
    [Fact]
    public async Task SetAsync_KeyIsStringValueIsString_HappyPath()
    {
        string key = Utils.NewGuidString();
        string value = Utils.NewGuidString();
        await client.SetAsync(cacheName, key, value);
        string? setValue = (await client.GetAsync(cacheName, key)).String();
        Assert.Equal(value, setValue);

        key = Utils.NewGuidString();
        value = Utils.NewGuidString();
        await client.SetAsync(cacheName, key, value, ttlSeconds: 15);
        setValue = (await client.GetAsync(cacheName, key)).String();
        Assert.Equal(value, setValue);
    }

    [Theory]
    [InlineData(null, "key")]
    [InlineData("cache", null)]
    public async Task GetAsync_NullChecksString_ThrowsException(string cacheName, string key)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetAsync(cacheName, key));
    }

    [Theory]
    [InlineData(null, "key", new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", "key", null)]
    public async Task SetAsync_NullChecksStringByteArray_ThrowsException(string cacheName, string key, byte[] value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAsync(cacheName, key, value));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAsync(cacheName, key, value, DefaultTtlSeconds));
    }

    // Also tests GetAsync(cacheName, string)
    [Fact]
    public async Task SetAsync_KeyIsStringValueIsByteArray_HappyPath()
    {
        string key = Utils.NewGuidString();
        byte[] value = Utils.NewGuidByteArray();
        await client.SetAsync(cacheName, key, value);
        byte[]? setValue = (await client.GetAsync(cacheName, key)).Bytes;
        Assert.Equal(value, setValue);

        key = Utils.NewGuidString();
        value = Utils.NewGuidByteArray();
        await client.SetAsync(cacheName, key, value, ttlSeconds: 15);
        setValue = (await client.GetAsync(cacheName, key)).Bytes;
        Assert.Equal(value, setValue);
    }

    [Fact]
    public async Task GetBatchAsync_NullCheckByteArray_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetBatchAsync(null!, new List<byte[]>()));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetBatchAsync("cache", (List<byte[]>)null!));

        var badList = new List<byte[]>(new byte[][] { Utils.NewGuidByteArray(), null! });
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetBatchAsync("cache", badList));

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetBatchAsync("cache", Utils.NewGuidByteArray(), null!));
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
        string? stringResult1 = result.Strings().ToList()[0];
        string? stringResult2 = result.Strings().ToList()[1];
        Assert.Equal(value1, stringResult1);
        Assert.Equal(value2, stringResult2);
    }

    [Fact]
    public async Task GetBatchAsync_KeysAreByteArray_HappyPath2()
    {
        string key1 = Utils.NewGuidString();
        string value1 = Utils.NewGuidString();
        string key2 = Utils.NewGuidString();
        string value2 = Utils.NewGuidString();
        await client.SetAsync(cacheName, key1, value1);
        await client.SetAsync(cacheName, key2, value2);

        CacheGetBatchResponse result = await client.GetBatchAsync(cacheName, key1, key2);
        string? stringResult1 = result.Strings().ToList()[0];
        string? stringResult2 = result.Strings().ToList()[1];
        Assert.Equal(value1, stringResult1);
        Assert.Equal(value2, stringResult2);
    }

    [Fact]
    public async Task GetBatchAsync_NullCheckString_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetBatchAsync(null!, new List<string>()));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetBatchAsync("cache", (List<string>)null!));

        List<string> strings = new(new string[] { "key1", "key2", null! });
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetBatchAsync("cache", strings));

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetBatchAsync("cache", "key1", "key2", null!));
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

        Assert.Equal(result.Strings(), new string[] { value1, value2, null! });
        Assert.Equal(result.Status, new CacheGetStatus[] { CacheGetStatus.HIT, CacheGetStatus.HIT, CacheGetStatus.MISS });
    }

    [Fact]
    public async Task GetBatchAsync_Failure()
    {
        // Set very small timeout for dataClientOperationTimeoutMilliseconds
        using SimpleCacheClient simpleCacheClient = new SimpleCacheClient(authToken, DefaultTtlSeconds, 1);
        List<string> keys = new() { Utils.NewGuidString(), Utils.NewGuidString(), Utils.NewGuidString(), Utils.NewGuidString() };
        await Assert.ThrowsAsync<MomentoSdk.Exceptions.TimeoutException>(async () => await simpleCacheClient.GetBatchAsync(cacheName, keys));
    }

    [Fact]
    public async Task SetBatchAsync_NullCheckByteArray_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetBatchAsync(null!, new Dictionary<byte[], byte[]>()));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetBatchAsync("cache", (Dictionary<byte[], byte[]>)null!));

        var badDictionary = new Dictionary<byte[], byte[]>() { { Utils.Utf8ToByteArray("asdf"), null! } };
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetBatchAsync("cache", badDictionary));
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
        Assert.Equal(value1, getResponse.Bytes);

        getResponse = await client.GetAsync(cacheName, key2);
        Assert.Equal(value2, getResponse.Bytes);
    }

    [Fact]
    public async Task SetBatchAsync_NullCheckStrings_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetBatchAsync(null!, new Dictionary<string, string>()));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetBatchAsync("cache", (Dictionary<string, string>)null!));

        var badDictionary = new Dictionary<string, string>() { { "asdf", null! } };
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetBatchAsync("cache", badDictionary));
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
        Assert.Equal(value1, getResponse.String());

        getResponse = await client.GetAsync(cacheName, key2);
        Assert.Equal(value2, getResponse.String());
    }

    [Fact]
    public async Task GetAsync_ExpiredTtl_HappyPath()
    {
        string key = Utils.NewGuidString();
        string value = Utils.NewGuidString();
        await client.SetAsync(cacheName, key, value, 1);
        await Task.Delay(3000);
        CacheGetResponse result = await client.GetAsync(cacheName, key);
        Assert.Equal(CacheGetStatus.MISS, result.Status);
    }

    [Theory]
    [InlineData(null, new byte[] { 0x00 })]
    [InlineData("cache", null)]
    public async Task DeleteAsync_NullChecksByte_ThrowsException(string cacheName, byte[] key)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DeleteAsync(cacheName, key));
    }

    [Fact]
    public async Task DeleteAsync_KeyIsByteArray_HappyPath()
    {
        // Set a key to then delete
        byte[] key = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        byte[] value = new byte[] { 0x05, 0x06, 0x07, 0x08 };
        await client.SetAsync(cacheName, key, value, ttlSeconds: 60);
        CacheGetResponse getResponse = await client.GetAsync(cacheName, key);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);

        // Delete
        await client.DeleteAsync(cacheName, key);

        // Check deleted
        getResponse = await client.GetAsync(cacheName, key);
        Assert.Equal(CacheGetStatus.MISS, getResponse.Status);
    }

    [Theory]
    [InlineData(null, "key")]
    [InlineData("cache", null)]
    public async Task DeleteAsync_NullChecksString_ThrowsException(string cacheName, string key)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DeleteAsync(cacheName, key));
    }

    [Fact]
    public async Task DeleteAsync_KeyIsString_HappyPath()
    {
        // Set a key to then delete
        string key = Utils.NewGuidString();
        string value = Utils.NewGuidString();
        await client.SetAsync(cacheName, key, value, ttlSeconds: 60);
        CacheGetResponse getResponse = await client.GetAsync(cacheName, key);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);

        // Delete
        await client.DeleteAsync(cacheName, key);

        // Check deleted
        getResponse = await client.GetAsync(cacheName, key);
        Assert.Equal(CacheGetStatus.MISS, getResponse.Status);
    }
}
