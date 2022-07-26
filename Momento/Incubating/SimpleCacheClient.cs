using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MomentoSdk.Responses;
using MomentoSdk.Incubating.Internal;
using MomentoSdk.Incubating.Responses;

namespace MomentoSdk.Incubating;

public class SimpleCacheClient : ISimpleCacheClient
{
    private readonly ISimpleCacheClient simpleCacheClient;
    private readonly ScsDataClient dataClient;

    public SimpleCacheClient(ISimpleCacheClient simpleCacheClient, string authToken, uint defaultTtlSeconds, uint? dataClientOperationTimeoutMilliseconds = null)
    {
        this.simpleCacheClient = simpleCacheClient;

        var claims = JwtUtils.DecodeJwt(authToken);
        this.dataClient = new(authToken, claims.CacheEndpoint, defaultTtlSeconds, dataClientOperationTimeoutMilliseconds);
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

    public CacheDictionarySetResponse DictionarySet(string cacheName, string dictionaryName, byte[] field, byte[] value, bool refreshTtl, uint? ttlSeconds = null)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (dictionaryName == null)
        {
            throw new ArgumentNullException(nameof(dictionaryName));
        }
        if (field == null)
        {
            throw new ArgumentNullException(nameof(field));
        }
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return this.dataClient.DictionarySet(cacheName, dictionaryName, field, value, refreshTtl, ttlSeconds);
    }

    public CacheDictionarySetResponse DictionarySet(string cacheName, string dictionaryName, string field, string value, bool refreshTtl, uint? ttlSeconds = null)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (dictionaryName == null)
        {
            throw new ArgumentNullException(nameof(dictionaryName));
        }
        if (field == null)
        {
            throw new ArgumentNullException(nameof(field));
        }
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return this.dataClient.DictionarySet(cacheName, dictionaryName, field, value, refreshTtl, ttlSeconds);
    }

    public async Task<CacheDictionarySetResponse> DictionarySetAsync(string cacheName, string dictionaryName, byte[] field, byte[] value, bool refreshTtl, uint? ttlSeconds = null)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (dictionaryName == null)
        {
            throw new ArgumentNullException(nameof(dictionaryName));
        }
        if (field == null)
        {
            throw new ArgumentNullException(nameof(field));
        }
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return await this.dataClient.DictionarySetAsync(cacheName, dictionaryName, field, value, refreshTtl, ttlSeconds);
    }

    public async Task<CacheDictionarySetResponse> DictionarySetAsync(string cacheName, string dictionaryName, string field, string value, bool refreshTtl, uint? ttlSeconds = null)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (dictionaryName == null)
        {
            throw new ArgumentNullException(nameof(dictionaryName));
        }
        if (field == null)
        {
            throw new ArgumentNullException(nameof(field));
        }
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return await this.dataClient.DictionarySetAsync(cacheName, dictionaryName, field, value, refreshTtl, ttlSeconds);
    }

    public CacheDictionaryGetResponse DictionaryGet(string cacheName, string dictionaryName, byte[] field)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (dictionaryName == null)
        {
            throw new ArgumentNullException(nameof(dictionaryName));
        }
        if (field == null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        return this.dataClient.DictionaryGet(cacheName, dictionaryName, field);
    }

    public CacheDictionaryGetResponse DictionaryGet(string cacheName, string dictionaryName, string field)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (dictionaryName == null)
        {
            throw new ArgumentNullException(nameof(dictionaryName));
        }
        if (field == null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        return this.dataClient.DictionaryGet(cacheName, dictionaryName, field);
    }

    public async Task<CacheDictionaryGetResponse> DictionaryGetAsync(string cacheName, string dictionaryName, byte[] field)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (dictionaryName == null)
        {
            throw new ArgumentNullException(nameof(dictionaryName));
        }
        if (field == null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        return await this.dataClient.DictionaryGetAsync(cacheName, dictionaryName, field);
    }

    public async Task<CacheDictionaryGetResponse> DictionaryGetAsync(string cacheName, string dictionaryName, string field)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (dictionaryName == null)
        {
            throw new ArgumentNullException(nameof(dictionaryName));
        }
        if (field == null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        return await this.dataClient.DictionaryGetAsync(cacheName, dictionaryName, field);
    }

    public CacheDictionarySetMultiResponse DictionarySetMulti(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<byte[], byte[]>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        return this.dataClient.DictionarySetMulti(cacheName, dictionaryName, items, refreshTtl, ttlSeconds);
    }

    public CacheDictionarySetMultiResponse DictionarySetMulti(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, string>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        return this.dataClient.DictionarySetMulti(cacheName, dictionaryName, items, refreshTtl, ttlSeconds);
    }

    public async Task<CacheDictionarySetMultiResponse> DictionarySetMultiAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<byte[], byte[]>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        return await this.dataClient.DictionarySetMultiAsync(cacheName, dictionaryName, items, refreshTtl, ttlSeconds);
    }

    public async Task<CacheDictionarySetMultiResponse> DictionarySetMultiAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, string>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        return await this.dataClient.DictionarySetMultiAsync(cacheName, dictionaryName, items, refreshTtl, ttlSeconds);
    }

    public CacheDictionaryGetMultiResponse DictionaryGetMulti(string cacheName, string dictionaryName, params byte[][] fields)
    {
        return this.dataClient.DictionaryGetMulti(cacheName, dictionaryName, fields);
    }

    public CacheDictionaryGetMultiResponse DictionaryGetMulti(string cacheName, string dictionaryName, params string[] fields)
    {
        return this.dataClient.DictionaryGetMulti(cacheName, dictionaryName, fields);
    }

    public CacheDictionaryGetMultiResponse DictionaryGetMulti(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        return this.dataClient.DictionaryGetMulti(cacheName, dictionaryName, fields);
    }

    public CacheDictionaryGetMultiResponse DictionaryGetMulti(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        return this.dataClient.DictionaryGetMulti(cacheName, dictionaryName, fields);
    }

    public async Task<CacheDictionaryGetMultiResponse> DictionaryGetMultiAsync(string cacheName, string dictionaryName, params byte[][] fields)
    {
        return await this.dataClient.DictionaryGetMultiAsync(cacheName, dictionaryName, fields);
    }

    public async Task<CacheDictionaryGetMultiResponse> DictionaryGetMultiAsync(string cacheName, string dictionaryName, params string[] fields)
    {
        return await this.dataClient.DictionaryGetMultiAsync(cacheName, dictionaryName, fields);
    }

    public async Task<CacheDictionaryGetMultiResponse> DictionaryGetMultiAsync(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        return await this.dataClient.DictionaryGetMultiAsync(cacheName, dictionaryName, fields);
    }

    public async Task<CacheDictionaryGetMultiResponse> DictionaryGetMultiAsync(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        return await this.dataClient.DictionaryGetMultiAsync(cacheName, dictionaryName, fields);
    }

    public CacheDictionaryGetAllResponse DictionaryGetAll(string cacheName, string dictionaryName)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (dictionaryName == null)
        {
            throw new ArgumentNullException(nameof(dictionaryName));
        }

        return this.dataClient.DictionaryGetAll(cacheName, dictionaryName);
    }

    public async Task<CacheDictionaryGetAllResponse> DictionaryGetAllAsync(string cacheName, string dictionaryName)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (dictionaryName == null)
        {
            throw new ArgumentNullException(nameof(dictionaryName));
        }

        return await this.dataClient.DictionaryGetAllAsync(cacheName, dictionaryName);
    }

    public void Dispose()
    {
        this.simpleCacheClient.Dispose();
        this.dataClient.Dispose();
    }
}
