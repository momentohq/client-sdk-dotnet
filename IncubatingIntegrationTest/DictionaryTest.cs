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
    public void DictionaryGet_NullChecksByteArray_ThrowsException(string cacheName, string dictionaryName, byte[] field)
    {
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGet(cacheName, dictionaryName, field));
    }

    [Theory]
    [InlineData(null, "my-dictionary", new byte[] { 0x00 }, new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 }, new byte[] { 0x00 })]
    [InlineData("cache", "my-dictionary", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-dictionary", new byte[] { 0x00 }, null)]
    public void DictionarySet_NullChecksByteArray_ThrowsException(string cacheName, string dictionaryName, byte[] field, byte[] value)
    {
        Assert.Throws<ArgumentNullException>(() => client.DictionarySet(cacheName, dictionaryName, field, value, false));
    }

    [Fact]
    public void DictionaryGet_FieldIsByteArray_DictionaryIsMissing()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var response = client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsByteArrayValueIsByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();

        client.DictionarySet(cacheName, dictionaryName, field, value, false);

        var getResponse = client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsByteArrayDictionaryIsPresent_FieldIsMissing()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();

        client.DictionarySet(cacheName, dictionaryName, field, value, false);

        var otherField = Utils.NewGuidByteArray();
        var response = client.DictionaryGet(cacheName, dictionaryName, otherField);

        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsByteArray_NoRefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();

        client.DictionarySet(cacheName, dictionaryName, field, value, false, ttlSeconds: 5);
        Thread.Sleep(100);

        client.DictionarySet(cacheName, dictionaryName, field, value, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsByteArray_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();

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
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var response = client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsStringValueIsString_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();

        client.DictionarySet(cacheName, dictionaryName, field, value, false);

        var getResponse = client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
    }

    [Fact]
    public void DictionarySetGet_DictionaryIsPresent_FieldIsMissing()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();

        client.DictionarySet(cacheName, dictionaryName, field, value, false);

        var otherField = Utils.NewGuidString();
        var response = client.DictionaryGet(cacheName, dictionaryName, otherField);

        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsString_NoRefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();

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
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();

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
    public async Task DictionaryGetAsync_NullChecksByteArray_ThrowsException(string cacheName, string dictionaryName, byte[] field)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetAsync(cacheName, dictionaryName, field));
    }

    [Theory]
    [InlineData(null, "my-dictionary", new byte[] { 0x00 }, new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 }, new byte[] { 0x00 })]
    [InlineData("cache", "my-dictionary", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-dictionary", new byte[] { 0x00 }, null)]
    public async Task DictionarySetAsync_NullChecksByteArray_ThrowsException(string cacheName, string dictionaryName, byte[] field, byte[] value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false));
    }

    [Fact]
    public async Task DictionaryGetAsync_FieldIsByteArray_DictionaryIsMissing()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsByteArrayValueIsByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false);

        var getResponse = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsByteArrayDictionaryIsPresent_FieldIsMissing()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false);

        var otherField = Utils.NewGuidByteArray();
        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, otherField);

        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsByteArray_NoRefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 5);
        await Task.Delay(100);

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 10);
        await Task.Delay(4900);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsByteArray_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 1);
        await Task.Delay(100);

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, true, ttlSeconds: 10);
        await Task.Delay(1000);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.Bytes);
    }

    [Theory]
    [InlineData(null, "my-dictionary", "my-field")]
    [InlineData("cache", null, "my-field")]
    [InlineData("cache", "my-dictionary", null)]
    public async Task DictionaryGetAsync_NullChecksString_ThrowsException(string cacheName, string dictionaryName, string field)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetAsync(cacheName, dictionaryName, field));
    }

    [Theory]
    [InlineData(null, "my-dictionary", "my-field", "my-value")]
    [InlineData("cache", null, "my-field", "my-value")]
    [InlineData("cache", "my-dictionary", null, "my-value")]
    [InlineData("cache", "my-dictionary", "my-field", null)]
    public async Task DictionarySetAsync_NullChecksString_ThrowsException(string cacheName, string dictionaryName, string field, string value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false));
    }

    [Fact]
    public async Task DictionaryGetAsync_FieldIsString_DictionaryIsMissing()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsStringValueIsString_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false);

        var getResponse = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
    }

    [Fact]
    public async Task DictionarySetGetAsync_DictionaryIsPresent_FieldIsMissing()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false);

        var otherField = Utils.NewGuidString();
        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, otherField);

        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsString_NoRefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 5);
        await Task.Delay(100);

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 10);
        await Task.Delay(4900);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsString_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 1);
        await Task.Delay(100);

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, true, ttlSeconds: 10);
        await Task.Delay(1000);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.String());
    }

    [Fact]
    public void DictionarySetBatch_NullChecksByteArray_ThrowsException()
    {
        var dictionaryName = Utils.NewGuidString();
        var dictionary = new Dictionary<byte[], byte[]>();
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetBatch(null!, dictionaryName, dictionary, false));
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetBatch(cacheName, null!, dictionary, false));
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetBatch(cacheName, dictionaryName, (IEnumerable<KeyValuePair<byte[], byte[]>>)null!, false));

        dictionary[Utils.NewGuidByteArray()] = null!;
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetBatch(cacheName, dictionaryName, dictionary, false));
    }

    [Fact]
    public void DictionarySetBatch_FieldsAreByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidByteArray();
        var value1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidByteArray();
        var value2 = Utils.NewGuidByteArray();

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
    public void DictionarySetBatch_FieldsAreByteArray_NoRefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();
        var content = new Dictionary<byte[], byte[]>() { { field, value } };

        client.DictionarySetBatch(cacheName, dictionaryName, content, false, ttlSeconds: 5);
        Thread.Sleep(100);

        client.DictionarySetBatch(cacheName, dictionaryName, content, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetBatch_FieldsAreByteArray_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();
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
        var dictionaryName = Utils.NewGuidString();
        var dictionary = new Dictionary<string, string>();
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetBatch(null!, dictionaryName, dictionary, false));
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetBatch(cacheName, null!, dictionary, false));
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetBatch(cacheName, dictionaryName, (IEnumerable<KeyValuePair<string, string>>)null!, false));

        dictionary[Utils.NewGuidString()] = null!;
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetBatch(cacheName, dictionaryName, dictionary, false));
    }

    [Fact]
    public void DictionarySetBatch_FieldsAreStrings_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var field2 = Utils.NewGuidString();
        var value2 = Utils.NewGuidString();

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
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();
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
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();
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
    public async Task DictionarySetBatchAsync_NullChecksByteArray_ThrowsException()
    {
        var dictionaryName = Utils.NewGuidString();
        var dictionary = new Dictionary<byte[], byte[]>();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(null!, dictionaryName, dictionary, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(cacheName, null!, dictionary, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(cacheName, dictionaryName, (IEnumerable<KeyValuePair<byte[], byte[]>>)null!, false));

        dictionary[Utils.NewGuidByteArray()] = null!;
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(cacheName, dictionaryName, dictionary, false));
    }

    [Fact]
    public async Task DictionarySetBatchAsync_FieldsAreByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidByteArray();
        var value1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidByteArray();
        var value2 = Utils.NewGuidByteArray();

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
    public async Task DictionarySetBatchAsync_FieldsAreByteArray_NoRefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();
        var content = new Dictionary<byte[], byte[]>() { { field, value } };

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, false, ttlSeconds: 5);
        await Task.Delay(100);

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, false, ttlSeconds: 10);
        await Task.Delay(4900);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task DictionarySetBatchAsync_FieldsAreByteArray_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();
        var content = new Dictionary<byte[], byte[]>() { { field, value } };

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, false, ttlSeconds: 1);
        await Task.Delay(100);

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, true, ttlSeconds: 10);
        await Task.Delay(1000);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.Bytes);
    }

    [Fact]
    public async Task DictionarySetBatchAsync_NullChecksStrings_ThrowsException()
    {
        var dictionaryName = Utils.NewGuidString();
        var dictionary = new Dictionary<string, string>();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(null!, dictionaryName, dictionary, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(cacheName, null!, dictionary, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(cacheName, dictionaryName, (IEnumerable<KeyValuePair<string, string>>)null!, false));

        dictionary[Utils.NewGuidString()] = null!;
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(cacheName, dictionaryName, dictionary, false));
    }

    [Fact]
    public async Task DictionarySetBatchAsync_FieldsAreStrings_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var field2 = Utils.NewGuidString();
        var value2 = Utils.NewGuidString();

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
    public async Task DictionarySetBatchAsync_FieldsAreStrings_NoRefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();
        var content = new Dictionary<string, string>() { { field, value } };

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, false, ttlSeconds: 5);
        await Task.Delay(100);

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, false, ttlSeconds: 10);
        await Task.Delay(4900);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task DictionarySetBatchAsync_FieldsAreStrings_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();
        var content = new Dictionary<string, string>() { { field, value } };

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, false, ttlSeconds: 1);
        await Task.Delay(100);

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, true, ttlSeconds: 10);
        await Task.Delay(1000);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.String());
    }

    [Fact]
    public void DictionaryGetBatch_NullChecksByteArrayParams_ThrowsException()
    {
        var dictionaryName = Utils.NewGuidString();
        var testData = new byte[][][] { new byte[][] { Utils.NewGuidByteArray(), Utils.NewGuidByteArray() }, new byte[][] { Utils.NewGuidByteArray(), null! } };

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
    public void DictionaryGetBatch_FieldsAreByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidByteArray();
        var value1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidByteArray();
        var value2 = Utils.NewGuidByteArray();
        var field3 = Utils.NewGuidByteArray();

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
        var dictionaryName = Utils.NewGuidString();
        var testData = new string[][] { new string[] { Utils.NewGuidString(), Utils.NewGuidString() }, new string[] { Utils.NewGuidString(), null! } };
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
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var field2 = Utils.NewGuidString();
        var value2 = Utils.NewGuidString();
        var field3 = Utils.NewGuidString();

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
    public async Task DictionaryGetBatchAsync_NullChecksByteArrayParams_ThrowsException()
    {
        var dictionaryName = Utils.NewGuidString();
        var testData = new byte[][][] { new byte[][] { Utils.NewGuidByteArray(), Utils.NewGuidByteArray() }, new byte[][] { Utils.NewGuidByteArray(), null! } };

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
    public async Task DictionaryGetBatchAsync_FieldsAreByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidByteArray();
        var value1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidByteArray();
        var value2 = Utils.NewGuidByteArray();
        var field3 = Utils.NewGuidByteArray();

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
    public async Task DictionaryGetBatchAsync_NullChecksStringParams_ThrowsException()
    {
        var dictionaryName = Utils.NewGuidString();
        var testData = new string[][] { new string[] { Utils.NewGuidString(), Utils.NewGuidString() }, new string[] { Utils.NewGuidString(), null! } };
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
    public async Task DictionaryGetBatchAsync_FieldsAreStrings_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var field2 = Utils.NewGuidString();
        var value2 = Utils.NewGuidString();
        var field3 = Utils.NewGuidString();

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
        var dictionaryName = Utils.NewGuidString();
        var response = client.DictionaryFetch(cacheName, dictionaryName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
        Assert.Null(response.ByteArrayDictionary);
        Assert.Null(response.StringDictionary());
    }

    [Fact]
    public void DictionaryFetch_HasContentString_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var field2 = Utils.NewGuidString();
        var value2 = Utils.NewGuidString();
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
    public void DictionaryFetch_HasContentByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidByteArray();
        var value1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidByteArray();
        var value2 = Utils.NewGuidByteArray();
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
    public async Task DictionaryFetchAsync_NullChecks_ThrowsException(string cacheName, string dictionaryName)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryFetchAsync(cacheName, dictionaryName));
    }

    [Fact]
    public async Task DictionaryFetchAsync_Missing_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var response = await client.DictionaryFetchAsync(cacheName, dictionaryName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
        Assert.Null(response.ByteArrayDictionary);
        Assert.Null(response.StringDictionary());
    }

    [Fact]
    public async Task DictionaryFetchAsync_HasContentString_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var field2 = Utils.NewGuidString();
        var value2 = Utils.NewGuidString();
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
    public async Task DictionaryFetchAsync_HasContentByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidByteArray();
        var value1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidByteArray();
        var value2 = Utils.NewGuidByteArray();
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
        var dictionaryName = Utils.NewGuidString();
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryFetch(cacheName, dictionaryName).Status);
        client.DictionaryDelete(cacheName, dictionaryName);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryFetch(cacheName, dictionaryName).Status);
    }

    [Fact]
    public void DictionaryDelete_DictionaryExists_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        client.DictionarySet(cacheName, dictionaryName, Utils.NewGuidString(), Utils.NewGuidString(), false);
        client.DictionarySet(cacheName, dictionaryName, Utils.NewGuidString(), Utils.NewGuidString(), false);
        client.DictionarySet(cacheName, dictionaryName, Utils.NewGuidString(), Utils.NewGuidString(), false);

        Assert.Equal(CacheGetStatus.HIT, client.DictionaryFetch(cacheName, dictionaryName).Status);
        client.DictionaryDelete(cacheName, dictionaryName);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryFetch(cacheName, dictionaryName).Status);
    }

    [Theory]
    [InlineData(null, "my-dictionary")]
    [InlineData("my-cache", null)]
    public async Task DictionaryDeleteAsync_NullChecks_ThrowsException(string cacheName, string dictionaryName)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryDeleteAsync(cacheName, dictionaryName));
    }

    [Fact]
    public async Task DictionaryDeleteAsync_DictionaryDoesNotExist_Noop()
    {
        var dictionaryName = Utils.NewGuidString();
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryFetchAsync(cacheName, dictionaryName)).Status);
        await client.DictionaryDeleteAsync(cacheName, dictionaryName);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryFetchAsync(cacheName, dictionaryName)).Status);
    }

    [Fact]
    public async Task DictionaryDeleteAsync_DictionaryExists_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        await client.DictionarySetAsync(cacheName, dictionaryName, Utils.NewGuidString(), Utils.NewGuidString(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, Utils.NewGuidString(), Utils.NewGuidString(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, Utils.NewGuidString(), Utils.NewGuidString(), false);

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
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidByteArray();
        var value1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidByteArray();

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
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var field2 = Utils.NewGuidString();

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
    public async Task DictionaryRemoveFieldAsync_NullChecksByte_ThrowsException(string cacheName, string dictionaryName, byte[] field)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field));
    }

    [Theory]
    [InlineData(null, "my-dictionary", "my-field")]
    [InlineData("my-cache", null, "my-field")]
    [InlineData("my-cache", "my-dictionary", null)]
    public async Task DictionaryRemoveFieldAsync_NullChecksString_ThrowsException(string cacheName, string dictionaryName, string field)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field));
    }

    [Fact]
    public async Task DictionaryRemoveFieldAsync_FieldIsByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidByteArray();
        var value1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidByteArray();

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
    public async Task DictionaryRemoveFieldAsync_FieldIsString_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var field2 = Utils.NewGuidString();

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
        var dictionaryName = Utils.NewGuidString();
        var testData = new byte[][][] { new byte[][] { Utils.NewGuidByteArray(), Utils.NewGuidByteArray() }, new byte[][] { Utils.NewGuidByteArray(), null! } };

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
        var dictionaryName = Utils.NewGuidString();
        var fields = new byte[][] { Utils.NewGuidByteArray(), Utils.NewGuidByteArray() };
        var otherField = Utils.NewGuidByteArray();

        // Test variadic args
        client.DictionarySet(cacheName, dictionaryName, fields[0], Utils.NewGuidByteArray(), false);
        client.DictionarySet(cacheName, dictionaryName, fields[1], Utils.NewGuidByteArray(), false);
        client.DictionarySet(cacheName, dictionaryName, otherField, Utils.NewGuidByteArray(), false);


        client.DictionaryRemoveFields(cacheName, dictionaryName, fields[0], fields[1]);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, fields[0]).Status);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, fields[1]).Status);
        Assert.Equal(CacheGetStatus.HIT, client.DictionaryGet(cacheName, dictionaryName, otherField).Status);

        // Test enumerable
        dictionaryName = Utils.NewGuidString();
        client.DictionarySet(cacheName, dictionaryName, fields[0], Utils.NewGuidByteArray(), false);
        client.DictionarySet(cacheName, dictionaryName, fields[1], Utils.NewGuidByteArray(), false);
        client.DictionarySet(cacheName, dictionaryName, otherField, Utils.NewGuidByteArray(), false);

        var fieldsList = new List<byte[]>(fields);
        client.DictionaryRemoveFields(cacheName, dictionaryName, fieldsList);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, fields[0]).Status);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, fields[1]).Status);
        Assert.Equal(CacheGetStatus.HIT, client.DictionaryGet(cacheName, dictionaryName, otherField).Status);
    }

    [Fact]
    public void DictionaryRemoveFields_NullChecksStringParams_ThrowsException()
    {
        var dictionaryName = Utils.NewGuidString();
        var testData = new string[][] { new string[] { Utils.NewGuidString(), Utils.NewGuidString() }, new string[] { Utils.NewGuidString(), null! } };
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
        var dictionaryName = Utils.NewGuidString();
        var fields = new string[] { Utils.NewGuidString(), Utils.NewGuidString() };
        var otherField = Utils.NewGuidString();

        // Test variadic args
        client.DictionarySet(cacheName, dictionaryName, fields[0], Utils.NewGuidString(), false);
        client.DictionarySet(cacheName, dictionaryName, fields[1], Utils.NewGuidString(), false);
        client.DictionarySet(cacheName, dictionaryName, otherField, Utils.NewGuidString(), false);


        client.DictionaryRemoveFields(cacheName, dictionaryName, fields[0], fields[1]);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, fields[0]).Status);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, fields[1]).Status);
        Assert.Equal(CacheGetStatus.HIT, client.DictionaryGet(cacheName, dictionaryName, otherField).Status);

        // Test enumerable
        dictionaryName = Utils.NewGuidString();
        client.DictionarySet(cacheName, dictionaryName, fields[0], Utils.NewGuidString(), false);
        client.DictionarySet(cacheName, dictionaryName, fields[1], Utils.NewGuidString(), false);
        client.DictionarySet(cacheName, dictionaryName, otherField, Utils.NewGuidString(), false);

        var fieldsList = new List<string>(fields);
        client.DictionaryRemoveFields(cacheName, dictionaryName, fieldsList);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, fields[0]).Status);
        Assert.Equal(CacheGetStatus.MISS, client.DictionaryGet(cacheName, dictionaryName, fields[1]).Status);
        Assert.Equal(CacheGetStatus.HIT, client.DictionaryGet(cacheName, dictionaryName, otherField).Status);
    }

    [Fact]
    public async Task DictionaryRemoveFieldsAsync_NullChecksByteArrayParams_ThrowsException()
    {
        var dictionaryName = Utils.NewGuidString();
        var testData = new byte[][][] { new byte[][] { Utils.NewGuidByteArray(), Utils.NewGuidByteArray() }, new byte[][] { Utils.NewGuidByteArray(), null! } };

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
    public async Task DictionaryRemoveFieldsAsync_ByteArrayParams_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var fields = new byte[][] { Utils.NewGuidByteArray(), Utils.NewGuidByteArray() };
        var otherField = Utils.NewGuidByteArray();

        // Test variadic args
        await client.DictionarySetAsync(cacheName, dictionaryName, fields[0], Utils.NewGuidByteArray(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, fields[1], Utils.NewGuidByteArray(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, otherField, Utils.NewGuidByteArray(), false);


        await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fields[0], fields[1]);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, fields[0])).Status);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, fields[1])).Status);
        Assert.Equal(CacheGetStatus.HIT, (await client.DictionaryGetAsync(cacheName, dictionaryName, otherField)).Status);

        // Test enumerable
        dictionaryName = Utils.NewGuidString();
        await client.DictionarySetAsync(cacheName, dictionaryName, fields[0], Utils.NewGuidByteArray(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, fields[1], Utils.NewGuidByteArray(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, otherField, Utils.NewGuidByteArray(), false);

        var fieldsList = new List<byte[]>(fields);
        await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fieldsList);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, fields[0])).Status);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, fields[1])).Status);
        Assert.Equal(CacheGetStatus.HIT, (await client.DictionaryGetAsync(cacheName, dictionaryName, otherField)).Status);
    }

    [Fact]
    public async Task DictionaryRemoveFieldsAsync_NullChecksStringParams_ThrowsException()
    {
        var dictionaryName = Utils.NewGuidString();
        var testData = new string[][] { new string[] { Utils.NewGuidString(), Utils.NewGuidString() }, new string[] { Utils.NewGuidString(), null! } };
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
    public async Task DictionaryRemoveFieldsAsync_StringParams_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var fields = new string[] { Utils.NewGuidString(), Utils.NewGuidString() };
        var otherField = Utils.NewGuidString();

        // Test variadic args
        await client.DictionarySetAsync(cacheName, dictionaryName, fields[0], Utils.NewGuidString(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, fields[1], Utils.NewGuidString(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, otherField, Utils.NewGuidString(), false);


        await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fields[0], fields[1]);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, fields[0])).Status);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, fields[1])).Status);
        Assert.Equal(CacheGetStatus.HIT, (await client.DictionaryGetAsync(cacheName, dictionaryName, otherField)).Status);

        // Test enumerable
        dictionaryName = Utils.NewGuidString();
        await client.DictionarySetAsync(cacheName, dictionaryName, fields[0], Utils.NewGuidString(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, fields[1], Utils.NewGuidString(), false);
        await client.DictionarySetAsync(cacheName, dictionaryName, otherField, Utils.NewGuidString(), false);

        var fieldsList = new List<string>(fields);
        await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fieldsList);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, fields[0])).Status);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, fields[1])).Status);
        Assert.Equal(CacheGetStatus.HIT, (await client.DictionaryGetAsync(cacheName, dictionaryName, otherField)).Status);
    }
}
