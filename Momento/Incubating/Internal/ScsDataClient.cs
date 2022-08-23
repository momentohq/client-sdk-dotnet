using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Incubating.Responses;
using Momento.Sdk.Internal;
using Momento.Sdk.Internal.ExtensionMethods;

namespace Momento.Sdk.Incubating.Internal;

internal sealed class ScsDataClient : ScsDataClientBase
{
    public ScsDataClient(string authToken, string endpoint, uint defaultTtlSeconds, uint? dataClientOperationTimeoutMilliseconds = null)
        : base(authToken, endpoint, defaultTtlSeconds, dataClientOperationTimeoutMilliseconds)
    {
    }

    private _DictionaryFieldValuePair[] ToSingletonFieldValuePair(byte[] field, byte[] value) => new _DictionaryFieldValuePair[] { new _DictionaryFieldValuePair() { Field = field.ToByteString(), Value = value.ToByteString() } };
    private _DictionaryFieldValuePair[] ToSingletonFieldValuePair(string field, string value) => new _DictionaryFieldValuePair[] { new _DictionaryFieldValuePair() { Field = field.ToByteString(), Value = value.ToByteString() } };

    public async Task<CacheDictionarySetResponse> DictionarySetAsync(string cacheName, string dictionaryName, byte[] field, byte[] value, bool refreshTtl, uint? ttlSeconds = null)
    {
        await SendDictionarySetBatchAsync(cacheName, dictionaryName, ToSingletonFieldValuePair(field, value), refreshTtl, ttlSeconds);
        return new CacheDictionarySetResponse();
    }

    public async Task<CacheDictionarySetResponse> DictionarySetAsync(string cacheName, string dictionaryName, string field, string value, bool refreshTtl, uint? ttlSeconds = null)
    {
        await SendDictionarySetBatchAsync(cacheName, dictionaryName, ToSingletonFieldValuePair(field, value), refreshTtl, ttlSeconds);
        return new CacheDictionarySetResponse();
    }

    public async Task<CacheDictionaryGetResponse> DictionaryGetAsync(string cacheName, string dictionaryName, byte[] field)
    {
        var response = await SendDictionaryGetBatchAsync(cacheName, dictionaryName, field.ToSingletonByteString());
        return new CacheDictionaryGetResponse(response);
    }

    public async Task<CacheDictionaryGetResponse> DictionaryGetAsync(string cacheName, string dictionaryName, string field)
    {
        var response = await SendDictionaryGetBatchAsync(cacheName, dictionaryName, field.ToSingletonByteString());
        return new CacheDictionaryGetResponse(response);
    }

    public async Task<CacheDictionarySetBatchResponse> DictionarySetBatchAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<byte[], byte[]>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        var protoItems = items.Select(kv => new _DictionaryFieldValuePair() { Field = kv.Key.ToByteString(), Value = kv.Value.ToByteString() });
        return await SendDictionarySetBatchAsync(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
    }

    public async Task<CacheDictionarySetBatchResponse> DictionarySetBatchAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, string>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        var protoItems = items.Select(kv => new _DictionaryFieldValuePair() { Field = kv.Key.ToByteString(), Value = kv.Value.ToByteString() });
        return await SendDictionarySetBatchAsync(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
    }

