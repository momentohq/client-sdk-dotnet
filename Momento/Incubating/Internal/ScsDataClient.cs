using System;
using CacheClient;
using Google.Protobuf;
using Grpc.Core;
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

    public CacheDictionarySetResponse DictionarySet(string cacheName, string dictionaryName, string key, string value, bool refreshTtl, uint? ttlSeconds = null)
    {
        var response = SendDictionarySet(cacheName, dictionaryName, Convert(key), Convert(value), refreshTtl, ttlSeconds);
        return new CacheDictionarySetResponse();
    }


    private _DictionarySetResponse SendDictionarySet(string cacheName, string dictionaryName, ByteString key, ByteString value, bool refreshTtl, uint? ttlSeconds = null)
    {
        _DictionarySetRequest request = new()
        {
            DictionaryName = Convert(dictionaryName),
            RefreshTtl = refreshTtl,
            TtlMilliseconds = ttlSecondsToMilliseconds(ttlSeconds)
        };
        request.DictionaryBody.Add(new _DictionaryKeyValuePair() { Key = key, Value = value });

        try
        {
            return this.grpcManager.Client.DictionarySet(request, new Metadata { { "cache", cacheName } }, deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public CacheDictionaryGetResponse DictionaryGet(string cacheName, string dictionaryName, string key)
    {
        var response = SendDictionaryGet(cacheName, dictionaryName, Convert(key));
        var responseAtom = response.DictionaryBody[0];
        return new CacheDictionaryGetResponse(responseAtom);
    }


    private _DictionaryGetResponse SendDictionaryGet(string cacheName, string dictionaryName, ByteString key)
    {
        _DictionaryGetRequest request = new()
        {
            DictionaryName = Convert(dictionaryName),
        };
        request.DictionaryKeys.Add(key);

        try
        {
            return this.grpcManager.Client.DictionaryGet(request, new Metadata { { "cache", cacheName } }, deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }
}
