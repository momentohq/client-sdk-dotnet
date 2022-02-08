using System;
using System.Threading.Tasks;
using MomentoSdk.Exceptions;

namespace MomentoSdk.Responses
{
    public class SimpleCacheClient : IDisposable
    {
        private readonly ScsControlClient controlClient;
        private readonly ScsDataClient dataClient;

        /// <summary>
        /// Client to perform operations against the Simple Cache Service
        /// </summary>
        /// <param name="authToken">Momento jwt</param>
        /// <param name="defaultTtlSeconds">Default Time to Live for the item in Cache</param>
        public SimpleCacheClient(string authToken, uint defaultTtlSeconds)
        {
            Claims claims = JwtUtils.DecodeJwt(authToken);
            string controlEndpoint = "https://" + claims.ControlEndpoint + ":443";
            string cacheEndpoint = "https://" + claims.CacheEndpoint + ":443";
            ScsControlClient controlClient = new ScsControlClient(authToken, controlEndpoint);
            ScsDataClient dataClient = new ScsDataClient(authToken, cacheEndpoint, defaultTtlSeconds);
            this.controlClient = controlClient;
            this.dataClient = dataClient;
        }

        public SimpleCacheClient(string authToken, uint defaultTtlSeconds, uint dataClientOperationTimeoutSeconds)
        {
            ValidateRequestTimeout(dataClientOperationTimeoutSeconds);
            Claims claims = JwtUtils.DecodeJwt(authToken);
            string controlEndpoint = "https://" + claims.ControlEndpoint + ":443";
            string cacheEndpoint = "https://" + claims.CacheEndpoint + ":443";
            ScsControlClient controlClient = new ScsControlClient(authToken, controlEndpoint);
            ScsDataClient dataClient = new ScsDataClient(authToken, cacheEndpoint, defaultTtlSeconds, dataClientOperationTimeoutSeconds);
            this.controlClient = controlClient;
            this.dataClient = dataClient;
        }

        /// <summary>
        /// Creates a cache if it doesn't exist.
        /// </summary>
        /// <param name="cacheName">Name of the cache to be created</param>
        /// <returns>The result of the create cache operation</returns>
        public Responses.CreateCacheResponse CreateCache(string cacheName)
        {
            return this.controlClient.CreateCache(cacheName);
        }

        /// <summary>
        /// Deletes a cache and all of the items within it
        /// </summary>
        /// <param name="cacheName">Name of the cache to be created</param>
        /// <returns>The result of the delete cache operation</returns>
        public Responses.DeleteCacheResponse DeleteCache(string cacheName)
        {
            return this.controlClient.DeleteCache(cacheName);
        }

        /// <summary>
        /// List all caches
        /// </summary>
        /// <param name="nextPageToken">A token to specify where to start paginating. This is the NextToken from a previous response.</param>
        /// <returns>The result of the list cache operation</returns>
        public Responses.ListCachesResponse ListCaches(string nextPageToken = null)
        {
            return this.controlClient.ListCaches(nextPageToken);
        }

        /// <summary>
        /// Sets the value in cache with a given Time To Live (TTL) seconds.
        /// </summary>
        /// <param name="key">The key to get</param>
        /// <param name="value">The value to be stored</param>
        /// <param name="ttlSeconds">Time to Live for the item in Cache. This ttl takes precedence over the TTL used when initializing a cache client</param>
        /// <returns>Future containing the result of the set operation</returns>
        public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value, uint ttlSeconds)
        {
            return await this.dataClient.SetAsync(cacheName, key, value, ttlSeconds);
        }

