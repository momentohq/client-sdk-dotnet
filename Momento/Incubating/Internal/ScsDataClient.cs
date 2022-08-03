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
        request.DictionaryBody.Add(new _DictionaryKeyValuePair() { Key = field, Value = value });

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
        request.DictionaryBody.Add(new _DictionaryKeyValuePair() { Key = field, Value = value });

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
        return SendDictionaryGet(cacheName, dictionaryName, field.ToByteString());
    }

    public CacheDictionaryGetResponse DictionaryGet(string cacheName, string dictionaryName, string field)
    {
        return SendDictionaryGet(cacheName, dictionaryName, field.ToByteString());
    }

    private CacheDictionaryGetResponse SendDictionaryGet(string cacheName, string dictionaryName, ByteString field)
    {
        _DictionaryGetRequest request = new() { DictionaryName = dictionaryName.ToByteString() };
        request.DictionaryKeys.Add(field);

        try
        {
            var response = this.grpcManager.Client.DictionaryGet(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
            // TODO: report specific case of unexpected zero-length dictionary body
            return new CacheDictionaryGetResponse(response.DictionaryBody[0]);

        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public async Task<CacheDictionaryGetResponse> DictionaryGetAsync(string cacheName, string dictionaryName, byte[] field)
    {
        return await SendDictionaryGetAsync(cacheName, dictionaryName, field.ToByteString());
    }

    public async Task<CacheDictionaryGetResponse> DictionaryGetAsync(string cacheName, string dictionaryName, string field)
    {
        return await SendDictionaryGetAsync(cacheName, dictionaryName, field.ToByteString());
    }

    private async Task<CacheDictionaryGetResponse> SendDictionaryGetAsync(string cacheName, string dictionaryName, ByteString field)
    {
        _DictionaryGetRequest request = new() { DictionaryName = dictionaryName.ToByteString() };
        request.DictionaryKeys.Add(field);

        try
        {
            var response = await this.grpcManager.Client.DictionaryGetAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
            // TODO: report specific case of unexpected zero-length dictionary body
            return new CacheDictionaryGetResponse(response.DictionaryBody[0]);
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public CacheDictionarySetBatchResponse DictionarySetBatch(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<byte[], byte[]>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        var protoItems = items.Select(kv => new _DictionaryKeyValuePair() { Key = kv.Key.ToByteString(), Value = kv.Value.ToByteString() });
        return SendDictionarySetBatch(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
    }

    public CacheDictionarySetBatchResponse DictionarySetBatch(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, string>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        var protoItems = items.Select(kv => new _DictionaryKeyValuePair() { Key = kv.Key.ToByteString(), Value = kv.Value.ToByteString() });
        return SendDictionarySetBatch(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
    }

    public CacheDictionarySetBatchResponse SendDictionarySetBatch(string cacheName, string dictionaryName, IEnumerable<_DictionaryKeyValuePair> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        _DictionarySetRequest request = new()
        {
            DictionaryName = dictionaryName.ToByteString(),
            RefreshTtl = refreshTtl,
            TtlMilliseconds = ttlSecondsToMilliseconds(ttlSeconds)
        };
        request.DictionaryBody.Add(items);

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
        var protoItems = items.Select(kv => new _DictionaryKeyValuePair() { Key = kv.Key.ToByteString(), Value = kv.Value.ToByteString() });
        return await SendDictionarySetBatchAsync(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
    }

    public async Task<CacheDictionarySetBatchResponse> DictionarySetBatchAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, string>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        var protoItems = items.Select(kv => new _DictionaryKeyValuePair() { Key = kv.Key.ToByteString(), Value = kv.Value.ToByteString() });
        return await SendDictionarySetBatchAsync(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
    }

    public async Task<CacheDictionarySetBatchResponse> SendDictionarySetBatchAsync(string cacheName, string dictionaryName, IEnumerable<_DictionaryKeyValuePair> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        _DictionarySetRequest request = new()
        {
            DictionaryName = dictionaryName.ToByteString(),
            RefreshTtl = refreshTtl,
            TtlMilliseconds = ttlSecondsToMilliseconds(ttlSeconds)
        };
        request.DictionaryBody.Add(items);

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
        return SendDictionaryGetBatch(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    public CacheDictionaryGetBatchResponse DictionaryGetBatch(string cacheName, string dictionaryName, params string[] fields)
    {
        return SendDictionaryGetBatch(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    public CacheDictionaryGetBatchResponse DictionaryGetBatch(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        return SendDictionaryGetBatch(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    public CacheDictionaryGetBatchResponse DictionaryGetBatch(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        return SendDictionaryGetBatch(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    private CacheDictionaryGetBatchResponse SendDictionaryGetBatch(string cacheName, string dictionaryName, IEnumerable<ByteString> fields)
    {
        _DictionaryGetRequest request = new() { DictionaryName = dictionaryName.ToByteString() };
        request.DictionaryKeys.Add(fields);

        _DictionaryGetResponse response;
        try
        {
            response = this.grpcManager.Client.DictionaryGet(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheDictionaryGetBatchResponse(response);
    }

    public async Task<CacheDictionaryGetBatchResponse> DictionaryGetBatchAsync(string cacheName, string dictionaryName, params byte[][] fields)
    {
        return await SendDictionaryGetBatchAsync(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    public async Task<CacheDictionaryGetBatchResponse> DictionaryGetBatchAsync(string cacheName, string dictionaryName, params string[] fields)
    {
        return await SendDictionaryGetBatchAsync(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    public async Task<CacheDictionaryGetBatchResponse> DictionaryGetBatchAsync(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        return await SendDictionaryGetBatchAsync(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    public async Task<CacheDictionaryGetBatchResponse> DictionaryGetBatchAsync(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        return await SendDictionaryGetBatchAsync(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    private async Task<CacheDictionaryGetBatchResponse> SendDictionaryGetBatchAsync(string cacheName, string dictionaryName, IEnumerable<ByteString> fields)
    {
        _DictionaryGetRequest request = new() { DictionaryName = dictionaryName.ToByteString() };
        request.DictionaryKeys.Add(fields);

        _DictionaryGetResponse response;
        try
        {
            response = await this.grpcManager.Client.DictionaryGetAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheDictionaryGetBatchResponse(response);
    }

    public CacheDictionaryFetchResponse DictionaryFetch(string cacheName, string dictionaryName)
    {
        _DictionaryGetAllRequest request = new() { DictionaryName = dictionaryName.ToByteString() };
        _DictionaryGetAllResponse response;
        try
        {
            response = this.grpcManager.Client.DictionaryGetAll(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheDictionaryFetchResponse(response);
    }

    public async Task<CacheDictionaryFetchResponse> DictionaryFetchAsync(string cacheName, string dictionaryName)
    {
        _DictionaryGetAllRequest request = new() { DictionaryName = dictionaryName.ToByteString() };
        _DictionaryGetAllResponse response;
        try
        {
            response = await this.grpcManager.Client.DictionaryGetAllAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
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
        request.Some.Keys.Add(field);

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
        request.Some.Keys.Add(field);

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
        request.Some.Keys.Add(fields);

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
        request.Some.Keys.Add(fields);

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
        return await SendSetAddAsync(cacheName, setName, element.ToByteString(), refreshTtl, ttlSeconds);
    }

    public async Task<CacheSetAddResponse> SetAddAsync(string cacheName, string setName, string element, bool refreshTtl, uint? ttlSeconds = null)
    {
        return await SendSetAddAsync(cacheName, setName, element.ToByteString(), refreshTtl, ttlSeconds);
    }

    private async Task<CacheSetAddResponse> SendSetAddAsync(string cacheName, string setName, ByteString element, bool refreshTtl, uint? ttlSeconds = null)
    {
        _SetUnionRequest request = new()
        {
            SetName = setName.ToByteString(),
            RefreshTtl = refreshTtl,
            TtlMilliseconds = ttlSecondsToMilliseconds(ttlSeconds)
        };
        request.Elements.Add(element);

        try
        {
            await this.grpcManager.Client.SetUnionAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheSetAddResponse();
    }

    public async Task<CacheSetAddBatchResponse> SetAddBatchAsync(string cacheName, string setName, IEnumerable<byte[]> elements, bool refreshTtl, uint? ttlSeconds = null)
    {
        return await SendSetAddBatchAsync(cacheName, setName, elements.Select(element => element.ToByteString()), refreshTtl, ttlSeconds);
    }

    public async Task<CacheSetAddBatchResponse> SetAddBatchAsync(string cacheName, string setName, IEnumerable<string> elements, bool refreshTtl, uint? ttlSeconds = null)
    {
        return await SendSetAddBatchAsync(cacheName, setName, elements.Select(element => element.ToByteString()), refreshTtl, ttlSeconds);
    }

    public async Task<CacheSetAddBatchResponse> SendSetAddBatchAsync(string cacheName, string setName, IEnumerable<ByteString> elements, bool refreshTtl, uint? ttlSeconds = null)
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
        return new CacheSetAddBatchResponse();
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
