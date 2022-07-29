using System;
using System.Linq;
using System.Threading.Tasks;
using MomentoSdk.Internal.ExtensionMethods;
using MomentoSdk.Responses;
using MomentoSdk.Exceptions;
using CacheClient;
using Google.Protobuf;
using Grpc.Core;
using System.Collections.Generic;

namespace MomentoSdk.Internal;

public class ScsDataClientBase : IDisposable
{
    protected readonly DataGrpcManager grpcManager;
    protected readonly uint defaultTtlSeconds;
    protected readonly uint dataClientOperationTimeoutMilliseconds;
    protected const uint DEFAULT_DEADLINE_MILLISECONDS = 5000;

    public ScsDataClientBase(string authToken, string endpoint, uint defaultTtlSeconds, uint? dataClientOperationTimeoutMilliseconds = null)
    {
        this.grpcManager = new(authToken, endpoint);
        this.defaultTtlSeconds = defaultTtlSeconds;
        this.dataClientOperationTimeoutMilliseconds = dataClientOperationTimeoutMilliseconds ?? DEFAULT_DEADLINE_MILLISECONDS;
    }

    protected Metadata MetadataWithCache(string cacheName)
    {
        return new Metadata() { { "cache", cacheName } };
    }
    protected DateTime CalculateDeadline()
    {
        return DateTime.UtcNow.AddMilliseconds(dataClientOperationTimeoutMilliseconds);
    }

    /// <summary>
    /// Converts TTL in seconds to milliseconds. Defaults to <c>defaultTtlSeconds</c>.
    /// </summary>
    /// <param name="ttlSeconds">The TTL to convert. Defaults to defaultTtlSeconds</param>
    /// <returns></returns>
    protected uint ttlSecondsToMilliseconds(uint? ttlSeconds = null)
    {
        return (ttlSeconds ?? defaultTtlSeconds) * 1000;
    }

    public void Dispose()
    {
        this.grpcManager.Dispose();
    }
}

internal sealed class ScsDataClient : ScsDataClientBase
{
    public ScsDataClient(string authToken, string endpoint, uint defaultTtlSeconds, uint? dataClientOperationTimeoutMilliseconds = null)
        : base(authToken, endpoint, defaultTtlSeconds, dataClientOperationTimeoutMilliseconds)
    {
    }

