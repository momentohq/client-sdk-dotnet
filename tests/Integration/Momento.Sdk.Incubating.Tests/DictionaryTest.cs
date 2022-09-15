using Momento.Sdk.Internal.ExtensionMethods;
using Momento.Sdk.Responses;

namespace Momento.Sdk.Incubating.Tests;

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
    public async Task DictionaryGetAsync_NullChecksFieldIsByteArray_ThrowsException(string cacheName, string dictionaryName, byte[] field)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetAsync(cacheName, dictionaryName, field));
    }

    [Theory]
    [InlineData(null, "my-dictionary", new byte[] { 0x00 }, new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 }, new byte[] { 0x00 })]
    [InlineData("cache", "my-dictionary", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-dictionary", new byte[] { 0x00 }, null)]
    public async Task DictionarySetAsync_NullChecksFieldIsByteArrayValueIsByteArray_ThrowsException(string cacheName, string dictionaryName, byte[] field, byte[] value)
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
    public async Task DictionarySetGetAsync_FieldIsByteArrayValueIsByteArray_NoRefreshTtl()
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
    public async Task DictionarySetGetAsync_FieldIsByteArrayValueIsByteArray_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 2);
        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, true, ttlSeconds: 10);
        await Task.Delay(2000);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.ByteArray);
    }

    [Theory]
    [InlineData(null, "my-dictionary", "field")]
    [InlineData("cache", null, "field")]
    [InlineData("cache", "my-dictionary", null)]
    public async Task DictionaryIncrementAsync_NullChecksFieldIsString_ThrowsException(string cacheName, string dictionaryName, string field)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryIncrementAsync(cacheName, dictionaryName, field, refreshTtl: true));
    }

    [Fact]
    public async Task DictionaryIncrementAsync_IncrementFromZero_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var fieldName = Utils.NewGuidString();

        var incrementResponse = await client.DictionaryIncrementAsync(cacheName, dictionaryName, fieldName, false, 1);
        Assert.Equal(1, incrementResponse.Value);

        incrementResponse = await client.DictionaryIncrementAsync(cacheName, dictionaryName, fieldName, false, 41);
        Assert.Equal(42, incrementResponse.Value);

        incrementResponse = await client.DictionaryIncrementAsync(cacheName, dictionaryName, fieldName, false, -1042);
        Assert.Equal(-1000, incrementResponse.Value);

        var getResponse = await client.DictionaryGetAsync(cacheName, dictionaryName, fieldName);
        Assert.Equal("-1000", getResponse.String());
    }

    [Fact]
    public async Task DictionaryIncrementAsync_IncrementFromZero_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();

        await client.DictionaryIncrementAsync(cacheName, dictionaryName, field, refreshTtl: false, ttlSeconds: 2);
        await client.DictionaryIncrementAsync(cacheName, dictionaryName, field, refreshTtl: true, ttlSeconds: 10);
        await Task.Delay(2000);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal("2", response.String());
    }

    [Fact]
    public async Task DictionaryIncrementAsync_IncrementFromZero_NoRefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();

        await client.DictionaryIncrementAsync(cacheName, dictionaryName, field, refreshTtl: false, ttlSeconds: 5);
        await Task.Delay(100);

        await client.DictionaryIncrementAsync(cacheName, dictionaryName, field, refreshTtl: false, ttlSeconds: 10);
        await Task.Delay(4900);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task DictionaryIncrementAsync_SetAndReset_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();

        // Set field
        await client.DictionarySetAsync(cacheName, dictionaryName, field, "10", false);
        var incrementResponse = await client.DictionaryIncrementAsync(cacheName, dictionaryName, field, amount: 0, refreshTtl: false);
        Assert.Equal(10, incrementResponse.Value);

        incrementResponse = await client.DictionaryIncrementAsync(cacheName, dictionaryName, field, amount: 90, refreshTtl: false);
        Assert.Equal(100, incrementResponse.Value);

        // Reset field
        await client.DictionarySetAsync(cacheName, dictionaryName, field, "0", false);
        incrementResponse = await client.DictionaryIncrementAsync(cacheName, dictionaryName, field, amount: 0, refreshTtl: false);
        Assert.Equal(0, incrementResponse.Value);
    }

    // failed precondition test

    [Theory]
    [InlineData(null, "my-dictionary", "my-field")]
    [InlineData("cache", null, "my-field")]
    [InlineData("cache", "my-dictionary", null)]
    public async Task DictionaryGetAsync_NullChecksFieldIsString_ThrowsException(string cacheName, string dictionaryName, string field)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetAsync(cacheName, dictionaryName, field));
    }

    [Theory]
    [InlineData(null, "my-dictionary", "my-field", "my-value")]
    [InlineData("cache", null, "my-field", "my-value")]
    [InlineData("cache", "my-dictionary", null, "my-value")]
    [InlineData("cache", "my-dictionary", "my-field", null)]
    public async Task DictionarySetAsync_NullChecksFieldIsStringValueIsString_ThrowsException(string cacheName, string dictionaryName, string field, string value)
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
    public async Task DictionarySetGetAsync_FieldIsStringValueIsString_NoRefreshTtl()
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
    public async Task DictionarySetGetAsync_FieldIsStringValueIsString_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 2);
        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, true, ttlSeconds: 10);
        await Task.Delay(2000);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.String());
    }

    [Theory]
    [InlineData(null, "my-dictionary", "my-field", new byte[] { 0x00 })]
    [InlineData("cache", null, "my-field", new byte[] { 0x00 })]
    [InlineData("cache", "my-dictionary", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-dictionary", "my-field", null)]
    public async Task DictionarySetAsync_NullChecksFieldIsStringValueIsByteArray_ThrowsException(string cacheName, string dictionaryName, string field, byte[] value)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false));
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsStringValueIsByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false);

        var getResponse = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsStringValueIsByteArray_NoRefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 5);
        await Task.Delay(100);

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 10);
        await Task.Delay(4900);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsStringValueIsByteArray_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();

        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 2);
        await client.DictionarySetAsync(cacheName, dictionaryName, field, value, true, ttlSeconds: 10);
        await Task.Delay(2000);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.ByteArray);
    }

    [Fact]
    public async Task DictionarySetBatchAsync_NullChecksFieldIsByteArrayValueIsByteArray_ThrowsException()
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
    public async Task DictionarySetBatchAsync_FieldsAreByteArrayValuesAreByteArray_HappyPath()
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
        Assert.Equal(value1, getResponse.ByteArray);

        getResponse = await client.DictionaryGetAsync(cacheName, dictionaryName, field2);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
        Assert.Equal(value2, getResponse.ByteArray);
    }

    [Fact]
    public async Task DictionarySetBatchAsync_FieldsAreByteArrayValuesAreByteArray_NoRefreshTtl()
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
    public async Task DictionarySetBatchAsync_FieldsAreByteArrayValuesAreByteArray_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();
        var content = new Dictionary<byte[], byte[]>() { { field, value } };

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, false, ttlSeconds: 2);
        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, true, ttlSeconds: 10);
        await Task.Delay(2000);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.ByteArray);
    }

    [Fact]
    public async Task DictionarySetBatchAsync_NullChecksFieldsAreStringValuesAreString_ThrowsException()
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
    public async Task DictionarySetBatchAsync_FieldsAreStringValuesAreString_HappyPath()
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
    public async Task DictionarySetBatchAsync_FieldsAreStringValuesAreString_NoRefreshTtl()
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
    public async Task DictionarySetBatchAsync_FieldsAreStringValuesAreString_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();
        var content = new Dictionary<string, string>() { { field, value } };

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, false, ttlSeconds: 2);
        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, true, ttlSeconds: 10);
        await Task.Delay(2000);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.String());
    }

    [Fact]
    public async Task DictionarySetBatchAsync_NullChecksFieldsAreStringValuesAreByteArray_ThrowsException()
    {
        var dictionaryName = Utils.NewGuidString();
        var dictionary = new Dictionary<string, string>();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(null!, dictionaryName, dictionary, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(cacheName, null!, dictionary, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(cacheName, dictionaryName, (IEnumerable<KeyValuePair<string, byte[]>>)null!, false));

        dictionary[Utils.NewGuidString()] = null!;
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetBatchAsync(cacheName, dictionaryName, dictionary, false));
    }

    [Fact]
    public async Task DictionarySetBatchAsync_FieldsAreStringValuesAreByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidString();
        var value1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidString();
        var value2 = Utils.NewGuidByteArray();

        var items = new Dictionary<string, byte[]>() { { field1, value1 }, { field2, value2 } };

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, items, false, 10);

        var getResponse = await client.DictionaryGetAsync(cacheName, dictionaryName, field1);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
        Assert.Equal(value1, getResponse.ByteArray);

        getResponse = await client.DictionaryGetAsync(cacheName, dictionaryName, field2);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
        Assert.Equal(value2, getResponse.ByteArray);
    }

    [Fact]
    public async Task DictionarySetBatchAsync_FieldsAreStringValuesAreByteArray_NoRefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();
        var content = new Dictionary<string, byte[]>() { { field, value } };

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, false, ttlSeconds: 5);
        await Task.Delay(100);

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, false, ttlSeconds: 10);
        await Task.Delay(4900);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async Task DictionarySetBatchAsync_FieldsAreStringValuesAreByteArray_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();
        var content = new Dictionary<string, byte[]>() { { field, value } };

        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, false, ttlSeconds: 2);
        await client.DictionarySetBatchAsync(cacheName, dictionaryName, content, true, ttlSeconds: 10);
        await Task.Delay(2000);

        var response = await client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.ByteArray);
    }

    [Fact]
    public async Task DictionaryGetBatchAsync_NullChecksFieldsAreByteArray_ThrowsException()
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
    public async Task DictionaryGetBatchAsync_FieldsAreByteArrayValuesAreByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidByteArray();
        var value1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidByteArray();
        var value2 = Utils.NewGuidByteArray();
        var field3 = Utils.NewGuidByteArray();

        await client.DictionarySetAsync(cacheName, dictionaryName, field1, value1, false, 10);
        await client.DictionarySetAsync(cacheName, dictionaryName, field2, value2, false, 10);

        var response = await client.DictionaryGetBatchAsync(cacheName, dictionaryName, new byte[][] { field1, field2, field3 });

        var status = new CacheGetStatus[] { CacheGetStatus.HIT, CacheGetStatus.HIT, CacheGetStatus.MISS };
        var values = new byte[]?[] { value1, value2, null };
        Assert.Equal(status, response.Status);
        Assert.Equal(values, response.ByteArrays);
    }

    [Fact]
    public async Task DictionaryGetBatchAsync_DictionaryMissing_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidByteArray();
        var field3 = Utils.NewGuidByteArray();

        var response = await client.DictionaryGetBatchAsync(cacheName, dictionaryName, new byte[][] { field1, field2, field3 });

        var status = new CacheGetStatus[] { CacheGetStatus.MISS, CacheGetStatus.MISS, CacheGetStatus.MISS };
        var byteArrays = new byte[]?[] { null, null, null };
        var strings = new string?[] { null, null, null };

        Assert.Equal(status, response.Status);
        Assert.Equal(byteArrays, response.ByteArrays);
        Assert.Equal(strings, response.Strings()!);
    }

    [Fact]
    public async Task DictionaryGetBatchAsync_NullChecksFieldsAreString_ThrowsException()
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
    public async Task DictionaryGetBatchAsync_FieldsAreString_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var field2 = Utils.NewGuidString();
        var value2 = Utils.NewGuidString();
        var field3 = Utils.NewGuidString();

        await client.DictionarySetAsync(cacheName, dictionaryName, field1, value1, false, 10);
        await client.DictionarySetAsync(cacheName, dictionaryName, field2, value2, false, 10);

        var response = await client.DictionaryGetBatchAsync(cacheName, dictionaryName, new string[] { field1, field2, field3 });

        var status = new CacheGetStatus[] { CacheGetStatus.HIT, CacheGetStatus.HIT, CacheGetStatus.MISS };
        var values = new string?[] { value1, value2, null };
        Assert.Equal(status, response.Status);
        Assert.Equal(values, response.Strings());
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
        Assert.Null(response.ByteArrayByteArrayDictionary);
        Assert.Null(response.StringStringDictionary());
    }

    [Fact]
    public async Task DictionaryFetchAsync_HasContentStringString_HappyPath()
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

        var fetchResponse = await client.DictionaryFetchAsync(cacheName, dictionaryName);

        Assert.Equal(CacheGetStatus.HIT, fetchResponse.Status);
        Assert.Equal(fetchResponse.StringStringDictionary(), contentDictionary);

        // Test field caching behavior
        Assert.Same(fetchResponse.StringStringDictionary(), fetchResponse.StringStringDictionary());
    }

    [Fact]
    public async Task DictionaryFetchAsync_HasContentStringByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidString();
        var value1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidString();
        var value2 = Utils.NewGuidByteArray();
        var contentDictionary = new Dictionary<string, byte[]>() {
            {field1, value1},
            {field2, value2}
        };

        await client.DictionarySetAsync(cacheName, dictionaryName, field1, value1, true, ttlSeconds: 10);
        await client.DictionarySetAsync(cacheName, dictionaryName, field2, value2, true, ttlSeconds: 10);

        var fetchResponse = await client.DictionaryFetchAsync(cacheName, dictionaryName);

        Assert.Equal(CacheGetStatus.HIT, fetchResponse.Status);
        Assert.Equal(fetchResponse.StringByteArrayDictionary(), contentDictionary);

        // Test field caching behavior
        Assert.Same(fetchResponse.StringByteArrayDictionary(), fetchResponse.StringByteArrayDictionary());
    }

    [Fact]
    public async Task DictionaryFetchAsync_HasContentByteArrayByteArray_HappyPath()
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

        var fetchResponse = await client.DictionaryFetchAsync(cacheName, dictionaryName);

        Assert.Equal(CacheGetStatus.HIT, fetchResponse.Status);

        // Exercise byte array dictionary structural equality comparer
        Assert.True(fetchResponse.ByteArrayByteArrayDictionary!.ContainsKey(field1));
        Assert.True(fetchResponse.ByteArrayByteArrayDictionary!.ContainsKey(field2));
        Assert.Equal(2, fetchResponse.ByteArrayByteArrayDictionary!.Count);

        // Exercise DictionaryEquals extension
        Assert.True(fetchResponse.ByteArrayByteArrayDictionary!.DictionaryEquals(contentDictionary));

        // Test field caching behavior
        Assert.Same(fetchResponse.ByteArrayByteArrayDictionary, fetchResponse.ByteArrayByteArrayDictionary);
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
    public async Task DictionaryRemoveFieldAsync_NullChecksFieldIsByteArray_ThrowsException(string cacheName, string dictionaryName, byte[] field)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field));
    }

    [Theory]
    [InlineData(null, "my-dictionary", "my-field")]
    [InlineData("my-cache", null, "my-field")]
    [InlineData("my-cache", "my-dictionary", null)]
    public async Task DictionaryRemoveFieldAsync_NullChecksFieldIsString_ThrowsException(string cacheName, string dictionaryName, string field)
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
        await client.DictionarySetAsync(cacheName, dictionaryName, field1, value1, false);
        Assert.Equal(CacheGetStatus.HIT, (await client.DictionaryGetAsync(cacheName, dictionaryName, field1)).Status);

        await client.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field1);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, field1)).Status);

        // Test no-op
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, field2)).Status);
        await client.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field2);
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
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, field2)).Status);
        await client.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field2);
        Assert.Equal(CacheGetStatus.MISS, (await client.DictionaryGetAsync(cacheName, dictionaryName, field2)).Status);
    }

    [Fact]
    public async Task DictionaryRemoveFieldsAsync_NullChecksFieldsAreByteArray_ThrowsException()
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
    public async Task DictionaryRemoveFieldsAsync_FieldsAreByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var fields = new byte[][] { Utils.NewGuidByteArray(), Utils.NewGuidByteArray() };
        var otherField = Utils.NewGuidByteArray();

        // Test enumerable
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
    public async Task DictionaryRemoveFieldsAsync_NullChecksFieldsAreString_ThrowsException()
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
    public async Task DictionaryRemoveFieldsAsync_FieldsAreString_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var fields = new string[] { Utils.NewGuidString(), Utils.NewGuidString() };
        var otherField = Utils.NewGuidString();

        // Test enumerable
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
