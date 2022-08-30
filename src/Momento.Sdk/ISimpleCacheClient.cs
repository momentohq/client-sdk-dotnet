using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Momento.Sdk.Responses;

namespace Momento.Sdk;

/// <summary>
/// Minimum viable functionality of a cache client.
///
/// Includes control operations and data operations.
/// </summary>
public interface ISimpleCacheClient : IDisposable
{
    /// <summary>
    /// Creates a cache if it doesn't exist.
    /// </summary>
    /// <param name="cacheName">Name of the cache to be created.</param>
    /// <returns>The result of the create cache operation</returns>
    public CreateCacheResponse CreateCache(string cacheName);

    /// <summary>
    /// Deletes a cache and all of the items within it.
    /// </summary>
    /// <param name="cacheName">Name of the cache to be deleted.</param>
    /// <returns>Result of the delete cache operation.</returns>
    public DeleteCacheResponse DeleteCache(string cacheName);

    /// <summary>
    /// List all caches.
    /// </summary>
    /// <param name="nextPageToken">A token to specify where to start paginating. This is the NextToken from a previous response.</param>
    /// <returns>Result of the list cache operation.</returns>
    public ListCachesResponse ListCaches(string? nextPageToken = null);

    /// <summary>
    /// Set the value in cache with a given time to live (TTL) seconds.
    /// </summary>
    /// <param name="cacheName">Name of the cache to store the item in.</param>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to be stored.</param>
    /// <param name="ttlSeconds">TTL for the item in cache. This TTL takes precedence over the TTL used when initializing a cache client. Defaults to client TTL.</param>
    /// <returns>Task object representing the result of the set operation.</returns>
    public Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value, uint? ttlSeconds = null);

    /// <summary>
    /// Get the cache value stored for the given key.
    /// </summary>
    /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
    /// <param name="key">The key to lookup.</param>
    /// <returns>Task object containing the status of the get operation and the associated value.</returns>
    public Task<CacheGetResponse> GetAsync(string cacheName, byte[] key);

    /// <summary>
    /// Remove the key from the cache.
    /// </summary>
    /// <param name="cacheName">Name of the cache to delete the key from.</param>
    /// <param name="key">The key to delete.</param>
    /// <returns>Task object representing the result of the delete operation.</returns>
    public Task<CacheDeleteResponse> DeleteAsync(string cacheName, byte[] key);

    /// <inheritdoc cref="SetAsync(string, byte[], byte[], uint?)" />
    public Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, uint? ttlSeconds = null);

    /// <inheritdoc cref="GetAsync(string, byte[])" />
    public Task<CacheGetResponse> GetAsync(string cacheName, string key);

    /// <inheritdoc cref="DeleteAsync(string, byte[])" />
    public Task<CacheDeleteResponse> DeleteAsync(string cacheName, string key);

    /// <inheritdoc cref="SetAsync(string, byte[], byte[], uint?)" />
    public Task<CacheSetResponse> SetAsync(string cacheName, string key, byte[] value, uint? ttlSeconds = null);

    /// <summary>
    /// Gets multiple values from the cache.
    /// </summary>
    /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
    /// <param name="keys">The keys to get.</param>
    /// <returns>Task object representing the statuses of the get operation and the associated values.</returns>
    public Task<CacheGetBatchResponse> GetBatchAsync(string cacheName, IEnumerable<byte[]> keys);

    /// <inheritdoc cref="GetBatchAsync(string, IEnumerable{byte[]})" />
    public Task<CacheGetBatchResponse> GetBatchAsync(string cacheName, IEnumerable<string> keys);

    /// <summary>
    /// Sets multiple items in the cache. Overwrites existing items.
    /// </summary>
    /// <param name="cacheName">Name of the cache to store the items in.</param>
    /// <param name="items">The items to set.</param>
    /// <param name="ttlSeconds">TTL for the item in cache. This TTL takes precedence over the TTL used when initializing a cache client. Defaults to client TTL.</param>
    /// <returns>Task object representing the result of the set operation.</returns>
    public Task<CacheSetBatchResponse> SetBatchAsync(string cacheName, IEnumerable<KeyValuePair<byte[], byte[]>> items, uint? ttlSeconds = null);

    /// <inheritdoc cref="SetBatchAsync(string, IEnumerable{KeyValuePair{byte[], byte[]}}, uint?)" />
    public Task<CacheSetBatchResponse> SetBatchAsync(string cacheName, IEnumerable<KeyValuePair<string, string>> items, uint? ttlSeconds = null);
}
