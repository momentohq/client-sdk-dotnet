using MomentoSdk.Internal.ExtensionMethods;
using MomentoSdk.Responses;

namespace IncubatingIntegrationTest;

[Collection("SimpleCacheClient")]
public class DictionaryTest : TestBase
{
    public DictionaryTest(SimpleCacheClientFixture fixture) : base(fixture)
    {
    }

    [Theory]
    [InlineData(null, "my-dictionary", new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-dictionary", null)]
    public void DictionaryGet_NullChecksBytes_ThrowsException(string cacheName, string dictionaryName, byte[] field)
    {
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGet(cacheName, dictionaryName, field));
    }

    [Theory]
    [InlineData(null, "my-dictionary", new byte[] { 0x00 }, new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 }, new byte[] { 0x00 })]
    [InlineData("cache", "my-dictionary", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-dictionary", new byte[] { 0x00 }, null)]
    public void DictionarySet_NullChecksBytes_ThrowsException(string cacheName, string dictionaryName, byte[] field, byte[] value)
    {
        Assert.Throws<ArgumentNullException>(() => client.DictionarySet(cacheName, dictionaryName, field, value, false));
    }

    [Fact]
    public void DictionaryGet_FieldIsBytes_DictionaryIsMissing()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var response = client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsBytesValueIsBytes_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();

        client.DictionarySet(cacheName, dictionaryName, field, value, false);

        var getResponse = client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsBytesDictionaryIsPresent_FieldIsMissing()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();

        client.DictionarySet(cacheName, dictionaryName, field, value, false);

        var otherField = Utils.GuidBytes();
        var response = client.DictionaryGet(cacheName, dictionaryName, otherField);

        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsBytes_NoRefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();

        client.DictionarySet(cacheName, dictionaryName, field, value, false, ttlSeconds: 5);
        Thread.Sleep(100);

        client.DictionarySet(cacheName, dictionaryName, field, value, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsBytes_RefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();

        client.DictionarySet(cacheName, dictionaryName, field, value, false, ttlSeconds: 1);
        Thread.Sleep(100);

        client.DictionarySet(cacheName, dictionaryName, field, value, true, ttlSeconds: 10);
        Thread.Sleep(1000);

        var response = client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.Bytes);
    }

    [Theory]
    [InlineData(null, "my-dictionary", "my-field")]
    [InlineData("cache", null, "my-field")]
    [InlineData("cache", "my-dictionary", null)]
    public void DictionaryGet_NullChecksString_ThrowsException(string cacheName, string dictionaryName, string field)
    {
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGet(cacheName, dictionaryName, field));
    }

    [Theory]
    [InlineData(null, "my-dictionary", "my-field", "my-value")]
    [InlineData("cache", null, "my-field", "my-value")]
    [InlineData("cache", "my-dictionary", null, "my-value")]
    [InlineData("cache", "my-dictionary", "my-field", null)]
    public void DictionarySet_NullChecksString_ThrowsException(string cacheName, string dictionaryName, string field, string value)
    {
        Assert.Throws<ArgumentNullException>(() => client.DictionarySet(cacheName, dictionaryName, field, value, false));
    }

    [Fact]
    public void DictionaryGet_FieldIsString_DictionaryIsMissing()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var response = client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsStringValueIsString_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();

        client.DictionarySet(cacheName, dictionaryName, field, value, false);

