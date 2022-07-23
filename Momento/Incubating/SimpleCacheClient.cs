using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MomentoSdk.Responses;
using MomentoSdk.Incubating.Responses;

namespace MomentoSdk.Incubating;

public class SimpleCacheClient : ISimpleCacheClient
{
    private readonly ISimpleCacheClient simpleCacheClient;

    public SimpleCacheClient(ISimpleCacheClient simpleCacheClient)
    {
        this.simpleCacheClient = simpleCacheClient;
    }

    public CreateCacheResponse CreateCache(string cacheName)
    {
        return this.simpleCacheClient.CreateCache(cacheName);
    }

    public DeleteCacheResponse DeleteCache(string cacheName)
    {
        return this.simpleCacheClient.DeleteCache(cacheName);
    }

    public ListCachesResponse ListCaches(string? nextPageToken = null)
    {
        return this.simpleCacheClient.ListCaches(nextPageToken);
    }

    public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value, uint? ttlSeconds = null)
    {
        return await this.simpleCacheClient.SetAsync(cacheName, key, value, ttlSeconds);
    }

    public async Task<CacheGetResponse> GetAsync(string cacheName, byte[] key)
    {
        return await this.simpleCacheClient.GetAsync(cacheName, key);
    }

    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, byte[] key)
    {
        return await this.simpleCacheClient.DeleteAsync(cacheName, key);
    }

    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, uint? ttlSeconds = null)
    {
        return await simpleCacheClient.SetAsync(cacheName, key, value, ttlSeconds);
    }

    public async Task<CacheGetResponse> GetAsync(string cacheName, string key)
    {
        return await this.simpleCacheClient.GetAsync(cacheName, key);
    }

    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, string key)
    {
        return await this.simpleCacheClient.DeleteAsync(cacheName, key);
    }

    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, byte[] value, uint? ttlSeconds = null)
    {
        return await this.simpleCacheClient.SetAsync(cacheName, key, value, ttlSeconds);
    }

    public async Task<CacheGetMultiResponse> GetMultiAsync(string cacheName, IEnumerable<byte[]> keys)
    {
        return await this.simpleCacheClient.GetMultiAsync(cacheName, keys);
    }

    public async Task<CacheGetMultiResponse> GetMultiAsync(string cacheName, IEnumerable<string> keys)
    {
        return await this.simpleCacheClient.GetMultiAsync(cacheName, keys);
    }

    public async Task<CacheGetMultiResponse> GetMultiAsync(string cacheName, params byte[][] keys)
    {
        return await this.simpleCacheClient.GetMultiAsync(cacheName, keys);
    }

    public async Task<CacheGetMultiResponse> GetMultiAsync(string cacheName, params string[] keys)
    {
        return await this.simpleCacheClient.GetMultiAsync(cacheName, keys);
    }

    public async Task<CacheSetMultiResponse> SetMultiAsync(string cacheName, IDictionary<byte[], byte[]> items, uint? ttlSeconds = null)
    {
        return await this.simpleCacheClient.SetMultiAsync(cacheName, items, ttlSeconds);
    }

    public async Task<CacheSetMultiResponse> SetMultiAsync(string cacheName, IDictionary<string, string> items, uint? ttlSeconds = null)
    {
        return await this.simpleCacheClient.SetMultiAsync(cacheName, items, ttlSeconds);
    }

    public CacheSetResponse Set(string cacheName, byte[] key, byte[] value, uint? ttlSeconds = null)
    {
        return this.simpleCacheClient.Set(cacheName, key, value, ttlSeconds);
    }

    public CacheGetResponse Get(string cacheName, byte[] key)
    {
        return this.simpleCacheClient.Get(cacheName, key);
    }

    public CacheDeleteResponse Delete(string cacheName, byte[] key)
    {
        return this.simpleCacheClient.Delete(cacheName, key);
    }

    public CacheSetResponse Set(string cacheName, string key, string value, uint? ttlSeconds = null)
    {
        return this.simpleCacheClient.Set(cacheName, key, value, ttlSeconds);
    }

    public CacheGetResponse Get(string cacheName, string key)
    {
        return this.simpleCacheClient.Get(cacheName, key);
    }

    public CacheGetMultiResponse GetMulti(string cacheName, IEnumerable<byte[]> keys)
    {
        return this.simpleCacheClient.GetMulti(cacheName, keys);
    }

    public CacheGetMultiResponse GetMulti(string cacheName, IEnumerable<string> keys)
    {
        return this.simpleCacheClient.GetMulti(cacheName, keys);
    }

    public CacheGetMultiResponse GetMulti(string cacheName, params byte[][] keys)
    {
        return this.simpleCacheClient.GetMulti(cacheName, keys);
    }

    public CacheGetMultiResponse GetMulti(string cacheName, params string[] keys)
    {
        return this.simpleCacheClient.GetMulti(cacheName, keys);
    }

    public CacheSetMultiResponse SetMulti(string cacheName, IDictionary<byte[], byte[]> items, uint? ttlSeconds = null)
    {
        return this.simpleCacheClient.SetMulti(cacheName, items, ttlSeconds);
    }

    public CacheSetMultiResponse SetMulti(string cacheName, IDictionary<string, string> items, uint? ttlSeconds = null)
    {
        return this.simpleCacheClient.SetMulti(cacheName, items, ttlSeconds);
    }

    public CacheDeleteResponse Delete(string cacheName, string key)
    {
        return this.simpleCacheClient.Delete(cacheName, key);
    }

    public CacheSetResponse Set(string cacheName, string key, byte[] value, uint? ttlSeconds = null)
    {
        return this.simpleCacheClient.Set(cacheName, key, value, ttlSeconds);
    }

    public CacheDictionarySetResponse DictionarySet(string cacheName, string dictionaryName, byte[] key, byte[] value, bool refreshTtl, uint? ttlSeconds = null)
    {
        throw new NotImplementedException();
    }

    public CacheDictionarySetResponse DictionarySet(string cacheName, string dictionaryName, string key, string value, bool refreshTtl, uint? ttlSeconds = null)
    {
        throw new NotImplementedException();
    }

    public async Task<CacheDictionarySetResponse> DictionarySetAsync(string cacheName, string dictionaryName, byte[] key, byte[] value, bool refreshTtl, uint? ttlSeconds = null)
    {
        return await Task.FromException<CacheDictionarySetResponse>(new NotImplementedException());
    }

    public async Task<CacheDictionarySetResponse> DictionarySetAsync(string cacheName, string dictionaryName, string key, string value, bool refreshTtl, uint? ttlSeconds = null)
    {
        return await Task.FromException<CacheDictionarySetResponse>(new NotImplementedException());
    }

    public CacheDictionaryGetResponse DictionaryGet(string cacheName, string dictionaryName, byte[] key)
    {
        throw new NotImplementedException();
    }

    public CacheDictionaryGetResponse DictionaryGet(string cacheName, string dictionaryName, string key)
    {
        throw new NotImplementedException();
    }

    public async Task<CacheDictionaryGetResponse> DictionaryGetAsync(string cacheName, string dictionaryName, byte[] key)
    {
        return await Task.FromException<CacheDictionaryGetResponse>(new NotImplementedException());
    }

    public async Task<CacheDictionaryGetResponse> DictionaryGetAsync(string cacheName, string dictionaryName, string key)
    {
        return await Task.FromException<CacheDictionaryGetResponse>(new NotImplementedException());
    }

    public CacheDictionarySetMultiResponse DictionarySetMulti(string cacheName, string dictionaryName, IDictionary<byte[], byte[]> dictionary, bool refreshTtl, uint? ttlSeconds = null)
    {
        throw new NotImplementedException();
    }

    public CacheDictionarySetMultiResponse DictionarySetMulti(string cacheName, string dictionaryName, IDictionary<string, string> dictionary, bool refreshTtl, uint? ttlSeconds = null)
    {
        throw new NotImplementedException();
    }

    public async Task<CacheDictionarySetMultiResponse> DictionarySetMultiAsync(string cacheName, string dictionaryName, IDictionary<string, string> dictionary, bool refreshTtl, uint? ttlSeconds = null)
    {
        return await Task.FromException<CacheDictionarySetMultiResponse>(new NotImplementedException());
    }

    public async Task<CacheDictionarySetMultiResponse> DictionarySetMultiAsync(string cacheName, string dictionaryName, IDictionary<byte[], byte[]> dictionary, bool refreshTtl, uint? ttlSeconds = null)
    {
        return await Task.FromException<CacheDictionarySetMultiResponse>(new NotImplementedException());
    }

    public CacheDictionaryGetMultiResponse DictionaryGetMulti(string cacheName, string dictionaryName, params byte[][] keys)
    {
        throw new NotImplementedException();
    }

    public CacheDictionaryGetMultiResponse DictionaryGetMulti(string cacheName, string dictionaryName, params string[] keys)
    {
        throw new NotImplementedException();
    }

    public async Task<CacheDictionaryGetMultiResponse> DictionaryGetMultiAsync(string cacheName, string dictionaryName, params byte[][] keys)
    {
        return await Task.FromException<CacheDictionaryGetMultiResponse>(new NotImplementedException());
    }

    public async Task<CacheDictionaryGetMultiResponse> DictionaryGetMultiAsync(string cacheName, string dictionaryName, params string[] keys)
    {
        return await Task.FromException<CacheDictionaryGetMultiResponse>(new NotImplementedException());
    }

    public CacheDictionaryGetAllResponse DictionaryGetAll(string cacheName, string dictionaryName)
    {
        throw new NotImplementedException();
    }

    public async Task<CacheDictionaryGetAllResponse> DictionaryGetAllAsync(string cacheName, string dictionaryName)
    {
        return await Task.FromException<CacheDictionaryGetAllResponse>(new NotImplementedException());
    }

    public void Dispose()
    {
        this.simpleCacheClient.Dispose();
    }
}
