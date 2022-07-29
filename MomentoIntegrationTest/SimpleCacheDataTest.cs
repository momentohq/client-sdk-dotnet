using System.Threading.Tasks;
using System.Collections.Generic;

namespace MomentoIntegrationTest;

[Collection("SimpleCacheClient")]
public class SimpleCacheDataTest
{
    private readonly string authToken;
    private const string CacheName = SimpleCacheClientFixture.CacheName;
    private const uint DefaultTtlSeconds = SimpleCacheClientFixture.DefaultTtlSeconds;
    private SimpleCacheClient client;

    // Test initialization
    public SimpleCacheDataTest(SimpleCacheClientFixture fixture)
    {
        client = fixture.Client;
        authToken = fixture.AuthToken;
    }

    [Theory]
    [InlineData(null, new byte[] { 0x00 }, new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", new byte[] { 0x00 }, null)]
    public async void SetAsync_NullChecksBytesBytes_ThrowsException(string cacheName, byte[] key, byte[] value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAsync(cacheName, key, value));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAsync(cacheName, key, value, DefaultTtlSeconds));
    }

    // Tests SetAsyc(cacheName, byte[], byte[]) as well as GetAsync(cacheName, byte[])
    [Fact]
    public async void SetAsync_KeyIsBytesValueIsBytes_HappyPath()
    {
        byte[] key = Utils.GuidBytes();
        byte[] value = Utils.GuidBytes();
        await client.SetAsync(CacheName, key, value);
        byte[]? setValue = (await client.GetAsync(CacheName, key)).Bytes;
        Assert.Equal(value, setValue);

        key = Utils.GuidBytes();
        value = Utils.GuidBytes();
        await client.SetAsync(CacheName, key, value, ttlSeconds: 15);
        setValue = (await client.GetAsync(CacheName, key)).Bytes;
        Assert.Equal(value, setValue);
    }

