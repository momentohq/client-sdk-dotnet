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
        var response = SendDictionarySet(cacheName, dictionaryName, Convert(field), Convert(value), refreshTtl, ttlSeconds);
        return new CacheDictionarySetResponse(dictionaryName, field, value);
    }

    public CacheDictionarySetResponse DictionarySet(string cacheName, string dictionaryName, string field, string value, bool refreshTtl, uint? ttlSeconds = null)
    {
        var response = SendDictionarySet(cacheName, dictionaryName, Convert(field), Convert(value), refreshTtl, ttlSeconds);
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
        var response = await SendDictionarySetAsync(cacheName, dictionaryName, Convert(field), Convert(value), refreshTtl, ttlSeconds);
        return new CacheDictionarySetResponse(dictionaryName, field, value);
    }

    public async Task<CacheDictionarySetResponse> DictionarySetAsync(string cacheName, string dictionaryName, string field, string value, bool refreshTtl, uint? ttlSeconds = null)
    {
        var response = await SendDictionarySetAsync(cacheName, dictionaryName, Convert(field), Convert(value), refreshTtl, ttlSeconds);
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
        var response = SendDictionaryGet(cacheName, dictionaryName, Convert(field));
        var responseAtom = response.DictionaryBody[0];
        return new CacheDictionaryGetResponse(responseAtom);
    }

    public CacheDictionaryGetResponse DictionaryGet(string cacheName, string dictionaryName, string field)
    {
        var response = SendDictionaryGet(cacheName, dictionaryName, Convert(field));
        var responseAtom = response.DictionaryBody[0];
        return new CacheDictionaryGetResponse(responseAtom);
    }

    private _DictionaryGetResponse SendDictionaryGet(string cacheName, string dictionaryName, ByteString field)
    {
        _DictionaryGetRequest request = new() { DictionaryName = Convert(dictionaryName) };
        request.DictionaryKeys.Add(field);

        try
        {
            return this.grpcManager.Client.DictionaryGet(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public async Task<CacheDictionaryGetResponse> DictionaryGetAsync(string cacheName, string dictionaryName, byte[] field)
    {
        var response = await SendDictionaryGetAsync(cacheName, dictionaryName, Convert(field));
        var responseAtom = response.DictionaryBody[0];
        return new CacheDictionaryGetResponse(responseAtom);
    }

    public async Task<CacheDictionaryGetResponse> DictionaryGetAsync(string cacheName, string dictionaryName, string field)
    {
        var response = await SendDictionaryGetAsync(cacheName, dictionaryName, Convert(field));
        var responseAtom = response.DictionaryBody[0];
        return new CacheDictionaryGetResponse(responseAtom);
    }

    private async Task<_DictionaryGetResponse> SendDictionaryGetAsync(string cacheName, string dictionaryName, ByteString field)
    {
        _DictionaryGetRequest request = new() { DictionaryName = Convert(dictionaryName) };
        request.DictionaryKeys.Add(field);

        try
        {
            return await this.grpcManager.Client.DictionaryGetAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public CacheDictionarySetMultiResponse DictionarySetMulti(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<byte[], byte[]>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        var protoItems = items.Select(kv => new _DictionaryKeyValuePair() { Key = Convert(kv.Key), Value = Convert(kv.Value) });
        var response = SendDictionarySetMulti(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
        return new CacheDictionarySetMultiResponse(dictionaryName, items);
    }

    public CacheDictionarySetMultiResponse DictionarySetMulti(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, string>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        var protoItems = items.Select(kv => new _DictionaryKeyValuePair() { Key = Convert(kv.Key), Value = Convert(kv.Value) });
        var response = SendDictionarySetMulti(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
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
        var response = await SendDictionarySetMultiAsync(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
        return new CacheDictionarySetMultiResponse(dictionaryName, items);
    }

    public async Task<CacheDictionarySetMultiResponse> DictionarySetMultiAsync(string cacheName, string dictionaryName, IEnumerable<KeyValuePair<string, string>> items, bool refreshTtl, uint? ttlSeconds = null)
    {
        var protoItems = items.Select(kv => new _DictionaryKeyValuePair() { Key = Convert(kv.Key), Value = Convert(kv.Value) });
        var response = await SendDictionarySetMultiAsync(cacheName, dictionaryName, protoItems, refreshTtl, ttlSeconds);
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

    // Get multi

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
}
