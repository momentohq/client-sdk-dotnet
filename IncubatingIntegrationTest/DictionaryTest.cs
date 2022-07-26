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

        this.client.DictionarySet(cacheName, dictionaryName, field, value, false, ttlSeconds: 1);
        Thread.Sleep(200);

        this.client.DictionarySet(cacheName, dictionaryName, field, value, false);
        Thread.Sleep(1000);

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
        Thread.Sleep(500);

        this.client.DictionarySet(cacheName, dictionaryName, field, value, true, ttlSeconds: 1);
        Thread.Sleep(600);

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

        this.client.DictionarySet(cacheName, dictionaryName, field, value, false, ttlSeconds: 1);
        Thread.Sleep(200);

        this.client.DictionarySet(cacheName, dictionaryName, field, value, false);
        Thread.Sleep(1000);

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
        Thread.Sleep(500);

        this.client.DictionarySet(cacheName, dictionaryName, field, value, true, ttlSeconds: 1);
        Thread.Sleep(600);

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

        await this.client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 1);
        Thread.Sleep(200);

        await this.client.DictionarySetAsync(cacheName, dictionaryName, field, value, false);
        Thread.Sleep(1000);

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
        Thread.Sleep(500);

        await this.client.DictionarySetAsync(cacheName, dictionaryName, field, value, true, ttlSeconds: 1);
        Thread.Sleep(600);

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

        await this.client.DictionarySetAsync(cacheName, dictionaryName, field, value, false, ttlSeconds: 1);
        Thread.Sleep(200);

        await this.client.DictionarySetAsync(cacheName, dictionaryName, field, value, false);
        Thread.Sleep(1000);

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
        Thread.Sleep(500);

        await this.client.DictionarySetAsync(cacheName, dictionaryName, field, value, true, ttlSeconds: 1);
        Thread.Sleep(600);

        var response = await this.client.DictionaryGetAsync(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.String());
    }
}
