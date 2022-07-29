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

    public CacheDictionarySetMultiResponse DictionarySetMulti(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<byte[], byte[]>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        var protoItems = items.Select(kv => new _DictionaryKeyValuePair() { Key = kv.Key.ToByteString(), Value = kv.Value.ToByteString() });
        return SendDictionarySetMulti(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
    }

    public CacheDictionarySetMultiResponse DictionarySetMulti(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, string>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        var protoItems = items.Select(kv => new _DictionaryKeyValuePair() { Key = kv.Key.ToByteString(), Value = kv.Value.ToByteString() });
        return SendDictionarySetMulti(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
    }

    public CacheDictionarySetMultiResponse SendDictionarySetMulti(string cacheName, string dictionaryName, IEnumerable<_DictionaryKeyValuePair> items, bool refreshTtl, uint? ttlSeconds = null)
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
        return new CacheDictionarySetMultiResponse();
    }

    public async Task<CacheDictionarySetMultiResponse> DictionarySetMultiAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<byte[], byte[]>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        var protoItems = items.Select(kv => new _DictionaryKeyValuePair() { Key = kv.Key.ToByteString(), Value = kv.Value.ToByteString() });
        return await SendDictionarySetMultiAsync(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
    }

    public async Task<CacheDictionarySetMultiResponse> DictionarySetMultiAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, string>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        var protoItems = items.Select(kv => new _DictionaryKeyValuePair() { Key = kv.Key.ToByteString(), Value = kv.Value.ToByteString() });
        return await SendDictionarySetMultiAsync(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
    }

    public async Task<CacheDictionarySetMultiResponse> SendDictionarySetMultiAsync(string cacheName, string dictionaryName, IEnumerable<_DictionaryKeyValuePair> items, bool refreshTtl, uint? ttlSeconds = null)
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
        return new CacheDictionarySetMultiResponse();
    }

    public CacheDictionaryGetMultiResponse DictionaryGetMulti(string cacheName, string dictionaryName, params byte[][] fields)
    {
        return SendDictionaryGetMulti(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    public CacheDictionaryGetMultiResponse DictionaryGetMulti(string cacheName, string dictionaryName, params string[] fields)
    {
        return SendDictionaryGetMulti(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    public CacheDictionaryGetMultiResponse DictionaryGetMulti(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        return SendDictionaryGetMulti(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    public CacheDictionaryGetMultiResponse DictionaryGetMulti(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        return SendDictionaryGetMulti(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    private CacheDictionaryGetMultiResponse SendDictionaryGetMulti(string cacheName, string dictionaryName, IEnumerable<ByteString> fields)
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
        return new CacheDictionaryGetMultiResponse(response);
    }

    public async Task<CacheDictionaryGetMultiResponse> DictionaryGetMultiAsync(string cacheName, string dictionaryName, params byte[][] fields)
    {
        return await SendDictionaryGetMultiAsync(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    public async Task<CacheDictionaryGetMultiResponse> DictionaryGetMultiAsync(string cacheName, string dictionaryName, params string[] fields)
    {
        return await SendDictionaryGetMultiAsync(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    public async Task<CacheDictionaryGetMultiResponse> DictionaryGetMultiAsync(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        return await SendDictionaryGetMultiAsync(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    public async Task<CacheDictionaryGetMultiResponse> DictionaryGetMultiAsync(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        return await SendDictionaryGetMultiAsync(cacheName, dictionaryName, fields.Select(field => field.ToByteString()));
    }

    private async Task<CacheDictionaryGetMultiResponse> SendDictionaryGetMultiAsync(string cacheName, string dictionaryName, IEnumerable<ByteString> fields)
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
        return new CacheDictionaryGetMultiResponse(response);
    }

    public CacheDictionaryGetAllResponse DictionaryGetAll(string cacheName, string dictionaryName)
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
        return new CacheDictionaryGetAllResponse(response);
    }

    public async Task<CacheDictionaryGetAllResponse> DictionaryGetAllAsync(string cacheName, string dictionaryName)
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
        return new CacheDictionaryGetAllResponse(response);
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
}
