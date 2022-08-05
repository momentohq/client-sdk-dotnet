using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CacheClient;
using Google.Protobuf;
using MomentoSdk.Internal;
using MomentoSdk.Internal.ExtensionMethods;
using MomentoSdk.Incubating.Responses;
using MomentoSdk.Exceptions;

namespace MomentoSdk.Incubating.Internal;

internal sealed class ScsDataClient : ScsDataClientBase
{
    public ScsDataClient(string authToken, string endpoint, uint defaultTtlSeconds, uint? dataClientOperationTimeoutMilliseconds = null)
        : base(authToken, endpoint, defaultTtlSeconds, dataClientOperationTimeoutMilliseconds)
    {
    }

    public CacheDictionarySetResponse DictionarySet(string cacheName, string dictionaryName, byte[] field, byte[] value, bool refreshTtl, uint? ttlSeconds = null)
    {
        return SendDictionarySet(cacheName, dictionaryName, field.ToByteString(), value.ToByteString(), refreshTtl, ttlSeconds);
    }

    public CacheDictionarySetResponse DictionarySet(string cacheName, string dictionaryName, string field, string value, bool refreshTtl, uint? ttlSeconds = null)
    {
        return SendDictionarySet(cacheName, dictionaryName, field.ToByteString(), value.ToByteString(), refreshTtl, ttlSeconds);
    }