        /// <summary>
        /// Sets the value in cache with a given Time To Live (TTL) seconds.
        /// </summary>
        /// <param name="key">The key under which the value is to be added</param>
        /// <param name="value">The value to be stored</param>
        /// <returns>Future containing the result of the set operation</returns>
        public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value)
        {
            return await this.dataClient.SetAsync(cacheName, key, value);
        }

        /// <summary>
        /// Get the cache value stored for the given key.
        /// </summary>
        /// <param name="key">The key to perform a cache lookup on</param>
        /// <returns>Future with CacheGetResponse containing the status of the get operation and the associated value data</returns>
        public async Task<CacheGetResponse> GetAsync(string cacheName, byte[] key)
        {
            return await this.dataClient.GetAsync(cacheName, key);
        }

        /// <summary>
        /// Sets the value in cache with a given Time To Live (TTL) seconds.
        /// </summary>
        /// <param name="key">The key under which the value is to be added</param>
        /// <param name="value">The value to be stored</param>
        /// <param name="ttlSeconds">Time to Live for the item in Cache. This ttl takes precedence over the TTL used when initializing a cache client</param>
        /// <returns>Future containing the result of the set operation</returns>
        public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, uint ttlSeconds)
        {
            return await this.dataClient.SetAsync(cacheName, key, value, ttlSeconds);
        }

        /// <summary>
        /// Sets the value in cache with a default Time To Live (TTL) seconds used initializing a cache client
        /// </summary>
        /// <param name="key">The value to be stored</param>
        /// <param name="value">The value to be stored</param>
        /// <returns>Future containing the result of the set operation</returns>
        public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value)
        {
            return await this.dataClient.SetAsync(cacheName, key, value);
        }

        /// <summary>
        /// Get the cache value stored for the given key.
        /// </summary>
        /// <param name="key">The key to perform a cache lookup on</param>
        /// <returns>Future with CacheGetResponse containing the status of the get operation and the associated value data</returns>
        public async Task<CacheGetResponse> GetAsync(string cacheName, string key)
        {
            return await this.dataClient.GetAsync(cacheName, key);
        }

        /// <summary>
        ///  Sets the value in the cache. If a value for this key is already present it will be replaced by the new value.
        /// </summary>
        /// <param name="key">The key under which the value is to be added</param>
        /// <param name="value">The value to be stored</param>
        /// <param name="ttlSeconds">Time to Live for the item in Cache. This ttl takes precedence over the TTL used when initializing a cache client</param>
        /// <returns>Result of the set operation</returns>
        public CacheSetResponse Set(string cacheName, byte[] key, byte[] value, uint ttlSeconds)
        {
            return this.dataClient.Set(cacheName, key, value, ttlSeconds);
        }

        /// <summary>
        /// Sets the value in the cache. If a value for this key is already present it will be replaced by the new value.
        /// </summary>
        /// <param name="key">The key under which the value is to be added</param>
        /// <param name="value">The value to be stored</param>
        /// <returns>Result of the set operation</returns>
        public CacheSetResponse Set(string cacheName, byte[] key, byte[] value)
        {
            return this.dataClient.Set(cacheName, key, value);
        }

        /// <summary>
        /// Get the cache value stored for the given key
        /// </summary>
        /// <param name="key">The key to get</param>
        /// <returns>CacheGetResponse containing the status of the get operation and the associated value data</returns>
        public CacheGetResponse Get(string cacheName, byte[] key)
        {
            return this.dataClient.Get(cacheName, key);
        }

        /// <summary>
        /// Sets the value in cache with a given Time To Live (TTL) seconds. If a value for this key is already present it will be replaced by the new value.
        /// </summary>
        /// <param name="key">The key under which the value is to be added</param>
        /// <param name="value">The value to be stored</param>
        /// <param name="ttlSeconds">Time to Live for the item in Cache. This ttl takes precedence over the TTL used when initializing a cache client</param>
        /// <returns>Result of the set operation</returns>
        public CacheSetResponse Set(string cacheName, string key, string value, uint ttlSeconds)
        {
            return this.dataClient.Set(cacheName, key, value, ttlSeconds);
        }

        /// <summary>
        /// Sets the value in the cache. If a value for this key is already present it will be replaced by the new value.
        /// The Time to Live (TTL) seconds defaults to the parameter used when initializing this Cache client
        /// </summary>
        /// <param name="key">The key under which the value is to be added</param>
        /// <param name="value">The value to be stored</param>
        /// <param name="ttlSeconds">Time to Live for the item in Cache. This ttl takes precedence over the TTL used when initializing a cache client</param>
        /// <returns>Result of the set operation</returns>
        public CacheSetResponse Set(string cacheName, string key, string value)
        {
            return this.dataClient.Set(cacheName, key, value);
        }

        /// <summary>
        /// Get the cache value stored for the given key
        /// </summary>
        /// <param name="key">The key to get</param>
        /// <returns>CacheGetResponse containing the status of the get operation and the associated value data</returns>
        public CacheGetResponse Get(string cacheName, string key)
        {
            return this.dataClient.Get(cacheName, key);
        }

        public void Dispose()
        {
            this.controlClient.Dispose();
            this.dataClient.Dispose();
            GC.SuppressFinalize(this);
        }

        private void ValidateRequestTimeout(uint requestTimeoutSeconds)
        {
            if (requestTimeoutSeconds == 0)
            {
                throw new InvalidArgumentException("Request timeout must be greater than zero.");
            }
        }
    }
}
