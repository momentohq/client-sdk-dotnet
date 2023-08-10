using System.Collections.Generic;
using System.Threading.Tasks;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Requests;

namespace Momento.Sdk.Tests;

/// <summary>
/// Client call wrapper to make test expectations easier to write.
/// </summary>
public class TestCacheClient : ICacheClient
{
    private readonly CacheClient client;

    public TestCacheClient(IConfiguration config, ICredentialProvider authProvider, TimeSpan defaultTtl)
    {
        client = new CacheClient(config, authProvider, defaultTtl);
    }

    /// <summary>
    /// Wait for the service to settle any slow replication or intermediate response cache.
    /// Momento doesn't have strict read-after-write consistency guarantees, but a little
    /// time should settle it in normal situations.
    /// </summary>
    /// <returns>task with action's result executed after the quiesce period</returns>
    private Task<T> QuiesceAndThenRun<T>(Func<Task<T>> action) {
        return ((Func<Task<T>>)(async () =>
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10));
            return await action();
        }))();
    }

    public Task<CreateCacheResponse> CreateCacheAsync(string cacheName)
    {
        return ((ICacheClient)client).CreateCacheAsync(cacheName);
    }

    public Task<CacheDeleteResponse> DeleteAsync(string cacheName, byte[] key)
    {
        return ((ICacheClient)client).DeleteAsync(cacheName, key);
    }

    public Task<CacheDeleteResponse> DeleteAsync(string cacheName, string key)
    {
        return ((ICacheClient)client).DeleteAsync(cacheName, key);
    }

    public Task<DeleteCacheResponse> DeleteCacheAsync(string cacheName)
    {
        return ((ICacheClient)client).DeleteCacheAsync(cacheName);
    }

    public Task<CacheDictionaryFetchResponse> DictionaryFetchAsync(string cacheName, string dictionaryName)
    {
        return ((ICacheClient)client).DictionaryFetchAsync(cacheName, dictionaryName);
    }

    public Task<CacheDictionaryGetFieldResponse> DictionaryGetFieldAsync(string cacheName, string dictionaryName, byte[] field)
    {
        return ((ICacheClient)client).DictionaryGetFieldAsync(cacheName, dictionaryName, field);
    }

    public Task<CacheDictionaryGetFieldResponse> DictionaryGetFieldAsync(string cacheName, string dictionaryName, string field)
    {
        return ((ICacheClient)client).DictionaryGetFieldAsync(cacheName, dictionaryName, field);
    }

    public Task<CacheDictionaryGetFieldsResponse> DictionaryGetFieldsAsync(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        return ((ICacheClient)client).DictionaryGetFieldsAsync(cacheName, dictionaryName, fields);
    }

    public Task<CacheDictionaryGetFieldsResponse> DictionaryGetFieldsAsync(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        return ((ICacheClient)client).DictionaryGetFieldsAsync(cacheName, dictionaryName, fields);
    }

    public Task<CacheDictionaryIncrementResponse> DictionaryIncrementAsync(string cacheName, string dictionaryName, string field, long amount = 1, CollectionTtl ttl = default)
    {
        return ((ICacheClient)client).DictionaryIncrementAsync(cacheName, dictionaryName, field, amount, ttl);
    }

    public Task<CacheDictionaryLengthResponse> DictionaryLengthAsync(string cacheName, string dictionaryName)
    {
        return ((ICacheClient)client).DictionaryLengthAsync(cacheName, dictionaryName);
    }

    public Task<CacheDictionaryRemoveFieldResponse> DictionaryRemoveFieldAsync(string cacheName, string dictionaryName, byte[] field)
    {
        return ((ICacheClient)client).DictionaryRemoveFieldAsync(cacheName, dictionaryName, field);
    }

    public Task<CacheDictionaryRemoveFieldResponse> DictionaryRemoveFieldAsync(string cacheName, string dictionaryName, string field)
    {
        return ((ICacheClient)client).DictionaryRemoveFieldAsync(cacheName, dictionaryName, field);
    }

    public Task<CacheDictionaryRemoveFieldsResponse> DictionaryRemoveFieldsAsync(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        return ((ICacheClient)client).DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fields);
    }

    public Task<CacheDictionaryRemoveFieldsResponse> DictionaryRemoveFieldsAsync(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        return ((ICacheClient)client).DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fields);
    }

    public Task<CacheDictionarySetFieldResponse> DictionarySetFieldAsync(string cacheName, string dictionaryName, byte[] field, byte[] value, CollectionTtl ttl = default)
    {
        return ((ICacheClient)client).DictionarySetFieldAsync(cacheName, dictionaryName, field, value, ttl);
    }

    public Task<CacheDictionarySetFieldResponse> DictionarySetFieldAsync(string cacheName, string dictionaryName, string field, string value, CollectionTtl ttl = default)
    {
        return ((ICacheClient)client).DictionarySetFieldAsync(cacheName, dictionaryName, field, value, ttl);
    }

    public Task<CacheDictionarySetFieldResponse> DictionarySetFieldAsync(string cacheName, string dictionaryName, string field, byte[] value, CollectionTtl ttl = default)
    {
        return ((ICacheClient)client).DictionarySetFieldAsync(cacheName, dictionaryName, field, value, ttl);
    }

    public Task<CacheDictionarySetFieldsResponse> DictionarySetFieldsAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<byte[], byte[]>> elements, CollectionTtl ttl = default)
    {
        return ((ICacheClient)client).DictionarySetFieldsAsync(cacheName, dictionaryName, elements, ttl);
    }

    public Task<CacheDictionarySetFieldsResponse> DictionarySetFieldsAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, string>> elements, CollectionTtl ttl = default)
    {
        return ((ICacheClient)client).DictionarySetFieldsAsync(cacheName, dictionaryName, elements, ttl);
    }

    public Task<CacheDictionarySetFieldsResponse> DictionarySetFieldsAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, byte[]>> elements, CollectionTtl ttl = default)
    {
        return ((ICacheClient)client).DictionarySetFieldsAsync(cacheName, dictionaryName, elements, ttl);
    }

    public void Dispose()
    {
        ((IDisposable)client).Dispose();
    }

    public Task<FlushCacheResponse> FlushCacheAsync(string cacheName)
    {
        return ((ICacheClient)client).FlushCacheAsync(cacheName);
    }

    public Task<CacheGetResponse> GetAsync(string cacheName, byte[] key)
    {
        return QuiesceAndThenRun(() => ((ICacheClient)client).GetAsync(cacheName, key));
    }

    public Task<CacheGetResponse> GetAsync(string cacheName, string key)
    {
        return ((ICacheClient)client).GetAsync(cacheName, key);
    }

    public Task<CacheIncrementResponse> IncrementAsync(string cacheName, string field, long amount = 1, TimeSpan? ttl = null)
    {
        return ((ICacheClient)client).IncrementAsync(cacheName, field, amount, ttl);
    }

    public Task<CacheIncrementResponse> IncrementAsync(string cacheName, byte[] field, long amount = 1, TimeSpan? ttl = null)
    {
        return ((ICacheClient)client).IncrementAsync(cacheName, field, amount, ttl);
    }

    public Task<CacheItemGetTtlResponse> ItemGetTtlAsync(string cacheName, byte[] key)
    {
        return ((ICacheClient)client).ItemGetTtlAsync(cacheName, key);
    }

    public Task<CacheItemGetTtlResponse> ItemGetTtlAsync(string cacheName, string key)
    {
        return ((ICacheClient)client).ItemGetTtlAsync(cacheName, key);
    }

    public Task<CacheKeyExistsResponse> KeyExistsAsync(string cacheName, byte[] key)
    {
        return ((ICacheClient)client).KeyExistsAsync(cacheName, key);
    }

    public Task<CacheKeyExistsResponse> KeyExistsAsync(string cacheName, string key)
    {
        return ((ICacheClient)client).KeyExistsAsync(cacheName, key);
    }

    public Task<CacheKeysExistResponse> KeysExistAsync(string cacheName, IEnumerable<byte[]> keys)
    {
        return ((ICacheClient)client).KeysExistAsync(cacheName, keys);
    }

    public Task<CacheKeysExistResponse> KeysExistAsync(string cacheName, IEnumerable<string> keys)
    {
        return ((ICacheClient)client).KeysExistAsync(cacheName, keys);
    }

    public Task<ListCachesResponse> ListCachesAsync()
    {
        return ((ICacheClient)client).ListCachesAsync();
    }

    public Task<CacheListConcatenateBackResponse> ListConcatenateBackAsync(string cacheName, string listName, IEnumerable<byte[]> values, int? truncateFrontToSize = null, CollectionTtl ttl = default)
    {
        return ((ICacheClient)client).ListConcatenateBackAsync(cacheName, listName, values, truncateFrontToSize, ttl);
    }

    public Task<CacheListConcatenateBackResponse> ListConcatenateBackAsync(string cacheName, string listName, IEnumerable<string> values, int? truncateFrontToSize = null, CollectionTtl ttl = default)
    {
        return ((ICacheClient)client).ListConcatenateBackAsync(cacheName, listName, values, truncateFrontToSize, ttl);
    }

    public Task<CacheListConcatenateFrontResponse> ListConcatenateFrontAsync(string cacheName, string listName, IEnumerable<byte[]> values, int? truncateBackToSize = null, CollectionTtl ttl = default)
    {
        return ((ICacheClient)client).ListConcatenateFrontAsync(cacheName, listName, values, truncateBackToSize, ttl);
    }

    public Task<CacheListConcatenateFrontResponse> ListConcatenateFrontAsync(string cacheName, string listName, IEnumerable<string> values, int? truncateBackToSize = null, CollectionTtl ttl = default)
    {
        return ((ICacheClient)client).ListConcatenateFrontAsync(cacheName, listName, values, truncateBackToSize, ttl);
    }

    public Task<CacheListFetchResponse> ListFetchAsync(string cacheName, string listName, int? startIndex = null, int? endIndex = null)
    {
        return ((ICacheClient)client).ListFetchAsync(cacheName, listName, startIndex, endIndex);
    }

    public Task<CacheListLengthResponse> ListLengthAsync(string cacheName, string listName)
    {
        return ((ICacheClient)client).ListLengthAsync(cacheName, listName);
    }

    public Task<CacheListPopBackResponse> ListPopBackAsync(string cacheName, string listName)
    {
        return ((ICacheClient)client).ListPopBackAsync(cacheName, listName);
    }

    public Task<CacheListPopFrontResponse> ListPopFrontAsync(string cacheName, string listName)
    {
        return ((ICacheClient)client).ListPopFrontAsync(cacheName, listName);
    }

    public Task<CacheListPushBackResponse> ListPushBackAsync(string cacheName, string listName, byte[] value, int? truncateFrontToSize = null, CollectionTtl ttl = default)
    {
        return ((ICacheClient)client).ListPushBackAsync(cacheName, listName, value, truncateFrontToSize, ttl);
    }

    public Task<CacheListPushBackResponse> ListPushBackAsync(string cacheName, string listName, string value, int? truncateFrontToSize = null, CollectionTtl ttl = default)
    {
        return ((ICacheClient)client).ListPushBackAsync(cacheName, listName, value, truncateFrontToSize, ttl);
    }

    public Task<CacheListPushFrontResponse> ListPushFrontAsync(string cacheName, string listName, byte[] value, int? truncateBackToSize = null, CollectionTtl ttl = default)
    {
        return ((ICacheClient)client).ListPushFrontAsync(cacheName, listName, value, truncateBackToSize, ttl);
    }

    public Task<CacheListPushFrontResponse> ListPushFrontAsync(string cacheName, string listName, string value, int? truncateBackToSize = null, CollectionTtl ttl = default)
    {
        return ((ICacheClient)client).ListPushFrontAsync(cacheName, listName, value, truncateBackToSize, ttl);
    }

    public Task<CacheListRemoveValueResponse> ListRemoveValueAsync(string cacheName, string listName, byte[] value)
    {
        return ((ICacheClient)client).ListRemoveValueAsync(cacheName, listName, value);
    }

    public Task<CacheListRemoveValueResponse> ListRemoveValueAsync(string cacheName, string listName, string value)
    {
        return ((ICacheClient)client).ListRemoveValueAsync(cacheName, listName, value);
    }

    public Task<CacheListRetainResponse> ListRetainAsync(string cacheName, string listName, int? startIndex = null, int? endIndex = null)
    {
        return ((ICacheClient)client).ListRetainAsync(cacheName, listName, startIndex, endIndex);
    }

    public Task<CacheSetAddElementResponse> SetAddElementAsync(string cacheName, string setName, byte[] element, CollectionTtl ttl = default)
    {
        return ((ICacheClient)client).SetAddElementAsync(cacheName, setName, element, ttl);
    }

    public Task<CacheSetAddElementResponse> SetAddElementAsync(string cacheName, string setName, string element, CollectionTtl ttl = default)
    {
        return ((ICacheClient)client).SetAddElementAsync(cacheName, setName, element, ttl);
    }

    public Task<CacheSetAddElementsResponse> SetAddElementsAsync(string cacheName, string setName, IEnumerable<byte[]> elements, CollectionTtl ttl = default)
    {
        return ((ICacheClient)client).SetAddElementsAsync(cacheName, setName, elements, ttl);
    }

    public Task<CacheSetAddElementsResponse> SetAddElementsAsync(string cacheName, string setName, IEnumerable<string> elements, CollectionTtl ttl = default)
    {
        return ((ICacheClient)client).SetAddElementsAsync(cacheName, setName, elements, ttl);
    }

    public Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value, TimeSpan? ttl = null)
    {
        return ((ICacheClient)client).SetAsync(cacheName, key, value, ttl);
    }

    public Task<CacheSetResponse> SetAsync(string cacheName, string key, byte[] value, TimeSpan? ttl = null)
    {
        return ((ICacheClient)client).SetAsync(cacheName, key, value, ttl);
    }

    public Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, TimeSpan? ttl = null)
    {
        return ((ICacheClient)client).SetAsync(cacheName, key, value, ttl);
    }

    public Task<CacheSetFetchResponse> SetFetchAsync(string cacheName, string setName)
    {
        return ((ICacheClient)client).SetFetchAsync(cacheName, setName);
    }

    public Task<CacheSetIfNotExistsResponse> SetIfNotExistsAsync(string cacheName, string key, string value, TimeSpan? ttl = null)
    {
        return ((ICacheClient)client).SetIfNotExistsAsync(cacheName, key, value, ttl);
    }

    public Task<CacheSetIfNotExistsResponse> SetIfNotExistsAsync(string cacheName, byte[] key, string value, TimeSpan? ttl = null)
    {
        return ((ICacheClient)client).SetIfNotExistsAsync(cacheName, key, value, ttl);
    }

    public Task<CacheSetIfNotExistsResponse> SetIfNotExistsAsync(string cacheName, string key, byte[] value, TimeSpan? ttl = null)
    {
        return ((ICacheClient)client).SetIfNotExistsAsync(cacheName, key, value, ttl);
    }

    public Task<CacheSetIfNotExistsResponse> SetIfNotExistsAsync(string cacheName, byte[] key, byte[] value, TimeSpan? ttl = null)
    {
        return ((ICacheClient)client).SetIfNotExistsAsync(cacheName, key, value, ttl);
    }

    public Task<CacheSetLengthResponse> SetLengthAsync(string cacheName, string setName)
    {
        return ((ICacheClient)client).SetLengthAsync(cacheName, setName);
    }

    public Task<CacheSetRemoveElementResponse> SetRemoveElementAsync(string cacheName, string setName, byte[] element)
    {
        return ((ICacheClient)client).SetRemoveElementAsync(cacheName, setName, element);
    }

    public Task<CacheSetRemoveElementResponse> SetRemoveElementAsync(string cacheName, string setName, string element)
    {
        return ((ICacheClient)client).SetRemoveElementAsync(cacheName, setName, element);
    }

    public Task<CacheSetRemoveElementsResponse> SetRemoveElementsAsync(string cacheName, string setName, IEnumerable<byte[]> elements)
    {
        return ((ICacheClient)client).SetRemoveElementsAsync(cacheName, setName, elements);
    }

    public Task<CacheSetRemoveElementsResponse> SetRemoveElementsAsync(string cacheName, string setName, IEnumerable<string> elements)
    {
        return ((ICacheClient)client).SetRemoveElementsAsync(cacheName, setName, elements);
    }

    public Task<CacheUpdateTtlResponse> UpdateTtlAsync(string cacheName, byte[] key, TimeSpan ttl)
    {
        return ((ICacheClient)client).UpdateTtlAsync(cacheName, key, ttl);
    }

    public Task<CacheUpdateTtlResponse> UpdateTtlAsync(string cacheName, string key, TimeSpan ttl)
    {
        return ((ICacheClient)client).UpdateTtlAsync(cacheName, key, ttl);
    }
}