    [Theory]
    [InlineData(null, new byte[] { 0x00 })]
    [InlineData("cache", null)]
    public async void GetAsync_NullChecksBytes_ThrowsException(string cacheName, byte[] key)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetAsync(cacheName, key));
    }

    [Theory]
    [InlineData(null, "key", "value")]
    [InlineData("cache", null, "value")]
    [InlineData("cache", "key", null)]
    public async void SetAsync_NullChecksStringString_ThrowsException(string cacheName, string key, string value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAsync(cacheName, key, value));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAsync(cacheName, key, value, DefaultTtlSeconds));
    }

    // Also tests GetAsync(cacheName, string)
    [Fact]
    public async void SetAsync_KeyIsStringValueIsString_HappyPath()
    {
        string key = Utils.GuidString();
        string value = Utils.GuidString();
        await client.SetAsync(CacheName, key, value);
        string? setValue = (await client.GetAsync(CacheName, key)).String();
        Assert.Equal(value, setValue);

        key = Utils.GuidString();
        value = Utils.GuidString();
        await client.SetAsync(CacheName, key, value, ttlSeconds: 15);
        setValue = (await client.GetAsync(CacheName, key)).String();
        Assert.Equal(value, setValue);
    }

    [Theory]
    [InlineData(null, "key")]
    [InlineData("cache", null)]
    public async void GetAsync_NullChecksString_ThrowsException(string cacheName, string key)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetAsync(cacheName, key));
    }

    [Theory]
    [InlineData(null, "key", new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", "key", null)]
    public async void SetAsync_NullChecksStringBytes_ThrowsException(string cacheName, string key, byte[] value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAsync(cacheName, key, value));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetAsync(cacheName, key, value, DefaultTtlSeconds));
    }

    // Also tests GetAsync(cacheName, string)
    [Fact]
    public async void SetAsync_KeyIsStringValueIsBytes_HappyPath()
    {
        string key = Utils.GuidString();
        byte[] value = Utils.GuidBytes();
        await client.SetAsync(CacheName, key, value);
        byte[]? setValue = (await client.GetAsync(CacheName, key)).Bytes;
        Assert.Equal(value, setValue);

        key = Utils.GuidString();
        value = Utils.GuidBytes();
        await client.SetAsync(CacheName, key, value, ttlSeconds: 15);
        setValue = (await client.GetAsync(CacheName, key)).Bytes;
        Assert.Equal(value, setValue);
    }

    [Fact]
    public async void GetBatchAsync_NullCheckBytes_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetBatchAsync(null!, new List<byte[]>()));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetBatchAsync("cache", (List<byte[]>)null!));

        var badList = new List<byte[]>(new byte[][] { Utils.GuidBytes(), null! });
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetBatchAsync("cache", badList));

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetBatchAsync("cache", Utils.GuidBytes(), null!));
    }

    [Fact]
    public async void GetBatchAsync_KeysAreBytes_HappyPath()
    {
        string key1 = Utils.GuidString();
        string value1 = Utils.GuidString();
        string key2 = Utils.GuidString();
        string value2 = Utils.GuidString();
        client.Set(CacheName, key1, value1);
        client.Set(CacheName, key2, value2);

        List<byte[]> keys = new() { Utils.Utf8ToBytes(key1), Utils.Utf8ToBytes(key2) };

        CacheGetBatchResponse result = await client.GetBatchAsync(CacheName, keys);
        string? stringResult1 = result.Strings()[0];
        string? stringResult2 = result.Strings()[1];
        Assert.Equal(value1, stringResult1);
        Assert.Equal(value2, stringResult2);
    }

    [Fact]
    public async void GetBatchAsync_KeysAreBytes_HappyPath2()
    {
        string key1 = Utils.GuidString();
        string value1 = Utils.GuidString();
        string key2 = Utils.GuidString();
        string value2 = Utils.GuidString();
        client.Set(CacheName, key1, value1);
        client.Set(CacheName, key2, value2);

        CacheGetBatchResponse result = await client.GetBatchAsync(CacheName, key1, key2);
        string? stringResult1 = result.Strings()[0];
        string? stringResult2 = result.Strings()[1];
        Assert.Equal(value1, stringResult1);
        Assert.Equal(value2, stringResult2);
    }

    [Fact]
    public async void GetBatchAsync_NullCheckString_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetBatchAsync(null!, new List<string>()));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetBatchAsync("cache", (List<string>)null!));

        List<string> strings = new(new string[] { "key1", "key2", null! });
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetBatchAsync("cache", strings));

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetBatchAsync("cache", "key1", "key2", null!));
    }

    [Fact]
    public async void GetBatchAsync_KeysAreString_HappyPath()
    {
        string key1 = Utils.GuidString();
        string value1 = Utils.GuidString();
        string key2 = Utils.GuidString();
        string value2 = Utils.GuidString();
        client.Set(CacheName, key1, value1);
        client.Set(CacheName, key2, value2);

        List<string> keys = new() { key1, key2, "key123123" };
        CacheGetBatchResponse result = await client.GetBatchAsync(CacheName, keys);

        Assert.Equal(result.Strings(), new string[] { value1, value2, null! });
        Assert.Equal(result.Status, new CacheGetStatus[] { CacheGetStatus.HIT, CacheGetStatus.HIT, CacheGetStatus.MISS });
    }

    [Fact]
    public void GetBatchAsync_Failure()
    {
        // Set very small timeout for dataClientOperationTimeoutMilliseconds
        SimpleCacheClient simpleCacheClient = new SimpleCacheClient(authToken, DefaultTtlSeconds, 1);
        List<string> keys = new() { Utils.GuidString(), Utils.GuidString(), Utils.GuidString(), Utils.GuidString() };
        Assert.ThrowsAsync<MomentoSdk.Exceptions.TimeoutException>(() => simpleCacheClient.GetBatchAsync(CacheName, keys));
    }

    [Fact]
    public async void SetBatchAsync_NullCheckBytes_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetBatchAsync(null!, new Dictionary<byte[], byte[]>()));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetBatchAsync("cache", (Dictionary<byte[], byte[]>)null!));

        var badDictionary = new Dictionary<byte[], byte[]>() { { Utils.Utf8ToBytes("asdf"), null! } };
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetBatchAsync("cache", badDictionary));
    }

    [Fact]
    public async void SetBatchAsync_ItemsAreBytes_HappyPath()
    {
        var key1 = Utils.GuidBytes();
        var key2 = Utils.GuidBytes();
        var value1 = Utils.GuidBytes();
        var value2 = Utils.GuidBytes();

        var dictionary = new Dictionary<byte[], byte[]>() {
                { key1, value1 },
                { key2, value2 }
            };
        await client.SetBatchAsync(CacheName, dictionary);

        var getResponse = await client.GetAsync(CacheName, key1);
        Assert.Equal(value1, getResponse.Bytes);

        getResponse = await client.GetAsync(CacheName, key2);
        Assert.Equal(value2, getResponse.Bytes);
    }

    [Fact]
    public async void SetBatchAsync_NullCheckStrings_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetBatchAsync(null!, new Dictionary<string, string>()));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetBatchAsync("cache", (Dictionary<string, string>)null!));

        var badDictionary = new Dictionary<string, string>() { { "asdf", null! } };
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetBatchAsync("cache", badDictionary));
    }

    [Fact]
    public async void SetBatchAsync_KeysAreString_HappyPath()
    {
        var key1 = Utils.GuidString();
        var key2 = Utils.GuidString();
        var value1 = Utils.GuidString();
        var value2 = Utils.GuidString();

        var dictionary = new Dictionary<string, string>() {
                { key1, value1 },
                { key2, value2 }
            };
        await client.SetBatchAsync(CacheName, dictionary);

        var getResponse = await client.GetAsync(CacheName, key1);
        Assert.Equal(value1, getResponse.String());

        getResponse = await client.GetAsync(CacheName, key2);
        Assert.Equal(value2, getResponse.String());
    }

    [Theory]
    [InlineData(null, new byte[] { 0x00 }, new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", new byte[] { 0x00 }, null)]
    public void Set_NullChecksBytesBytes_ThrowsException(string cacheName, byte[] key, byte[] value)
    {
        Assert.Throws<ArgumentNullException>(() => client.Set(cacheName, key, value));
        Assert.Throws<ArgumentNullException>(() => client.Set(cacheName, key, value, DefaultTtlSeconds));
    }

    // Tests Set(cacheName, byte[], byte[]) as well as Get(cacheName, byte[])
    [Fact]
    public void Set_KeyIsBytesValueIsBytes_HappyPath()
    {
        byte[] key = Utils.GuidBytes();
        byte[] value = Utils.GuidBytes();
        client.Set(CacheName, key, value);
        byte[]? setValue = client.Get(CacheName, key).Bytes;
        Assert.Equal(value, setValue);

        key = Utils.GuidBytes();
        value = Utils.GuidBytes();
        client.Set(CacheName, key, value, ttlSeconds: 15);
        setValue = client.Get(CacheName, key).Bytes;
        Assert.Equal(value, setValue);
    }

    [Fact]
    public void Get_NullChecksBytes_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => client.Get("cache", (byte[])null!));
        Assert.Throws<ArgumentNullException>(() => client.Get(null!, new byte[] { 0x00 }));
    }

    [Fact]
    public async void Get_ExpiredTtl_HappyPath()
    {
        string key = Utils.GuidString();
        string value = Utils.GuidString();
        client.Set(CacheName, key, value, 1);
        await Task.Delay(3000);
        CacheGetResponse result = client.Get(CacheName, key);
        Assert.Equal(CacheGetStatus.MISS, result.Status);
    }

    [Fact]
    public void Get_Miss_HappyPath()
    {
        CacheGetResponse result = client.Get(CacheName, Utils.GuidString());
        Assert.Equal(CacheGetStatus.MISS, result.Status);
        Assert.Null(result.String());
        Assert.Null(result.Bytes);
    }

    [Fact]
    public void Get_CacheDoesntExist_ThrowsException()
    {
        SimpleCacheClient simpleCacheClient = new SimpleCacheClient(authToken, DefaultTtlSeconds);
        Assert.Throws<NotFoundException>(() => simpleCacheClient.Get("non-existent-cache", Utils.GuidString()));
    }

    [Fact]
    public void Set_CacheDoesntExist_ThrowsException()
    {
        SimpleCacheClient simpleCacheClient = new SimpleCacheClient(authToken, DefaultTtlSeconds);
        Assert.Throws<NotFoundException>(() => simpleCacheClient.Set("non-existent-cache", Utils.GuidString(), Utils.GuidString()));
    }

    [Theory]
    [InlineData(null, "key", "value")]
    [InlineData("cache", null, "value")]
    [InlineData("cache", "key", null)]
    public void Set_NullChecksStringString_ThrowsException(string cacheName, string key, string value)
    {
        Assert.Throws<ArgumentNullException>(() => client.Set(cacheName, key, value));
        Assert.Throws<ArgumentNullException>(() => client.Set(cacheName, key, value, DefaultTtlSeconds));
    }

    // Tests Set(cacheName, string, string) as well as Get(cacheName, string)
    [Fact]
    public void Set_KeyIsStringValueIsString_HappyPath()
    {
        string key = Utils.GuidString();
        string value = Utils.GuidString();
        client.Set(CacheName, key, value);
        string? setValue = client.Get(CacheName, key).String();
        Assert.Equal(value, setValue);

        key = Utils.GuidString();
        value = Utils.GuidString();
        client.Set(CacheName, key, value, ttlSeconds: 15);
        setValue = client.Get(CacheName, key).String();
        Assert.Equal(value, setValue);
    }

    [Theory]
    [InlineData(null, "key")]
    [InlineData("cache", null)]
    public void Get_NullChecksString_ThrowsException(string cacheName, string key)
    {
        Assert.Throws<ArgumentNullException>(() => client.Get(cacheName, key));
    }

    [Theory]
    [InlineData(null, "key", new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", "key", null)]
    public void Set_NullChecksStringBytes_ThrowsException(string cacheName, string key, byte[] value)
    {
        Assert.Throws<ArgumentNullException>(() => client.Set(cacheName, key, value));
        Assert.Throws<ArgumentNullException>(() => client.Set(cacheName, key, value, DefaultTtlSeconds));
    }

    // Tests Set(cacheName, string, byte[]) as well as Get(cacheName, string)
    [Fact]
    public void Set_KeyIsStringValueIsBytes_HappyPath()
    {
        string key = Utils.GuidString();
        byte[] value = Utils.GuidBytes();
        client.Set(CacheName, key, value);
        byte[]? setValue = client.Get(CacheName, key).Bytes;
        Assert.Equal(value, setValue);

        key = Utils.GuidString();
        value = Utils.GuidBytes();
        client.Set(CacheName, key, value, ttlSeconds: 15);
        setValue = client.Get(CacheName, key).Bytes;
        Assert.Equal(value, setValue);
    }

    [Fact]
    public void GetBatch_NullCheckBytes_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => client.GetBatch(null!, new List<byte[]>()));
        Assert.Throws<ArgumentNullException>(() => client.GetBatch("cache", (List<byte[]>)null!));

        var badList = new List<byte[]>(new byte[][] { Utils.GuidBytes(), null! });
        Assert.Throws<ArgumentNullException>(() => client.GetBatch("cache", badList));

        Assert.Throws<ArgumentNullException>(() => client.GetBatch("cache", Utils.GuidBytes(), null!));
    }

    [Fact]
    public void GetBatch_KeysAreBytes_HappyPath()
    {
        string key1 = Utils.GuidString();
        string value1 = Utils.GuidString();
        string key2 = Utils.GuidString();
        string value2 = Utils.GuidString();
        client.Set(CacheName, key1, value1);
        client.Set(CacheName, key2, value2);

        List<byte[]> keys = new() { Utils.Utf8ToBytes(key1), Utils.Utf8ToBytes(key2) };

        CacheGetBatchResponse result = client.GetBatch(CacheName, keys);
        string? stringResult1 = result.Strings()[0];
        string? stringResult2 = result.Strings()[1];
        Assert.Equal(value1, stringResult1);
        Assert.Equal(value2, stringResult2);
    }

    [Fact]
    public void GetBatch_KeysAreBytes_HappyPath2()
    {
        string key1 = Utils.GuidString();
        string value1 = Utils.GuidString();
        string key2 = Utils.GuidString();
        string value2 = Utils.GuidString();
        client.Set(CacheName, key1, value1);
        client.Set(CacheName, key2, value2);

        CacheGetBatchResponse result = client.GetBatch(CacheName, key1, key2);
        string? stringResult1 = result.Strings()[0];
        string? stringResult2 = result.Strings()[1];
        Assert.Equal(value1, stringResult1);
        Assert.Equal(value2, stringResult2);
    }

    [Fact]
    public void GetBatch_NullCheckString_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => client.GetBatch(null!, new List<string>()));
        Assert.Throws<ArgumentNullException>(() => client.GetBatch("cache", (List<string>)null!));

        List<string> strings = new(new string[] { Utils.GuidString(), Utils.GuidString(), null! });
        Assert.Throws<ArgumentNullException>(() => client.GetBatch("cache", strings));

        Assert.Throws<ArgumentNullException>(() => client.GetBatch("cache", Utils.GuidString(), Utils.GuidString(), null!));
    }

    [Fact]
    public void GetBatch_KeysAreString_HappyPath()
    {
        string key1 = Utils.GuidString();
        string value1 = Utils.GuidString();
        string key2 = Utils.GuidString();
        string value2 = Utils.GuidString();
        client.Set(CacheName, key1, value1);
        client.Set(CacheName, key2, value2);

        List<string> keys = new() { key1, key2, "key123123" };
        CacheGetBatchResponse result = client.GetBatch(CacheName, keys);

        Assert.Equal(result.Strings(), new string[] { value1, value2, null! });
        Assert.Equal(result.Status, new CacheGetStatus[] { CacheGetStatus.HIT, CacheGetStatus.HIT, CacheGetStatus.MISS });
    }

    [Fact]
    public void GetBatch_Failure()
    {
        // Set very small timeout for dataClientOperationTimeoutMilliseconds
        SimpleCacheClient simpleCacheClient = new SimpleCacheClient(authToken, DefaultTtlSeconds, 1);
        List<string> keys = new() { Utils.GuidString(), Utils.GuidString(), Utils.GuidString(), Utils.GuidString() };
        Assert.Throws<MomentoSdk.Exceptions.TimeoutException>(() => simpleCacheClient.GetBatch(CacheName, keys));
    }

    [Fact]
    public void SetBatch_NullCheckBytes_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => client.SetBatch(null!, new Dictionary<byte[], byte[]>()));
        Assert.Throws<ArgumentNullException>(() => client.SetBatch("cache", (Dictionary<byte[], byte[]>)null!));

        var badDictionary = new Dictionary<byte[], byte[]>() { { Utils.GuidBytes(), null! } };
        Assert.Throws<ArgumentNullException>(() => client.SetBatch("cache", badDictionary));
    }

    [Fact]
    public void SetBatch_ItemsAreBytes_HappyPath()
    {
        var key1 = Utils.GuidBytes();
        var key2 = Utils.GuidBytes();
        var value1 = Utils.GuidBytes();
        var value2 = Utils.GuidBytes();

        var dictionary = new Dictionary<byte[], byte[]>() {
                    { key1, value1 },
                    { key2, value2 }
                };
        client.SetBatch(CacheName, dictionary);

        var getResponse = client.Get(CacheName, key1);
        Assert.Equal(value1, getResponse.Bytes);

        getResponse = client.Get(CacheName, key2);
        Assert.Equal(value2, getResponse.Bytes);
    }


    [Fact]
    public void SetBatch_NullCheckString_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => client.SetBatch(null!, new Dictionary<string, string>()));
        Assert.Throws<ArgumentNullException>(() => client.SetBatch("cache", (Dictionary<string, string>)null!));

        var badDictionary = new Dictionary<string, string>() { { "asdf", null! } };
        Assert.Throws<ArgumentNullException>(() => client.SetBatch("cache", badDictionary));
    }

    [Fact]
    public void SetBatch_KeysAreString_HappyPath()
    {
        var key1 = Utils.GuidString();
        var key2 = Utils.GuidString();
        var value1 = Utils.GuidString();
        var value2 = Utils.GuidString();

        var dictionary = new Dictionary<string, string>() {
                    { key1, value1 },
                    { key2, value2 }
                };
        client.SetBatch(CacheName, dictionary);

        var getResponse = client.Get(CacheName, key1);
        Assert.Equal(value1, getResponse.String());

        getResponse = client.Get(CacheName, key2);
        Assert.Equal(value2, getResponse.String());
    }

    [Theory]
    [InlineData(null, new byte[] { 0x00 })]
    [InlineData("cache", null)]
    public void Delete_NullChecksBytes_ThrowsException(string cacheName, byte[] key)
    {
        Assert.Throws<ArgumentNullException>(() => client.Delete(cacheName, key));
    }

    [Fact]
    public void Delete_KeyIsBytes_HappyPath()
    {
        // Set a key to then delete
        byte[] key = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        byte[] value = new byte[] { 0x05, 0x06, 0x07, 0x08 };
        client.Set(CacheName, key, value, ttlSeconds: 60);
        CacheGetResponse getResponse = client.Get(CacheName, key);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);

        // Delete
        client.Delete(CacheName, key);

        // Check deleted
        getResponse = client.Get(CacheName, key);
        Assert.Equal(CacheGetStatus.MISS, getResponse.Status);
    }

    [Theory]
    [InlineData(null, new byte[] { 0x00 })]
    [InlineData("cache", null)]
    public async void DeleteAsync_NullChecksByte_ThrowsException(string cacheName, byte[] key)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DeleteAsync(cacheName, key));
    }

    [Fact]
    public async void DeleteAsync_KeyIsBytes_HappyPath()
    {
        // Set a key to then delete
        byte[] key = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        byte[] value = new byte[] { 0x05, 0x06, 0x07, 0x08 };
        await client.SetAsync(CacheName, key, value, ttlSeconds: 60);
        CacheGetResponse getResponse = await client.GetAsync(CacheName, key);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);

        // Delete
        await client.DeleteAsync(CacheName, key);

        // Check deleted
        getResponse = await client.GetAsync(CacheName, key);
        Assert.Equal(CacheGetStatus.MISS, getResponse.Status);
    }

    [Theory]
    [InlineData(null, "key")]
    [InlineData("cache", null)]
    public void Delete_NullChecksString_ThrowsException(string cacheName, string key)
    {
        Assert.Throws<ArgumentNullException>(() => client.Delete(cacheName, key));
    }

    [Fact]
    public void Delete_KeyIsString_HappyPath()
    {
        // Set a key to then delete
        string key = Utils.GuidString();
        string value = Utils.GuidString();
        client.Set(CacheName, key, value, ttlSeconds: 60);
        CacheGetResponse getResponse = client.Get(CacheName, key);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);

        // Delete
        client.Delete(CacheName, key);

        // Check deleted
        getResponse = client.Get(CacheName, key);
        Assert.Equal(CacheGetStatus.MISS, getResponse.Status);
    }

    [Theory]
    [InlineData(null, "key")]
    [InlineData("cache", null)]
    public async void DeleteAsync_NullChecksString_ThrowsException(string cacheName, string key)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DeleteAsync(cacheName, key));
    }

    [Fact]
    public async void DeleteAsync_KeyIsString_HappyPath()
    {
        // Set a key to then delete
        string key = Utils.GuidString();
        string value = Utils.GuidString();
        await client.SetAsync(CacheName, key, value, ttlSeconds: 60);
        CacheGetResponse getResponse = await client.GetAsync(CacheName, key);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);

        // Delete
        await client.DeleteAsync(CacheName, key);

        // Check deleted
        getResponse = await client.GetAsync(CacheName, key);
        Assert.Equal(CacheGetStatus.MISS, getResponse.Status);
    }
}
