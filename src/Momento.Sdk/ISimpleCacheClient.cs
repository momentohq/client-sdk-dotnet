using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Momento.Sdk.Requests;
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
    /// Flushes all the items within a cache.
    /// </summary>
    /// <param name="cacheName">Name of the cache from which all items will be flushed.</param>
    /// <returns>
    /// Task representing the result of the flush cache operation. The
    /// response object is resolved to a type-safe object of one of
    /// the following subtypes:
    /// <list type="bullet">
    /// <item><description>FlushCacheResponse.Success</description></item>
    /// <item><description>FlushCacheResponse.Error</description></item>
    /// </list>
    /// Pattern matching can be used to operate on the appropriate subtype.
    /// For example:
    /// <code>
    /// if (response is FlushCacheResponse.Error errorResponse)
    /// {
    ///     // handle error as appropriate
    /// }
    /// </code>
    ///</returns>
    public Task<FlushCacheResponse> FlushCacheAsync(string cacheName);

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

    /// <summary>
    /// Set the dictionary field to a value with a given time to live (TTL) seconds.
    /// </summary>
    /// <remark>
    /// Creates the data structure if it does not exist
    /// </remark>
    /// <param name="cacheName">Name of the cache to store the dictionary in.</param>
    /// <param name="dictionaryName">The dictionary to set.</param>
    /// <param name="field">The field in the dictionary to set.</param>
    /// <param name="value">The value to be stored.</param>
    /// <param name="ttl">TTL for the dictionary in cache. This TTL takes precedence over the TTL used when initializing a cache client. Defaults to client TTL.</param>
    /// <returns>Task representing the result of the cache operation.</returns>
    public Task<CacheDictionarySetFieldResponse> DictionarySetFieldAsync(string cacheName, string dictionaryName, byte[] field, byte[] value, CollectionTtl ttl = default(CollectionTtl));

    /// <inheritdoc cref="DictionarySetFieldAsync(string, string, byte[], byte[], CollectionTtl)"/>
    public Task<CacheDictionarySetFieldResponse> DictionarySetFieldAsync(string cacheName, string dictionaryName, string field, string value, CollectionTtl ttl = default(CollectionTtl));

    /// <inheritdoc cref="DictionarySetFieldAsync(string, string, byte[], byte[], CollectionTtl)"/>
    public Task<CacheDictionarySetFieldResponse> DictionarySetFieldAsync(string cacheName, string dictionaryName, string field, byte[] value, CollectionTtl ttl = default(CollectionTtl));

    /// <summary>
    /// Get the cache value stored for the given dictionary and field.
    /// </summary>
    /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
    /// <param name="dictionaryName">The dictionary to lookup.</param>
    /// <param name="field">The field in the dictionary to lookup.</param>
    /// <returns>Task representing the status of the get operation and the associated value.</returns>
    public Task<CacheDictionaryGetFieldResponse> DictionaryGetFieldAsync(string cacheName, string dictionaryName, byte[] field);

    /// <inheritdoc cref="DictionaryGetFieldAsync(string, string, byte[])"/>
    public Task<CacheDictionaryGetFieldResponse> DictionaryGetFieldAsync(string cacheName, string dictionaryName, string field);

    /// <summary>
    /// Set several dictionary field-value pairs in the cache.
    /// </summary>
    /// <inheritdoc cref="DictionarySetFieldAsync(string, string, byte[], byte[], CollectionTtl)" path="remark"/>
    /// <param name="cacheName">Name of the cache to store the dictionary in.</param>
    /// <param name="dictionaryName">The dictionary to set.</param>
    /// <param name="items">The field-value pairs in the dictionary to set.</param>
    /// <param name="ttl">TTL for the dictionary in cache. This TTL takes precedence over the TTL used when initializing a cache client. Defaults to client TTL.</param>
    /// <returns>Task representing the result of the cache operation.</returns>
    public Task<CacheDictionarySetFieldsResponse> DictionarySetFieldsAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<byte[], byte[]>> items, CollectionTtl ttl = default(CollectionTtl));

    /// <inheritdoc cref="DictionarySetFieldsAsync(string, string, IEnumerable{KeyValuePair{byte[], byte[]}}, CollectionTtl)"/>
    public Task<CacheDictionarySetFieldsResponse> DictionarySetFieldsAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, string>> items, CollectionTtl ttl = default(CollectionTtl));

    /// <inheritdoc cref="DictionarySetFieldsAsync(string, string, IEnumerable{KeyValuePair{byte[], byte[]}}, CollectionTtl)"/>
    public Task<CacheDictionarySetFieldsResponse> DictionarySetFieldsAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, byte[]>> items, CollectionTtl ttl = default(CollectionTtl));

    /// <summary>
    /// <para>Add an integer quantity to a dictionary value.</para>
    ///
    /// <para>Incrementing the value of a missing field sets the value to <paramref name="amount"/>.</para>
    /// <para>Incrementing a value that was not set using this method or not the string representation of an integer
    /// results in an error with <see cref="FailedPreconditionException"/>.</para>
    /// </summary>
    /// <inheritdoc cref="DictionarySetFieldAsync(string, string, byte[], byte[], CollectionTtl)" path="remark"/>
    /// <param name="cacheName">Name of the cache to store the dictionary in.</param>
    /// <param name="dictionaryName">The dictionary to set.</param>
    /// <param name="field"></param>
    /// <param name="amount">The quantity to add to the value. May be positive, negative, or zero. Defaults to 1.</param>
    /// <param name="ttl">TTL for the dictionary in cache. This TTL takes precedence over the TTL used when initializing a cache client. Defaults to client TTL.</param>
    /// <returns>Task representing the result of the cache operation.</returns>
    /// <example>
    /// The following illustrates a typical workflow:
    /// <code>
    ///     var response = await client.DictionaryIncrementAsync(cacheName, "my dictionary", "counter", amount: 42);
    ///     if (response is CacheDictionaryIncrementResponse.Success success)
    ///     {
    ///         Console.WriteLine($"Current value is {success.Value}");
    ///     }
    ///     else if (response is CacheDictionaryIncrementResponse.Error error)
    ///     {
    ///         Console.WriteLine($"Got an error: {error.Message}");
    ///     }
    ///
    ///     // Reset the counter. Note we use the string representation of an integer.
    ///     var setResponse = await client.DictionarySetFieldAsync(cacheName, "my dictionary", "counter", "0");
    ///     if (setResponse is CacheDictionarySetFieldResponse.Error) { /* handle error */ }
    ///
    ///     // Retrieve the counter. The integer is represented as a string.
    ///     var getResponse = await client.DictionaryGetFieldAsync(cacheName, "my dictionary", "counter");
    ///     if (getResponse is CacheDictionaryGetFieldResponse.Hit getHit)
    ///     {
    ///         Console.WriteLine(getHit.String());
    ///     }
    ///     else if (getResponse is CacheDictionaryGetFieldResponse.Error) { /* handle error */ }
    ///
    ///     // Here we try incrementing a value that isn't an integer. This results in an error with <see cref="FailedPreconditionException"/>
    ///     setResponse = await client.DictionarySetFieldAsync(cacheName, "my dictionary", "counter", "0123ABC");
    ///     if (setResponse is CacheDictionarySetFieldResponse.Error) { /* handle error */ }
    ///
    ///     var incrementResponse = await client.DictionaryIncrementAsync(cacheName, "my dictionary", "counter", amount: 42);
    ///     if (incrementResponse is CacheDictionaryIncrementResponse.Error badIncrement)
    ///     {
    ///         Console.WriteLine($"Could not increment dictionary field: {badIncrement.Message}");
    ///     }
    /// </code>
    /// </example>
    public Task<CacheDictionaryIncrementResponse> DictionaryIncrementAsync(string cacheName, string dictionaryName, string field, long amount = 1, CollectionTtl ttl = default(CollectionTtl));

    /// <summary>
    /// Get several values from a dictionary.
    /// </summary>
    /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
    /// <param name="dictionaryName">The dictionary to lookup.</param>
    /// <param name="fields">The fields in the dictionary to lookup.</param>
    /// <returns>Task representing the status and associated value for each field.</returns>
    public Task<CacheDictionaryGetFieldsResponse> DictionaryGetFieldsAsync(string cacheName, string dictionaryName, IEnumerable<byte[]> fields);

    /// <inheritdoc cref="DictionaryGetFieldsAsync(string, string, IEnumerable{byte[]})"/>
    public Task<CacheDictionaryGetFieldsResponse> DictionaryGetFieldsAsync(string cacheName, string dictionaryName, IEnumerable<string> fields);

    /// <summary>
    /// Fetch the entire dictionary from the cache.
    /// </summary>
    /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
    /// <param name="dictionaryName">The dictionary to fetch.</param>
    /// <returns>Task representing with the status of the fetch operation and the associated dictionary.</returns>
    public Task<CacheDictionaryFetchResponse> DictionaryFetchAsync(string cacheName, string dictionaryName);

    /// <summary>
    /// Remove a field from a dictionary.
    ///
    /// Performs a no-op if <paramref name="dictionaryName"/> or <paramref name="field"/> does not exist.
    /// </summary>
    /// <param name="cacheName">Name of the cache to lookup the dictionary in.</param>
    /// <param name="dictionaryName">Name of the dictionary to remove the field from.</param>
    /// <param name="field">Name of the field to remove from the dictionary.</param>
    /// <returns>Task representing the result of the cache operation.</returns>
    public Task<CacheDictionaryRemoveFieldResponse> DictionaryRemoveFieldAsync(string cacheName, string dictionaryName, byte[] field);

    /// <inheritdoc cref="DictionaryRemoveFieldAsync(string, string, byte[])"/>
    public Task<CacheDictionaryRemoveFieldResponse> DictionaryRemoveFieldAsync(string cacheName, string dictionaryName, string field);

    /// <summary>
    /// Remove fields from a dictionary.
    ///
    /// Performs a no-op if <paramref name="dictionaryName"/> or a particular field does not exist.
    /// </summary>
    /// <param name="cacheName">Name of the cache to lookup the dictionary in.</param>
    /// <param name="dictionaryName">Name of the dictionary to remove the field from.</param>
    /// <param name="fields">Name of the fields to remove from the dictionary.</param>
    /// <returns>Task representing the result of the cache operation.</returns>
    public Task<CacheDictionaryRemoveFieldsResponse> DictionaryRemoveFieldsAsync(string cacheName, string dictionaryName, IEnumerable<byte[]> fields);

    /// <inheritdoc cref="DictionaryRemoveFieldsAsync(string, string, IEnumerable{byte[]})"/>
    public Task<CacheDictionaryRemoveFieldsResponse> DictionaryRemoveFieldsAsync(string cacheName, string dictionaryName, IEnumerable<string> fields);

    /// <summary>
    /// Add an element to a set in the cache.
    ///
    /// After this operation, the set will contain the union
    /// of the element passed in and the elements of the set.
    /// </summary>
    /// <inheritdoc cref="DictionarySetFieldAsync(string, string, byte[], byte[], CollectionTtl)" path="remark"/>
    /// <param name="cacheName">Name of the cache to store the set in.</param>
    /// <param name="setName">The set to add the element to.</param>
    /// <param name="element">The data to add to the set.</param>
    /// <param name="ttl">TTL for the set in cache. This TTL takes precedence over the TTL used when initializing a cache client. Defaults to client TTL.</param>
    /// <returns>Task representing the result of the cache operation.</returns>
    public Task<CacheSetAddElementResponse> SetAddElementAsync(string cacheName, string setName, byte[] element, CollectionTtl ttl = default(CollectionTtl));

    /// <inheritdoc cref="SetAddElementAsync(string, string, byte[], CollectionTtl)"/>
    public Task<CacheSetAddElementResponse> SetAddElementAsync(string cacheName, string setName, string element, CollectionTtl ttl = default(CollectionTtl));

    /// <summary>
    /// Add several elements to a set in the cache.
    ///
    /// After this operation, the set will contain the union
    /// of the elements passed in and the elements of the set.
    /// </summary>
    /// <inheritdoc cref="DictionarySetFieldAsync(string, string, byte[], byte[], CollectionTtl)" path="remark"/>
    /// <param name="cacheName">Name of the cache to store the set in.</param>
    /// <param name="setName">The set to add elements to.</param>
    /// <param name="elements">The data to add to the set.</param>
    /// <param name="ttl">TTL for the set in cache. This TTL takes precedence over the TTL used when initializing a cache client. Defaults to client TTL.</param>
    /// <returns>Task representing the result of the cache operation.</returns>
    public Task<CacheSetAddElementsResponse> SetAddElementsAsync(string cacheName, string setName, IEnumerable<byte[]> elements, CollectionTtl ttl = default(CollectionTtl));

    /// <inheritdoc cref="SetAddElementsAsync(string, string, IEnumerable{byte[]}, CollectionTtl)"/>
    public Task<CacheSetAddElementsResponse> SetAddElementsAsync(string cacheName, string setName, IEnumerable<string> elements, CollectionTtl ttl = default(CollectionTtl));

    /// <summary>
    /// Remove an element from a set.
    ///
    /// Performs a no-op if <paramref name="setName"/> or <paramref name="element"/> does not exist.
    /// </summary>
    /// <param name="cacheName">Name of the cache to lookup the set in.</param>
    /// <param name="setName">The set to remove the element from.</param>
    /// <param name="element">The data to remove from the set.</param>
    /// <returns>Task representing the result of the cache operation.</returns>
    public Task<CacheSetRemoveElementResponse> SetRemoveElementAsync(string cacheName, string setName, byte[] element);

    /// <inheritdoc cref="SetRemoveElementAsync(string, string, byte[])"/>
    public Task<CacheSetRemoveElementResponse> SetRemoveElementAsync(string cacheName, string setName, string element);

    /// <summary>
    /// Remove elements from a set.
    ///
    /// Performs a no-op if <paramref name="setName"/> or any of <paramref name="elements"/> do not exist.
    /// </summary>
    /// <param name="cacheName">Name of the cache to lookup the set in.</param>
    /// <param name="setName">The set to remove the elements from.</param>
    /// <param name="elements">The data to remove from the set.</param>
    /// <returns>Task representing the result of the cache operation.</returns>
    public Task<CacheSetRemoveElementsResponse> SetRemoveElementsAsync(string cacheName, string setName, IEnumerable<byte[]> elements);

    /// <inheritdoc cref="SetRemoveElementsAsync(string, string, IEnumerable{byte[]})"/>
    public Task<CacheSetRemoveElementsResponse> SetRemoveElementsAsync(string cacheName, string setName, IEnumerable<string> elements);

    /// <summary>
    /// Fetch the entire set from the cache.
    /// </summary>
    /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
    /// <param name="setName">The set to fetch.</param>
    /// <returns>Task representing with the status of the fetch operation and the associated set.</returns>
    public Task<CacheSetFetchResponse> SetFetchAsync(string cacheName, string setName);

    /// <summary>
    /// Push multiple values to the beginning of a list.
    /// </summary>
    /// <param name="cacheName">Name of the cache to store the list in.</param>
    /// <param name="listName">The list to concatenate the values on.</param>
    /// <param name="values">The values to concatenate to the front of the list.</param>
    /// <param name="ttl">TTL for the list in cache. This TTL takes precedence over the TTL used when initializing a cache client. Defaults to client TTL.</param>
    /// <param name="truncateBackToSize">Ensure the list does not exceed this length. Remove excess from the end of the list. Must be a positive number.</param>
    /// <returns>Task representing the result of the push operation.</returns>
    public Task<CacheListConcatenateFrontResponse> ListConcatenateFrontAsync(string cacheName, string listName, IEnumerable<byte[]> values, int? truncateBackToSize = null, CollectionTtl ttl = default(CollectionTtl));

    /// <inheritdoc cref="ListConcatenateFrontAsync(string, string, IEnumerable{byte[]}, int?, CollectionTtl)"/>
    public Task<CacheListConcatenateFrontResponse> ListConcatenateFrontAsync(string cacheName, string listName, IEnumerable<string> value, int? truncateBackToSize = null, CollectionTtl ttl = default(CollectionTtl));

    /// <summary>
    /// Push multiple values to the back of a list.
    /// </summary>
    /// <param name="cacheName">Name of the cache to store the list in.</param>
    /// <param name="listName">The list to concatenate the values on.</param>
    /// <param name="values">The values to concatenate to the front of the list.</param>
    /// <param name="ttl">TTL for the list in cache. This TTL takes precedence over the TTL used when initializing a cache client. Defaults to client TTL.</param>
    /// <param name="truncateFrontToSize">Ensure the list does not exceed this length. Remove excess from the front of the list. Must be a positive number.</param>
    /// <returns>Task representing the result of the push operation.</returns>
    public Task<CacheListConcatenateBackResponse> ListConcatenateBackAsync(string cacheName, string listName, IEnumerable<byte[]> values, int? truncateFrontToSize = null, CollectionTtl ttl = default(CollectionTtl));

    /// <inheritdoc cref="ListConcatenateBackAsync(string, string, IEnumerable{byte[]}, int?, CollectionTtl)"/>
    public Task<CacheListConcatenateBackResponse> ListConcatenateBackAsync(string cacheName, string listName, IEnumerable<string> value, int? truncateFrontToSize = null, CollectionTtl ttl = default(CollectionTtl));

    /// <summary>
    /// Push a value to the beginning of a list.
    /// </summary>
    /// <inheritdoc cref="DictionarySetFieldAsync(string, string, byte[], byte[], CollectionTtl)" path="remark"/>
    /// <param name="cacheName">Name of the cache to store the list in.</param>
    /// <param name="listName">The list to push the value on.</param>
    /// <param name="value">The value to push to the front of the list.</param>
    /// <param name="ttl">TTL for the list in cache. This TTL takes precedence over the TTL used when initializing a cache client. Defaults to client TTL.</param>
    /// <param name="truncateBackToSize">Ensure the list does not exceed this length. Remove excess from the end of the list. Must be a positive number.</param>
    /// <returns>Task representing the result of the push operation.</returns>
    public Task<CacheListPushFrontResponse> ListPushFrontAsync(string cacheName, string listName, byte[] value, int? truncateBackToSize = null, CollectionTtl ttl = default(CollectionTtl));

    /// <inheritdoc cref="ListPushFrontAsync(string, string, byte[], int?, CollectionTtl)"/>
    public Task<CacheListPushFrontResponse> ListPushFrontAsync(string cacheName, string listName, string value, int? truncateBackToSize = null, CollectionTtl ttl = default(CollectionTtl));

    /// <summary>
    /// Push a value to the end of a list.
    /// </summary>
    /// <inheritdoc cref="DictionarySetFieldAsync(string, string, byte[], byte[], CollectionTtl)" path="remark"/>
    /// <param name="cacheName">Name of the cache to store the list in.</param>
    /// <param name="listName">The list to push the value on.</param>
    /// <param name="value">The value to push to the back of the list.</param>
    /// <param name="ttl">TTL for the list in cache. This TTL takes precedence over the TTL used when initializing a cache client. Defaults to client TTL.</param>
    /// <param name="truncateFrontToSize">Ensure the list does not exceed this length. Remove excess from the beginning of the list. Must be a positive number.</param>
    /// <returns>Task representing the result of the push operation.</returns>
    public Task<CacheListPushBackResponse> ListPushBackAsync(string cacheName, string listName, byte[] value, int? truncateFrontToSize = null, CollectionTtl ttl = default(CollectionTtl));

    /// <inheritdoc cref="ListPushBackAsync(string, string, byte[], int?, CollectionTtl)"/>
    public Task<CacheListPushBackResponse> ListPushBackAsync(string cacheName, string listName, string value, int? truncateFrontToSize = null, CollectionTtl ttl = default(CollectionTtl));

    /// <summary>
    /// Retrieve and remove the first item from a list.
    /// </summary>
    /// <param name="cacheName">Name of the cache to read the list from.</param>
    /// <param name="listName">The list to pop from.</param>
    /// <returns>Task representing the status and associated value for the pop operation.</returns>
    public Task<CacheListPopFrontResponse> ListPopFrontAsync(string cacheName, string listName);

    /// <summary>
    /// Retrieve and remove the last item from a list.
    /// </summary>
    /// <param name="cacheName">Name of the cache to read the list from.</param>
    /// <param name="listName">The list to pop from.</param>
    /// <returns>Task representing the status and associated value for the pop operation.</returns>
    public Task<CacheListPopBackResponse> ListPopBackAsync(string cacheName, string listName);

    /// <summary>
    /// Fetch the entire list from the cache.
    /// </summary>
    /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
    /// <param name="listName">The list to fetch.</param>
    /// <returns>Task representing with the status of the fetch operation and the associated list.</returns>
    public Task<CacheListFetchResponse> ListFetchAsync(string cacheName, string listName);

    /// <summary>
    /// Remove all elements in a list equal to a particular value.
    /// </summary>
    /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
    /// <param name="listName">The list to remove elements from.</param>
    /// <param name="value">The value to completely remove from the list.</param>
    /// <returns>Task representing the result of the cache operation.</returns>
    public Task<CacheListRemoveValueResponse> ListRemoveValueAsync(string cacheName, string listName, byte[] value);

    /// <inheritdoc cref="ListRemoveValueAsync(string, string, byte[])"/>
    public Task<CacheListRemoveValueResponse> ListRemoveValueAsync(string cacheName, string listName, string value);

    /// <summary>
    /// Calculate the length of a list in the cache.
    ///
    /// A list that does not exist is interpreted to have length 0.
    /// </summary>
    /// <param name="cacheName">Name of the cache to perform the lookup in.</param>
    /// <param name="listName">The list to calculate length.</param>
    /// <returns>Task representing the length of the list.</returns>
    public Task<CacheListLengthResponse> ListLengthAsync(string cacheName, string listName);
}
