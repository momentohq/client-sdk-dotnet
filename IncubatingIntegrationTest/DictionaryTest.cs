using MomentoSdk.Responses;

namespace IncubatingIntegrationTest;

[Collection("SimpleCacheClient")]
public class DictionaryTest
{
    private readonly SimpleCacheClient client;
    private readonly string cacheName = SimpleCacheClientFixture.CacheName;
    private const uint defaultTtlSeconds = SimpleCacheClientFixture.DefaultTtlSeconds;

    public DictionaryTest(SimpleCacheClientFixture fixture)
    {
        this.client = fixture.Client;
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
        var response = this.client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsBytesValueIsBytes_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();

        var setResponse = this.client.DictionarySet(cacheName, dictionaryName, field, value, false);
        Assert.Equal(setResponse.DictionaryName, dictionaryName);
        Assert.Equal(setResponse.FieldToByteArray(), field);
        Assert.Equal(setResponse.ValueToByteArray(), value);

        var getResponse = this.client.DictionaryGet(cacheName, dictionaryName, field);

        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsBytesDictionaryIsPresent_FieldIsMissing()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();

        this.client.DictionarySet(cacheName, dictionaryName, field, value, false);

        var otherField = Utils.GuidBytes();
        var response = this.client.DictionaryGet(cacheName, dictionaryName, otherField);

        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsBytes_NoRefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();

        this.client.DictionarySet(cacheName, dictionaryName, field, value, false, ttlSeconds: 5);
        Thread.Sleep(100);

        this.client.DictionarySet(cacheName, dictionaryName, field, value, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = this.client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsBytes_RefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();

        this.client.DictionarySet(cacheName, dictionaryName, field, value, false, ttlSeconds: 1);
        Thread.Sleep(100);

        this.client.DictionarySet(cacheName, dictionaryName, field, value, true, ttlSeconds: 10);
        Thread.Sleep(1000);

        var response = this.client.DictionaryGet(cacheName, dictionaryName, field);
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
        var response = this.client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsStringValueIsString_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();

        var setResponse = this.client.DictionarySet(cacheName, dictionaryName, field, value, false);
        Assert.Equal(setResponse.DictionaryName, dictionaryName);
        Assert.Equal(setResponse.FieldToString(), field);
        Assert.Equal(setResponse.ValueToString(), value);

        var getResponse = this.client.DictionaryGet(cacheName, dictionaryName, field);

        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
    }

    [Fact]
    public void DictionarySetGet_DictionaryIsPresent_FieldIsMissing()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();

        this.client.DictionarySet(cacheName, dictionaryName, field, value, false);

        var otherField = Utils.GuidString();
        var response = this.client.DictionaryGet(cacheName, dictionaryName, otherField);

        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsString_NoRefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();

        this.client.DictionarySet(cacheName, dictionaryName, field, value, false, ttlSeconds: 5);
        Thread.Sleep(100);

        this.client.DictionarySet(cacheName, dictionaryName, field, value, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = this.client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsString_RefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();

        this.client.DictionarySet(cacheName, dictionaryName, field, value, false, ttlSeconds: 1);
        Thread.Sleep(100);

        this.client.DictionarySet(cacheName, dictionaryName, field, value, true, ttlSeconds: 10);
        Thread.Sleep(1000);

        var response = this.client.DictionaryGet(cacheName, dictionaryName, field);
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
        var response = await this.client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async void DictionarySetGetAsync_FieldIsBytesValueIsBytes_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();

        var setResponse = await this.client.DictionarySetAsync(cacheName, dictionaryName, field, value, false);
        Assert.Equal(setResponse.DictionaryName, dictionaryName);
        Assert.Equal(setResponse.FieldToByteArray(), field);
        Assert.Equal(setResponse.ValueToByteArray(), value);

        var getResponse = await this.client.DictionaryGetAsync(cacheName, dictionaryName, field);

        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
    }

    [Fact]
    public async void DictionarySetGetAsync_FieldIsBytesDictionaryIsPresent_FieldIsMissing()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();

        await this.client.DictionarySetAsync(cacheName, dictionaryName, field, value, false);

        var otherField = Utils.GuidBytes();
        var response = await this.client.DictionaryGetAsync(cacheName, dictionaryName, otherField);

        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async void DictionarySetGetAsync_FieldIsBytes_NoRefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();

        await this.client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 5);
        Thread.Sleep(100);