    public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value, uint? ttlSeconds = null)
    {
        await this.SendSetAsync(cacheName, value: value.ToByteString(), key: key.ToByteString(), ttlSeconds: ttlSeconds);
        return new CacheSetResponse();
    }

    public async Task<CacheGetResponse> GetAsync(string cacheName, byte[] key)
    {
        _GetResponse resp = await this.SendGetAsync(cacheName, key.ToByteString());
        return new CacheGetResponse(resp);
    }

    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, byte[] key)
    {
        await this.SendDeleteAsync(cacheName, key.ToByteString());
        return new CacheDeleteResponse();
    }

    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, uint? ttlSeconds = null)
    {
        await this.SendSetAsync(cacheName, key: key.ToByteString(), value: value.ToByteString(), ttlSeconds: ttlSeconds);
        return new CacheSetResponse();
    }

    public async Task<CacheGetResponse> GetAsync(string cacheName, string key)
    {
        _GetResponse resp = await this.SendGetAsync(cacheName, key.ToByteString());
        return new CacheGetResponse(resp);
    }

    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, string key)
    {
        await this.SendDeleteAsync(cacheName, key.ToByteString());
        return new CacheDeleteResponse();
    }

    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, byte[] value, uint? ttlSeconds = null)
    {
        await this.SendSetAsync(cacheName, value: value.ToByteString(), key: key.ToByteString(), ttlSeconds: ttlSeconds);
        return new CacheSetResponse();
    }

    public async Task<CacheGetMultiResponse> GetMultiAsync(string cacheName, IEnumerable<string> keys)
    {
        return await GetMultiAsync(cacheName, keys.Select(key => key.ToByteString()));
    }

    public async Task<CacheGetMultiResponse> GetMultiAsync(string cacheName, IEnumerable<byte[]> keys)
    {
        return await GetMultiAsync(cacheName, keys.Select(key => key.ToByteString()));
    }

    public async Task<CacheGetMultiResponse> GetMultiAsync(string cacheName, IEnumerable<ByteString> keys)
    {
        // Gather the tasks
        var tasks = keys.Select(key => SendGetAsync(cacheName, key));

        // Run the tasks
        var continuation = Task.WhenAll(tasks);
        try
        {
            await continuation;
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }

        // Handle failures
        if (continuation.Status == TaskStatus.Faulted)
        {
            throw CacheExceptionMapper.Convert(continuation.Exception);
        }
        else if (continuation.Status != TaskStatus.RanToCompletion)
        {
            throw CacheExceptionMapper.Convert(new Exception(String.Format("Failure issuing multi-get: {0}", continuation.Status)));
        }

        // Package results
        var results = continuation.Result.Select(response => new CacheGetResponse(response));
        return new CacheGetMultiResponse(results);
    }

    public async Task<CacheSetMultiResponse> SetMultiAsync(string cacheName, IEnumerable<KeyValuePair<string, string>> items, uint? ttlSeconds = null)
    {
        await SendSetMultiAsync(cacheName: cacheName,
            items: items.Select(item => new KeyValuePair<ByteString, ByteString>(item.Key.ToByteString(), item.Value.ToByteString())),
            ttlSeconds: ttlSeconds);
        return new CacheSetMultiResponse();
    }

    public async Task<CacheSetMultiResponse> SetMultiAsync(string cacheName, IEnumerable<KeyValuePair<byte[], byte[]>> items, uint? ttlSeconds = null)
    {
        await SendSetMultiAsync(cacheName: cacheName,
            items: items.Select(item => new KeyValuePair<ByteString, ByteString>(item.Key.ToByteString(), item.Value.ToByteString())),
            ttlSeconds: ttlSeconds);
        return new CacheSetMultiResponse();
    }

    public async Task SendSetMultiAsync(string cacheName, IEnumerable<KeyValuePair<ByteString, ByteString>> items, uint? ttlSeconds = null)
    {
        // Gather the tasks
        var tasks = items.Select(item => SendSetAsync(cacheName, item.Key, item.Value, ttlSeconds));

        // Run the tasks
        var continuation = Task.WhenAll(tasks);
        try
        {
            await continuation;
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }

        // Handle failures
        if (continuation.Status == TaskStatus.Faulted)
        {
            throw CacheExceptionMapper.Convert(continuation.Exception);
        }
        else if (continuation.Status != TaskStatus.RanToCompletion)
        {
            throw CacheExceptionMapper.Convert(new Exception(String.Format("Failure issuing multi-set: {0}", continuation.Status)));
        }
    }

    public CacheSetResponse Set(string cacheName, byte[] key, byte[] value, uint? ttlSeconds = null)
    {
        this.SendSet(cacheName, key: key.ToByteString(), value: value.ToByteString(), ttlSeconds: ttlSeconds);
        return new CacheSetResponse();
    }

    public CacheGetResponse Get(string cacheName, byte[] key)
    {
        _GetResponse resp = this.SendGet(cacheName, key.ToByteString());
        return new CacheGetResponse(resp);
    }

    public CacheDeleteResponse Delete(string cacheName, byte[] key)
    {
        this.SendDelete(cacheName, key.ToByteString());
        return new CacheDeleteResponse();
    }

    public CacheSetResponse Set(string cacheName, string key, string value, uint? ttlSeconds = null)
    {
        this.SendSet(cacheName, key: key.ToByteString(), value: value.ToByteString(), ttlSeconds: ttlSeconds);
        return new CacheSetResponse();
    }

    public CacheGetResponse Get(string cacheName, string key)
    {
        _GetResponse resp = this.SendGet(cacheName, key.ToByteString());
        return new CacheGetResponse(resp);
    }

    public CacheDeleteResponse Delete(string cacheName, string key)
    {
        this.SendDelete(cacheName, key.ToByteString());
        return new CacheDeleteResponse();
    }

    public CacheSetResponse Set(string cacheName, string key, byte[] value, uint? ttlSeconds = null)
    {
        this.SendSet(cacheName, key: key.ToByteString(), value: value.ToByteString(), ttlSeconds: ttlSeconds);
        return new CacheSetResponse();
    }

    private async Task<_SetResponse> SendSetAsync(string cacheName, ByteString key, ByteString value, uint? ttlSeconds = null)
    {
        _SetRequest request = new _SetRequest() { CacheBody = value, CacheKey = key, TtlMilliseconds = ttlSecondsToMilliseconds(ttlSeconds) };
        try
        {
            return await this.grpcManager.Client.SetAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    private _GetResponse SendGet(string cacheName, ByteString key)
    {
        _GetRequest request = new _GetRequest() { CacheKey = key };
        try
        {
            return this.grpcManager.Client.Get(request, new Metadata { { "cache", cacheName } }, deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    private async Task<_GetResponse> SendGetAsync(string cacheName, ByteString key)
    {
        _GetRequest request = new _GetRequest() { CacheKey = key };
        try
        {
            return await this.grpcManager.Client.GetAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    private _SetResponse SendSet(string cacheName, ByteString key, ByteString value, uint? ttlSeconds = null)
    {
        _SetRequest request = new _SetRequest() { CacheBody = value, CacheKey = key, TtlMilliseconds = ttlSecondsToMilliseconds(ttlSeconds) };
        try
        {
            return this.grpcManager.Client.Set(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    private _DeleteResponse SendDelete(string cacheName, ByteString key)
    {
        _DeleteRequest request = new _DeleteRequest() { CacheKey = key };
        try
        {
            return this.grpcManager.Client.Delete(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    private async Task<_DeleteResponse> SendDeleteAsync(string cacheName, ByteString key)
    {
        _DeleteRequest request = new _DeleteRequest() { CacheKey = key };
        try
        {
            return await this.grpcManager.Client.DeleteAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }
}
