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
        byte[]? setValue = (await client.GetAsync(CacheName, key)).Bytes();
        Assert.Equal(value, setValue);

        key = Utils.GuidBytes();
        value = Utils.GuidBytes();
        await client.SetAsync(CacheName, key, value, ttlSeconds: 15);
        setValue = (await client.GetAsync(CacheName, key)).Bytes();
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
        byte[]? setValue = (await client.GetAsync(CacheName, key)).Bytes();
        Assert.Equal(value, setValue);

        key = Utils.GuidString();
        value = Utils.GuidBytes();
        await client.SetAsync(CacheName, key, value, ttlSeconds: 15);
        setValue = (await client.GetAsync(CacheName, key)).Bytes();
        Assert.Equal(value, setValue);
    }

    [Fact]
    public async void GetMultiAsync_NullCheckBytes_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetMultiAsync(null!, new List<byte[]>()));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetMultiAsync("cache", (List<byte[]>)null!));

        var badList = new List<byte[]>(new byte[][] { Utils.GuidBytes(), null! });
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetMultiAsync("cache", badList));

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetMultiAsync("cache", Utils.GuidBytes(), null!));
    }

    [Fact]
    public async void GetMultiAsync_KeysAreBytes_HappyPath()
    {
        string key1 = Utils.GuidString();
        string value1 = Utils.GuidString();
        string key2 = Utils.GuidString();
        string value2 = Utils.GuidString();
        client.Set(CacheName, key1, value1);
        client.Set(CacheName, key2, value2);

        List<byte[]> keys = new() { Utils.Utf8ToBytes(key1), Utils.Utf8ToBytes(key2) };

        CacheGetMultiResponse result = await client.GetMultiAsync(CacheName, keys);
        string? stringResult1 = result.Strings()[0];
        string? stringResult2 = result.Strings()[1];
        Assert.Equal(value1, stringResult1);
        Assert.Equal(value2, stringResult2);
    }

    [Fact]
    public async void GetMultiAsync_KeysAreBytes_HappyPath2()
    {
        string key1 = Utils.GuidString();
        string value1 = Utils.GuidString();
        string key2 = Utils.GuidString();
        string value2 = Utils.GuidString();
        client.Set(CacheName, key1, value1);
        client.Set(CacheName, key2, value2);

        CacheGetMultiResponse result = await client.GetMultiAsync(CacheName, key1, key2);
        string? stringResult1 = result.Strings()[0];
        string? stringResult2 = result.Strings()[1];
        Assert.Equal(value1, stringResult1);
        Assert.Equal(value2, stringResult2);
    }

    [Fact]
    public async void GetMultiAsync_NullCheckString_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetMultiAsync(null!, new List<string>()));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetMultiAsync("cache", (List<string>)null!));

        List<string> strings = new(new string[] { "key1", "key2", null! });
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetMultiAsync("cache", strings));

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.GetMultiAsync("cache", "key1", "key2", null!));
    }

    [Fact]
    public async void GetMultiAsync_KeysAreString_HappyPath()
    {
        string key1 = Utils.GuidString();
        string value1 = Utils.GuidString();
        string key2 = Utils.GuidString();
        string value2 = Utils.GuidString();
        client.Set(CacheName, key1, value1);
        client.Set(CacheName, key2, value2);

        List<string> keys = new() { key1, key2, "key123123" };
        CacheGetMultiResponse result = await client.GetMultiAsync(CacheName, keys);

        Assert.Equal(result.Strings(), new string[] { value1, value2, null! });
        Assert.Equal(result.Status(), new CacheGetStatus[] { CacheGetStatus.HIT, CacheGetStatus.HIT, CacheGetStatus.MISS });
    }

    [Fact]
    public void GetMultiAsync_Failure()
    {
        // Set very small timeout for dataClientOperationTimeoutMilliseconds
        SimpleCacheClient simpleCacheClient = new SimpleCacheClient(authToken, DefaultTtlSeconds, 1);
        List<string> keys = new() { Utils.GuidString(), Utils.GuidString(), Utils.GuidString(), Utils.GuidString() };
        Assert.ThrowsAsync<MomentoSdk.Exceptions.TimeoutException>(() => simpleCacheClient.GetMultiAsync(CacheName, keys));
    }

    [Fact]
    public async void SetMultiAsync_NullCheckBytes_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetMultiAsync(null!, new Dictionary<byte[], byte[]>()));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetMultiAsync("cache", (Dictionary<byte[], byte[]>)null!));

        var badDictionary = new Dictionary<byte[], byte[]>() { { Utils.Utf8ToBytes("asdf"), null! } };
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetMultiAsync("cache", badDictionary));
    }

    [Fact]
    public async void SetMultiAsync_ItemsAreBytes_HappyPath()
    {
        var key1 = Utils.GuidBytes();
        var key2 = Utils.GuidBytes();
        var value1 = Utils.GuidBytes();
        var value2 = Utils.GuidBytes();

        var dictionary = new Dictionary<byte[], byte[]>() {
                { key1, value1 },
                { key2, value2 }
            };
        CacheSetMultiResponse response = await client.SetMultiAsync(CacheName, dictionary);
        Assert.Equal(dictionary, response.Bytes());

        var getResponse = await client.GetAsync(CacheName, key1);
        Assert.Equal(value1, getResponse.Bytes());

        getResponse = await client.GetAsync(CacheName, key2);
        Assert.Equal(value2, getResponse.Bytes());
    }

    [Fact]
    public async void SetMultiAsync_NullCheckStrings_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetMultiAsync(null!, new Dictionary<string, string>()));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetMultiAsync("cache", (Dictionary<string, string>)null!));

        var badDictionary = new Dictionary<string, string>() { { "asdf", null! } };
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SetMultiAsync("cache", badDictionary));
    }

    [Fact]
    public async void SetMultiAsync_KeysAreString_HappyPath()
    {
        var key1 = Utils.GuidString();
        var key2 = Utils.GuidString();
        var value1 = Utils.GuidString();
        var value2 = Utils.GuidString();

        var dictionary = new Dictionary<string, string>() {
                { key1, value1 },
                { key2, value2 }
            };
        CacheSetMultiResponse response = await client.SetMultiAsync(CacheName, dictionary);
        Assert.Equal(dictionary, response.Strings());

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
        byte[]? setValue = client.Get(CacheName, key).Bytes();
        Assert.Equal(value, setValue);

        key = Utils.GuidBytes();
        value = Utils.GuidBytes();
        client.Set(CacheName, key, value, ttlSeconds: 15);
        setValue = client.Get(CacheName, key).Bytes();
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
        Assert.Null(result.Bytes());
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
        byte[]? setValue = client.Get(CacheName, key).Bytes();
        Assert.Equal(value, setValue);

        key = Utils.GuidString();
        value = Utils.GuidBytes();
        client.Set(CacheName, key, value, ttlSeconds: 15);
        setValue = client.Get(CacheName, key).Bytes();
        Assert.Equal(value, setValue);
    }

    [Fact]
    public void GetMulti_NullCheckBytes_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => client.GetMulti(null!, new List<byte[]>()));
        Assert.Throws<ArgumentNullException>(() => client.GetMulti("cache", (List<byte[]>)null!));

        var badList = new List<byte[]>(new byte[][] { Utils.GuidBytes(), null! });
        Assert.Throws<ArgumentNullException>(() => client.GetMulti("cache", badList));

        Assert.Throws<ArgumentNullException>(() => client.GetMulti("cache", Utils.GuidBytes(), null!));
    }

    [Fact]
    public void GetMulti_KeysAreBytes_HappyPath()
    {
        string key1 = Utils.GuidString();
        string value1 = Utils.GuidString();
        string key2 = Utils.GuidString();
        string value2 = Utils.GuidString();
        client.Set(CacheName, key1, value1);
        client.Set(CacheName, key2, value2);

        List<byte[]> keys = new() { Utils.Utf8ToBytes(key1), Utils.Utf8ToBytes(key2) };

        CacheGetMultiResponse result = client.GetMulti(CacheName, keys);
        string? stringResult1 = result.Strings()[0];
        string? stringResult2 = result.Strings()[1];
        Assert.Equal(value1, stringResult1);
        Assert.Equal(value2, stringResult2);
    }

    [Fact]
    public void GetMulti_KeysAreBytes_HappyPath2()
    {
        string key1 = Utils.GuidString();
        string value1 = Utils.GuidString();
        string key2 = Utils.GuidString();
        string value2 = Utils.GuidString();
        client.Set(CacheName, key1, value1);
        client.Set(CacheName, key2, value2);

        CacheGetMultiResponse result = client.GetMulti(CacheName, key1, key2);
        string? stringResult1 = result.Strings()[0];
        string? stringResult2 = result.Strings()[1];
        Assert.Equal(value1, stringResult1);
        Assert.Equal(value2, stringResult2);
    }

    [Fact]
    public void GetMulti_NullCheckString_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => client.GetMulti(null!, new List<string>()));
        Assert.Throws<ArgumentNullException>(() => client.GetMulti("cache", (List<string>)null!));

        List<string> strings = new(new string[] { Utils.GuidString(), Utils.GuidString(), null! });
        Assert.Throws<ArgumentNullException>(() => client.GetMulti("cache", strings));

        Assert.Throws<ArgumentNullException>(() => client.GetMulti("cache", Utils.GuidString(), Utils.GuidString(), null!));
    }

    [Fact]
    public void GetMulti_KeysAreString_HappyPath()
    {
        string key1 = Utils.GuidString();
        string value1 = Utils.GuidString();
        string key2 = Utils.GuidString();
        string value2 = Utils.GuidString();
        client.Set(CacheName, key1, value1);
        client.Set(CacheName, key2, value2);

        List<string> keys = new() { key1, key2, "key123123" };
        CacheGetMultiResponse result = client.GetMulti(CacheName, keys);

        Assert.Equal(result.Strings(), new string[] { value1, value2, null! });
        Assert.Equal(result.Status(), new CacheGetStatus[] { CacheGetStatus.HIT, CacheGetStatus.HIT, CacheGetStatus.MISS });
    }

    [Fact]
    public void GetMulti_Failure()
    {
        // Set very small timeout for dataClientOperationTimeoutMilliseconds
        SimpleCacheClient simpleCacheClient = new SimpleCacheClient(authToken, DefaultTtlSeconds, 1);
        List<string> keys = new() { Utils.GuidString(), Utils.GuidString(), Utils.GuidString(), Utils.GuidString() };
        Assert.Throws<MomentoSdk.Exceptions.TimeoutException>(() => simpleCacheClient.GetMulti(CacheName, keys));
    }

    [Fact]
    public void SetMulti_NullCheckBytes_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => client.SetMulti(null!, new Dictionary<byte[], byte[]>()));
        Assert.Throws<ArgumentNullException>(() => client.SetMulti("cache", (Dictionary<byte[], byte[]>)null!));

        var badDictionary = new Dictionary<byte[], byte[]>() { { Utils.GuidBytes(), null! } };
        Assert.Throws<ArgumentNullException>(() => client.SetMulti("cache", badDictionary));
    }

    [Fact]
    public void SetMulti_ItemsAreBytes_HappyPath()
    {
        var key1 = Utils.GuidBytes();
        var key2 = Utils.GuidBytes();
        var value1 = Utils.GuidBytes();
        var value2 = Utils.GuidBytes();

        var dictionary = new Dictionary<byte[], byte[]>() {
                    { key1, value1 },
                    { key2, value2 }
                };
        CacheSetMultiResponse response = client.SetMulti(CacheName, dictionary);
        Assert.Equal(dictionary, response.Bytes());

        var getResponse = client.Get(CacheName, key1);
        Assert.Equal(value1, getResponse.Bytes());

        getResponse = client.Get(CacheName, key2);
        Assert.Equal(value2, getResponse.Bytes());
    }


    [Fact]
    public void SetMulti_NullCheckString_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => client.SetMulti(null!, new Dictionary<string, string>()));
        Assert.Throws<ArgumentNullException>(() => client.SetMulti("cache", (Dictionary<string, string>)null!));

        var badDictionary = new Dictionary<string, string>() { { "asdf", null! } };
        Assert.Throws<ArgumentNullException>(() => client.SetMulti("cache", badDictionary));
    }

    [Fact]
    public void SetMulti_KeysAreString_HappyPath()
    {
        var key1 = Utils.GuidString();
        var key2 = Utils.GuidString();
        var value1 = Utils.GuidString();
        var value2 = Utils.GuidString();

        var dictionary = new Dictionary<string, string>() {
                    { key1, value1 },
                    { key2, value2 }
                };
        CacheSetMultiResponse response = client.SetMulti(CacheName, dictionary);
        Assert.Equal(dictionary, response.Strings());

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
