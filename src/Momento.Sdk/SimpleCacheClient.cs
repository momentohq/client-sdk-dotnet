using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;
using Momento.Sdk.Internal.ExtensionMethods;
using Momento.Sdk.Requests;
using Momento.Sdk.Responses;

namespace Momento.Sdk;

/// <summary>
/// Client to perform control and data operations against the Simple Cache Service.
/// 
/// See <see href="https://github.com/momentohq/client-sdk-examples/tree/main/dotnet/MomentoExamples">the examples repo</see> for complete workflows.
/// </summary>
public class SimpleCacheClient : ISimpleCacheClient
{
    private readonly ScsControlClient controlClient;
    private readonly List<ScsDataClient> dataClients;
    private int nextDataClientIndex = 0;

    /// <inheritdoc cref="Momento.Sdk.Config.IConfiguration" />
    protected readonly IConfiguration config;
    /// <inheritdoc cref="Microsoft.Extensions.Logging.ILogger" />
    protected readonly ILogger _logger;


    /// <summary>
    /// Client to perform operations against the Simple Cache Service.
    /// </summary>
    /// <param name="config">Configuration to use for the transport, retries, middlewares. See <see cref="Configurations"/> for out-of-the-box configuration choices, eg <see cref="Configurations.Laptop.Latest"/></param>
    /// <param name="authProvider">Momento auth provider.</param>
    /// <param name="defaultTtl">Default time to live for the item in cache.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="defaultTtl"/> is zero or negative.</exception>
    public SimpleCacheClient(IConfiguration config, ICredentialProvider authProvider, TimeSpan defaultTtl)
    {
        this.config = config;
        var _loggerFactory = config.LoggerFactory;
        this._logger = _loggerFactory.CreateLogger<SimpleCacheClient>();
        Utils.ArgumentStrictlyPositive(defaultTtl, "defaultTtl");
        this.controlClient = new(_loggerFactory, authProvider.AuthToken, authProvider.ControlEndpoint);
        this.dataClients = new List<ScsDataClient>();
        for (var i = 1; i <= config.TransportStrategy.GrpcConfig.MinNumGrpcChannels; i++)
        {
            this.dataClients.Add(new(config, authProvider.AuthToken, authProvider.CacheEndpoint, defaultTtl));
        }
    }

    /// <inheritdoc />
    public async Task<CreateCacheResponse> CreateCacheAsync(string cacheName)
    {
        return await this.controlClient.CreateCacheAsync(cacheName);
    }

    /// <inheritdoc />
    public async Task<DeleteCacheResponse> DeleteCacheAsync(string cacheName)
    {
        return await this.controlClient.DeleteCacheAsync(cacheName);
    }

    /// <inheritdoc />
    public async Task<FlushCacheResponse> FlushCacheAsync(string cacheName)
    {
        return await this.controlClient.FlushCacheAsync(cacheName);
    }

    /// <inheritdoc />
    public async Task<ListCachesResponse> ListCachesAsync(string? nextPageToken = null)
    {
        return await this.controlClient.ListCachesAsync(nextPageToken);
    }