    private CacheDictionarySetResponse SendDictionarySet(string cacheName, string dictionaryName, ByteString field, ByteString value, bool refreshTtl, uint? ttlSeconds = null)
    {
        _DictionarySetRequest request = new()
        {
            DictionaryName = dictionaryName.ToByteString(),
            RefreshTtl = refreshTtl,
            TtlMilliseconds = ttlSecondsToMilliseconds(ttlSeconds)
        };
        request.Items.Add(new _DictionaryFieldValuePair() { Field = field, Value = value });

        try
        {
            this.grpcManager.Client.DictionarySet(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheDictionarySetResponse();
    }

    public async Task<CacheDictionarySetResponse> DictionarySetAsync(string cacheName, string dictionaryName, byte[] field, byte[] value, bool refreshTtl, uint? ttlSeconds = null)
    {
        return await SendDictionarySetAsync(cacheName, dictionaryName, field.ToByteString(), value.ToByteString(), refreshTtl, ttlSeconds);
    }

    public async Task<CacheDictionarySetResponse> DictionarySetAsync(string cacheName, string dictionaryName, string field, string value, bool refreshTtl, uint? ttlSeconds = null)
    {
        return await SendDictionarySetAsync(cacheName, dictionaryName, field.ToByteString(), value.ToByteString(), refreshTtl, ttlSeconds);
    }


    private async Task<CacheDictionarySetResponse> SendDictionarySetAsync(string cacheName, string dictionaryName, ByteString field, ByteString value, bool refreshTtl, uint? ttlSeconds = null)
    {
        _DictionarySetRequest request = new()
        {
            DictionaryName = dictionaryName.ToByteString(),
            RefreshTtl = refreshTtl,
            TtlMilliseconds = ttlSecondsToMilliseconds(ttlSeconds)
        };
        request.Items.Add(new _DictionaryFieldValuePair() { Field = field, Value = value });

        try
        {
            await this.grpcManager.Client.DictionarySetAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheDictionarySetResponse();
    }

    public CacheDictionaryGetResponse DictionaryGet(string cacheName, string dictionaryName, byte[] field)
    {
        var response = SendDictionaryGetBatch(cacheName, dictionaryName, new ByteString[] { field.ToByteString() });
        return new CacheDictionaryGetResponse(response);
    }

    public CacheDictionaryGetResponse DictionaryGet(string cacheName, string dictionaryName, string field)
    {
        var response = SendDictionaryGetBatch(cacheName, dictionaryName, new ByteString[] { field.ToByteString() });
        return new CacheDictionaryGetResponse(response);
    }

    public async Task<CacheDictionaryGetResponse> DictionaryGetAsync(string cacheName, string dictionaryName, byte[] field)
    {
        var response = await SendDictionaryGetBatchAsync(cacheName, dictionaryName, new ByteString[] { field.ToByteString() });
        return new CacheDictionaryGetResponse(response);
    }

    public async Task<CacheDictionaryGetResponse> DictionaryGetAsync(string cacheName, string dictionaryName, string field)
    {
        var response = await SendDictionaryGetBatchAsync(cacheName, dictionaryName, new ByteString[] { field.ToByteString() });
        return new CacheDictionaryGetResponse(response);
    }

    public CacheDictionarySetBatchResponse DictionarySetBatch(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<byte[], byte[]>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        var protoItems = items.Select(kv => new _DictionaryFieldValuePair() { Field = kv.Key.ToByteString(), Value = kv.Value.ToByteString() });
        return SendDictionarySetBatch(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
    }

    public CacheDictionarySetBatchResponse DictionarySetBatch(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, string>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        var protoItems = items.Select(kv => new _DictionaryFieldValuePair() { Field = kv.Key.ToByteString(), Value = kv.Value.ToByteString() });
        return SendDictionarySetBatch(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
    }

    public CacheDictionarySetBatchResponse SendDictionarySetBatch(string cacheName, string dictionaryName, IEnumerable<_DictionaryFieldValuePair> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        _DictionarySetRequest request = new()
        {
            DictionaryName = dictionaryName.ToByteString(),
            RefreshTtl = refreshTtl,
            TtlMilliseconds = ttlSecondsToMilliseconds(ttlSeconds)
        };
        request.Items.Add(items);

        try
        {
            this.grpcManager.Client.DictionarySet(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheDictionarySetBatchResponse();
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
            TtlMilliseconds = ttlSecondsToMilliseconds(ttlSeconds)
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

    public CacheDictionaryGetBatchResponse DictionaryGetBatch(string cacheName, string dictionaryName, params byte[][] fields)
    {
        var response = SendDictionaryGetBatch(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
        return new CacheDictionaryGetBatchResponse(response, fields.Length);
    }

    public CacheDictionaryGetBatchResponse DictionaryGetBatch(string cacheName, string dictionaryName, params string[] fields)
    {
        var response = SendDictionaryGetBatch(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
        return new CacheDictionaryGetBatchResponse(response, fields.Length);
    }

    public CacheDictionaryGetBatchResponse DictionaryGetBatch(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        var response = SendDictionaryGetBatch(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
        return new CacheDictionaryGetBatchResponse(response, fields.Count());
    }

    public CacheDictionaryGetBatchResponse DictionaryGetBatch(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        var response = SendDictionaryGetBatch(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
        return new CacheDictionaryGetBatchResponse(response, fields.Count());
    }

    private _DictionaryGetResponse SendDictionaryGetBatch(string cacheName, string dictionaryName, IEnumerable<ByteString> fields)
    {
        _DictionaryGetRequest request = new() { DictionaryName = dictionaryName.ToByteString() };
        request.Fields.Add(fields);

        try
        {
            return this.grpcManager.Client.DictionaryGet(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public async Task<CacheDictionaryGetBatchResponse> DictionaryGetBatchAsync(string cacheName, string dictionaryName, params byte[][] fields)
    {
        var response = await SendDictionaryGetBatchAsync(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
        return new CacheDictionaryGetBatchResponse(response, fields.Length);
    }

    public async Task<CacheDictionaryGetBatchResponse> DictionaryGetBatchAsync(string cacheName, string dictionaryName, params string[] fields)
    {
        var response = await SendDictionaryGetBatchAsync(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
        return new CacheDictionaryGetBatchResponse(response, fields.Length);
    }

    public async Task<CacheDictionaryGetBatchResponse> DictionaryGetBatchAsync(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        var response = await SendDictionaryGetBatchAsync(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
        return new CacheDictionaryGetBatchResponse(response, fields.Count());
    }

    public async Task<CacheDictionaryGetBatchResponse> DictionaryGetBatchAsync(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        var response = await SendDictionaryGetBatchAsync(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
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

    public CacheDictionaryFetchResponse DictionaryFetch(string cacheName, string dictionaryName)
    {
        _DictionaryFetchRequest request = new() { DictionaryName = dictionaryName.ToByteString() };
        _DictionaryFetchResponse response;
        try
        {
            response = this.grpcManager.Client.DictionaryFetch(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheDictionaryFetchResponse(response);
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

    public CacheDictionaryDeleteResponse DictionaryDelete(string cacheName, string dictionaryName)
    {
        _DictionaryDeleteRequest request = new()
        {
            DictionaryName = dictionaryName.ToByteString(),
            All = new()
        };
        try
        {
            this.grpcManager.Client.DictionaryDelete(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheDictionaryDeleteResponse();
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

    public CacheDictionaryRemoveFieldResponse DictionaryRemoveField(string cacheName, string dictionaryName, byte[] field)
    {
        return DictionaryRemoveField(cacheName, dictionaryName, field.ToByteString());
    }

    public CacheDictionaryRemoveFieldResponse DictionaryRemoveField(string cacheName, string dictionaryName, string field)
    {
        return DictionaryRemoveField(cacheName, dictionaryName, field.ToByteString());
    }

    public CacheDictionaryRemoveFieldResponse DictionaryRemoveField(string cacheName, string dictionaryName, ByteString field)
    {
        _DictionaryDeleteRequest request = new()
        {
            DictionaryName = dictionaryName.ToByteString(),
            Some = new()
        };
        request.Some.Fields.Add(field);

        try
        {
            this.grpcManager.Client.DictionaryDelete(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheDictionaryRemoveFieldResponse();
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

    public CacheDictionaryRemoveFieldsResponse DictionaryRemoveFields(string cacheName, string dictionaryName, params byte[][] fields)
    {
        return DictionaryRemoveFields(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    public CacheDictionaryRemoveFieldsResponse DictionaryRemoveFields(string cacheName, string dictionaryName, params string[] fields)
    {
        return DictionaryRemoveFields(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    public CacheDictionaryRemoveFieldsResponse DictionaryRemoveFields(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        return DictionaryRemoveFields(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    public CacheDictionaryRemoveFieldsResponse DictionaryRemoveFields(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        return DictionaryRemoveFields(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    public CacheDictionaryRemoveFieldsResponse DictionaryRemoveFields(string cacheName, string dictionaryName, IEnumerable<ByteString> fields)
    {
        _DictionaryDeleteRequest request = new()
        {
            DictionaryName = dictionaryName.ToByteString(),
            Some = new()
        };
        request.Some.Fields.Add(fields);

        try
        {
            this.grpcManager.Client.DictionaryDelete(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheDictionaryRemoveFieldsResponse();
    }

    public async Task<CacheDictionaryRemoveFieldsResponse> DictionaryRemoveFieldsAsync(string cacheName, string dictionaryName, params byte[][] fields)
    {
        return await DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    public async Task<CacheDictionaryRemoveFieldsResponse> DictionaryRemoveFieldsAsync(string cacheName, string dictionaryName, params string[] fields)
    {
        return await DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    public async Task<CacheDictionaryRemoveFieldsResponse> DictionaryRemoveFieldsAsync(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        return await DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    public async Task<CacheDictionaryRemoveFieldsResponse> DictionaryRemoveFieldsAsync(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        return await DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
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
        await SendSetAddBatchAsync(cacheName, setName, new ByteString[] { element.ToByteString() }, refreshTtl, ttlSeconds);
        return new CacheSetAddResponse();
    }

    public async Task<CacheSetAddResponse> SetAddAsync(string cacheName, string setName, string element, bool refreshTtl, uint? ttlSeconds = null)
    {
        await SendSetAddBatchAsync(cacheName, setName, new ByteString[] { element.ToByteString() }, refreshTtl, ttlSeconds);
        return new CacheSetAddResponse();
    }

    public async Task<CacheSetAddBatchResponse> SetAddBatchAsync(string cacheName, string setName, IEnumerable<byte[]> elements, bool refreshTtl, uint? ttlSeconds = null)
    {
        await SendSetAddBatchAsync(cacheName, setName, elements.Select(element => element.ToByteString()), refreshTtl, ttlSeconds);
        return new CacheSetAddBatchResponse();
    }

    public async Task<CacheSetAddBatchResponse> SetAddBatchAsync(string cacheName, string setName, IEnumerable<string> elements, bool refreshTtl, uint? ttlSeconds = null)
    {
        await SendSetAddBatchAsync(cacheName, setName, elements.Select(element => element.ToByteString()), refreshTtl, ttlSeconds);
        return new CacheSetAddBatchResponse();
    }

    public async Task SendSetAddBatchAsync(string cacheName, string setName, IEnumerable<ByteString> elements, bool refreshTtl, uint? ttlSeconds = null)
    {
        _SetUnionRequest request = new()
        {
            SetName = setName.ToByteString(),
            RefreshTtl = refreshTtl,
            TtlMilliseconds = ttlSecondsToMilliseconds(ttlSeconds)
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
}
