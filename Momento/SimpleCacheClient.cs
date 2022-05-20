﻿using System;
using System.Threading.Tasks;
using MomentoSdk.Exceptions;
using System.Collections.Generic;

namespace MomentoSdk.Responses
{
    public class SimpleCacheClient : IDisposable
    {
        private readonly ScsControlClient controlClient;
        private readonly ScsDataClient dataClient;

        /// <summary>
        /// Client to perform operations against the Simple Cache Service.
        /// </summary>
        /// <param name="authToken">Momento JWT.</param>
        /// <param name="defaultTtlSeconds">Default time to live for the item in cache.</param>
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

        /// <summary>
        /// Client to perform operations against the Simple Cache Service.
        /// </summary>
        /// <param name="authToken">Momento JWT.</param>
        /// <param name="defaultTtlSeconds">Default time to live for the item in cache.</param>
        /// <param name="dataClientOperationTimeoutMilliseconds">Deadline (timeout) for communicating to the server.</param>
        public SimpleCacheClient(string authToken, uint defaultTtlSeconds, uint dataClientOperationTimeoutMilliseconds)
        {
            ValidateRequestTimeout(dataClientOperationTimeoutMilliseconds);
            Claims claims = JwtUtils.DecodeJwt(authToken);
            string controlEndpoint = "https://" + claims.ControlEndpoint + ":443";
            string cacheEndpoint = "https://" + claims.CacheEndpoint + ":443";
            ScsControlClient controlClient = new ScsControlClient(authToken, controlEndpoint);
            ScsDataClient dataClient = new ScsDataClient(authToken, cacheEndpoint, defaultTtlSeconds, dataClientOperationTimeoutMilliseconds);
            this.controlClient = controlClient;
            this.dataClient = dataClient;
        }

        /// <summary>
        /// Creates a cache if it doesn't exist.
        /// </summary>
        /// <param name="cacheName">Name of the cache to be created.</param>
        /// <returns>The result of the create cache operation</returns>
        public Responses.CreateCacheResponse CreateCache(string cacheName)
        {
            if (cacheName == null)
            {
                throw new ArgumentNullException(nameof(cacheName));
            }
            return this.controlClient.CreateCache(cacheName);
        }

        /// <summary>
        /// Deletes a cache and all of the items within it.
        /// </summary>
        /// <param name="cacheName">Name of the cache to be deleted.</param>
        /// <returns>The result of the delete cache operation.</returns>
        public Responses.DeleteCacheResponse DeleteCache(string cacheName)
        {
            if (cacheName == null)
            {
                throw new ArgumentNullException(nameof(cacheName));
            }
            return this.controlClient.DeleteCache(cacheName);
        }

        /// <summary>
        /// List all caches.
        /// </summary>
        /// <param name="nextPageToken">A token to specify where to start paginating. This is the NextToken from a previous response.</param>
        /// <returns>The result of the list cache operation.</returns>
        public Responses.ListCachesResponse ListCaches(string nextPageToken = null)
        {
            return this.controlClient.ListCaches(nextPageToken);
        }

        /// <summary>
        /// Sets the value in cache with a given time to live (TTL) seconds.
        /// </summary>
        /// <param name="cacheName">Name of the cache to store the item in.</param>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The value to be stored.</param>
        /// <param name="ttlSeconds">TTL for the item in cache. This TTL takes precedence over the TTL used when initializing a cache client.</param>
        /// <returns>Future containing the result of the set operation.</returns>
        public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value, uint ttlSeconds)
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

