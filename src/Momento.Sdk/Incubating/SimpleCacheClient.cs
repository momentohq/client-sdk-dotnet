using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Momento.Sdk.Incubating.Internal;
using Momento.Sdk.Incubating.Responses;
using Momento.Sdk.Responses;
using Utils = Momento.Sdk.Internal.Utils;

namespace Momento.Sdk.Incubating;

/// <summary>
/// Incubating cache client.
///
/// This enables preview features not ready for general release.
/// </summary>
public class SimpleCacheClient : ISimpleCacheClient
{
    private readonly ISimpleCacheClient simpleCacheClient;
    private readonly ScsDataClient dataClient;

    /// <summary>
    /// Client to perform operations against the Simple Cache Service.
    /// 
    /// Enables preview features.
    /// </summary>
    /// <param name="simpleCacheClient">Instance of release cache client to delegate operations to.</param>
    /// <param name="authToken">Momento JWT.</param>
    /// <param name="defaultTtlSeconds">Default time to live for the item in cache.</param>
    /// <param name="dataClientOperationTimeoutMilliseconds">Deadline (timeout) for communicating to the server. Defaults to 5 seconds.</param>
    public SimpleCacheClient(ISimpleCacheClient simpleCacheClient, string authToken, uint defaultTtlSeconds, uint? dataClientOperationTimeoutMilliseconds = null)
    {
        this.simpleCacheClient = simpleCacheClient;

        var claims = Momento.Sdk.Internal.JwtUtils.DecodeJwt(authToken);
        this.dataClient = new(authToken, claims.CacheEndpoint, defaultTtlSeconds, dataClientOperationTimeoutMilliseconds);
    }

    /// <inheritdoc />
    public CreateCacheResponse CreateCache(string cacheName)
    {
        return this.simpleCacheClient.CreateCache(cacheName);
    }

    /// <inheritdoc />
    public DeleteCacheResponse DeleteCache(string cacheName)
    {
        return this.simpleCacheClient.DeleteCache(cacheName);
    }

    /// <inheritdoc />
    public ListCachesResponse ListCaches(string? nextPageToken = null)
    {
        return this.simpleCacheClient.ListCaches(nextPageToken);
    }