    /// <inheritdoc />
    public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value, TimeSpan? ttl = null)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(key, nameof(key));
            Utils.ArgumentNotNull(value, nameof(value));
            Utils.ArgumentStrictlyPositive(ttl, nameof(ttl));
        }
        catch (ArgumentNullException e)
        {
            return new CacheSetResponse.Error(new InvalidArgumentException(e.Message));
        }
        catch (ArgumentOutOfRangeException e)
        {
            return new CacheSetResponse.Error(new InvalidArgumentException(e.Message));
        }
        return await this.NextDataClient().SetAsync(cacheName, key, value, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheGetResponse> GetAsync(string cacheName, byte[] key)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(key, nameof(key));
        }
        catch (ArgumentNullException e)
        {
            return new CacheGetResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.NextDataClient().GetAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, byte[] key)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(key, nameof(key));
        }
        catch (ArgumentNullException e)
        {
            return new CacheDeleteResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.NextDataClient().DeleteAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, TimeSpan? ttl = null)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(key, nameof(key));
            Utils.ArgumentNotNull(value, nameof(value));
            Utils.ArgumentStrictlyPositive(ttl, nameof(ttl));
        }
        catch (ArgumentNullException e)
        {
            return new CacheSetResponse.Error(new InvalidArgumentException(e.Message));
        }
        catch (ArgumentOutOfRangeException e)
        {
            return new CacheSetResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.NextDataClient().SetAsync(cacheName, key, value, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheGetResponse> GetAsync(string cacheName, string key)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(key, nameof(key));
        }
        catch (ArgumentNullException e)
        {
            return new CacheGetResponse.Error(new InvalidArgumentException(e.Message));
        }
        return await this.NextDataClient().GetAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, string key)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(key, nameof(key));
        }
        catch (ArgumentNullException e)
        {
            return new CacheDeleteResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.NextDataClient().DeleteAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, byte[] value, TimeSpan? ttl = null)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(key, nameof(key));
            Utils.ArgumentNotNull(value, nameof(value));
            Utils.ArgumentStrictlyPositive(ttl, nameof(ttl));
        }
        catch (ArgumentNullException e)
        {
            return new CacheSetResponse.Error(new InvalidArgumentException(e.Message));
        }
        catch (ArgumentOutOfRangeException e)
        {
            return new CacheSetResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.NextDataClient().SetAsync(cacheName, key, value, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheDictionarySetFieldResponse> DictionarySetFieldAsync(string cacheName, string dictionaryName, byte[] field, byte[] value, CollectionTtl ttl = default(CollectionTtl))
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
            Utils.ArgumentNotNull(field, nameof(field));
            Utils.ArgumentNotNull(value, nameof(value));
        }
        catch (ArgumentNullException e)
        {
            return new CacheDictionarySetFieldResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.DictionarySetFieldAsync(cacheName, dictionaryName, field, value, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheDictionarySetFieldResponse> DictionarySetFieldAsync(string cacheName, string dictionaryName, string field, string value, CollectionTtl ttl = default(CollectionTtl))
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
            Utils.ArgumentNotNull(field, nameof(field));
            Utils.ArgumentNotNull(value, nameof(value));
        }
        catch (ArgumentNullException e)
        {
            return new CacheDictionarySetFieldResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.DictionarySetFieldAsync(cacheName, dictionaryName, field, value, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheDictionarySetFieldResponse> DictionarySetFieldAsync(string cacheName, string dictionaryName, string field, byte[] value, CollectionTtl ttl = default(CollectionTtl))
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
            Utils.ArgumentNotNull(field, nameof(field));
            Utils.ArgumentNotNull(value, nameof(value));
        }
        catch (ArgumentNullException e)
        {
            return new CacheDictionarySetFieldResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.DictionarySetFieldAsync(cacheName, dictionaryName, field, value, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheDictionaryGetFieldResponse> DictionaryGetFieldAsync(string cacheName, string dictionaryName, byte[] field)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
            Utils.ArgumentNotNull(field, nameof(field));
        }
        catch (ArgumentNullException e)
        {
            return new CacheDictionaryGetFieldResponse.Error(field?.ToByteString(), new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
    }

    /// <inheritdoc />
    public async Task<CacheDictionaryGetFieldResponse> DictionaryGetFieldAsync(string cacheName, string dictionaryName, string field)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
            Utils.ArgumentNotNull(field, nameof(field));
        }
        catch (ArgumentNullException e)
        {
            return new CacheDictionaryGetFieldResponse.Error(field?.ToByteString(), new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
    }

    /// <inheritdoc />
    public async Task<CacheDictionarySetFieldsResponse> DictionarySetFieldsAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<byte[], byte[]>> items, CollectionTtl ttl = default(CollectionTtl))
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
            Utils.ArgumentNotNull(items, nameof(items));
            Utils.KeysAndValuesNotNull(items, nameof(items));
        }
        catch (ArgumentNullException e)
        {
            return new CacheDictionarySetFieldsResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.DictionarySetFieldsAsync(cacheName, dictionaryName, items, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheDictionarySetFieldsResponse> DictionarySetFieldsAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, string>> items, CollectionTtl ttl = default(CollectionTtl))
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
            Utils.ArgumentNotNull(items, nameof(items));
            Utils.KeysAndValuesNotNull(items, nameof(items));
        }
        catch (ArgumentNullException e)
        {
            return new CacheDictionarySetFieldsResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.DictionarySetFieldsAsync(cacheName, dictionaryName, items, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheDictionarySetFieldsResponse> DictionarySetFieldsAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, byte[]>> items, CollectionTtl ttl = default(CollectionTtl))
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
            Utils.ArgumentNotNull(items, nameof(items));
            Utils.KeysAndValuesNotNull(items, nameof(items));
        }
        catch (ArgumentNullException e)
        {
            return new CacheDictionarySetFieldsResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.DictionarySetFieldsAsync(cacheName, dictionaryName, items, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheDictionaryIncrementResponse> DictionaryIncrementAsync(string cacheName, string dictionaryName, string field, long amount = 1, CollectionTtl ttl = default(CollectionTtl))
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
            Utils.ArgumentNotNull(field, nameof(field));
        }
        catch (ArgumentNullException e)
        {
            return new CacheDictionaryIncrementResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.DictionaryIncrementAsync(cacheName, dictionaryName, field, amount, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheDictionaryGetFieldsResponse> DictionaryGetFieldsAsync(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
            Utils.ArgumentNotNull(fields, nameof(fields));
            Utils.ElementsNotNull(fields, nameof(fields));
        }
        catch (ArgumentNullException e)
        {
            return new CacheDictionaryGetFieldsResponse.Error(new InvalidArgumentException(e.Message));
        }
        return await this.dataClient.DictionaryGetFieldsAsync(cacheName, dictionaryName, fields);
    }

    /// <inheritdoc />
    public async Task<CacheDictionaryGetFieldsResponse> DictionaryGetFieldsAsync(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
            Utils.ArgumentNotNull(fields, nameof(fields));
            Utils.ElementsNotNull(fields, nameof(fields));
        }
        catch (ArgumentNullException e)
        {
            return new CacheDictionaryGetFieldsResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.DictionaryGetFieldsAsync(cacheName, dictionaryName, fields);
    }

    /// <inheritdoc />
    public async Task<CacheDictionaryFetchResponse> DictionaryFetchAsync(string cacheName, string dictionaryName)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
        }
        catch (ArgumentNullException e)
        {
            return new CacheDictionaryFetchResponse.Error(new InvalidArgumentException(e.Message));
        }


        return await this.dataClient.DictionaryFetchAsync(cacheName, dictionaryName);
    }

    /// <inheritdoc />
    public async Task<CacheDictionaryRemoveFieldResponse> DictionaryRemoveFieldAsync(string cacheName, string dictionaryName, byte[] field)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
            Utils.ArgumentNotNull(field, nameof(field));
        }

        catch (ArgumentNullException e)
        {
            return new CacheDictionaryRemoveFieldResponse.Error(new InvalidArgumentException(e.Message));
        }
        return await this.dataClient.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field);
    }

    /// <inheritdoc />
    public async Task<CacheDictionaryRemoveFieldResponse> DictionaryRemoveFieldAsync(string cacheName, string dictionaryName, string field)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
            Utils.ArgumentNotNull(field, nameof(field));
        }

        catch (ArgumentNullException e)
        {
            return new CacheDictionaryRemoveFieldResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field);
    }

    /// <inheritdoc />
    public async Task<CacheDictionaryRemoveFieldsResponse> DictionaryRemoveFieldsAsync(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
            Utils.ArgumentNotNull(fields, nameof(fields));
            Utils.ElementsNotNull(fields, nameof(fields));
        }

        catch (ArgumentNullException e)
        {
            return new CacheDictionaryRemoveFieldsResponse.Error(new InvalidArgumentException(e.Message));
        }
        return await this.dataClient.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fields);
    }

    /// <inheritdoc />
    public async Task<CacheDictionaryRemoveFieldsResponse> DictionaryRemoveFieldsAsync(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(dictionaryName, nameof(dictionaryName));
            Utils.ArgumentNotNull(fields, nameof(fields));
            Utils.ElementsNotNull(fields, nameof(fields));
        }

        catch (ArgumentNullException e)
        {
            return new CacheDictionaryRemoveFieldsResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fields);
    }

    /// <inheritdoc />
    public async Task<CacheSetAddElementResponse> SetAddElementAsync(string cacheName, string setName, byte[] element, CollectionTtl ttl = default(CollectionTtl))
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(setName, nameof(setName));
            Utils.ArgumentNotNull(element, nameof(element));
        }

        catch (ArgumentNullException e)
        {
            return new CacheSetAddElementResponse.Error(new InvalidArgumentException(e.Message));
        }
        return await this.dataClient.SetAddElementAsync(cacheName, setName, element, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheSetAddElementResponse> SetAddElementAsync(string cacheName, string setName, string element, CollectionTtl ttl = default(CollectionTtl))
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(setName, nameof(setName));
            Utils.ArgumentNotNull(element, nameof(element));
        }
        catch (ArgumentNullException e)
        {
            return new CacheSetAddElementResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.SetAddElementAsync(cacheName, setName, element, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheSetAddElementsResponse> SetAddElementsAsync(string cacheName, string setName, IEnumerable<byte[]> elements, CollectionTtl ttl = default(CollectionTtl))
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(setName, nameof(setName));
            Utils.ArgumentNotNull(elements, nameof(elements));
            Utils.ElementsNotNull(elements, nameof(elements));
        }

        catch (ArgumentNullException e)
        {
            return new CacheSetAddElementsResponse.Error(new InvalidArgumentException(e.Message));
        }
        return await this.dataClient.SetAddElementsAsync(cacheName, setName, elements, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheSetAddElementsResponse> SetAddElementsAsync(string cacheName, string setName, IEnumerable<string> elements, CollectionTtl ttl = default(CollectionTtl))
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(setName, nameof(setName));
            Utils.ArgumentNotNull(elements, nameof(elements));
            Utils.ElementsNotNull(elements, nameof(elements));
        }

        catch (ArgumentNullException e)
        {
            return new CacheSetAddElementsResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.SetAddElementsAsync(cacheName, setName, elements, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheSetRemoveElementResponse> SetRemoveElementAsync(string cacheName, string setName, byte[] element)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(setName, nameof(setName));
            Utils.ArgumentNotNull(element, nameof(element));
        }

        catch (ArgumentNullException e)
        {
            return new CacheSetRemoveElementResponse.Error(new InvalidArgumentException(e.Message));
        }
        return await this.dataClient.SetRemoveElementAsync(cacheName, setName, element);
    }

    /// <inheritdoc />
    public async Task<CacheSetRemoveElementResponse> SetRemoveElementAsync(string cacheName, string setName, string element)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(setName, nameof(setName));
            Utils.ArgumentNotNull(element, nameof(element));
        }

        catch (ArgumentNullException e)
        {
            return new CacheSetRemoveElementResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.SetRemoveElementAsync(cacheName, setName, element);
    }

    /// <inheritdoc />
    public async Task<CacheSetRemoveElementsResponse> SetRemoveElementsAsync(string cacheName, string setName, IEnumerable<byte[]> elements)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(setName, nameof(setName));
            Utils.ArgumentNotNull(elements, nameof(elements));
            Utils.ElementsNotNull(elements, nameof(elements));
        }
        catch (ArgumentNullException e)
        {
            return new CacheSetRemoveElementsResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.SetRemoveElementsAsync(cacheName, setName, elements);
    }

    /// <inheritdoc />
    public async Task<CacheSetRemoveElementsResponse> SetRemoveElementsAsync(string cacheName, string setName, IEnumerable<string> elements)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(setName, nameof(setName));
            Utils.ArgumentNotNull(elements, nameof(elements));
            Utils.ElementsNotNull(elements, nameof(elements));
        }
        catch (ArgumentNullException e)
        {
            return new CacheSetRemoveElementsResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.SetRemoveElementsAsync(cacheName, setName, elements);
    }

    /// <inheritdoc />
    public async Task<CacheSetFetchResponse> SetFetchAsync(string cacheName, string setName)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(setName, nameof(setName));
        }
        catch (ArgumentNullException e)
        {
            return new CacheSetFetchResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.SetFetchAsync(cacheName, setName);
    }

    /// <inheritdoc />
    public async Task<CacheListConcatenateFrontResponse> ListConcatenateFrontAsync(string cacheName, string listName, IEnumerable<byte[]> values, int? truncateBackToSize = null, CollectionTtl ttl = default(CollectionTtl))
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(listName, nameof(listName));
            Utils.ArgumentNotNull(values, nameof(values));
            Utils.ElementsNotNull(values, nameof(values));
            Utils.ArgumentStrictlyPositive(truncateBackToSize, nameof(truncateBackToSize));
        }
        catch (ArgumentNullException e)
        {
            return new CacheListConcatenateFrontResponse.Error(new InvalidArgumentException(e.Message));
        }
        catch (ArgumentOutOfRangeException e)
        {
            return new CacheListConcatenateFrontResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.ListConcatenateFrontAsync(cacheName, listName, values, truncateBackToSize, ttl);
    }


    /// <inheritdoc />
    public async Task<CacheListConcatenateFrontResponse> ListConcatenateFrontAsync(string cacheName, string listName, IEnumerable<string> values, int? truncateBackToSize = null, CollectionTtl ttl = default(CollectionTtl))
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(listName, nameof(listName));
            Utils.ArgumentNotNull(values, nameof(values));
            Utils.ElementsNotNull(values, nameof(values));
            Utils.ArgumentStrictlyPositive(truncateBackToSize, nameof(truncateBackToSize));
        }
        catch (ArgumentNullException e)
        {
            return new CacheListConcatenateFrontResponse.Error(new InvalidArgumentException(e.Message));
        }
        catch (ArgumentOutOfRangeException e)
        {
            return new CacheListConcatenateFrontResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.ListConcatenateFrontAsync(cacheName, listName, values, truncateBackToSize, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheListConcatenateBackResponse> ListConcatenateBackAsync(string cacheName, string listName, IEnumerable<byte[]> values, int? truncateFrontToSize = null, CollectionTtl ttl = default(CollectionTtl))
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(listName, nameof(listName));
            Utils.ArgumentNotNull(values, nameof(values));
            Utils.ElementsNotNull(values, nameof(values));
            Utils.ArgumentStrictlyPositive(truncateFrontToSize, nameof(truncateFrontToSize));
        }
        catch (ArgumentNullException e)
        {
            return new CacheListConcatenateBackResponse.Error(new InvalidArgumentException(e.Message));
        }
        catch (ArgumentOutOfRangeException e)
        {
            return new CacheListConcatenateBackResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.ListConcatenateBackAsync(cacheName, listName, values, truncateFrontToSize, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheListConcatenateBackResponse> ListConcatenateBackAsync(string cacheName, string listName, IEnumerable<string> values, int? truncateFrontToSize = null, CollectionTtl ttl = default(CollectionTtl))
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(listName, nameof(listName));
            Utils.ArgumentNotNull(values, nameof(values));
            Utils.ElementsNotNull(values, nameof(values));
            Utils.ArgumentStrictlyPositive(truncateFrontToSize, nameof(truncateFrontToSize));
        }
        catch (ArgumentNullException e)
        {
            return new CacheListConcatenateBackResponse.Error(new InvalidArgumentException(e.Message));
        }
        catch (ArgumentOutOfRangeException e)
        {
            return new CacheListConcatenateBackResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.ListConcatenateBackAsync(cacheName, listName, values, truncateFrontToSize, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheListPushFrontResponse> ListPushFrontAsync(string cacheName, string listName, byte[] value, int? truncateBackToSize = null, CollectionTtl ttl = default(CollectionTtl))
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(listName, nameof(listName));
            Utils.ArgumentNotNull(value, nameof(value));
            Utils.ArgumentStrictlyPositive(truncateBackToSize, nameof(truncateBackToSize));
        }
        catch (ArgumentNullException e)
        {
            return new CacheListPushFrontResponse.Error(new InvalidArgumentException(e.Message));
        }
        catch (ArgumentOutOfRangeException e)
        {
            return new CacheListPushFrontResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.ListPushFrontAsync(cacheName, listName, value, truncateBackToSize, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheListPushFrontResponse> ListPushFrontAsync(string cacheName, string listName, string value, int? truncateBackToSize = null, CollectionTtl ttl = default(CollectionTtl))
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(listName, nameof(listName));
            Utils.ArgumentNotNull(value, nameof(value));
            Utils.ArgumentStrictlyPositive(truncateBackToSize, nameof(truncateBackToSize));
        }
        catch (ArgumentNullException e)
        {
            return new CacheListPushFrontResponse.Error(new InvalidArgumentException(e.Message));
        }
        catch (ArgumentOutOfRangeException e)
        {
            return new CacheListPushFrontResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.ListPushFrontAsync(cacheName, listName, value, truncateBackToSize, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheListPushBackResponse> ListPushBackAsync(string cacheName, string listName, byte[] value, int? truncateFrontToSize = null, CollectionTtl ttl = default(CollectionTtl))
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(listName, nameof(listName));
            Utils.ArgumentNotNull(value, nameof(value));
            Utils.ArgumentStrictlyPositive(truncateFrontToSize, nameof(truncateFrontToSize));
        }
        catch (ArgumentNullException e)
        {
            return new CacheListPushBackResponse.Error(new InvalidArgumentException(e.Message));
        }
        catch (ArgumentOutOfRangeException e)
        {
            return new CacheListPushBackResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.ListPushBackAsync(cacheName, listName, value, truncateFrontToSize, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheListPushBackResponse> ListPushBackAsync(string cacheName, string listName, string value, int? truncateFrontToSize = null, CollectionTtl ttl = default(CollectionTtl))
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(listName, nameof(listName));
            Utils.ArgumentNotNull(value, nameof(value));
            Utils.ArgumentStrictlyPositive(truncateFrontToSize, nameof(truncateFrontToSize));
        }
        catch (ArgumentNullException e)
        {
            return new CacheListPushBackResponse.Error(new InvalidArgumentException(e.Message));
        }
        catch (ArgumentOutOfRangeException e)
        {
            return new CacheListPushBackResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.ListPushBackAsync(cacheName, listName, value, truncateFrontToSize, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheListPopFrontResponse> ListPopFrontAsync(string cacheName, string listName)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(listName, nameof(listName));
        }
        catch (ArgumentNullException e)
        {
            return new CacheListPopFrontResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.ListPopFrontAsync(cacheName, listName);
    }

    /// <inheritdoc />
    public async Task<CacheListPopBackResponse> ListPopBackAsync(string cacheName, string listName)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(listName, nameof(listName));
        }
        catch (ArgumentNullException e)
        {
            return new CacheListPopBackResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.ListPopBackAsync(cacheName, listName);
    }

    /// <inheritdoc />
    public async Task<CacheListFetchResponse> ListFetchAsync(string cacheName, string listName)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(listName, nameof(listName));
        }
        catch (ArgumentNullException e)
        {
            return new CacheListFetchResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.ListFetchAsync(cacheName, listName);
    }

    /// <inheritdoc />
    public async Task<CacheListRemoveValueResponse> ListRemoveValueAsync(string cacheName, string listName, byte[] value)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(listName, nameof(listName));
            Utils.ArgumentNotNull(value, nameof(value));
        }
        catch (ArgumentNullException e)
        {
            return new CacheListRemoveValueResponse.Error(new InvalidArgumentException(e.Message));
        }


        return await this.dataClient.ListRemoveValueAsync(cacheName, listName, value);
    }

    /// <inheritdoc />
    public async Task<CacheListRemoveValueResponse> ListRemoveValueAsync(string cacheName, string listName, string value)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(listName, nameof(listName));
            Utils.ArgumentNotNull(value, nameof(value));
        }
        catch (ArgumentNullException e)
        {
            return new CacheListRemoveValueResponse.Error(new InvalidArgumentException(e.Message));
        }


        return await this.dataClient.ListRemoveValueAsync(cacheName, listName, value);
    }

    /// <inheritdoc />
    public async Task<CacheListLengthResponse> ListLengthAsync(string cacheName, string listName)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(listName, nameof(listName));
        }

        catch (ArgumentNullException e)
        {
            return new CacheListLengthResponse.Error(new InvalidArgumentException(e.Message));
        }
        return await this.dataClient.ListLengthAsync(cacheName, listName);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.controlClient.Dispose();
        foreach (var dataClient in this.dataClients)
        {
            dataClient.Dispose();
        }
        GC.SuppressFinalize(this);
    }


    private ScsDataClient NextDataClient()
    {
        return this.dataClients[Interlocked.Increment(ref this.nextDataClientIndex) % this.dataClients.Count];
    }
}
