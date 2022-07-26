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

    // String Async

    // Bytes Async
}