        /// <summary>
        /// Sets the value in cache with a given time to live (TTL) seconds.
        /// </summary>
        /// <param name="cacheName">Name of the cache to store the item in.</param>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The value to be stored.</param>
        /// <returns>Future containing the result of the set operation.</returns>
        public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value)
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

            return await this.dataClient.SetAsync(cacheName, key, value);
        }

        /// <summary>
        /// Get the cache value stored for the given key.
        /// </summary>
        /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
        /// <param name="key">The key to lookup.</param>
        /// <returns>Future with CacheGetResponse containing the status of the get operation and the associated value data.</returns>
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

        /// <summary>
        /// Remove the key from the cache.
        /// </summary>
        /// <param name="cacheName">Name of the cache to delete the key from.</param>
        /// <param name="key">The key to delete.</param>
        /// <returns>Future containing the result of the delete operation.</returns>
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

        /// <summary>
        /// Sets the value in cache with a given time to live (TTL) seconds.
        /// </summary>
        /// <param name="cacheName">Name of the cache to store the item in.</param>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The value to be stored.</param>
        /// <param name="ttlSeconds">TTL for the item in cache. This TTL takes precedence over the TTL used when initializing a cache client.</param>
        /// <returns>Future containing the result of the set operation</returns>
        public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, uint ttlSeconds)
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

        /// <summary>
        /// Sets the value in cache with a default time to live (TTL) seconds used initializing a cache client.
        /// </summary>
        /// <param name="cacheName">Name of the cache to store the item in.</param>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The value to be stored.</param>
        /// <returns>Future containing the result of the set operation.</returns>
        public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value)
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

            return await this.dataClient.SetAsync(cacheName, key, value);
        }

        /// <summary>
        /// Get the cache value stored for the given key.
        /// </summary>
        /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
        /// <param name="key">The key to lookup.</param>
        /// <returns>Future with CacheGetResponse containing the status of the get operation and the associated value data</returns>
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

        /// <summary>
        /// Remove the key from the cache.
        /// </summary>
        /// <param name="cacheName">Name of the cache to delete the key from.</param>
        /// <param name="key">The key to delete.</param>
        /// <returns>Future containing the result of the delete operation.</returns>
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

        /// <summary>
        /// Sets the value in cache with a given time to live (TTL) seconds.
        /// </summary>
        /// <param name="cacheName">Name of the cache to store the item in.</param>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The value to be stored.</param>
        /// <param name="ttlSeconds">TTL for the item in cache. This TTL takes precedence over the TTL used when initializing a cache client</param>
        /// <returns>Future containing the result of the set operation</returns>
        public async Task<CacheSetResponse> SetAsync(string cacheName, string key, byte[] value, uint ttlSeconds)
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

        /// <summary>
        /// Sets the value in cache with a default time to live (TTL) seconds used initializing a cache client
        /// </summary>
        /// <param name="cacheName">Name of the cache to store the item in.</param>
        /// <param name="key">The value to be stored.</param>
        /// <param name="value">The value to be stored.</param>
        /// <returns>Future containing the result of the set operation.</returns>
        public async Task<CacheSetResponse> SetAsync(string cacheName, string key, byte[] value)
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

            return await this.dataClient.SetAsync(cacheName, key, value);
        }

        /// <summary>
        /// Executes a list of passed Get operations in parallel.
        /// </summary>
        /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
        /// <param name="keys">The keys to get.</param>
        /// <returns>Future with CacheMultiGetResponse containing the status of the get operation and the associated value data.</returns>
        public async Task<CacheMultiGetResponse> MultiGetAsync(string cacheName, List<byte[]> keys)
        {
	    if (cacheName == null)
            {
                throw new ArgumentNullException(nameof(cacheName));
            }
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            return await this.dataClient.MultiGetAsync(cacheName, keys);
        }

        /// <summary>
        /// Executes a list of passed Get operations in parallel.
        /// </summary>
        /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
        /// <param name="keys">The keys to get.</param>
        /// <returns>Future with CacheMultiGetResponse containing the status of the get operation and the associated value data.</returns>
        public async Task<CacheMultiGetResponse> MultiGetAsync(string cacheName, List<string> keys)
        {
	    if (cacheName == null)
            {
                throw new ArgumentNullException(nameof(cacheName));
            }
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            return await this.dataClient.MultiGetAsync(cacheName, keys);
        }

        /// <summary>
        /// Executes a list of passed Get operations in parallel.
        /// </summary>
        /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
        /// <param name="failureResponses">Failed responses to perform a cache lookup on.</param>
        /// <returns>Future with CacheMultiGetResponse containing the status of the get operation and the associated value data.</returns>
        public async Task<CacheMultiGetResponse> MultiGetAsync(string cacheName, List<CacheMultiGetFailureResponse> failureResponses)
        {
	    if (cacheName == null)
            {
                throw new ArgumentNullException(nameof(cacheName));
            }
            if (failureResponses == null)
            {
                throw new ArgumentNullException(nameof(failureResponses));
            }

            return await this.dataClient.MultiGetAsync(cacheName, failureResponses);
        }

        /// <summary>
        ///  Sets the value in the cache. If a value for this key is already present it will be replaced by the new value.
        /// </summary>
        /// <param name="cacheName">Name of the cache to store the item in.</param>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The value to be stored.</param>
        /// <param name="ttlSeconds">Time to live (TTL) for the item in Cache. This TTL takes precedence over the TTL used when initializing a cache client.</param>
        /// <returns>Result of the set operation</returns>
        public CacheSetResponse Set(string cacheName, byte[] key, byte[] value, uint ttlSeconds)
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

        /// <summary>
        /// Sets the value in the cache. If a value for this key is already present it will be replaced by the new value.
        /// </summary>
        /// <param name="cacheName">Name of the cache to store the item in.</param>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The value to be stored.</param>
        /// <returns>Result of the set operation.</returns>
        public CacheSetResponse Set(string cacheName, byte[] key, byte[] value)
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

	    return this.dataClient.Set(cacheName, key, value);
        }

        /// <summary>
        /// Get the cache value stored for the given key.
        /// </summary>
        /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
        /// <param name="key">The key to lookup.</param>
        /// <returns>CacheGetResponse containing the status of the get operation and the associated value data.</returns>
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

        /// <summary>
        /// Remove the key from the cache.
        /// </summary>
        /// <param name="cacheName">Name of the cache to delete the key from.</param>
        /// <param name="key">The key to delete.</param>
        /// <returns>Future containing the result of the delete operation.</returns>
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

        /// <summary>
        /// Sets the value in cache with a given time to live (TTL) seconds. If a value for this key is already present it will be replaced by the new value.
        /// </summary>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The value to be stored.</param>
        /// <param name="ttlSeconds">TTL for the item in cache. This TTL takes precedence over the TTL used when initializing a cache client.</param>
        /// <returns>Result of the set operation.</returns>
        public CacheSetResponse Set(string cacheName, string key, string value, uint ttlSeconds)
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

        /// <summary>
        /// Sets the value in the cache. If a value for this key is already present it will be replaced by the new value.
        /// The time to live (TTL) seconds defaults to the parameter used when initializing this cache client.
        /// </summary>
        /// <param name="cacheName">Name of the cache to store the item in.</param>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The value to be stored.</param>
        /// <param name="ttlSeconds">TTL for the item in Cache. This TTL takes precedence over the TTL used when initializing a cache client.</param>
        /// <returns>Result of the set operation</returns>
        public CacheSetResponse Set(string cacheName, string key, string value)
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

            return this.dataClient.Set(cacheName, key, value);
        }

        /// <summary>
        /// Get the cache value stored for the given key.
        /// </summary>
        /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
        /// <param name="key">The key to lookup.</param>
        /// <returns>CacheGetResponse containing the status of the get operation and the associated value data.</returns>
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

        /// <summary>
        /// Remove the key from the cache.
        /// </summary>
        /// <param name="cacheName">Name of the cache to delete the key from.</param>
        /// <param name="key">The key to delete.</param>
        /// <returns>Future containing the result of the delete operation.</returns>
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

        /// <summary>
        /// Sets the value in cache with a given time to live (TTL) seconds. If a value for this key is already present it will be replaced by the new value.
        /// </summary>
        /// <param name="cacheName">Name of the cache to store the item in.</param>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The value to be stored.</param>
        /// <param name="ttlSeconds">TTL for the item in cache. This TTL takes precedence over the TTL used when initializing a cache client.</param>
        /// <returns>Result of the set operation</returns>
        public CacheSetResponse Set(string cacheName, string key, byte[] value, uint ttlSeconds)
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

        /// <summary>
        /// Sets the value in the cache. If a value for this key is already present it will be replaced by the new value.
        /// The time to live (TTL) seconds defaults to the parameter used when initializing this cache client.
        /// </summary>
        /// <param name="cacheName">Name of the cache to store the item in.</param>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The value to be stored.</param>
        /// <param name="ttlSeconds">TTL for the item in cache. This TTL takes precedence over the TTL used when initializing a cache client.</param>
        /// <returns>Result of the set operation</returns>
        public CacheSetResponse Set(string cacheName, string key, byte[] value)
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

            return this.dataClient.Set(cacheName, key, value);
        }

        public void Dispose()
        {
            this.controlClient.Dispose();
            this.dataClient.Dispose();
            GC.SuppressFinalize(this);
        }

        private void ValidateRequestTimeout(uint requestTimeoutMilliseconds)
        {
            if (requestTimeoutMilliseconds == 0)
            {
                throw new InvalidArgumentException("Request timeout must be greater than zero.");
            }
        }
    }
}