        await this.client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = await this.client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async void DictionarySetGetAsync_FieldIsBytes_RefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();

        await this.client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 1);
        Thread.Sleep(100);

        await this.client.DictionarySetAsync(cacheName, dictionaryName, field, value, true, ttlSeconds: 10);
        Thread.Sleep(1000);

        var response = await this.client.DictionaryGetAsync(cacheName, dictionaryName, field);
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
        var response = await this.client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async void DictionarySetGetAsync_FieldIsStringValueIsString_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();

        var setResponse = await this.client.DictionarySetAsync(cacheName, dictionaryName, field, value, false);
        Assert.Equal(setResponse.DictionaryName, dictionaryName);
        Assert.Equal(setResponse.FieldToString(), field);
        Assert.Equal(setResponse.ValueToString(), value);

        var getResponse = await this.client.DictionaryGetAsync(cacheName, dictionaryName, field);

        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
    }

    [Fact]
    public async void DictionarySetGetAsync_DictionaryIsPresent_FieldIsMissing()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();

        await this.client.DictionarySetAsync(cacheName, dictionaryName, field, value, false);

        var otherField = Utils.GuidString();
        var response = await this.client.DictionaryGetAsync(cacheName, dictionaryName, otherField);

        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async void DictionarySetGetAsync_FieldIsString_NoRefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();

        await this.client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 5);
        Thread.Sleep(100);

        await this.client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = await this.client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async void DictionarySetGetAsync_FieldIsString_RefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();

        await this.client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 1);
        Thread.Sleep(100);

        await this.client.DictionarySetAsync(cacheName, dictionaryName, field, value, true, ttlSeconds: 10);
        Thread.Sleep(1000);

        var response = await this.client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.String());
    }

    [Fact]
    public void DictionarySetMulti_NullChecksBytes_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var dictionary = new Dictionary<byte[], byte[]>();
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetMulti(null!, dictionaryName, dictionary, false));
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetMulti(cacheName, null!, dictionary, false));
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetMulti(cacheName, dictionaryName, (IEnumerable<KeyValuePair<byte[], byte[]>>)null!, false));
    }

    [Fact]
    public void DictionarySetMulti_FieldsAreBytes_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidBytes();
        var value1 = Utils.GuidBytes();
        var field2 = Utils.GuidBytes();
        var value2 = Utils.GuidBytes();

        var items = new Dictionary<byte[], byte[]>() { { field1, value1 }, { field2, value2 } };

        var setResponse = this.client.DictionarySetMulti(cacheName, dictionaryName, items, false, 10);

        Assert.Equal(dictionaryName, setResponse.DictionaryName);
        Assert.Equal(items, setResponse.ItemsAsByteArrays());

        var response = this.client.DictionaryGet(cacheName, dictionaryName, field1);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value1, response.Bytes);

        response = this.client.DictionaryGet(cacheName, dictionaryName, field2);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value2, response.Bytes);
    }

    [Fact]
    public void DictionarySetMulti_FieldsAreBytes_NoRefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();
        var content = new Dictionary<byte[], byte[]>() { { field, value } };

        this.client.DictionarySetMulti(cacheName, dictionaryName, content, false, ttlSeconds: 5);
        Thread.Sleep(100);

        this.client.DictionarySetMulti(cacheName, dictionaryName, content, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = this.client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetMulti_FieldsAreBytes_RefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();
        var content = new Dictionary<byte[], byte[]>() { { field, value } };

        this.client.DictionarySetMulti(cacheName, dictionaryName, content, false, ttlSeconds: 1);
        Thread.Sleep(100);

        this.client.DictionarySetMulti(cacheName, dictionaryName, content, true, ttlSeconds: 10);
        Thread.Sleep(1000);

        var response = this.client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.Bytes);
    }

    [Fact]
    public void DictionarySetMulti_NullChecksStrings_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var dictionary = new Dictionary<string, string>();
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetMulti(null!, dictionaryName, dictionary, false));
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetMulti(cacheName, null!, dictionary, false));
        Assert.Throws<ArgumentNullException>(() => client.DictionarySetMulti(cacheName, dictionaryName, (IEnumerable<KeyValuePair<string, string>>)null!, false));
    }

    [Fact]
    public void DictionarySetMulti_FieldsAreStrings_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidString();
        var value1 = Utils.GuidString();
        var field2 = Utils.GuidString();
        var value2 = Utils.GuidString();

        var items = new Dictionary<string, string>() { { field1, value1 }, { field2, value2 } };

        var setResponse = this.client.DictionarySetMulti(cacheName, dictionaryName, items, false, 10);

        Assert.Equal(dictionaryName, setResponse.DictionaryName);
        Assert.Equal(items, setResponse.ItemsAsStrings());

        var getResponse = this.client.DictionaryGet(cacheName, dictionaryName, field1);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
        Assert.Equal(value1, getResponse.String());

        getResponse = this.client.DictionaryGet(cacheName, dictionaryName, field2);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
        Assert.Equal(value2, getResponse.String());
    }

    [Fact]
    public void DictionarySetMulti_FieldsAreStrings_NoRefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();
        var content = new Dictionary<string, string>() { { field, value } };

        this.client.DictionarySetMulti(cacheName, dictionaryName, content, false, ttlSeconds: 5);
        Thread.Sleep(100);

        this.client.DictionarySetMulti(cacheName, dictionaryName, content, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = this.client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetMulti_FieldsAreStrings_RefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();
        var content = new Dictionary<string, string>() { { field, value } };

        this.client.DictionarySetMulti(cacheName, dictionaryName, content, false, ttlSeconds: 1);
        Thread.Sleep(100);

        this.client.DictionarySetMulti(cacheName, dictionaryName, content, true, ttlSeconds: 10);
        Thread.Sleep(1000);

        var response = this.client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.String());
    }

    [Fact]
    public async void DictionarySetMultiAsync_NullChecksBytes_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var dictionary = new Dictionary<byte[], byte[]>();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetMultiAsync(null!, dictionaryName, dictionary, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetMultiAsync(cacheName, null!, dictionary, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetMultiAsync(cacheName, dictionaryName, (IEnumerable<KeyValuePair<byte[], byte[]>>)null!, false));
    }

    [Fact]
    public async void DictionarySetMultiAsync_FieldsAreBytes_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidBytes();
        var value1 = Utils.GuidBytes();
        var field2 = Utils.GuidBytes();
        var value2 = Utils.GuidBytes();

        var items = new Dictionary<byte[], byte[]>() { { field1, value1 }, { field2, value2 } };

        var setResponse = await this.client.DictionarySetMultiAsync(cacheName, dictionaryName, items, false, 10);

        Assert.Equal(dictionaryName, setResponse.DictionaryName);
        Assert.Equal(items, setResponse.ItemsAsByteArrays());

        var getResponse = await this.client.DictionaryGetAsync(cacheName, dictionaryName, field1);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
        Assert.Equal(value1, getResponse.Bytes);

        getResponse = await this.client.DictionaryGetAsync(cacheName, dictionaryName, field2);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
        Assert.Equal(value2, getResponse.Bytes);
    }

    [Fact]
    public async void DictionarySetMultiAsync_FieldsAreBytes_NoRefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();
        var content = new Dictionary<byte[], byte[]>() { { field, value } };

        await this.client.DictionarySetMultiAsync(cacheName, dictionaryName, content, false, ttlSeconds: 5);
        Thread.Sleep(100);

        await this.client.DictionarySetMultiAsync(cacheName, dictionaryName, content, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = await this.client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async void DictionarySetMultiAsync_FieldsAreBytes_RefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidBytes();
        var value = Utils.GuidBytes();
        var content = new Dictionary<byte[], byte[]>() { { field, value } };

        await this.client.DictionarySetMultiAsync(cacheName, dictionaryName, content, false, ttlSeconds: 1);
        Thread.Sleep(100);

        await this.client.DictionarySetMultiAsync(cacheName, dictionaryName, content, true, ttlSeconds: 10);
        Thread.Sleep(1000);

        var response = await this.client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.Bytes);
    }

    [Fact]
    public async void DictionarySetMultiAsync_NullChecksStrings_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var dictionary = new Dictionary<string, string>();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetMultiAsync(null!, dictionaryName, dictionary, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetMultiAsync(cacheName, null!, dictionary, false));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionarySetMultiAsync(cacheName, dictionaryName, (IEnumerable<KeyValuePair<string, string>>)null!, false));
    }

    [Fact]
    public async void DictionarySetMultiAsync_FieldsAreStrings_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidString();
        var value1 = Utils.GuidString();
        var field2 = Utils.GuidString();
        var value2 = Utils.GuidString();

        var items = new Dictionary<string, string>() { { field1, value1 }, { field2, value2 } };

        var setResponse = await this.client.DictionarySetMultiAsync(cacheName, dictionaryName, items, false, 10);

        Assert.Equal(dictionaryName, setResponse.DictionaryName);
        Assert.Equal(items, setResponse.ItemsAsStrings());

        var getResponse = await this.client.DictionaryGetAsync(cacheName, dictionaryName, field1);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
        Assert.Equal(value1, getResponse.String());

        getResponse = await this.client.DictionaryGetAsync(cacheName, dictionaryName, field2);
        Assert.Equal(CacheGetStatus.HIT, getResponse.Status);
        Assert.Equal(value2, getResponse.String());
    }

    [Fact]
    public async void DictionarySetMultiAsync_FieldsAreStrings_NoRefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();
        var content = new Dictionary<string, string>() { { field, value } };

        await this.client.DictionarySetMultiAsync(cacheName, dictionaryName, content, false, ttlSeconds: 5);
        Thread.Sleep(100);

        await this.client.DictionarySetMultiAsync(cacheName, dictionaryName, content, false, ttlSeconds: 10);
        Thread.Sleep(4900);

        var response = await this.client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async void DictionarySetMultiAsync_FieldsAreStrings_RefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();
        var content = new Dictionary<string, string>() { { field, value } };

        await this.client.DictionarySetMultiAsync(cacheName, dictionaryName, content, false, ttlSeconds: 1);
        Thread.Sleep(100);

        await this.client.DictionarySetMultiAsync(cacheName, dictionaryName, content, true, ttlSeconds: 10);
        Thread.Sleep(1000);

        var response = await this.client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.String());
    }

    [Fact]
    public void DictionaryGetMulti_NullChecksByteArrayParams_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var fields = new byte[][] { new byte[] { 0x00 }, new byte[] { 0x00 } };
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetMulti(null!, dictionaryName, fields));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetMulti(cacheName, null!, fields));

        var fieldsList = new List<byte[]>(fields);
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetMulti(null!, dictionaryName, fieldsList));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetMulti(cacheName, null!, fieldsList));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetMulti(cacheName, null!, (List<byte[]>)null!));
    }

    [Fact]
    public void DictionaryGetMulti_FieldsAreBytes_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidBytes();
        var value1 = Utils.GuidBytes();
        var field2 = Utils.GuidBytes();
        var value2 = Utils.GuidBytes();
        var field3 = Utils.GuidBytes();

        this.client.DictionarySet(cacheName, dictionaryName, field1, value1, false, 10);
        this.client.DictionarySet(cacheName, dictionaryName, field2, value2, false, 10);

        var response1 = this.client.DictionaryGetMulti(cacheName, dictionaryName, field1, field2, field3);
        var response2 = this.client.DictionaryGetMulti(cacheName, dictionaryName, new List<byte[]>() { field1, field2, field3 });

        var status = new CacheGetStatus[] { CacheGetStatus.HIT, CacheGetStatus.HIT, CacheGetStatus.MISS };
        var values = new byte[]?[] { value1, value2, null };
        Assert.Equal(status, response1.Status);
        Assert.Equal(status, response2.Status);
        Assert.Equal(values, response1.Bytes);
        Assert.Equal(values, response2.Bytes);
    }

    [Fact]
    public void DictionaryGetMulti_NullChecksStringParams_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var fields = new string[] { Utils.GuidString(), Utils.GuidString() };
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetMulti(null!, dictionaryName, fields));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetMulti(cacheName, null!, fields));

        var fieldsList = new List<string>(fields);
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetMulti(null!, dictionaryName, fieldsList));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetMulti(cacheName, null!, fieldsList));
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetMulti(cacheName, dictionaryName, (List<string>)null!));
    }

    [Fact]
    public void DictionaryGetMulti_FieldsAreStrings_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidString();
        var value1 = Utils.GuidString();
        var field2 = Utils.GuidString();
        var value2 = Utils.GuidString();
        var field3 = Utils.GuidString();

        this.client.DictionarySet(cacheName, dictionaryName, field1, value1, false, 10);
        this.client.DictionarySet(cacheName, dictionaryName, field2, value2, false, 10);

        var response1 = this.client.DictionaryGetMulti(cacheName, dictionaryName, field1, field2, field3);
        var response2 = this.client.DictionaryGetMulti(cacheName, dictionaryName, new string[] { field1, field2, field3 });

        var status = new CacheGetStatus[] { CacheGetStatus.HIT, CacheGetStatus.HIT, CacheGetStatus.MISS };
        var values = new string?[] { value1, value2, null };
        Assert.Equal(status, response1.Status);
        Assert.Equal(status, response2.Status);
        Assert.Equal(values, response1.Strings());
        Assert.Equal(values, response2.Strings());
    }

    [Fact]
    public async void DictionaryGetMultiAsync_NullChecksByteArrayParams_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var fields = new byte[][] { new byte[] { 0x00 }, new byte[] { 0x00 } };
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetMultiAsync(null!, dictionaryName, fields));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetMultiAsync(cacheName, null!, fields));

        var fieldsList = new List<byte[]>(fields);
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetMultiAsync(null!, dictionaryName, fieldsList));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetMultiAsync(cacheName, null!, fieldsList));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetMultiAsync(cacheName, null!, (List<byte[]>)null!));
    }

    [Fact]
    public async void DictionaryGetMultiAsync_FieldsAreBytes_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidBytes();
        var value1 = Utils.GuidBytes();
        var field2 = Utils.GuidBytes();
        var value2 = Utils.GuidBytes();
        var field3 = Utils.GuidBytes();

        await this.client.DictionarySetAsync(cacheName, dictionaryName, field1, value1, false, 10);
        await this.client.DictionarySetAsync(cacheName, dictionaryName, field2, value2, false, 10);

        var response1 = await this.client.DictionaryGetMultiAsync(cacheName, dictionaryName, field1, field2, field3);
        var response2 = await this.client.DictionaryGetMultiAsync(cacheName, dictionaryName, new byte[][] { field1, field2, field3 });

        var status = new CacheGetStatus[] { CacheGetStatus.HIT, CacheGetStatus.HIT, CacheGetStatus.MISS };
        var values = new byte[]?[] { value1, value2, null };
        Assert.Equal(status, response1.Status);
        Assert.Equal(status, response2.Status);
        Assert.Equal(values, response1.Bytes);
        Assert.Equal(values, response2.Bytes);
    }

    [Fact]
    public async void DictionaryGetMultiAsync_NullChecksStringParams_ThrowsException()
    {
        var dictionaryName = Utils.GuidString();
        var fields = new string[] { Utils.GuidString(), Utils.GuidString() };
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetMultiAsync(null!, dictionaryName, fields));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetMultiAsync(cacheName, null!, fields));

        var fieldsList = new List<string>(fields);
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetMultiAsync(null!, dictionaryName, fieldsList));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetMultiAsync(cacheName, null!, fieldsList));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetMultiAsync(cacheName, dictionaryName, (List<string>)null!));
    }

    [Fact]
    public async void DictionaryGetMultiAsync_FieldsAreStrings_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var field1 = Utils.GuidString();
        var value1 = Utils.GuidString();
        var field2 = Utils.GuidString();
        var value2 = Utils.GuidString();
        var field3 = Utils.GuidString();

        await this.client.DictionarySetAsync(cacheName, dictionaryName, field1, value1, false, 10);
        await this.client.DictionarySetAsync(cacheName, dictionaryName, field2, value2, false, 10);

        var response1 = await this.client.DictionaryGetMultiAsync(cacheName, dictionaryName, field1, field2, field3);
        var response2 = await this.client.DictionaryGetMultiAsync(cacheName, dictionaryName, new string[] { field1, field2, field3 });

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
    public void DictionaryGetAll_NullChecks_ThrowsException(string cacheName, string dictionaryName)
    {
        Assert.Throws<ArgumentNullException>(() => client.DictionaryGetAll(cacheName, dictionaryName));
    }

    [Fact]
    public void DictionaryGetAll_Missing_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var response = this.client.DictionaryGetAll(cacheName, dictionaryName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionaryGetAll_HasContentString_HappyPath()
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

        this.client.DictionarySet(cacheName, dictionaryName, field1, value1, true, ttlSeconds: 10);
        this.client.DictionarySet(cacheName, dictionaryName, field2, value2, true, ttlSeconds: 10);

        var getAllResponse = client.DictionaryGetAll(cacheName, dictionaryName);

        Assert.Equal(CacheGetStatus.HIT, getAllResponse.Status);
        Assert.Equal(getAllResponse.StringDictionary(), contentDictionary);
    }

    [Fact]
    public void DictionaryGetAll_HasContentBytes_HappyPath()
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

        this.client.DictionarySet(cacheName, dictionaryName, Utils.Utf8ToBytes(field1), Utils.Utf8ToBytes(value1), true, ttlSeconds: 10);
        this.client.DictionarySet(cacheName, dictionaryName, Utils.Utf8ToBytes(field2), Utils.Utf8ToBytes(value2), true, ttlSeconds: 10);

        var getAllResponse = client.DictionaryGetAll(cacheName, dictionaryName);

        Assert.Equal(CacheGetStatus.HIT, getAllResponse.Status);
        Assert.Equal(getAllResponse.StringDictionary(), contentDictionary);
    }

    [Theory]
    [InlineData(null, "my-dictionary")]
    [InlineData("cache", null)]
    public async void DictionaryGetAllAsync_NullChecks_ThrowsException(string cacheName, string dictionaryName)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await client.DictionaryGetAllAsync(cacheName, dictionaryName));
    }

    [Fact]
    public async void DictionaryGetAllAsync_Missing_HappyPath()
    {
        var dictionaryName = Utils.GuidString();
        var response = await this.client.DictionaryGetAllAsync(cacheName, dictionaryName);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public async void DictionaryGetAllAsync_HasContentString_HappyPath()
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

        await this.client.DictionarySetAsync(cacheName, dictionaryName, field1, value1, true, ttlSeconds: 10);
        await this.client.DictionarySetAsync(cacheName, dictionaryName, field2, value2, true, ttlSeconds: 10);

        var getAllResponse = await client.DictionaryGetAllAsync(cacheName, dictionaryName);

        Assert.Equal(CacheGetStatus.HIT, getAllResponse.Status);
        Assert.Equal(getAllResponse.StringDictionary(), contentDictionary);
    }

    [Fact]
    public async void DictionaryGetAllAsync_HasContentBytes_HappyPath()
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

        await this.client.DictionarySetAsync(cacheName, dictionaryName, Utils.Utf8ToBytes(field1), Utils.Utf8ToBytes(value1), true, ttlSeconds: 10);
        await this.client.DictionarySetAsync(cacheName, dictionaryName, Utils.Utf8ToBytes(field2), Utils.Utf8ToBytes(value2), true, ttlSeconds: 10);

        var getAllResponse = await client.DictionaryGetAllAsync(cacheName, dictionaryName);

        Assert.Equal(CacheGetStatus.HIT, getAllResponse.Status);
        Assert.Equal(getAllResponse.StringDictionary(), contentDictionary);
    }
}
