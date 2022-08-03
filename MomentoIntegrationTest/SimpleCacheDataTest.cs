using System.Linq;
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
        await client.SetAsync(CacheName, key, value);
        byte[]? setValue = (await client.GetAsync(CacheName, key)).Bytes;
        Assert.Equal(value, setValue);

        key = Utils.NewGuidByteArray();
        value = Utils.NewGuidByteArray();
        await client.SetAsync(CacheName, key, value, ttlSeconds: 15);
        setValue = (await client.GetAsync(CacheName, key)).Bytes;
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
        await client.SetAsync(CacheName, key, value);
        string? setValue = (await client.GetAsync(CacheName, key)).String();
        Assert.Equal(value, setValue);

        key = Utils.NewGuidString();
        value = Utils.NewGuidString();
        await client.SetAsync(CacheName, key, value, ttlSeconds: 15);
        setValue = (await client.GetAsync(CacheName, key)).String();
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
        await client.SetAsync(CacheName, key, value);
        byte[]? setValue = (await client.GetAsync(CacheName, key)).Bytes;
        Assert.Equal(value, setValue);

        key = Utils.NewGuidString();
        value = Utils.NewGuidByteArray();
        await client.SetAsync(CacheName, key, value, ttlSeconds: 15);
        setValue = (await client.GetAsync(CacheName, key)).Bytes;
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
        client.Set(CacheName, key1, value1);
        client.Set(CacheName, key2, value2);

        List<byte[]> keys = new() { Utils.Utf8ToByteArray(key1), Utils.Utf8ToByteArray(key2) };

        CacheGetBatchResponse result = await client.GetBatchAsync(CacheName, keys);
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
        client.Set(CacheName, key1, value1);
        client.Set(CacheName, key2, value2);

        CacheGetBatchResponse result = await client.GetBatchAsync(CacheName, key1, key2);
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
        List<string> keys = new() { Utils.NewGuidString(), Utils.NewGuidString(), Utils.NewGuidString(), Utils.NewGuidString() };
        Assert.ThrowsAsync<MomentoSdk.Exceptions.TimeoutException>(() => simpleCacheClient.GetBatchAsync(CacheName, keys));
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
        await client.SetBatchAsync(CacheName, dictionary);

        var getResponse = await client.GetAsync(CacheName, key1);
        Assert.Equal(value1, getResponse.Bytes);

        getResponse = await client.GetAsync(CacheName, key2);
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
    public void Set_NullChecksByteArrayByteArray_ThrowsException(string cacheName, byte[] key, byte[] value)
    {
        Assert.Throws<ArgumentNullException>(() => client.Set(cacheName, key, value));
        Assert.Throws<ArgumentNullException>(() => client.Set(cacheName, key, value, DefaultTtlSeconds));
    }

    // Tests Set(cacheName, byte[], byte[]) as well as Get(cacheName, byte[])
    [Fact]
    public void Set_KeyIsByteArrayValueIsByteArray_HappyPath()
    {
        byte[] key = Utils.NewGuidByteArray();
        byte[] value = Utils.NewGuidByteArray();
        client.Set(CacheName, key, value);
        byte[]? setValue = client.Get(CacheName, key).Bytes;
        Assert.Equal(value, setValue);

        key = Utils.NewGuidByteArray();
        value = Utils.NewGuidByteArray();
        client.Set(CacheName, key, value, ttlSeconds: 15);
        setValue = client.Get(CacheName, key).Bytes;
        Assert.Equal(value, setValue);
    }

    [Fact]
    public void Get_NullChecksByteArray_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => client.Get("cache", (byte[])null!));
        Assert.Throws<ArgumentNullException>(() => client.Get(null!, new byte[] { 0x00 }));
    }

    [Fact]
    public async Task Get_ExpiredTtl_HappyPath()
    {
        string key = Utils.NewGuidString();
        string value = Utils.NewGuidString();
        client.Set(CacheName, key, value, 1);
        await Task.Delay(3000);
        CacheGetResponse result = client.Get(CacheName, key);
        Assert.Equal(CacheGetStatus.MISS, result.Status);
    }

    [Fact]
    public void Get_Miss_HappyPath()
    {
        CacheGetResponse result = client.Get(CacheName, Utils.NewGuidString());
        Assert.Equal(CacheGetStatus.MISS, result.Status);
        Assert.Null(result.String());
        Assert.Null(result.Bytes);
    }

    [Fact]
    public void Get_CacheDoesntExist_ThrowsException()
    {
        SimpleCacheClient simpleCacheClient = new SimpleCacheClient(authToken, DefaultTtlSeconds);
        Assert.Throws<NotFoundException>(() => simpleCacheClient.Get("non-existent-cache", Utils.NewGuidString()));
    }

    [Fact]
    public void Set_CacheDoesntExist_ThrowsException()
    {
        SimpleCacheClient simpleCacheClient = new SimpleCacheClient(authToken, DefaultTtlSeconds);
        Assert.Throws<NotFoundException>(() => simpleCacheClient.Set("non-existent-cache", Utils.NewGuidString(), Utils.NewGuidString()));
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
        string key = Utils.NewGuidString();
        string value = Utils.NewGuidString();
        client.Set(CacheName, key, value);
        string? setValue = client.Get(CacheName, key).String();
        Assert.Equal(value, setValue);

        key = Utils.NewGuidString();
        value = Utils.NewGuidString();
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
    public void Set_NullChecksStringByteArray_ThrowsException(string cacheName, string key, byte[] value)
    {
        Assert.Throws<ArgumentNullException>(() => client.Set(cacheName, key, value));
        Assert.Throws<ArgumentNullException>(() => client.Set(cacheName, key, value, DefaultTtlSeconds));
    }

    // Tests Set(cacheName, string, byte[]) as well as Get(cacheName, string)
    [Fact]
    public void Set_KeyIsStringValueIsByteArray_HappyPath()
    {
        string key = Utils.NewGuidString();
        byte[] value = Utils.NewGuidByteArray();
        client.Set(CacheName, key, value);
        byte[]? setValue = client.Get(CacheName, key).Bytes;
        Assert.Equal(value, setValue);

        key = Utils.NewGuidString();
        value = Utils.NewGuidByteArray();
        client.Set(CacheName, key, value, ttlSeconds: 15);
        setValue = client.Get(CacheName, key).Bytes;
        Assert.Equal(value, setValue);
    }

    [Fact]
    public void GetBatch_NullCheckByteArray_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => client.GetBatch(null!, new List<byte[]>()));
        Assert.Throws<ArgumentNullException>(() => client.GetBatch("cache", (List<byte[]>)null!));

        var badList = new List<byte[]>(new byte[][] { Utils.NewGuidByteArray(), null! });
        Assert.Throws<ArgumentNullException>(() => client.GetBatch("cache", badList));

        Assert.Throws<ArgumentNullException>(() => client.GetBatch("cache", Utils.NewGuidByteArray(), null!));
    }

    [Fact]
    public void GetBatch_KeysAreByteArray_HappyPath()
    {
        string key1 = Utils.NewGuidString();
        string value1 = Utils.NewGuidString();
        string key2 = Utils.NewGuidString();
        string value2 = Utils.NewGuidString();
        client.Set(CacheName, key1, value1);
        client.Set(CacheName, key2, value2);

        List<byte[]> keys = new() { Utils.Utf8ToByteArray(key1), Utils.Utf8ToByteArray(key2) };

        CacheGetBatchResponse result = client.GetBatch(CacheName, keys);
        string? stringResult1 = result.Strings().ToList()[0];
        string? stringResult2 = result.Strings().ToList()[1];
        Assert.Equal(value1, stringResult1);
        Assert.Equal(value2, stringResult2);
    }

    [Fact]
    public void GetBatch_KeysAreByteArray_HappyPath2()
    {
        string key1 = Utils.NewGuidString();
        string value1 = Utils.NewGuidString();
        string key2 = Utils.NewGuidString();
        string value2 = Utils.NewGuidString();
        client.Set(CacheName, key1, value1);
        client.Set(CacheName, key2, value2);

        CacheGetBatchResponse result = client.GetBatch(CacheName, key1, key2);
        string? stringResult1 = result.Strings().ToList()[0];
        string? stringResult2 = result.Strings().ToList()[1];
        Assert.Equal(value1, stringResult1);
        Assert.Equal(value2, stringResult2);
    }

    [Fact]
    public void GetBatch_NullCheckString_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => client.GetBatch(null!, new List<string>()));
        Assert.Throws<ArgumentNullException>(() => client.GetBatch("cache", (List<string>)null!));

        List<string> strings = new(new string[] { Utils.NewGuidString(), Utils.NewGuidString(), null! });
        Assert.Throws<ArgumentNullException>(() => client.GetBatch("cache", strings));

        Assert.Throws<ArgumentNullException>(() => client.GetBatch("cache", Utils.NewGuidString(), Utils.NewGuidString(), null!));
    }

    [Fact]
    public void GetBatch_KeysAreString_HappyPath()
    {
        string key1 = Utils.NewGuidString();
        string value1 = Utils.NewGuidString();
        string key2 = Utils.NewGuidString();
        string value2 = Utils.NewGuidString();
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
        List<string> keys = new() { Utils.NewGuidString(), Utils.NewGuidString(), Utils.NewGuidString(), Utils.NewGuidString() };
        Assert.Throws<MomentoSdk.Exceptions.TimeoutException>(() => simpleCacheClient.GetBatch(CacheName, keys));
    }

    [Fact]
    public void SetBatch_NullCheckByteArray_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => client.SetBatch(null!, new Dictionary<byte[], byte[]>()));
        Assert.Throws<ArgumentNullException>(() => client.SetBatch("cache", (Dictionary<byte[], byte[]>)null!));

        var badDictionary = new Dictionary<byte[], byte[]>() { { Utils.NewGuidByteArray(), null! } };
        Assert.Throws<ArgumentNullException>(() => client.SetBatch("cache", badDictionary));
    }

    [Fact]
    public void SetBatch_ItemsAreByteArray_HappyPath()
    {
        var key1 = Utils.NewGuidByteArray();
        var key2 = Utils.NewGuidByteArray();
        var value1 = Utils.NewGuidByteArray();
        var value2 = Utils.NewGuidByteArray();

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
        var key1 = Utils.NewGuidString();
        var key2 = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var value2 = Utils.NewGuidString();

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
    public void Delete_NullChecksByteArray_ThrowsException(string cacheName, byte[] key)
    {
        Assert.Throws<ArgumentNullException>(() => client.Delete(cacheName, key));
    }

    [Fact]
    public void Delete_KeyIsByteArray_HappyPath()
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
        string key = Utils.NewGuidString();
        string value = Utils.NewGuidString();
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