    public async Task<CacheDictionarySetBatchResponse> SendDictionarySetBatchAsync(string cacheName, string dictionaryName, IEnumerable<_DictionaryFieldValuePair> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        _DictionarySetRequest request = new()
        {
            DictionaryName = dictionaryName.ToByteString(),
            RefreshTtl = refreshTtl,
            TtlMilliseconds = TtlSecondsToMilliseconds(ttlSeconds)
        };
        request.Items.Add(items);

        try
        {
            await this.grpcManager.Client.DictionarySetAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheDictionarySetBatchResponse();
    }

    public async Task<CacheDictionaryGetBatchResponse> DictionaryGetBatchAsync(string cacheName, string dictionaryName, params byte[][] fields)
    {
        var response = await SendDictionaryGetBatchAsync(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
        return new CacheDictionaryGetBatchResponse(response, fields.Length);
    }

    public async Task<CacheDictionaryGetBatchResponse> DictionaryGetBatchAsync(string cacheName, string dictionaryName, params string[] fields)
    {
        var response = await SendDictionaryGetBatchAsync(cacheName, dictionaryName, fields.ToEnumerableByteString());
        return new CacheDictionaryGetBatchResponse(response, fields.Length);
    }

    public async Task<CacheDictionaryGetBatchResponse> DictionaryGetBatchAsync(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        var response = await SendDictionaryGetBatchAsync(cacheName, dictionaryName, fields.ToEnumerableByteString());
        return new CacheDictionaryGetBatchResponse(response, fields.Count());
    }

    public async Task<CacheDictionaryGetBatchResponse> DictionaryGetBatchAsync(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        var response = await SendDictionaryGetBatchAsync(cacheName, dictionaryName, fields.ToEnumerableByteString());
        return new CacheDictionaryGetBatchResponse(response, fields.Count());
    }

    private async Task<_DictionaryGetResponse> SendDictionaryGetBatchAsync(string cacheName, string dictionaryName, IEnumerable<ByteString> fields)
    {
        _DictionaryGetRequest request = new() { DictionaryName = dictionaryName.ToByteString() };
        request.Fields.Add(fields);

        try
        {
            return await this.grpcManager.Client.DictionaryGetAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public async Task<CacheDictionaryFetchResponse> DictionaryFetchAsync(string cacheName, string dictionaryName)
    {
        _DictionaryFetchRequest request = new() { DictionaryName = dictionaryName.ToByteString() };
        _DictionaryFetchResponse response;
        try
        {
            response = await this.grpcManager.Client.DictionaryFetchAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheDictionaryFetchResponse(response);
    }

    public async Task<CacheDictionaryDeleteResponse> DictionaryDeleteAsync(string cacheName, string dictionaryName)
    {
        _DictionaryDeleteRequest request = new()
        {
            DictionaryName = dictionaryName.ToByteString(),
            All = new()
        };
        try
        {
            await this.grpcManager.Client.DictionaryDeleteAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheDictionaryDeleteResponse();
    }

    public async Task<CacheDictionaryRemoveFieldResponse> DictionaryRemoveFieldAsync(string cacheName, string dictionaryName, byte[] field)
    {
        return await DictionaryRemoveFieldAsync(cacheName, dictionaryName, field.ToByteString());
    }

    public async Task<CacheDictionaryRemoveFieldResponse> DictionaryRemoveFieldAsync(string cacheName, string dictionaryName, string field)
    {
        return await DictionaryRemoveFieldAsync(cacheName, dictionaryName, field.ToByteString());
    }

    public async Task<CacheDictionaryRemoveFieldResponse> DictionaryRemoveFieldAsync(string cacheName, string dictionaryName, ByteString field)
    {
        _DictionaryDeleteRequest request = new()
        {
            DictionaryName = dictionaryName.ToByteString(),
            Some = new()
        };
        request.Some.Fields.Add(field);

        try
        {
            await this.grpcManager.Client.DictionaryDeleteAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheDictionaryRemoveFieldResponse();
    }

    public async Task<CacheDictionaryRemoveFieldsResponse> DictionaryRemoveFieldsAsync(string cacheName, string dictionaryName, params byte[][] fields)
    {
        return await DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fields.ToEnumerableByteString());
    }

    public async Task<CacheDictionaryRemoveFieldsResponse> DictionaryRemoveFieldsAsync(string cacheName, string dictionaryName, params string[] fields)
    {
        return await DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fields.ToEnumerableByteString());
    }

    public async Task<CacheDictionaryRemoveFieldsResponse> DictionaryRemoveFieldsAsync(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        return await DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fields.ToEnumerableByteString());
    }

    public async Task<CacheDictionaryRemoveFieldsResponse> DictionaryRemoveFieldsAsync(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        return await DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fields.ToEnumerableByteString());
    }

    public async Task<CacheDictionaryRemoveFieldsResponse> DictionaryRemoveFieldsAsync(string cacheName, string dictionaryName, IEnumerable<ByteString> fields)
    {
        _DictionaryDeleteRequest request = new()
        {
            DictionaryName = dictionaryName.ToByteString(),
            Some = new()
        };
        request.Some.Fields.Add(fields);

        try
        {
            await this.grpcManager.Client.DictionaryDeleteAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheDictionaryRemoveFieldsResponse();
    }

    public async Task<CacheSetAddResponse> SetAddAsync(string cacheName, string setName, byte[] element, bool refreshTtl, uint? ttlSeconds = null)
    {
        await SendSetAddBatchAsync(cacheName, setName, element.ToSingletonByteString(), refreshTtl, ttlSeconds);
        return new CacheSetAddResponse();
    }

    public async Task<CacheSetAddResponse> SetAddAsync(string cacheName, string setName, string element, bool refreshTtl, uint? ttlSeconds = null)
    {
        await SendSetAddBatchAsync(cacheName, setName, element.ToSingletonByteString(), refreshTtl, ttlSeconds);
        return new CacheSetAddResponse();
    }

    public async Task<CacheSetAddBatchResponse> SetAddBatchAsync(string cacheName, string setName, IEnumerable<byte[]> elements, bool refreshTtl, uint? ttlSeconds = null)
    {
        await SendSetAddBatchAsync(cacheName, setName, elements.ToEnumerableByteString(), refreshTtl, ttlSeconds);
        return new CacheSetAddBatchResponse();
    }

    public async Task<CacheSetAddBatchResponse> SetAddBatchAsync(string cacheName, string setName, IEnumerable<string> elements, bool refreshTtl, uint? ttlSeconds = null)
    {
        await SendSetAddBatchAsync(cacheName, setName, elements.ToEnumerableByteString(), refreshTtl, ttlSeconds);
        return new CacheSetAddBatchResponse();
    }

    public async Task SendSetAddBatchAsync(string cacheName, string setName, IEnumerable<ByteString> elements, bool refreshTtl, uint? ttlSeconds = null)
    {
        _SetUnionRequest request = new()
        {
            SetName = setName.ToByteString(),
            RefreshTtl = refreshTtl,
            TtlMilliseconds = TtlSecondsToMilliseconds(ttlSeconds)
        };
        request.Elements.Add(elements);

        try
        {
            await this.grpcManager.Client.SetUnionAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public async Task<CacheSetFetchResponse> SetFetchAsync(string cacheName, string setName)
    {
        _SetFetchRequest request = new() { SetName = setName.ToByteString() };
        _SetFetchResponse response;

        try
        {
            response = await this.grpcManager.Client.SetFetchAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheSetFetchResponse(response);
    }

    public async Task<CacheSetDeleteResponse> SetDeleteAsync(string cacheName, string setName)
    {
        _SetDifferenceRequest request = new()
        {
            SetName = setName.ToByteString(),
            Subtrahend = new() { Identity = new() }
        };

        try
        {
            await this.grpcManager.Client.SetDifferenceAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheSetDeleteResponse();
    }

    public async Task<CacheListPushFrontResponse> ListPushFrontAsync(string cacheName, string listName, byte[] value, bool refreshTtl, uint? ttlSeconds = null)
    {
        await SendListPushFrontAsync(cacheName, listName, value.ToByteString(), refreshTtl, ttlSeconds);
        return new CacheListPushFrontResponse();
    }

    public async Task<CacheListPushFrontResponse> ListPushFrontAsync(string cacheName, string listName, string value, bool refreshTtl, uint? ttlSeconds = null)
    {
        await SendListPushFrontAsync(cacheName, listName, value.ToByteString(), refreshTtl, ttlSeconds);
        return new CacheListPushFrontResponse();
    }

    public async Task SendListPushFrontAsync(string cacheName, string listName, ByteString value, bool refreshTtl, uint? ttlSeconds = null)
    {
        _ListPushFrontRequest request = new()
        {
            ListName = listName.ToByteString(),
            RefreshTtl = refreshTtl,
            TtlMilliseconds = TtlSecondsToMilliseconds(ttlSeconds)
        };
        request.Values.Add(value);

        try
        {
            await this.grpcManager.Client.ListPushFrontAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public async Task<CacheListPushBackResponse> ListPushBackAsync(string cacheName, string listName, byte[] value, bool refreshTtl, uint? ttlSeconds = null)
    {
        await SendListPushBackAsync(cacheName, listName, value.ToByteString(), refreshTtl, ttlSeconds);
        return new CacheListPushBackResponse();
    }

    public async Task<CacheListPushBackResponse> ListPushBackAsync(string cacheName, string listName, string value, bool refreshTtl, uint? ttlSeconds = null)
    {
        await SendListPushBackAsync(cacheName, listName, value.ToByteString(), refreshTtl, ttlSeconds);
        return new CacheListPushBackResponse();
    }

    public async Task SendListPushBackAsync(string cacheName, string listName, ByteString value, bool refreshTtl, uint? ttlSeconds = null)
    {
        _ListPushBackRequest request = new()
        {
            ListName = listName.ToByteString(),
            RefreshTtl = refreshTtl,
            TtlMilliseconds = TtlSecondsToMilliseconds(ttlSeconds)
        };
        request.Values.Add(value);

        try
        {
            await this.grpcManager.Client.ListPushBackAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public async Task<CacheListPopFrontResponse> ListPopFrontAsync(string cacheName, string listName)
    {
        _ListPopFrontRequest request = new() { ListName = listName.ToByteString() };
        _ListPopFrontResponse response;

        try
        {
            response = await this.grpcManager.Client.ListPopFrontAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheListPopFrontResponse(response);
    }

    public async Task<CacheListPopBackResponse> ListPopBackAsync(string cacheName, string listName)
    {
        _ListPopBackRequest request = new() { ListName = listName.ToByteString() };
        _ListPopBackResponse response;

        try
        {
            response = await this.grpcManager.Client.ListPopBackAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheListPopBackResponse(response);
    }

    public async Task<CacheListFetchResponse> ListFetchAsync(string cacheName, string listName)
    {
        _ListFetchRequest request = new() { ListName = listName.ToByteString() };
        _ListFetchResponse response;

        try
        {
            response = await this.grpcManager.Client.ListFetchAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheListFetchResponse(response);
    }
}
