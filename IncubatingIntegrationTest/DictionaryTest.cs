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

        this.client.DictionarySet(cacheName, dictionaryName, field, value, false);
        var response = this.client.DictionaryGet(cacheName, dictionaryName, field);

        Assert.Equal(CacheGetStatus.HIT, response.Status);
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

        this.client.DictionarySet(cacheName, dictionaryName, field, value, false, ttlSeconds: 2);
        Thread.Sleep(1500);

        this.client.DictionarySet(cacheName, dictionaryName, field, value, false);
        Thread.Sleep(600);

        var response = this.client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.MISS, response.Status);
    }

    [Fact]
    public void DictionarySetGet_FieldIsString_RefreshTtl()
    {
        var dictionaryName = Utils.GuidString();
        var field = Utils.GuidString();
        var value = Utils.GuidString();

        this.client.DictionarySet(cacheName, dictionaryName, field, value, false, ttlSeconds: 2);
        Thread.Sleep(1500);

        this.client.DictionarySet(cacheName, dictionaryName, field, value, true, ttlSeconds: 2);
        Thread.Sleep(600);

        var response = this.client.DictionaryGet(cacheName, dictionaryName, field);
        Assert.Equal(CacheGetStatus.HIT, response.Status);
        Assert.Equal(value, response.String());
    }

    // Bytes

    // String Async

    // Bytes Async
}
