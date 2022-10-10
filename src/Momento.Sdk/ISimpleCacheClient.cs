using System;
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
    /// <returns>
    /// Task representing the result of the create cache operation. This result
    /// is resolved to a type-safe object of one of
    /// the following subtypes:
    /// <list type="bullet">
    /// <item><description>CreateCacheResponse.Success</description></item>
    /// <item><description>CreateCacheResponse.Error</description></item>
    /// </list>
    /// Pattern matching can be used to operate on the appropriate subtype.
    /// For example:
    /// <code>
    /// if (response is CreateCacheResponse.Error errorResponse)
    /// {
    ///     // handle error as appropriate
    /// }
    /// </code>
    /// </returns>
    public Task<CreateCacheResponse> CreateCacheAsync(string cacheName);

    /// <summary>
    /// Deletes a cache and all of the items within it.
    /// </summary>
    /// <param name="cacheName">Name of the cache to be deleted.</param>
    /// <returns>
    /// Task representing the result of the delete cache operation. The
    /// response object is resolved to a type-safe object of one of
    /// the following subtypes:
    /// <list type="bullet">
    /// <item><description>DeleteCacheResponse.Success</description></item>
    /// <item><description>DeleteCacheResponse.Error</description></item>
    /// </list>
    /// Pattern matching can be used to operate on the appropriate subtype.
    /// For example:
    /// <code>
    /// if (response is DeleteCacheResponse.Error errorResponse)
    /// {
    ///     // handle error as appropriate
    /// }
    /// </code>
    ///</returns>
    public Task<DeleteCacheResponse> DeleteCacheAsync(string cacheName);

    /// <summary>
    /// List all caches.
    /// </summary>
    /// <param name="nextPageToken">A token to specify where to start paginating. This is the NextToken from a previous response.</param>
    /// <returns>
    /// Task representing the result of the list cache operation. The
    /// response object is resolved to a type-safe object of one of
    /// the following subtypes:
    /// <list type="bullet">
    /// <item><description>ListCachesResponse.Success</description></item>
    /// <item><description>ListCachesResponse.Error</description></item>
    /// </list>
    /// Pattern matching can be used to operate on the appropriate subtype.
    /// For example:
    /// <code>
    /// if (response is ListCachesResponse.Error errorResponse)
    /// {
    ///     // handle error as appropriate
    /// }
    /// </code>
    /// </returns>
    public Task<ListCachesResponse> ListCachesAsync(string? nextPageToken = null);

    /// <summary>
    /// Set the value in cache with a given time to live (TTL) seconds.
    /// </summary>
    /// <param name="cacheName">Name of the cache to store the item in.</param>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to be stored.</param>
    /// <param name="ttl">TTL for the item in cache. This TTL takes precedence over the TTL used when initializing a cache client. Defaults to client TTL. If specified must be strictly positive.</param>
    /// <returns>
    /// Task object representing the result of the set operation. The
    /// response object is resolved to a type-safe object of one of
    /// the following subtypes:
    /// <list type="bullet">
    /// <item><description>CacheSetResponse.Success</description></item>
    /// <item><description>CacheSetResponse.Error</description></item>
    /// </list>
    /// Pattern matching can be used to operate on the appropriate subtype.
    /// For example:
    /// <code>
    /// if (response is CacheSetResponse.Error errorResponse)
    /// {
    ///     // handle error as appropriate
    /// }
    /// </code>
    /// </returns>
    public Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value, TimeSpan? ttl = null);

    /// <summary>
    /// Get the cache value stored for the given key.
    /// </summary>
    /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
    /// <param name="key">The key to lookup.</param>
    /// <returns>
    /// Task object containing the status of the get operation and the associated value. The
    /// response object is resolved to a type-safe object of one of
    /// the following subtypes:
    /// <list type="bullet">
    /// <item><description>CacheGetResponse.Hit</description></item>
    /// <item><description>CacheGetResponse.Miss</description></item>
    /// <item><description>CacheGetResponse.Error</description></item>
    /// </list>
    /// Pattern matching can be used to operate on the appropriate subtype.
    /// For example:
    /// <code>
    /// if (response is CacheGetResponse.Hit hitResponse)
    /// {
    ///     return hitResponse.ValueString;
    /// } else if (response is CacheGetResponse.Error errorResponse)
    /// {
    ///     // handle error as appropriate
    /// }
    /// </code>
    /// </returns>
    public Task<CacheGetResponse> GetAsync(string cacheName, byte[] key);

    /// <summary>
    /// Remove the key from the cache.
    /// </summary>
    /// <param name="cacheName">Name of the cache to delete the key from.</param>
    /// <param name="key">The key to delete.</param>
    /// <returns>
    /// Task object representing the result of the delete operation. The
    /// response object is resolved to a type-safe object of one of
    /// the following subtypes:
    /// <list type="bullet">
    /// <item><description>CacheDeleteResponse.Success</description></item>
    /// <item><description>CacheDeleteResponse.Error</description></item>
    /// </list>
    /// Pattern matching can be used to operate on the appropriate subtype.
    /// For example:
    /// <code>
    /// if (response is CacheDeleteResponse.Error errorResponse)
    /// {
    ///     // handle error as appropriate
    /// }
    /// </code>
    /// </returns>
    public Task<CacheDeleteResponse> DeleteAsync(string cacheName, byte[] key);

    /// <inheritdoc cref="SetAsync(string, byte[], byte[], TimeSpan?)"/>
    public Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, TimeSpan? ttl = null);

    /// <inheritdoc cref="GetAsync(string, byte[])"/>
    public Task<CacheGetResponse> GetAsync(string cacheName, string key);

    /// <inheritdoc cref="DeleteAsync(string, byte[])"/>
    public Task<CacheDeleteResponse> DeleteAsync(string cacheName, string key);

    /// <inheritdoc cref="SetAsync(string, byte[], byte[], TimeSpan?)"/>
    public Task<CacheSetResponse> SetAsync(string cacheName, string key, byte[] value, TimeSpan? ttl = null);
}
