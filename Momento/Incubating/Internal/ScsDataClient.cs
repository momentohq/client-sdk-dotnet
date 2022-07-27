using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CacheClient;
using Google.Protobuf;
using MomentoSdk.Internal;
using MomentoSdk.Incubating.Responses;
using MomentoSdk.Exceptions;

namespace MomentoSdk.Incubating.Internal;

internal sealed class ScsDataClient : ScsDataClientBase
{
    public ScsDataClient(string authToken, string host, uint defaultTtlSeconds, uint? dataClientOperationTimeoutMilliseconds = null)
        : base(authToken, host, defaultTtlSeconds, dataClientOperationTimeoutMilliseconds)
    {
    }

    public CacheDictionarySetResponse DictionarySet(string cacheName, string dictionaryName, byte[] field, byte[] value, bool refreshTtl, uint? ttlSeconds = null)
    {
        SendDictionarySet(cacheName, dictionaryName, Convert(field), Convert(value), refreshTtl, ttlSeconds);
        return new CacheDictionarySetResponse(dictionaryName, field, value);
    }

    public CacheDictionarySetResponse DictionarySet(string cacheName, string dictionaryName, string field, string value, bool refreshTtl, uint? ttlSeconds = null)
    {
        SendDictionarySet(cacheName, dictionaryName, Convert(field), Convert(value), refreshTtl, ttlSeconds);
        return new CacheDictionarySetResponse(dictionaryName, field, value);
    }