        var getResponse = client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
    }

    [Fact]
    public void DictionarySetGet_DictionaryIsPresent_FieldIsMissing()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();

        client.DictionarySet(cacheName, dictionaryName, field, value, false);

        var otherField = Utils.GuidString();
        var response = client.DictionaryGet(cacheName, dictionaryName, otherField);

        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsString_NoRefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();

        client.DictionarySet(cacheName, dictionaryName, field, value, false, ttlSeconds: 5);
        Thread.Sleep(100);

        client.DictionarySet(cacheName, dictionaryName, field, value, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsString_RefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();

        client.DictionarySet(cacheName, dictionaryName, field, value, false, ttlSeconds: 1);
        Thread.Sleep(100);

        client.DictionarySet(cacheName, dictionaryName, field, value, true, ttlSeconds: 10);
        Thread.Sleep(1000);

        var response = client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.String());
    }

    [Theory]
    [InlineData(null, "my-dictionary", new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-dictionary", null)]
    public async void DictionaryGetAsync_NullChecksBytes_ThrowsException(string cacheName, string dictionaryName, byte[] field)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetAsync(cacheName, dictionaryName, field));
    }

    [Theory]
    [InlineData(null, "my-dictionary", new byte[] { 0x00 }, new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 }, new byte[] { 0x00 })]
    [InlineData("cache", "my-dictionary", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-dictionary", new byte[] { 0x00 }, null)]
    public async void DictionarySetAsync_NullChecksBytes_ThrowsException(string cacheName, string dictionaryName, byte[] field, byte[] value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false));
    }

    [Fact]
    public async void DictionaryGetAsync_FieldIsBytes_DictionaryIsMissing()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async void DictionarySetGetAsync_FieldIsBytesValueIsBytes_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false);

        var getResponse = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
    }

    [Fact]
    public async void DictionarySetGetAsync_FieldIsBytesDictionaryIsPresent_FieldIsMissing()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false);

        var otherField = Utils.GuidBytes();
        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, otherField);

        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async void DictionarySetGetAsync_FieldIsBytes_NoRefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 5);
        Thread.Sleep(100);

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async void DictionarySetGetAsync_FieldIsBytes_RefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 1);
        Thread.Sleep(100);

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, true, ttlSeconds: 10);
        Thread.Sleep(1000);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.Bytes);
    }

    [Theory]
    [InlineData(null, "my-dictionary", "my-field")]
    [InlineData("cache", null, "my-field")]
    [InlineData("cache", "my-dictionary", null)]
    public async void DictionaryGetAsync_NullChecksString_ThrowsException(string cacheName, string dictionaryName, string field)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetAsync(cacheName, dictionaryName, field));
    }

    [Theory]
    [InlineData(null, "my-dictionary", "my-field", "my-value")]
    [InlineData("cache", null, "my-field", "my-value")]
    [InlineData("cache", "my-dictionary", null, "my-value")]
    [InlineData("cache", "my-dictionary", "my-field", null)]
    public async void DictionarySetAsync_NullChecksString_ThrowsException(string cacheName, string dictionaryName, string field, string value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false));
    }

    [Fact]
    public async void DictionaryGetAsync_FieldIsString_DictionaryIsMissing()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async void DictionarySetGetAsync_FieldIsStringValueIsString_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false);

        var getResponse = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
    }

    [Fact]
    public async void DictionarySetGetAsync_DictionaryIsPresent_FieldIsMissing()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false);

        var otherField = Utils.GuidString();
        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, otherField);

        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async void DictionarySetGetAsync_FieldIsString_NoRefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 5);
        Thread.Sleep(100);

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async void DictionarySetGetAsync_FieldIsString_RefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 1);
        Thread.Sleep(100);

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, true, ttlSeconds: 10);
        Thread.Sleep(1000);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.String());
    }

    [Fact]
    public void DictionarySetBatch_NullChecksBytes_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var dictionary = new Dictionary<byte[], byte[]>();
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetBatch(null!, dictionaryName, dictionary, false));
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetBatch(cacheName, null!, dictionary, false));
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetBatch(cacheName, dictionaryName, (IEnumerable<KeyValuePair<byte[], byte[]>>)null!, false));

        dictionary[Utils.GuidBytes()] = null!;
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetBatch(cacheName, dictionaryName, dictionary, false));
    }

    [Fact]
    public void DictionarySetBatch_FieldsAreBytes_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidBytes();
        var value1 = Utils.GuidBytes();
        var field2 = Utils.GuidBytes();
        var value2 = Utils.GuidBytes();

        var items = new Dictionary<byte[], byte[]>() { { field1, value1 }, { field2, value2 } };

        client.DictionarySetBatch(cacheName, dictionaryName, items, false, 10);

        var response = client.DictionaryGet(cacheName, dictionaryName, field1);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value1, response.Bytes);

        response = client.DictionaryGet(cacheName, dictionaryName, field2);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value2, response.Bytes);
    }

    [Fact]
    public void DictionarySetBatch_FieldsAreBytes_NoRefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();
        var content = new Dictionary<byte[], byte[]>() { { field, value } };

        client.DictionarySetBatch(cacheName, dictionaryName, content, false, ttlSeconds: 5);
        Thread.Sleep(100);

        client.DictionarySetBatch(cacheName, dictionaryName, content, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetBatch_FieldsAreBytes_RefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();
        var content = new Dictionary<byte[], byte[]>() { { field, value } };

        client.DictionarySetBatch(cacheName, dictionaryName, content, false, ttlSeconds: 1);
        Thread.Sleep(100);

        client.DictionarySetBatch(cacheName, dictionaryName, content, true, ttlSeconds: 10);
        Thread.Sleep(1000);

        var response = client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.Bytes);
    }

    [Fact]
    public void DictionarySetBatch_NullChecksStrings_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var dictionary = new Dictionary<string, string>();
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetBatch(null!, dictionaryName, dictionary, false));
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetBatch(cacheName, null!, dictionary, false));
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetBatch(cacheName, dictionaryName, (IEnumerable<KeyValuePair<string, string>>)null!, false));

        dictionary[Utils.GuidString()] = null!;
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetBatch(cacheName, dictionaryName, dictionary, false));
    }

    [Fact]
    public void DictionarySetBatch_FieldsAreStrings_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidString();
        var value1 = Utils.GuidString();
        var field2 = Utils.GuidString();
        var value2 = Utils.GuidString();

        var items = new Dictionary<string, string>() { { field1, value1 }, { field2, value2 } };

        client.DictionarySetBatch(cacheName, dictionaryName, items, false, 10);

        var getResponse = client.DictionaryGet(cacheName, dictionaryName, field1);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
        Assert.Equal(value1, getResponse.String());

        getResponse = client.DictionaryGet(cacheName, dictionaryName, field2);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
        Assert.Equal(value2, getResponse.String());
    }

    [Fact]
    public void DictionarySetBatch_FieldsAreStrings_NoRefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();
        var content = new Dictionary<string, string>() { { field, value } };

        client.DictionarySetBatch(cacheName, dictionaryName, content, false, ttlSeconds: 5);
        Thread.Sleep(100);

        client.DictionarySetBatch(cacheName, dictionaryName, content, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetBatch_FieldsAreStrings_RefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();
        var content = new Dictionary<string, string>() { { field, value } };

        client.DictionarySetBatch(cacheName, dictionaryName, content, false, ttlSeconds: 1);
        Thread.Sleep(100);

        client.DictionarySetBatch(cacheName, dictionaryName, content, true, ttlSeconds: 10);
        Thread.Sleep(1000);

        var response = client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.String());
    }

    [Fact]
    public async void DictionarySetBatchAsync_NullChecksBytes_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var dictionary = new Dictionary<byte[], byte[]>();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(null!, dictionaryName, dictionary, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(cacheName, null!, dictionary, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(cacheName, dictionaryName, (IEnumerable<KeyValuePair<byte[], byte[]>>)null!, false));

        dictionary[Utils.GuidBytes()] = null!;
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(cacheName, dictionaryName, dictionary, false));
    }

    [Fact]
    public async void DictionarySetBatchAsync_FieldsAreBytes_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidBytes();
        var value1 = Utils.GuidBytes();
        var field2 = Utils.GuidBytes();
        var value2 = Utils.GuidBytes();

        var items = new Dictionary<byte[], byte[]>() { { field1, value1 }, { field2, value2 } };

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, items, false, 10);

        var getResponse = await client.DictionaryGetAsync(cacheName, dictionaryName, field1);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
        Assert.Equal(value1, getResponse.Bytes);

        getResponse = await client.DictionaryGetAsync(cacheName, dictionaryName, field2);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
        Assert.Equal(value2, getResponse.Bytes);
    }

    [Fact]
    public async void DictionarySetBatchAsync_FieldsAreBytes_NoRefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();
        var content = new Dictionary<byte[], byte[]>() { { field, value } };

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, false, ttlSeconds: 5);
        Thread.Sleep(100);

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async void DictionarySetBatchAsync_FieldsAreBytes_RefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();
        var content = new Dictionary<byte[], byte[]>() { { field, value } };

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, false, ttlSeconds: 1);
        Thread.Sleep(100);

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, true, ttlSeconds: 10);
        Thread.Sleep(1000);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.Bytes);
    }

    [Fact]
    public async void DictionarySetBatchAsync_NullChecksStrings_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var dictionary = new Dictionary<string, string>();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(null!, dictionaryName, dictionary, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(cacheName, null!, dictionary, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(cacheName, dictionaryName, (IEnumerable<KeyValuePair<string, string>>)null!, false));

        dictionary[Utils.GuidString()] = null!;
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(cacheName, dictionaryName, dictionary, false));
    }

    [Fact]
    public async void DictionarySetBatchAsync_FieldsAreStrings_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidString();
        var value1 = Utils.GuidString();
        var field2 = Utils.GuidString();
        var value2 = Utils.GuidString();

        var items = new Dictionary<string, string>() { { field1, value1 }, { field2, value2 } };

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, items, false, 10);

        var getResponse = await client.DictionaryGetAsync(cacheName, dictionaryName, field1);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
        Assert.Equal(value1, getResponse.String());

        getResponse = await client.DictionaryGetAsync(cacheName, dictionaryName, field2);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
        Assert.Equal(value2, getResponse.String());
    }

    [Fact]
    public async void DictionarySetBatchAsync_FieldsAreStrings_NoRefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();
        var content = new Dictionary<string, string>() { { field, value } };

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, false, ttlSeconds: 5);
        Thread.Sleep(100);

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async void DictionarySetBatchAsync_FieldsAreStrings_RefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();
        var content = new Dictionary<string, string>() { { field, value } };

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, false, ttlSeconds: 1);
        Thread.Sleep(100);

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, true, ttlSeconds: 10);
        Thread.Sleep(1000);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.String());
    }

    [Fact]
    public void DictionaryGetBatch_NullChecksByteArrayParams_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var testData = new byte[][][] { new byte[][] { Utils.GuidBytes(), Utils.GuidBytes() }, new byte[][] { Utils.GuidBytes(), null! } };

        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetBatch(null!, dictionaryName, testData[0]));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetBatch(cacheName, null!, testData[0]));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetBatch(cacheName, dictionaryName, (byte[][])null!));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetBatch(cacheName, dictionaryName, testData[1]));

        var fieldsList = new List<byte[]>(testData[0]);
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetBatch(null!, dictionaryName, fieldsList));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetBatch(cacheName, null!, fieldsList));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetBatch(cacheName, dictionaryName, (List<byte[]>)null!));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetBatch(cacheName, dictionaryName, new List<byte[]>(testData[1])));
    }

    [Fact]
    public void DictionaryGetBatch_FieldsAreBytes_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidBytes();
        var value1 = Utils.GuidBytes();
        var field2 = Utils.GuidBytes();
        var value2 = Utils.GuidBytes();
        var field3 = Utils.GuidBytes();

        client.DictionarySet(cacheName, dictionaryName, field1, value1, false, 10);
        client.DictionarySet(cacheName, dictionaryName, field2, value2, false, 10);

        var response1 = client.DictionaryGetBatch(cacheName, dictionaryName, field1, field2, field3);
        var response2 = client.DictionaryGetBatch(cacheName, dictionaryName, new List<byte[]>() { field1, field2, field3 });

        var status = new CacheGetStatus[] { CacheGetStatus.HIT, CacheGetStatus.HIT, CacheGetStatus.MISS };
        var values = new byte[]?[] { value1, value2, null };
        Assert.Equal(status, response1.Status);
        Assert.Equal(status, response2.Status);
        Assert.Equal(values, response1.Bytes);
        Assert.Equal(values, response2.Bytes);
    }

    [Fact]
    public void DictionaryGetBatch_NullChecksStringParams_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var testData = new string[][] { new string[] { Utils.GuidString(), Utils.GuidString() }, new string[] { Utils.GuidString(), null! } };
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetBatch(null!, dictionaryName, testData[0]));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetBatch(cacheName, null!, testData[0]));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetBatch(cacheName, dictionaryName, (string[])null!));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetBatch(cacheName, dictionaryName, testData[1]));

        var fieldsList = new List<string>(testData[0]);
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetBatch(null!, dictionaryName, fieldsList));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetBatch(cacheName, null!, fieldsList));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetBatch(cacheName, dictionaryName, (List<string>)null!));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetBatch(cacheName, dictionaryName, new List<string>(testData[1])));
    }

    [Fact]
    public void DictionaryGetBatch_FieldsAreStrings_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidString();
        var value1 = Utils.GuidString();
        var field2 = Utils.GuidString();
        var value2 = Utils.GuidString();
        var field3 = Utils.GuidString();

        client.DictionarySet(cacheName, dictionaryName, field1, value1, false, 10);
        client.DictionarySet(cacheName, dictionaryName, field2, value2, false, 10);

        var response1 = client.DictionaryGetBatch(cacheName, dictionaryName, field1, field2, field3);
        var response2 = client.DictionaryGetBatch(cacheName, dictionaryName, new string[] { field1, field2, field3 });

        var status = new CacheGetStatus[] { CacheGetStatus.HIT, CacheGetStatus.HIT, CacheGetStatus.MISS };
        var values = new string?[] { value1, value2, null };
        Assert.Equal(status, response1.Status);
        Assert.Equal(status, response2.Status);
        Assert.Equal(values, response1.Strings());
        Assert.Equal(values, response2.Strings());
    }

    [Fact]
    public async void DictionaryGetBatchAsync_NullChecksByteArrayParams_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var testData = new byte[][][] { new byte[][] { Utils.GuidBytes(), Utils.GuidBytes() }, new byte[][] { Utils.GuidBytes(), null! } };

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetBatchAsync(null!, dictionaryName, testData[0]));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetBatchAsync(cacheName, null!, testData[0]));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetBatchAsync(cacheName, dictionaryName, (byte[][])null!));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetBatchAsync(cacheName, dictionaryName, testData[1]));

        var fieldsList = new List<byte[]>(testData[0]);
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetBatchAsync(null!, dictionaryName, fieldsList));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetBatchAsync(cacheName, null!, fieldsList));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetBatchAsync(cacheName, dictionaryName, (List<byte[]>)null!));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetBatchAsync(cacheName, dictionaryName, new List<byte[]>(testData[1])));
    }

    [Fact]
    public async void DictionaryGetBatchAsync_FieldsAreBytes_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidBytes();
        var value1 = Utils.GuidBytes();
        var field2 = Utils.GuidBytes();
        var value2 = Utils.GuidBytes();
        var field3 = Utils.GuidBytes();

        await client.DictionarySetAsync(cacheName, dictionaryName, field1, value1, false, 10);
        await client.DictionarySetAsync(cacheName, dictionaryName, field2, value2, false, 10);

        var response1 = await client.DictionaryGetBatchAsync(cacheName, dictionaryName, field1, field2, field3);
        var response2 = await client.DictionaryGetBatchAsync(cacheName, dictionaryName, new byte[][] { field1, field2, field3 });

        var status = new CacheGetStatus[] { CacheGetStatus.HIT, CacheGetStatus.HIT, CacheGetStatus.MISS };
        var values = new byte[]?[] { value1, value2, null };
        Assert.Equal(status, response1.Status);
        Assert.Equal(status, response2.Status);
        Assert.Equal(values, response1.Bytes);
        Assert.Equal(values, response2.Bytes);
    }

    [Fact]
    public async void DictionaryGetBatchAsync_NullChecksStringParams_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var testData = new string[][] { new string[] { Utils.GuidString(), Utils.GuidString() }, new string[] { Utils.GuidString(), null! } };
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetBatchAsync(null!, dictionaryName, testData[0]));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetBatchAsync(cacheName, null!, testData[0]));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetBatchAsync(cacheName, dictionaryName, (string[])null!));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetBatchAsync(cacheName, dictionaryName, testData[1]));

        var fieldsList = new List<string>(testData[0]);
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetBatchAsync(null!, dictionaryName, fieldsList));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetBatchAsync(cacheName, null!, fieldsList));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetBatchAsync(cacheName, dictionaryName, (List<string>)null!));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetBatchAsync(cacheName, dictionaryName, new List<string>(testData[1])));
    }

    [Fact]
    public async void DictionaryGetBatchAsync_FieldsAreStrings_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidString();
        var value1 = Utils.GuidString();
        var field2 = Utils.GuidString();
        var value2 = Utils.GuidString();
        var field3 = Utils.GuidString();

        await client.DictionarySetAsync(cacheName, dictionaryName, field1, value1, false, 10);
        await client.DictionarySetAsync(cacheName, dictionaryName, field2, value2, false, 10);

        var response1 = await client.DictionaryGetBatchAsync(cacheName, dictionaryName, field1, field2, field3);
        var response2 = await client.DictionaryGetBatchAsync(cacheName, dictionaryName, new string[] { field1, field2, field3 });

        var status = new CacheGetStatus[] { CacheGetStatus.HIT, CacheGetStatus.HIT, CacheGetStatus.MISS };
        var values = new string?[] { value1, value2, null };
        Assert.Equal(status, response1.Status);
        Assert.Equal(status, response2.Status);
        Assert.Equal(values, response1.Strings());
        Assert.Equal(values, response2.Strings());
    }

    [Theory]
    [InlineData(null, "my-dictionary")]
    [InlineData("cache", null)]
    public void DictionaryFetch_NullChecks_ThrowsException(string cacheName, string dictionaryName)
    {
        Assert.Throws<ArgumentNullException>(() => client.DictionaryFetch(cacheName, dictionaryName));
    }

    [Fact]
    public void DictionaryFetch_Missing_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var response = client.DictionaryFetch(cacheName, dictionaryName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionaryFetch_HasContentString_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidString();
        var value1 = Utils.GuidString();
        var field2 = Utils.GuidString();
        var value2 = Utils.GuidString();
        var contentDictionary = new Dictionary<string, string>() {
            {field1, value1},
            {field2, value2}
        };

        client.DictionarySet(cacheName, dictionaryName, field1, value1, true, ttlSeconds: 10);
        client.DictionarySet(cacheName, dictionaryName, field2, value2, true, ttlSeconds: 10);

        var getAllResponse = client.DictionaryFetch(cacheName, dictionaryName);

        Assert.Equal(CacheGetStatus.HIT, getAllResponse.Status);
        Assert.Equal(getAllResponse.StringDictionary(), contentDictionary);
    }

    [Fact]
    public void DictionaryFetch_HasContentBytes_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidBytes();
        var value1 = Utils.GuidBytes();
        var field2 = Utils.GuidBytes();
        var value2 = Utils.GuidBytes();
        var contentDictionary = new Dictionary<byte[], byte[]>() {
            {field1, value1},
            {field2, value2}
        };

        client.DictionarySet(cacheName, dictionaryName, field1, value1, true, ttlSeconds: 10);
        client.DictionarySet(cacheName, dictionaryName, field2, value2, true, ttlSeconds: 10);

        var getAllResponse = client.DictionaryFetch(cacheName, dictionaryName);

        Assert.Equal(CacheGetStatus.HIT, getAllResponse.Status);

        // Exercise byte array dictionary structural equality comparer
        Assert.True(getAllResponse.ByteArrayDictionary!.ContainsKey(field1));
        Assert.True(getAllResponse.ByteArrayDictionary!.ContainsKey(field2));
        Assert.Equal(2, getAllResponse.ByteArrayDictionary!.Count);

        // Exercise DictionaryEquals extension
        Assert.True(getAllResponse.ByteArrayDictionary!.DictionaryEquals(contentDictionary));
    }

    [Theory]
    [InlineData(null, "my-dictionary")]
    [InlineData("cache", null)]
    public async void DictionaryFetchAsync_NullChecks_ThrowsException(string cacheName, string dictionaryName)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryFetchAsync(cacheName, dictionaryName));
    }

    [Fact]
    public async void DictionaryFetchAsync_Missing_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var response = await client.DictionaryFetchAsync(cacheName, dictionaryName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async void DictionaryFetchAsync_HasContentString_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidString();
        var value1 = Utils.GuidString();
        var field2 = Utils.GuidString();
        var value2 = Utils.GuidString();
        var contentDictionary = new Dictionary<string, string>() {
            {field1, value1},
            {field2, value2}
        };

        await client.DictionarySetAsync(cacheName, dictionaryName, field1, value1, true, ttlSeconds: 10);
        await client.DictionarySetAsync(cacheName, dictionaryName, field2, value2, true, ttlSeconds: 10);

        var getAllResponse = await client.DictionaryFetchAsync(cacheName, dictionaryName);

        Assert.Equal(CacheGetStatus.HIT, getAllResponse.Status);
        Assert.Equal(getAllResponse.StringDictionary(), contentDictionary);
    }

    [Fact]
    public async void DictionaryFetchAsync_HasContentBytes_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidBytes();
        var value1 = Utils.GuidBytes();
        var field2 = Utils.GuidBytes();
        var value2 = Utils.GuidBytes();
        var contentDictionary = new Dictionary<byte[], byte[]>() {
            {field1, value1},
            {field2, value2}
        };

        await client.DictionarySetAsync(cacheName, dictionaryName, field1, value1, true, ttlSeconds: 10);
        await client.DictionarySetAsync(cacheName, dictionaryName, field2, value2, true, ttlSeconds: 10);

        var getAllResponse = await client.DictionaryFetchAsync(cacheName, dictionaryName);

        Assert.Equal(CacheGetStatus.HIT, getAllResponse.Status);

        // Exercise byte array dictionary structural equality comparer
        Assert.True(getAllResponse.ByteArrayDictionary!.ContainsKey(field1));
        Assert.True(getAllResponse.ByteArrayDictionary!.ContainsKey(field2));
        Assert.Equal(2, getAllResponse.ByteArrayDictionary!.Count);

        // Exercise DictionaryEquals extension
        Assert.True(getAllResponse.ByteArrayDictionary!.DictionaryEquals(contentDictionary));
    }

    [Theory]
    [InlineData(null, "my-dictionary")]
    [InlineData("my-cache", null)]
    public void DictionaryDelete_NullChecks_ThrowsException(string cacheName, string dictionaryName)
    {
        Assert.Throws<ArgumentNullException>(() => client.DictionaryDelete(cacheName, dictionaryName));
    }

    [Fact]
    public void DictionaryDelete_DictionaryDoesNotExist_Noop()
    {
        var dictionaryName = Utils.GuidString();
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryFetch(cacheName, dictionaryName).Status);
        client.DictionaryDelete(cacheName, dictionaryName);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryFetch(cacheName, dictionaryName).Status);
    }

    [Fact]
    public void DictionaryDelete_DictionaryExists_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        client.DictionarySet(cacheName, dictionaryName, Utils.GuidString(), Utils.GuidString(), false);
        client.DictionarySet(cacheName, dictionaryName, Utils.GuidString(), Utils.GuidString(), false);
        client.DictionarySet(cacheName, dictionaryName, Utils.GuidString(), Utils.GuidString(), false);

        Assert.Equal(CacheGetStatus.HIT, client.DictionaryFetch(cacheName, dictionaryName).Status);
        client.DictionaryDelete(cacheName, dictionaryName);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryFetch(cacheName, dictionaryName).Status);
    }

    [Theory]
    [InlineData(null, "my-dictionary")]
    [InlineData("my-cache", null)]
    public async void DictionaryDeleteAsync_NullChecks_ThrowsException(string cacheName, string dictionaryName)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryDeleteAsync(cacheName, dictionaryName));
    }

    [Fact]
    public async void DictionaryDeleteAsync_DictionaryDoesNotExist_Noop()
    {
        var dictionaryName = Utils.GuidString();
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryFetchAsync(cacheName, dictionaryName)).Status);
        await client.DictionaryDeleteAsync(cacheName, dictionaryName);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryFetchAsync(cacheName, dictionaryName)).Status);
    }

    [Fact]
    public async void DictionaryDeleteAsync_DictionaryExists_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        await client.DictionarySetAsync(cacheName, dictionaryName, Utils.GuidString(), Utils.GuidString(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, Utils.GuidString(), Utils.GuidString(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, Utils.GuidString(), Utils.GuidString(), false);

        Assert.Equal(CacheGetStatus.HIT, (await client.DictionaryFetchAsync(cacheName, dictionaryName)).Status);
        await client.DictionaryDeleteAsync(cacheName, dictionaryName);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryFetchAsync(cacheName, dictionaryName)).Status);
    }

    [Theory]
    [InlineData(null, "my-dictionary", new byte[] { 0x00 })]
    [InlineData("my-cache", null, new byte[] { 0x00 })]
    [InlineData("my-cache", "my-dictionary", null)]
    public void DictionaryRemoveField_NullChecksByte_ThrowsException(string cacheName, string dictionaryName, byte[] field)
    {
        Assert.Throws<ArgumentNullException>(() => client.DictionaryRemoveField(cacheName, dictionaryName, field));
    }

    [Theory]
    [InlineData(null, "my-dictionary", "my-field")]
    [InlineData("my-cache", null, "my-field")]
    [InlineData("my-cache", "my-dictionary", null)]
    public void DictionaryRemoveField_NullChecksString_ThrowsException(string cacheName, string dictionaryName, string field)
    {
        Assert.Throws<ArgumentNullException>(() => client.DictionaryRemoveField(cacheName, dictionaryName, field));
    }

    [Fact]
    public void DictionaryRemoveField_FieldIsByteArray_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidBytes();
        var value1 = Utils.GuidBytes();
        var field2 = Utils.GuidBytes();

        // Add a field then delete it
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, field1).Status);
        client.DictionarySet(cacheName, dictionaryName, field1, value1, false);
        Assert.Equal(CacheGetStatus.HIT, client.DictionaryGet(cacheName, dictionaryName, field1).Status);

        client.DictionaryRemoveField(cacheName, dictionaryName, field1);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, field1).Status);

        // Test no-op
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, field2).Status);
        client.DictionaryRemoveField(cacheName, dictionaryName, field2);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, field2).Status);
    }

    [Fact]
    public void DictionaryRemoveField_FieldIsString_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidString();
        var value1 = Utils.GuidString();
        var field2 = Utils.GuidString();

        // Add a field then delete it
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, field1).Status);
        client.DictionarySet(cacheName, dictionaryName, field1, value1, false);
        Assert.Equal(CacheGetStatus.HIT, client.DictionaryGet(cacheName, dictionaryName, field1).Status);

        client.DictionaryRemoveField(cacheName, dictionaryName, field1);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, field1).Status);

        // Test no-op
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, field2).Status);
        client.DictionaryRemoveField(cacheName, dictionaryName, field2);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, field2).Status);
    }

    [Theory]
    [InlineData(null, "my-dictionary", new byte[] { 0x00 })]
    [InlineData("my-cache", null, new byte[] { 0x00 })]
    [InlineData("my-cache", "my-dictionary", null)]
    public async void DictionaryRemoveFieldAsync_NullChecksByte_ThrowsException(string cacheName, string dictionaryName, byte[] field)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field));
    }

    [Theory]
    [InlineData(null, "my-dictionary", "my-field")]
    [InlineData("my-cache", null, "my-field")]
    [InlineData("my-cache", "my-dictionary", null)]
    public async void DictionaryRemoveFieldAsync_NullChecksString_ThrowsException(string cacheName, string dictionaryName, string field)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field));
    }

    [Fact]
    public async void DictionaryRemoveFieldAsync_FieldIsByteArray_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidBytes();
        var value1 = Utils.GuidBytes();
        var field2 = Utils.GuidBytes();

        // Add a field then delete it
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, field1)).Status);
        client.DictionarySet(cacheName, dictionaryName, field1, value1, false);
        Assert.Equal(CacheGetStatus.HIT, (await client.DictionaryGetAsync(cacheName, dictionaryName, field1)).Status);

        client.DictionaryRemoveField(cacheName, dictionaryName, field1);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, field1)).Status);

        // Test no-op
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, field2)).Status);
        client.DictionaryRemoveField(cacheName, dictionaryName, field2);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, field2)).Status);
    }

    [Fact]
    public async void DictionaryRemoveFieldAsync_FieldIsString_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidString();
        var value1 = Utils.GuidString();
        var field2 = Utils.GuidString();

        // Add a field then delete it
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, field1)).Status);
        await client.DictionarySetAsync(cacheName, dictionaryName, field1, value1, false);
        Assert.Equal(CacheGetStatus.HIT, (await client.DictionaryGetAsync(cacheName, dictionaryName, field1)).Status);

        await client.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field1);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, field1)).Status);

        // Test no-op
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, field2).Status);
        client.DictionaryRemoveField(cacheName, dictionaryName, field2);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, field2).Status);
    }

    [Fact]
    public void DictionaryRemoveFields_NullChecksByteArrayParams_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var testData = new byte[][][] { new byte[][] { Utils.GuidBytes(), Utils.GuidBytes() }, new byte[][] { Utils.GuidBytes(), null! } };

        Assert.Throws<ArgumentNullException>(() => client.DictionaryRemoveFields(null!, dictionaryName, testData[0]));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryRemoveFields(cacheName, null!, testData[0]));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryRemoveFields(cacheName, dictionaryName, (byte[][])null!));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryRemoveFields(cacheName, dictionaryName, testData[1]));

        var fieldsList = new List<byte[]>(testData[0]);
        Assert.Throws<ArgumentNullException>(() => client.DictionaryRemoveFields(null!, dictionaryName, fieldsList));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryRemoveFields(cacheName, null!, fieldsList));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryRemoveFields(cacheName, dictionaryName, (List<byte[]>)null!));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryRemoveFields(cacheName, dictionaryName, new List<byte[]>(testData[1])));
    }

    [Fact]
    public void DictionaryRemoveFields_ByteArrayParams_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var fields = new byte[][] { Utils.GuidBytes(), Utils.GuidBytes() };
        var otherField = Utils.GuidBytes();

        // Test variadic args
        client.DictionarySet(cacheName, dictionaryName, fields[0], Utils.GuidBytes(), false);
        client.DictionarySet(cacheName, dictionaryName, fields[1], Utils.GuidBytes(), false);
        client.DictionarySet(cacheName, dictionaryName, otherField, Utils.GuidBytes(), false);


        client.DictionaryRemoveFields(cacheName, dictionaryName, fields[0], fields[1]);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, fields[0]).Status);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, fields[1]).Status);
        Assert.Equal(CacheGetStatus.HIT, client.DictionaryGet(cacheName, dictionaryName, otherField).Status);

        // Test enumerable
        dictionaryName = Utils.GuidString();
        client.DictionarySet(cacheName, dictionaryName, fields[0], Utils.GuidBytes(), false);
        client.DictionarySet(cacheName, dictionaryName, fields[1], Utils.GuidBytes(), false);
        client.DictionarySet(cacheName, dictionaryName, otherField, Utils.GuidBytes(), false);

        var fieldsList = new List<byte[]>(fields);
        client.DictionaryRemoveFields(cacheName, dictionaryName, fieldsList);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, fields[0]).Status);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, fields[1]).Status);
        Assert.Equal(CacheGetStatus.HIT, client.DictionaryGet(cacheName, dictionaryName, otherField).Status);
    }

    [Fact]
    public void DictionaryRemoveFields_NullChecksStringParams_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var testData = new string[][] { new string[] { Utils.GuidString(), Utils.GuidString() }, new string[] { Utils.GuidString(), null! } };
        Assert.Throws<ArgumentNullException>(() => client.DictionaryRemoveFields(null!, dictionaryName, testData[0]));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryRemoveFields(cacheName, null!, testData[0]));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryRemoveFields(cacheName, dictionaryName, (string[])null!));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryRemoveFields(cacheName, dictionaryName, testData[1]));

        var fieldsList = new List<string>(testData[0]);
        Assert.Throws<ArgumentNullException>(() => client.DictionaryRemoveFields(null!, dictionaryName, fieldsList));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryRemoveFields(cacheName, null!, fieldsList));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryRemoveFields(cacheName, dictionaryName, (List<string>)null!));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryRemoveFields(cacheName, dictionaryName, new List<string>(testData[1])));
    }

    [Fact]
    public void DictionaryRemoveFields_StringParams_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var fields = new string[] { Utils.GuidString(), Utils.GuidString() };
        var otherField = Utils.GuidString();

        // Test variadic args
        client.DictionarySet(cacheName, dictionaryName, fields[0], Utils.GuidString(), false);
        client.DictionarySet(cacheName, dictionaryName, fields[1], Utils.GuidString(), false);
        client.DictionarySet(cacheName, dictionaryName, otherField, Utils.GuidString(), false);


        client.DictionaryRemoveFields(cacheName, dictionaryName, fields[0], fields[1]);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, fields[0]).Status);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, fields[1]).Status);
        Assert.Equal(CacheGetStatus.HIT, client.DictionaryGet(cacheName, dictionaryName, otherField).Status);

        // Test enumerable
        dictionaryName = Utils.GuidString();
        client.DictionarySet(cacheName, dictionaryName, fields[0], Utils.GuidString(), false);
        client.DictionarySet(cacheName, dictionaryName, fields[1], Utils.GuidString(), false);
        client.DictionarySet(cacheName, dictionaryName, otherField, Utils.GuidString(), false);

        var fieldsList = new List<string>(fields);
        client.DictionaryRemoveFields(cacheName, dictionaryName, fieldsList);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, fields[0]).Status);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, fields[1]).Status);
        Assert.Equal(CacheGetStatus.HIT, client.DictionaryGet(cacheName, dictionaryName, otherField).Status);
    }

    [Fact]
    public async void DictionaryRemoveFieldsAsync_NullChecksByteArrayParams_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var testData = new byte[][][] { new byte[][] { Utils.GuidBytes(), Utils.GuidBytes() }, new byte[][] { Utils.GuidBytes(), null! } };

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldsAsync(null!, dictionaryName, testData[0]));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldsAsync(cacheName, null!, testData[0]));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, (byte[][])null!));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, testData[1]));

        var fieldsList = new List<byte[]>(testData[0]);
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldsAsync(null!, dictionaryName, fieldsList));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldsAsync(cacheName, null!, fieldsList));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, (List<byte[]>)null!));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, new List<byte[]>(testData[1])));
    }

    [Fact]
    public async void DictionaryRemoveFieldsAsync_ByteArrayParams_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var fields = new byte[][] { Utils.GuidBytes(), Utils.GuidBytes() };
        var otherField = Utils.GuidBytes();

        // Test variadic args
        await client.DictionarySetAsync(cacheName, dictionaryName, fields[0], Utils.GuidBytes(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, fields[1], Utils.GuidBytes(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, otherField, Utils.GuidBytes(), false);


        await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fields[0], fields[1]);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, fields[0])).Status);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, fields[1])).Status);
        Assert.Equal(CacheGetStatus.HIT, (await client.DictionaryGetAsync(cacheName, dictionaryName, otherField)).Status);

        // Test enumerable
        dictionaryName = Utils.GuidString();
        await client.DictionarySetAsync(cacheName, dictionaryName, fields[0], Utils.GuidBytes(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, fields[1], Utils.GuidBytes(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, otherField, Utils.GuidBytes(), false);

        var fieldsList = new List<byte[]>(fields);
        await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fieldsList);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, fields[0])).Status);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, fields[1])).Status);
        Assert.Equal(CacheGetStatus.HIT, (await client.DictionaryGetAsync(cacheName, dictionaryName, otherField)).Status);
    }

    [Fact]
    public async void DictionaryRemoveFieldsAsync_NullChecksStringParams_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var testData = new string[][] { new string[] { Utils.GuidString(), Utils.GuidString() }, new string[] { Utils.GuidString(), null! } };
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldsAsync(null!, dictionaryName, testData[0]));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldsAsync(cacheName, null!, testData[0]));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, (string[])null!));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, testData[1]));

        var fieldsList = new List<string>(testData[0]);
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldsAsync(null!, dictionaryName, fieldsList));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldsAsync(cacheName, null!, fieldsList));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, (List<string>)null!));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, new List<string>(testData[1])));
    }

    [Fact]
    public async void DictionaryRemoveFieldsAsync_StringParams_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var fields = new string[] { Utils.GuidString(), Utils.GuidString() };
        var otherField = Utils.GuidString();

        // Test variadic args
        await client.DictionarySetAsync(cacheName, dictionaryName, fields[0], Utils.GuidString(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, fields[1], Utils.GuidString(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, otherField, Utils.GuidString(), false);


        await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fields[0], fields[1]);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, fields[0])).Status);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, fields[1])).Status);
        Assert.Equal(CacheGetStatus.HIT, (await client.DictionaryGetAsync(cacheName, dictionaryName, otherField)).Status);

        // Test enumerable
        dictionaryName = Utils.GuidString();
        await client.DictionarySetAsync(cacheName, dictionaryName, fields[0], Utils.GuidString(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, fields[1], Utils.GuidString(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, otherField, Utils.GuidString(), false);

        var fieldsList = new List<string>(fields);
        await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fieldsList);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, fields[0])).Status);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, fields[1])).Status);
        Assert.Equal(CacheGetStatus.HIT, (await client.DictionaryGetAsync(cacheName, dictionaryName, otherField)).Status);
    }
}
