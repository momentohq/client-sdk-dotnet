using System;
using System.Linq;
using System.Threading.Tasks;
using MomentoSdk.Internal;
using MomentoSdk.Responses;
using MomentoSdk.Exceptions;
using System.Collections.Generic;

namespace MomentoSdk;

public class SimpleCacheClient : ISimpleCacheClient
{
    private readonly ScsControlClient controlClient;
    private readonly ScsDataClient dataClient;

    /// <summary>
    /// Client to perform operations against the Simple Cache Service.
    /// </summary>
    /// <param name="authToken">Momento JWT.</param>
    /// <param name="defaultTtlSeconds">Default time to live for the item in cache.</param>
    /// <param name="dataClientOperationTimeoutMilliseconds">Deadline (timeout) for communicating to the server. Defaults to 5 seconds.</param>
    public SimpleCacheClient(string authToken, uint defaultTtlSeconds, uint? dataClientOperationTimeoutMilliseconds = null)
    {
        ValidateRequestTimeout(dataClientOperationTimeoutMilliseconds);
        Claims claims = JwtUtils.DecodeJwt(authToken);

        this.controlClient = new(authToken, claims.ControlEndpoint);
        this.dataClient = new(authToken, claims.CacheEndpoint, defaultTtlSeconds, dataClientOperationTimeoutMilliseconds);
    }