    /// <inheritdoc />
    public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value, uint? ttlSeconds = null)
    {
        return await this.simpleCacheClient.SetAsync(cacheName, key, value, ttlSeconds);
    }

    /// <inheritdoc />
    public async Task<CacheGetResponse> GetAsync(string cacheName, byte[] key)
    {
        return await this.simpleCacheClient.GetAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, byte[] key)
    {
        return await this.simpleCacheClient.DeleteAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, uint? ttlSeconds = null)
    {
        return await simpleCacheClient.SetAsync(cacheName, key, value, ttlSeconds);
    }

    /// <inheritdoc />
    public async Task<CacheGetResponse> GetAsync(string cacheName, string key)
    {
        return await this.simpleCacheClient.GetAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, string key)
    {
        return await this.simpleCacheClient.DeleteAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, byte[] value, uint? ttlSeconds = null)
    {
        return await this.simpleCacheClient.SetAsync(cacheName, key, value, ttlSeconds);
    }

    /// <inheritdoc />
    public async Task<CacheGetBatchResponse> GetBatchAsync(string cacheName, IEnumerable<byte[]> keys)
    {
        return await this.simpleCacheClient.GetBatchAsync(cacheName, keys);
    }

    /// <inheritdoc />
    public async Task<CacheGetBatchResponse> GetBatchAsync(string cacheName, IEnumerable<string> keys)
    {
        return await this.simpleCacheClient.GetBatchAsync(cacheName, keys);
    }

    /// <inheritdoc />
    public async Task<CacheSetBatchResponse> SetBatchAsync(string cacheName, IEnumerable<KeyValuePair<byte[], byte[]>> items, uint? ttlSeconds = null)
    {
        return await this.simpleCacheClient.SetBatchAsync(cacheName, items, ttlSeconds);
    }

    /// <inheritdoc />
    public async Task<CacheSetBatchResponse> SetBatchAsync(string cacheName, IEnumerable<KeyValuePair<string, string>> items, uint? ttlSeconds = null)
    {
        return await this.simpleCacheClient.SetBatchAsync(cacheName, items, ttlSeconds);
    }

    /// <summary>
    /// Set the dictionary field to a value with a given time to live (TTL) seconds.
    /// </summary>
    /// <remark>
    /// Creates the data structure if it does not exist and sets the TTL.
    /// If the data structure already exists and <paramref name="refreshTtl"/> is <see langword="true"/>,
    /// then update the TTL to <paramref name="ttlSeconds"/>, otherwise leave the TTL unchanged.
    /// </remark>
    /// <param name="cacheName">Name of the cache to store the dictionary in.</param>
    /// <param name="dictionaryName">The dictionary to set.</param>
    /// <param name="field">The field in the dictionary to set.</param>
    /// <param name="value">The value to be stored.</param>
    /// <param name="refreshTtl">Update the dictionary TTL if the dictionary already exists.</param>
    /// <param name="ttlSeconds">TTL for the dictionary in cache. This TTL takes precedence over the TTL used when initializing a cache client. Defaults to client TTL.</param>
    /// <returns>Task representing the result of the cache operation.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/>, <paramref name="dictionaryName"/>, <paramref name="field"/>, <paramref name="value"/> is <see langword="null"/>.</exception>
    public async Task<CacheDictionarySetResponse> DictionarySetAsync(string cacheName, string dictionaryName, byte[] field, byte[] value, bool refreshTtl, uint? ttlSeconds = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
        Utils.ArgumentNotNull(field, nameof(field));
        Utils.ArgumentNotNull(value, nameof(value));

        return await this.dataClient.DictionarySetAsync(cacheName, dictionaryName, field, value, refreshTtl, ttlSeconds);
    }

    /// <inheritdoc cref="DictionarySetAsync(string, string, byte[], byte[], bool, uint?)"/>
    public async Task<CacheDictionarySetResponse> DictionarySetAsync(string cacheName, string dictionaryName, string field, string value, bool refreshTtl, uint? ttlSeconds = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
        Utils.ArgumentNotNull(field, nameof(field));
        Utils.ArgumentNotNull(value, nameof(value));

        return await this.dataClient.DictionarySetAsync(cacheName, dictionaryName, field, value, refreshTtl, ttlSeconds);
    }

    /// <inheritdoc cref="DictionarySetAsync(string, string, byte[], byte[], bool, uint?)"/>
    public async Task<CacheDictionarySetResponse> DictionarySetAsync(string cacheName, string dictionaryName, string field, byte[] value, bool refreshTtl, uint? ttlSeconds = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
        Utils.ArgumentNotNull(field, nameof(field));
        Utils.ArgumentNotNull(value, nameof(value));

        return await this.dataClient.DictionarySetAsync(cacheName, dictionaryName, field, value, refreshTtl, ttlSeconds);
    }

    /// <summary>
    /// Get the cache value stored for the given dictionary and field.
    /// </summary>
    /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
    /// <param name="dictionaryName">The dictionary to lookup.</param>
    /// <param name="field">The field in the dictionary to lookup.</param>
    /// <returns>Task representing the status of the get operation and the associated value.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/>, <paramref name="dictionaryName"/>, <paramref name="field"/> is <see langword="null"/>.</exception>
    public async Task<CacheDictionaryGetResponse> DictionaryGetAsync(string cacheName, string dictionaryName, byte[] field)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
        Utils.ArgumentNotNull(field, nameof(field));

        return await this.dataClient.DictionaryGetAsync(cacheName, dictionaryName, field);
    }

    /// <inheritdoc cref="DictionaryGetAsync(string, string, byte[])"/>
    public async Task<CacheDictionaryGetResponse> DictionaryGetAsync(string cacheName, string dictionaryName, string field)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
        Utils.ArgumentNotNull(field, nameof(field));

        return await this.dataClient.DictionaryGetAsync(cacheName, dictionaryName, field);
    }

    /// <summary>
    /// Set several dictionary field-value pairs in the cache.
    /// </summary>
    /// <inheritdoc cref="DictionarySetAsync(string, string, byte[], byte[], bool, uint?)" path="remark"/>
    /// <param name="cacheName">Name of the cache to store the dictionary in.</param>
    /// <param name="dictionaryName">The dictionary to set.</param>
    /// <param name="items">The field-value pairs in the dictionary to set.</param>
    /// <param name="refreshTtl">Update the dictionary TTL if the dictionary already exists.</param>
    /// <param name="ttlSeconds">TTL for the dictionary in cache. This TTL takes precedence over the TTL used when initializing a cache client. Defaults to client TTL.</param>
    /// <returns>Task representing the result of the cache operation.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/>, <paramref name="dictionaryName"/>, <paramref name="items"/> is <see langword="null"/>.</exception>
    public async Task<CacheDictionarySetBatchResponse> DictionarySetBatchAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<byte[], byte[]>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
        Utils.ArgumentNotNull(items, nameof(items));
        Utils.KeysAndValuesNotNull(items, nameof(items));

        return await this.dataClient.DictionarySetBatchAsync(cacheName, dictionaryName, items, refreshTtl, ttlSeconds);
    }

    /// <inheritdoc cref="DictionarySetBatchAsync(string, string, IEnumerable{KeyValuePair{byte[], byte[]}}, bool, uint?)"/>
    public async Task<CacheDictionarySetBatchResponse> DictionarySetBatchAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, string>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
        Utils.ArgumentNotNull(items, nameof(items));
        Utils.KeysAndValuesNotNull(items, nameof(items));

        return await this.dataClient.DictionarySetBatchAsync(cacheName, dictionaryName, items, refreshTtl, ttlSeconds);
    }

    /// <inheritdoc cref="DictionarySetBatchAsync(string, string, IEnumerable{KeyValuePair{byte[], byte[]}}, bool, uint?)"/>
    public async Task<CacheDictionarySetBatchResponse> DictionarySetBatchAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, byte[]>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
        Utils.ArgumentNotNull(items, nameof(items));
        Utils.KeysAndValuesNotNull(items, nameof(items));

        return await this.dataClient.DictionarySetBatchAsync(cacheName, dictionaryName, items, refreshTtl, ttlSeconds);
    }

    /// <summary>
    /// Get several values from a dictionary.
    /// </summary>
    /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
    /// <param name="dictionaryName">The dictionary to lookup.</param>
    /// <param name="fields">The fields in the dictionary to lookup.</param>
    /// <returns>Task representing the status and associated value for each field.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/>, <paramref name="dictionaryName"/>, <paramref name="fields"/> is <see langword="null"/>.</exception>
    public async Task<CacheDictionaryGetBatchResponse> DictionaryGetBatchAsync(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
        Utils.ArgumentNotNull(fields, nameof(fields));
        Utils.ElementsNotNull(fields, nameof(fields));

        return await this.dataClient.DictionaryGetBatchAsync(cacheName, dictionaryName, fields);
    }

    /// <inheritdoc cref="DictionaryGetBatchAsync(string, string, IEnumerable{byte[]})"/>
    public async Task<CacheDictionaryGetBatchResponse> DictionaryGetBatchAsync(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
        Utils.ArgumentNotNull(fields, nameof(fields));
        Utils.ElementsNotNull(fields, nameof(fields));

        return await this.dataClient.DictionaryGetBatchAsync(cacheName, dictionaryName, fields);
    }

    /// <summary>
    /// Fetch the entire dictionary from the cache.
    /// </summary>
    /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
    /// <param name="dictionaryName">The dictionary to fetch.</param>
    /// <returns>Task representing with the status of the fetch operation and the associated dictionary.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/>, <paramref name="dictionaryName"/> is <see langword="null"/>.</exception>
    public async Task<CacheDictionaryFetchResponse> DictionaryFetchAsync(string cacheName, string dictionaryName)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));

        return await this.dataClient.DictionaryFetchAsync(cacheName, dictionaryName);
    }

    /// <summary>
    /// Remove the dictionary from the cache.
    ///
    /// Performs a no-op if <paramref name="dictionaryName"/> does not exist.
    /// </summary>
    /// <param name="cacheName">Name of the cache to delete the dictionary from.</param>
    /// <param name="dictionaryName">Name of the dictionary to delete.</param>
    /// <returns>Task representing the result of the delete operation.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/> or <paramref name="dictionaryName"/> is <see langword="null"/>.</exception>
    public async Task<CacheDictionaryDeleteResponse> DictionaryDeleteAsync(string cacheName, string dictionaryName)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));

        return await this.dataClient.DictionaryDeleteAsync(cacheName, dictionaryName);
    }

    /// <summary>
    /// Remove a field from a dictionary.
    ///
    /// Performs a no-op if <paramref name="dictionaryName"/> or <paramref name="field"/> does not exist.
    /// </summary>
    /// <param name="cacheName">Name of the cache to lookup the dictionary in.</param>
    /// <param name="dictionaryName">Name of the dictionary to remove the field from.</param>
    /// <param name="field">Name of the field to remove from the dictionary.</param>
    /// <returns>Task representing the result of the cache operation.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/>, <paramref name="dictionaryName"/>, <paramref name="field"/> is <see langword="null"/>.</exception>
    public async Task<CacheDictionaryRemoveFieldResponse> DictionaryRemoveFieldAsync(string cacheName, string dictionaryName, byte[] field)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
        Utils.ArgumentNotNull(field, nameof(field));

        return await this.dataClient.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field);
    }

    /// <inheritdoc cref="DictionaryRemoveFieldAsync(string, string, byte[])"/>
    public async Task<CacheDictionaryRemoveFieldResponse> DictionaryRemoveFieldAsync(string cacheName, string dictionaryName, string field)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
        Utils.ArgumentNotNull(field, nameof(field));

        return await this.dataClient.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field);
    }

    /// <summary>
    /// Remove fields from a dictionary.
    ///
    /// Performs a no-op if <paramref name="dictionaryName"/> or a particular field does not exist.
    /// </summary>
    /// <param name="cacheName">Name of the cache to lookup the dictionary in.</param>
    /// <param name="dictionaryName">Name of the dictionary to remove the field from.</param>
    /// <param name="fields">Name of the fields to remove from the dictionary.</param>
    /// <returns>Task representing the result of the cache operation.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/>, <paramref name="dictionaryName"/>, <paramref name="fields"/> is <see langword="null"/>.</exception>
    public async Task<CacheDictionaryRemoveFieldsResponse> DictionaryRemoveFieldsAsync(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
        Utils.ArgumentNotNull(fields, nameof(fields));
        Utils.ElementsNotNull(fields, nameof(fields));

        return await this.dataClient.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fields);
    }

    /// <inheritdoc cref="DictionaryRemoveFieldsAsync(string, string, IEnumerable{byte[]})"/>
    public async Task<CacheDictionaryRemoveFieldsResponse> DictionaryRemoveFieldsAsync(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
        Utils.ArgumentNotNull(fields, nameof(fields));
        Utils.ElementsNotNull(fields, nameof(fields));

        return await this.dataClient.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fields);
    }

    /// <summary>
    /// Add an element to a set in the cache.
    ///
    /// After this operation, the set will contain the union
    /// of the element passed in and the elements of the set.
    /// </summary>
    /// <inheritdoc cref="DictionarySetAsync(string, string, byte[], byte[], bool, uint?)" path="remark"/>
    /// <param name="cacheName">Name of the cache to store the set in.</param>
    /// <param name="setName">The set to add the element to.</param>
    /// <param name="element">The data to add to the set.</param>
    /// <param name="refreshTtl">Update <paramref name="setName"/>'s TTL if it already exists.</param>
    /// <param name="ttlSeconds">TTL for the set in cache. This TTL takes precedence over the TTL used when initializing a cache client. Defaults to client TTL.</param>
    /// <returns>Task representing the result of the cache operation.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/>, <paramref name="setName"/>, <paramref name="element"/> is <see langword="null"/>.</exception>
    public async Task<CacheSetAddResponse> SetAddAsync(string cacheName, string setName, byte[] element, bool refreshTtl, uint? ttlSeconds = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(setName, nameof(setName));
        Utils.ArgumentNotNull(element, nameof(element));

        return await this.dataClient.SetAddAsync(cacheName, setName, element, refreshTtl, ttlSeconds);
    }

    /// <inheritdoc cref="SetAddAsync(string, string, byte[], bool, uint?)"/>
    public async Task<CacheSetAddResponse> SetAddAsync(string cacheName, string setName, string element, bool refreshTtl, uint? ttlSeconds = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(setName, nameof(setName));
        Utils.ArgumentNotNull(element, nameof(element));

        return await this.dataClient.SetAddAsync(cacheName, setName, element, refreshTtl, ttlSeconds);
    }

    /// <summary>
    /// Add several elements to a set in the cache.
    ///
    /// After this operation, the set will contain the union
    /// of the elements passed in and the elements of the set.
    /// </summary>
    /// <inheritdoc cref="DictionarySetAsync(string, string, byte[], byte[], bool, uint?)" path="remark"/>
    /// <param name="cacheName">Name of the cache to store the set in.</param>
    /// <param name="setName">The set to add elements to.</param>
    /// <param name="elements">The data to add to the set.</param>
    /// <param name="refreshTtl">Update <paramref name="setName"/>'s TTL if it already exists.</param>
    /// <param name="ttlSeconds">TTL for the set in cache. This TTL takes precedence over the TTL used when initializing a cache client. Defaults to client TTL.</param>
    /// <returns>Task representing the result of the cache operation.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/>, <paramref name="setName"/>, <paramref name="elements"/> is <see langword="null"/>.</exception>
    public async Task<CacheSetAddBatchResponse> SetAddBatchAsync(string cacheName, string setName, IEnumerable<byte[]> elements, bool refreshTtl, uint? ttlSeconds = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(setName, nameof(setName));
        Utils.ArgumentNotNull(elements, nameof(elements));
        Utils.ElementsNotNull(elements, nameof(elements));

        return await this.dataClient.SetAddBatchAsync(cacheName, setName, elements, refreshTtl, ttlSeconds);
    }

    /// <inheritdoc cref="SetAddBatchAsync(string, string, IEnumerable{byte[]}, bool, uint?)"/>
    public async Task<CacheSetAddBatchResponse> SetAddBatchAsync(string cacheName, string setName, IEnumerable<string> elements, bool refreshTtl, uint? ttlSeconds = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(setName, nameof(setName));
        Utils.ArgumentNotNull(elements, nameof(elements));
        Utils.ElementsNotNull(elements, nameof(elements));

        return await this.dataClient.SetAddBatchAsync(cacheName, setName, elements, refreshTtl, ttlSeconds);
    }

    /// <summary>
    /// Remove an element from a set.
    ///
    /// Performs a no-op if <paramref name="setName"/> or <paramref name="element"/> does not exist.
    /// </summary>
    /// <param name="cacheName">Name of the cache to lookup the set in.</param>
    /// <param name="setName">The set to remove the element from.</param>
    /// <param name="element">The data to remove from the set.</param>
    /// <returns>Task representing the result of the cache operation.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/>, <paramref name="setName"/>, <paramref name="element"/> is <see langword="null"/>.</exception>
    public async Task<CacheSetRemoveElementResponse> SetRemoveElementAsync(string cacheName, string setName, byte[] element)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(setName, nameof(setName));
        Utils.ArgumentNotNull(element, nameof(element));

        return await this.dataClient.SetRemoveElementAsync(cacheName, setName, element);
    }

    /// <inheritdoc cref="SetRemoveElementAsync(string, string, byte[])"/>
    public async Task<CacheSetRemoveElementResponse> SetRemoveElementAsync(string cacheName, string setName, string element)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(setName, nameof(setName));
        Utils.ArgumentNotNull(element, nameof(element));

        return await this.dataClient.SetRemoveElementAsync(cacheName, setName, element);
    }

    /// <summary>
    /// Remove elements from a set.
    ///
    /// Performs a no-op if <paramref name="setName"/> or any of <paramref name="elements"/> do not exist.
    /// </summary>
    /// <param name="cacheName">Name of the cache to lookup the set in.</param>
    /// <param name="setName">The set to remove the elements from.</param>
    /// <param name="elements">The data to remove from the set.</param>
    /// <returns>Task representing the result of the cache operation.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/>, <paramref name="setName"/>, <paramref name="elements"/> is <see langword="null"/>.</exception>
    public async Task<CacheSetRemoveElementsResponse> SetRemoveElementsAsync(string cacheName, string setName, IEnumerable<byte[]> elements)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(setName, nameof(setName));
        Utils.ArgumentNotNull(elements, nameof(elements));
        Utils.ElementsNotNull(elements, nameof(elements));

        return await this.dataClient.SetRemoveElementsAsync(cacheName, setName, elements);
    }

    /// <inheritdoc cref="SetRemoveElementsAsync(string, string, IEnumerable{byte[]})"/>
    public async Task<CacheSetRemoveElementsResponse> SetRemoveElementsAsync(string cacheName, string setName, IEnumerable<string> elements)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(setName, nameof(setName));
        Utils.ArgumentNotNull(elements, nameof(elements));
        Utils.ElementsNotNull(elements, nameof(elements));

        return await this.dataClient.SetRemoveElementsAsync(cacheName, setName, elements);
    }

    /// <summary>
    /// Fetch the entire set from the cache.
    /// </summary>
    /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
    /// <param name="setName">The set to fetch.</param>
    /// <returns>Task representing with the status of the fetch operation and the associated set.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/> or <paramref name="setName"/> is <see langword="null"/>.</exception>
    public async Task<CacheSetFetchResponse> SetFetchAsync(string cacheName, string setName)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(setName, nameof(setName));

        return await this.dataClient.SetFetchAsync(cacheName, setName);
    }

    /// <summary>
    /// Remove the set from the cache.
    ///
    /// Performs a no-op if <paramref name="setName"/> does not exist.
    /// </summary>
    /// <param name="cacheName">Name of the cache to delete the set from.</param>
    /// <param name="setName">Name of the set to delete.</param>
    /// <returns>Task representing the result of the delete operation.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/> or <paramref name="setName"/> is <see langword="null"/>.</exception>
    public async Task<CacheSetDeleteResponse> SetDeleteAsync(string cacheName, string setName)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(setName, nameof(setName));

        return await this.dataClient.SetDeleteAsync(cacheName, setName);
    }

    /// <summary>
    /// Push a value to the beginning of a list.
    /// </summary>
    /// <inheritdoc cref="DictionarySetAsync(string, string, byte[], byte[], bool, uint?)" path="remark"/>
    /// <param name="cacheName">Name of the cache to store the list in.</param>
    /// <param name="listName">The list to push the value on.</param>
    /// <param name="value">The value to push to the front of the list.</param>
    /// <param name="refreshTtl">Update <paramref name="listName"/>'s TTL if it already exists.</param>
    /// <param name="ttlSeconds">TTL for the list in cache. This TTL takes precedence over the TTL used when initializing a cache client. Defaults to client TTL.</param>
    /// <param name="truncateBackToSize">Ensure the list does not exceed this length. Remove excess from the end of the list. Must be a positive number.</param>
    /// <returns>Task representing the result of the push operation.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/> or <paramref name="listName"/> or <paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="truncateBackToSize"/> is zero.</exception>
    public async Task<CacheListPushFrontResponse> ListPushFrontAsync(string cacheName, string listName, byte[] value, bool refreshTtl, uint? ttlSeconds = null, uint? truncateBackToSize = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(listName, nameof(listName));
        Utils.ArgumentNotNull(value, nameof(value));
        Utils.ArgumentStrictlyPositive(truncateBackToSize, nameof(truncateBackToSize));

        return await this.dataClient.ListPushFrontAsync(cacheName, listName, value, refreshTtl, ttlSeconds);
    }

    /// <inheritdoc cref="ListPushFrontAsync(string, string, byte[], bool, uint?, uint?)"/>
    public async Task<CacheListPushFrontResponse> ListPushFrontAsync(string cacheName, string listName, string value, bool refreshTtl, uint? ttlSeconds = null, uint? truncateBackToSize = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(listName, nameof(listName));
        Utils.ArgumentNotNull(value, nameof(value));
        Utils.ArgumentStrictlyPositive(truncateBackToSize, nameof(truncateBackToSize));

        return await this.dataClient.ListPushFrontAsync(cacheName, listName, value, refreshTtl, ttlSeconds);
    }

    /// <summary>
    /// Push a value to the end of a list.
    /// </summary>
    /// <inheritdoc cref="DictionarySetAsync(string, string, byte[], byte[], bool, uint?)" path="remark"/>
    /// <param name="cacheName">Name of the cache to store the list in.</param>
    /// <param name="listName">The list to push the value on.</param>
    /// <param name="value">The value to push to the back of the list.</param>
    /// <param name="refreshTtl">Update <paramref name="listName"/>'s TTL if it already exists.</param>
    /// <param name="ttlSeconds">TTL for the list in cache. This TTL takes precedence over the TTL used when initializing a cache client. Defaults to client TTL.</param>
    /// <param name="truncateFrontToSize">Ensure the list does not exceed this length. Remove excess from the beginning of the list. Must be a positive number.</param>
    /// <returns>Task representing the result of the push operation.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/> or <paramref name="listName"/> or <paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="truncateFrontToSize"/> is zero.</exception>
    public async Task<CacheListPushBackResponse> ListPushBackAsync(string cacheName, string listName, byte[] value, bool refreshTtl, uint? ttlSeconds = null, uint? truncateFrontToSize = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(listName, nameof(listName));
        Utils.ArgumentNotNull(value, nameof(value));
        Utils.ArgumentStrictlyPositive(truncateFrontToSize, nameof(truncateFrontToSize));

        return await this.dataClient.ListPushBackAsync(cacheName, listName, value, refreshTtl, ttlSeconds);
    }

    /// <inheritdoc cref="ListPushBackAsync(string, string, byte[], bool, uint?, uint?)"/>
    public async Task<CacheListPushBackResponse> ListPushBackAsync(string cacheName, string listName, string value, bool refreshTtl, uint? ttlSeconds = null, uint? truncateFrontToSize = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(listName, nameof(listName));
        Utils.ArgumentNotNull(value, nameof(value));
        Utils.ArgumentStrictlyPositive(truncateFrontToSize, nameof(truncateFrontToSize));

        return await this.dataClient.ListPushBackAsync(cacheName, listName, value, refreshTtl, ttlSeconds);
    }

    /// <summary>
    /// Retrieve and remove the first item from a list.
    /// </summary>
    /// <param name="cacheName">Name of the cache to read the list from.</param>
    /// <param name="listName">The list to pop from.</param>
    /// <returns>Task representing the status and associated value for the pop operation.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/> or <paramref name="listName"/> is <see langword="null"/>.</exception>
    public async Task<CacheListPopFrontResponse> ListPopFrontAsync(string cacheName, string listName)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(listName, nameof(listName));

        return await this.dataClient.ListPopFrontAsync(cacheName, listName);
    }

    /// <summary>
    /// Retrieve and remove the last item from a list.
    /// </summary>
    /// <param name="cacheName">Name of the cache to read the list from.</param>
    /// <param name="listName">The list to pop from.</param>
    /// <returns>Task representing the status and associated value for the pop operation.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/> or <paramref name="listName"/> is <see langword="null"/>.</exception>
    public async Task<CacheListPopBackResponse> ListPopBackAsync(string cacheName, string listName)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(listName, nameof(listName));

        return await this.dataClient.ListPopBackAsync(cacheName, listName);
    }

    /// <summary>
    /// Fetch the entire list from the cache.
    /// </summary>
    /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
    /// <param name="listName">The list to fetch.</param>
    /// <returns>Task representing with the status of the fetch operation and the associated list.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/> or <paramref name="listName"/> is <see langword="null"/>.</exception>
    public async Task<CacheListFetchResponse> ListFetchAsync(string cacheName, string listName)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(listName, nameof(listName));

        return await this.dataClient.ListFetchAsync(cacheName, listName);
    }

    /// <summary>
    /// Remove all elements in a list equal to a particular value.
    /// </summary>
    /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
    /// <param name="listName">The list to remove elements from.</param>
    /// <param name="value">The value to completely remove from the list.</param>
    /// <returns>Task representing the result of the cache operation.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/>, <paramref name="listName"/>, or <paramref name="value"/> is <see langword="null"/>.</exception>
    public async Task<CacheListRemoveAllResponse> ListRemoveAllAsync(string cacheName, string listName, byte[] value)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(listName, nameof(listName));
        Utils.ArgumentNotNull(value, nameof(value));

        return await this.dataClient.ListRemoveAllAsync(cacheName, listName, value);
    }

    /// <inheritdoc cref="ListRemoveAllAsync(string, string, byte[])"/>
    public async Task<CacheListRemoveAllResponse> ListRemoveAllAsync(string cacheName, string listName, string value)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(listName, nameof(listName));
        Utils.ArgumentNotNull(value, nameof(value));

        return await this.dataClient.ListRemoveAllAsync(cacheName, listName, value);
    }

    /// <summary>
    /// Calculate the length of a list in the cache.
    ///
    /// A list that does not exist is interpreted to have length 0.
    /// </summary>
    /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
    /// <param name="listName">The list to calculate length.</param>
    /// <returns>Task representing the length of the list.</returns>
    /// <exception cref="ArgumentNullException">Any of <paramref name="cacheName"/> or <paramref name="listName"/> is <see langword="null"/>.</exception>
    public async Task<CacheListLengthResponse> ListLengthAsync(string cacheName, string listName)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(listName, nameof(listName));

        return await this.dataClient.ListLengthAsync(cacheName, listName);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.simpleCacheClient.Dispose();
        this.dataClient.Dispose();
    }
}