    private _DictionarySetResponse SendDictionarySet(string cacheName, string dictionaryName, ByteString field, ByteString value, bool refreshTtl, uint? ttlSeconds = null)
    {
        _DictionarySetRequest request = new()
        {
            DictionaryName = Convert(dictionaryName),
            RefreshTtl = refreshTtl,
            TtlMilliseconds = ttlSecondsToMilliseconds(ttlSeconds)
        };
        request.DictionaryBody.Add(new _DictionaryKeyValuePair() { Key = field, Value = value });

        try
        {
            return this.grpcManager.Client.DictionarySet(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public async Task<CacheDictionarySetResponse> DictionarySetAsync(string cacheName, string dictionaryName, byte[] field, byte[] value, bool refreshTtl, uint? ttlSeconds = null)
    {
        await SendDictionarySetAsync(cacheName, dictionaryName, Convert(field), Convert(value), refreshTtl, ttlSeconds);
        return new CacheDictionarySetResponse(dictionaryName, field, value);
    }

    public async Task<CacheDictionarySetResponse> DictionarySetAsync(string cacheName, string dictionaryName, string field, string value, bool refreshTtl, uint? ttlSeconds = null)
    {
        await SendDictionarySetAsync(cacheName, dictionaryName, Convert(field), Convert(value), refreshTtl, ttlSeconds);
        return new CacheDictionarySetResponse(dictionaryName, field, value);
    }


    private async Task<_DictionarySetResponse> SendDictionarySetAsync(string cacheName, string dictionaryName, ByteString field, ByteString value, bool refreshTtl, uint? ttlSeconds = null)
    {
        _DictionarySetRequest request = new()
        {
            DictionaryName = Convert(dictionaryName),
            RefreshTtl = refreshTtl,
            TtlMilliseconds = ttlSecondsToMilliseconds(ttlSeconds)
        };
        request.DictionaryBody.Add(new _DictionaryKeyValuePair() { Key = field, Value = value });

        try
        {
            return await this.grpcManager.Client.DictionarySetAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public CacheDictionaryGetResponse DictionaryGet(string cacheName, string dictionaryName, byte[] field)
    {
        return SendDictionaryGet(cacheName, dictionaryName, Convert(field));
    }

    public CacheDictionaryGetResponse DictionaryGet(string cacheName, string dictionaryName, string field)
    {
        return SendDictionaryGet(cacheName, dictionaryName, Convert(field));
    }

    private CacheDictionaryGetResponse SendDictionaryGet(string cacheName, string dictionaryName, ByteString field)
    {
        _DictionaryGetRequest request = new() { DictionaryName = Convert(dictionaryName) };
        request.DictionaryKeys.Add(field);

        try
        {
            var response = this.grpcManager.Client.DictionaryGet(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
            return new CacheDictionaryGetResponse(response.DictionaryBody[0]);

        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public async Task<CacheDictionaryGetResponse> DictionaryGetAsync(string cacheName, string dictionaryName, byte[] field)
    {
        return await SendDictionaryGetAsync(cacheName, dictionaryName, Convert(field));
    }

    public async Task<CacheDictionaryGetResponse> DictionaryGetAsync(string cacheName, string dictionaryName, string field)
    {
        return await SendDictionaryGetAsync(cacheName, dictionaryName, Convert(field));
    }

    private async Task<CacheDictionaryGetResponse> SendDictionaryGetAsync(string cacheName, string dictionaryName, ByteString field)
    {
        _DictionaryGetRequest request = new() { DictionaryName = Convert(dictionaryName) };
        request.DictionaryKeys.Add(field);

        try
        {
            var response = await this.grpcManager.Client.DictionaryGetAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
            return new CacheDictionaryGetResponse(response.DictionaryBody[0]);
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public CacheDictionarySetMultiResponse DictionarySetMulti(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<byte[], byte[]>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        var protoItems = items.Select(kv => new _DictionaryKeyValuePair() { Key = Convert(kv.Key), Value = Convert(kv.Value) });
        SendDictionarySetMulti(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
        return new CacheDictionarySetMultiResponse(dictionaryName, items);
    }

    public CacheDictionarySetMultiResponse DictionarySetMulti(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, string>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        var protoItems = items.Select(kv => new _DictionaryKeyValuePair() { Key = Convert(kv.Key), Value = Convert(kv.Value) });
        SendDictionarySetMulti(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
        return new CacheDictionarySetMultiResponse(dictionaryName, items);
    }

    public _DictionarySetResponse SendDictionarySetMulti(string cacheName, string dictionaryName, IEnumerable<_DictionaryKeyValuePair> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        _DictionarySetRequest request = new()
        {
            DictionaryName = Convert(dictionaryName),
            RefreshTtl = refreshTtl,
            TtlMilliseconds = ttlSecondsToMilliseconds(ttlSeconds)
        };
        request.DictionaryBody.Add(items);

        try
        {
            return this.grpcManager.Client.DictionarySet(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public async Task<CacheDictionarySetMultiResponse> DictionarySetMultiAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<byte[], byte[]>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        var protoItems = items.Select(kv => new _DictionaryKeyValuePair() { Key = Convert(kv.Key), Value = Convert(kv.Value) });
        await SendDictionarySetMultiAsync(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
        return new CacheDictionarySetMultiResponse(dictionaryName, items);
    }

    public async Task<CacheDictionarySetMultiResponse> DictionarySetMultiAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, string>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        var protoItems = items.Select(kv => new _DictionaryKeyValuePair() { Key = Convert(kv.Key), Value = Convert(kv.Value) });
        await SendDictionarySetMultiAsync(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
        return new CacheDictionarySetMultiResponse(dictionaryName, items);
    }

    public async Task<_DictionarySetResponse> SendDictionarySetMultiAsync(string cacheName, string dictionaryName, IEnumerable<_DictionaryKeyValuePair> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        _DictionarySetRequest request = new()
        {
            DictionaryName = Convert(dictionaryName),
            RefreshTtl = refreshTtl,
            TtlMilliseconds = ttlSecondsToMilliseconds(ttlSeconds)
        };
        request.DictionaryBody.Add(items);

        try
        {
            return await this.grpcManager.Client.DictionarySetAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public CacheDictionaryGetMultiResponse DictionaryGetMulti(string cacheName, string dictionaryName, params byte[][] fields)
    {
        return SendDictionaryGetMulti(cacheName, dictionaryName, fields.Select(field => Convert(field)));
    }

    public CacheDictionaryGetMultiResponse DictionaryGetMulti(string cacheName, string dictionaryName, params string[] fields)
    {
        return SendDictionaryGetMulti(cacheName, dictionaryName, fields.Select(field => Convert(field)));
    }

    public CacheDictionaryGetMultiResponse DictionaryGetMulti(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        return SendDictionaryGetMulti(cacheName, dictionaryName, fields.Select(field => Convert(field)));
    }

    public CacheDictionaryGetMultiResponse DictionaryGetMulti(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        return SendDictionaryGetMulti(cacheName, dictionaryName, fields.Select(field => Convert(field)));
    }

    private CacheDictionaryGetMultiResponse SendDictionaryGetMulti(string cacheName, string dictionaryName, IEnumerable<ByteString> fields)
    {
        _DictionaryGetRequest request = new() { DictionaryName = Convert(dictionaryName) };
        request.DictionaryKeys.Add(fields);

        try
        {
            var response = this.grpcManager.Client.DictionaryGet(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
            return new CacheDictionaryGetMultiResponse(response);
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public async Task<CacheDictionaryGetMultiResponse> DictionaryGetMultiAsync(string cacheName, string dictionaryName, params byte[][] fields)
    {
        return await SendDictionaryGetMultiAsync(cacheName, dictionaryName, fields.Select(field => Convert(field)));
    }

    public async Task<CacheDictionaryGetMultiResponse> DictionaryGetMultiAsync(string cacheName, string dictionaryName, params string[] fields)
    {
        return await SendDictionaryGetMultiAsync(cacheName, dictionaryName, fields.Select(field => Convert(field)));
    }

    public async Task<CacheDictionaryGetMultiResponse> DictionaryGetMultiAsync(string cacheName, string dictionaryName, IEnumerable<byte[]> fields)
    {
        return await SendDictionaryGetMultiAsync(cacheName, dictionaryName, fields.Select(field => Convert(field)));
    }

    public async Task<CacheDictionaryGetMultiResponse> DictionaryGetMultiAsync(string cacheName, string dictionaryName, IEnumerable<string> fields)
    {
        return await SendDictionaryGetMultiAsync(cacheName, dictionaryName, fields.Select(field => Convert(field)));
    }

    private async Task<CacheDictionaryGetMultiResponse> SendDictionaryGetMultiAsync(string cacheName, string dictionaryName, IEnumerable<ByteString> fields)
    {
        _DictionaryGetRequest request = new() { DictionaryName = Convert(dictionaryName) };
        request.DictionaryKeys.Add(fields);

        try
        {
            var response = await this.grpcManager.Client.DictionaryGetAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
            return new CacheDictionaryGetMultiResponse(response);
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public CacheDictionaryGetAllResponse DictionaryGetAll(string cacheName, string dictionaryName)
    {
        _DictionaryGetAllRequest request = new() { DictionaryName = Convert(dictionaryName) };
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
        _DictionaryGetAllRequest request = new() { DictionaryName = Convert(dictionaryName) };
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
            DictionaryName = Convert(dictionaryName),
            All = new _DictionaryDeleteRequest.Types.All()
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
            DictionaryName = Convert(dictionaryName),
            All = new _DictionaryDeleteRequest.Types.All()
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
        return DictionaryRemoveField(cacheName, dictionaryName, Convert(field));
    }

    public CacheDictionaryRemoveFieldResponse DictionaryRemoveField(string cacheName, string dictionaryName, string field)
    {
        return DictionaryRemoveField(cacheName, dictionaryName, Convert(field));
    }

    public CacheDictionaryRemoveFieldResponse DictionaryRemoveField(string cacheName, string dictionaryName, ByteString field)
    {
        _DictionaryDeleteRequest request = new()
        {
            DictionaryName = Convert(dictionaryName),
            Some = new _DictionaryDeleteRequest.Types.Some()
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
        return await DictionaryRemoveFieldAsync(cacheName, dictionaryName, Convert(field));
    }

    public async Task<CacheDictionaryRemoveFieldResponse> DictionaryRemoveFieldAsync(string cacheName, string dictionaryName, string field)
    {
        return await DictionaryRemoveFieldAsync(cacheName, dictionaryName, Convert(field));
    }

    public async Task<CacheDictionaryRemoveFieldResponse> DictionaryRemoveFieldAsync(string cacheName, string dictionaryName, ByteString field)
    {
        _DictionaryDeleteRequest request = new()
        {
            DictionaryName = Convert(dictionaryName),
            Some = new _DictionaryDeleteRequest.Types.Some()
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
}