    public CreateCacheResponse CreateCache(string cacheName)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        return this.controlClient.CreateCache(cacheName);
    }

    public DeleteCacheResponse DeleteCache(string cacheName)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        return this.controlClient.DeleteCache(cacheName);
    }

    public ListCachesResponse ListCaches(string? nextPageToken = null)
    {
        return this.controlClient.ListCaches(nextPageToken);
    }

    public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value, uint? ttlSeconds = null)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return await this.dataClient.SetAsync(cacheName, key, value, ttlSeconds);
    }

    public async Task<CacheGetResponse> GetAsync(string cacheName, byte[] key)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        return await this.dataClient.GetAsync(cacheName, key);
    }

    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, byte[] key)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        return await this.dataClient.DeleteAsync(cacheName, key);
    }

    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, uint? ttlSeconds = null)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return await this.dataClient.SetAsync(cacheName, key, value, ttlSeconds);
    }

    public async Task<CacheGetResponse> GetAsync(string cacheName, string key)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        return await this.dataClient.GetAsync(cacheName, key);
    }

    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, string key)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        return await this.dataClient.DeleteAsync(cacheName, key);
    }

    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, byte[] value, uint? ttlSeconds = null)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return await this.dataClient.SetAsync(cacheName, key, value, ttlSeconds);
    }

    public async Task<CacheGetMultiResponse> GetMultiAsync(string cacheName, IEnumerable<byte[]> keys)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (keys == null)
        {
            throw new ArgumentNullException(nameof(keys));
        }
        if (keys.Any(key => key == null))
        {
            throw new ArgumentNullException(nameof(keys), "Each key must be non-null");
        }

        return await this.dataClient.GetMultiAsync(cacheName, keys);
    }

    public async Task<CacheGetMultiResponse> GetMultiAsync(string cacheName, IEnumerable<string> keys)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (keys == null)
        {
            throw new ArgumentNullException(nameof(keys));
        }
        if (keys.Any(key => key == null))
        {
            throw new ArgumentNullException(nameof(keys), "Each key must be non-null");
        }


        return await this.dataClient.GetMultiAsync(cacheName, keys);
    }

    public async Task<CacheGetMultiResponse> GetMultiAsync(string cacheName, params byte[][] keys)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (keys == null)
        {
            throw new ArgumentNullException(nameof(keys));
        }
        if (keys.Any(key => key == null))
        {
            throw new ArgumentNullException(nameof(keys), "Each key must be non-null");
        }

        return await this.dataClient.GetMultiAsync(cacheName, keys);
    }

    public async Task<CacheGetMultiResponse> GetMultiAsync(string cacheName, params string[] keys)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (keys == null || keys.Any(key => key == null))
        {
            throw new ArgumentNullException(nameof(keys));
        }
        if (keys.Any(key => key == null))
        {
            throw new ArgumentNullException(nameof(keys), "Each key must be non-null");
        }

        return await this.dataClient.GetMultiAsync(cacheName, keys);
    }

    public async Task<CacheSetMultiResponse> SetMultiAsync(string cacheName, IDictionary<byte[], byte[]> items, uint? ttlSeconds = null)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }
        if (items.Values.Any(value => value == null))
        {
            throw new ArgumentNullException(nameof(items), "Each value must be non-null");
        }

        await this.dataClient.SetMultiAsync(cacheName, items, ttlSeconds);
        return new CacheSetMultiResponse(items);
    }

    public async Task<CacheSetMultiResponse> SetMultiAsync(string cacheName, IDictionary<string, string> items, uint? ttlSeconds = null)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }
        if (items.Values.Any(value => value == null))
        {
            throw new ArgumentNullException(nameof(items), "Each value must be non-null");
        }

        await this.dataClient.SetMultiAsync(cacheName, items, ttlSeconds);
        return new CacheSetMultiResponse(items);
    }

    /// <summary>
    ///  Sets the value in the cache. If a value for this key is already present it will be replaced by the new value.
    /// </summary>
    /// <param name="cacheName">Name of the cache to store the item in.</param>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to be stored.</param>
    /// <param name="ttlSeconds">Time to live (TTL) for the item in Cache. This TTL takes precedence over the TTL used when initializing a cache client. Defaults to client TTL.</param>
    /// <returns>Result of the set operation</returns>
    public CacheSetResponse Set(string cacheName, byte[] key, byte[] value, uint? ttlSeconds = null)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return this.dataClient.Set(cacheName, key, value, ttlSeconds);
    }

    public CacheGetResponse Get(string cacheName, byte[] key)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        return this.dataClient.Get(cacheName, key);
    }

    public CacheDeleteResponse Delete(string cacheName, byte[] key)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        return this.dataClient.Delete(cacheName, key);
    }

    public CacheSetResponse Set(string cacheName, string key, string value, uint? ttlSeconds = null)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return this.dataClient.Set(cacheName, key, value, ttlSeconds);
    }

    public CacheGetResponse Get(string cacheName, string key)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        return this.dataClient.Get(cacheName, key);
    }

    public CacheGetMultiResponse GetMulti(string cacheName, IEnumerable<byte[]> keys)
    {
        try
        {
            return GetMultiAsync(cacheName, keys).Result;
        }
        catch (AggregateException e)
        {
            throw e.GetBaseException();
        }
    }

    public CacheGetMultiResponse GetMulti(string cacheName, IEnumerable<string> keys)
    {
        try
        {
            return GetMultiAsync(cacheName, keys).Result;
        }
        catch (AggregateException e)
        {
            throw e.GetBaseException();
        }
    }

    public CacheGetMultiResponse GetMulti(string cacheName, params byte[][] keys)
    {
        try
        {
            return GetMultiAsync(cacheName, keys).Result;
        }
        catch (AggregateException e)
        {
            throw e.GetBaseException();
        }
    }

    public CacheGetMultiResponse GetMulti(string cacheName, params string[] keys)
    {
        try
        {
            return GetMultiAsync(cacheName, keys).Result;
        }
        catch (AggregateException e)
        {
            throw e.GetBaseException();
        }
    }

    public CacheSetMultiResponse SetMulti(string cacheName, IDictionary<byte[], byte[]> items, uint? ttlSeconds = null)
    {
        try
        {
            return SetMultiAsync(cacheName, items, ttlSeconds).Result;
        }
        catch (AggregateException e)
        {
            throw e.GetBaseException();
        }
    }

    public CacheSetMultiResponse SetMulti(string cacheName, IDictionary<string, string> items, uint? ttlSeconds = null)
    {
        try
        {
            return SetMultiAsync(cacheName, items, ttlSeconds).Result;
        }
        catch (AggregateException e)
        {
            throw e.GetBaseException();
        }
    }

    public CacheDeleteResponse Delete(string cacheName, string key)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        return this.dataClient.Delete(cacheName, key);
    }

    public CacheSetResponse Set(string cacheName, string key, byte[] value, uint? ttlSeconds = null)
    {
        if (cacheName == null)
        {
            throw new ArgumentNullException(nameof(cacheName));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        if (value == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        return this.dataClient.Set(cacheName, key, value, ttlSeconds);
    }

    public void Dispose()
    {
        this.controlClient.Dispose();
        this.dataClient.Dispose();
        GC.SuppressFinalize(this);
    }

    private void ValidateRequestTimeout(uint? requestTimeoutMilliseconds = null)
    {
        if (requestTimeoutMilliseconds == null)
        {
            return;
        }
        if (requestTimeoutMilliseconds == 0)
        {
            throw new InvalidArgumentException("Request timeout must be greater than zero.");
        }
    }
}
