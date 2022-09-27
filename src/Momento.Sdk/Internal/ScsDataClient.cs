using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Protos.CacheClient;
using Momento.Sdk.Config;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal.ExtensionMethods;
using Momento.Sdk.Responses;

namespace Momento.Sdk.Internal;

public class ScsDataClientBase : IDisposable
{
    protected readonly DataGrpcManager grpcManager;
    protected readonly uint defaultTtlSeconds;
    protected readonly uint dataClientOperationTimeoutMilliseconds;
    protected readonly ILogger _logger;

    public ScsDataClientBase(IConfiguration config, string authToken, string endpoint, uint defaultTtlSeconds, ILoggerFactory loggerFactory)
    {
        this.grpcManager = new(config, authToken, endpoint, loggerFactory);
        this.defaultTtlSeconds = defaultTtlSeconds;
        this.dataClientOperationTimeoutMilliseconds = config.TransportStrategy.GrpcConfig.DeadlineMilliseconds;
        this._logger = loggerFactory.CreateLogger<ScsDataClient>();
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
    protected uint TtlSecondsToMilliseconds(uint? ttlSeconds = null)
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
    public ScsDataClient(IConfiguration config, string authToken, string endpoint, uint defaultTtlSeconds, ILoggerFactory loggerFactory)
        : base(config, authToken, endpoint, defaultTtlSeconds, loggerFactory)
    {

    }

    public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value, uint? ttlSeconds = null)
    {
        return await this.SendSetAsync(cacheName, value: value.ToByteString(), key: key.ToByteString(), ttlSeconds: ttlSeconds);
    }

    public async Task<CacheGetResponse> GetAsync(string cacheName, byte[] key)
    {
        return await this.SendGetAsync(cacheName, key.ToByteString());
    }

    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, byte[] key)
    {
        return await this.SendDeleteAsync(cacheName, key.ToByteString());
    }

    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, uint? ttlSeconds = null)
    {
        return await this.SendSetAsync(cacheName, key: key.ToByteString(), value: value.ToByteString(), ttlSeconds: ttlSeconds);
    }

    public async Task<CacheGetResponse> GetAsync(string cacheName, string key)
    {
        return await this.SendGetAsync(cacheName, key.ToByteString());
    }

    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, string key)
    {
        return await this.SendDeleteAsync(cacheName, key.ToByteString());
    }

    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, byte[] value, uint? ttlSeconds = null)
    {
        return await this.SendSetAsync(cacheName, value: value.ToByteString(), key: key.ToByteString(), ttlSeconds: ttlSeconds);
    }

    public async Task<CacheGetBatchResponse> GetBatchAsync(string cacheName, IEnumerable<string> keys)
    {
        return await GetBatchAsync(cacheName, keys.Select(key => key.ToByteString()));
    }

    public async Task<CacheGetBatchResponse> GetBatchAsync(string cacheName, IEnumerable<byte[]> keys)
    {
        return await GetBatchAsync(cacheName, keys.Select(key => key.ToByteString()));
    }

    public async Task<CacheGetBatchResponse> GetBatchAsync(string cacheName, IEnumerable<ByteString> keys)
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
            return new CacheGetBatchResponse.Error(CacheExceptionMapper.Convert(e));
        }

        // Handle failures
        if (continuation.Status == TaskStatus.Faulted)
        {
            return new CacheGetBatchResponse.Error(
                CacheExceptionMapper.Convert(continuation.Exception)
            );
        }
        else if (continuation.Status != TaskStatus.RanToCompletion)
        {
            return new CacheGetBatchResponse.Error(
                CacheExceptionMapper.Convert(
                    new Exception(String.Format("Failure issuing multi-get: {0}", continuation.Status))
                )
            );
        }

        // preserve old behavior of failing on first error
        foreach (CacheGetResponse response in continuation.Result)
        {
            if (response is CacheGetResponse.Error errorResponse)
            {
                return new CacheGetBatchResponse.Error(errorResponse.Exception);
            }
        }

        // Package results
        return new CacheGetBatchResponse.Success(continuation.Result);
    }

    public async Task<CacheSetBatchResponse> SetBatchAsync(string cacheName, IEnumerable<KeyValuePair<string, string>> items, uint? ttlSeconds = null)
    {
        return await SendSetBatchAsync(cacheName: cacheName,
            items: items.Select(item => new KeyValuePair<ByteString, ByteString>(item.Key.ToByteString(), item.Value.ToByteString())),
            ttlSeconds: ttlSeconds);
    }

    public async Task<CacheSetBatchResponse> SetBatchAsync(string cacheName, IEnumerable<KeyValuePair<byte[], byte[]>> items, uint? ttlSeconds = null)
    {
        return await SendSetBatchAsync(cacheName: cacheName,
            items: items.Select(item => new KeyValuePair<ByteString, ByteString>(item.Key.ToByteString(), item.Value.ToByteString())),
            ttlSeconds: ttlSeconds);
    }

    public async Task<CacheSetBatchResponse> SendSetBatchAsync(string cacheName, IEnumerable<KeyValuePair<ByteString, ByteString>> items, uint? ttlSeconds = null)
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
            return new CacheSetBatchResponse.Error(
                CacheExceptionMapper.Convert(e)
            );
        }

        // Handle failures
        if (continuation.Status == TaskStatus.Faulted)
        {
            return new CacheSetBatchResponse.Error(
                CacheExceptionMapper.Convert(continuation.Exception)
            );
        }
        else if (continuation.Status != TaskStatus.RanToCompletion)
        {
            return new CacheSetBatchResponse.Error(
                CacheExceptionMapper.Convert(
                    new Exception(String.Format("Failure issuing multi-set: {0}", continuation.Status))
                )
            );
        }
        return new CacheSetBatchResponse.Success();
    }

    private async Task<CacheSetResponse> SendSetAsync(string cacheName, ByteString key, ByteString value, uint? ttlSeconds = null)
    {
        _SetRequest request = new _SetRequest() { CacheBody = value, CacheKey = key, TtlMilliseconds = TtlSecondsToMilliseconds(ttlSeconds) };
        try
        {
            await this.grpcManager.Client.SetAsync(request, new CallOptions(headers: MetadataWithCache(cacheName), deadline: CalculateDeadline()));
        }
        catch (Exception e)
        {
            var exc = CacheExceptionMapper.Convert(e);
            if (exc.TransportDetails != null)
            {
                exc.TransportDetails.Grpc.Metadata = MetadataWithCache(cacheName);
            }
            return new CacheSetResponse.Error(exc);
        }
        return new CacheSetResponse.Success();
    }

    private async Task<CacheGetResponse> SendGetAsync(string cacheName, ByteString key)
    {
        _GetRequest request = new _GetRequest() { CacheKey = key };
        _GetResponse response;
        try
        {
            response = await this.grpcManager.Client.GetAsync(request, new CallOptions(headers: MetadataWithCache(cacheName), deadline: CalculateDeadline()));
        }
        catch (Exception e)
        {
            var exc = CacheExceptionMapper.Convert(e);
            if (exc.TransportDetails != null)
            {
                exc.TransportDetails.Grpc.Metadata = MetadataWithCache(cacheName);
            }
            return new CacheGetResponse.Error(exc);
        }

        if (response.Result == ECacheResult.Miss)
        {
            return new CacheGetResponse.Miss();
        }
        return new CacheGetResponse.Hit(response);
    }

    private async Task<CacheDeleteResponse> SendDeleteAsync(string cacheName, ByteString key)
    {
        _DeleteRequest request = new _DeleteRequest() { CacheKey = key };
        try
        {
            await this.grpcManager.Client.DeleteAsync(request, new CallOptions(headers: MetadataWithCache(cacheName), deadline: CalculateDeadline()));
        }
        catch (Exception e)
        {
            var exc = CacheExceptionMapper.Convert(e);
            if (exc.TransportDetails != null)
            {
                exc.TransportDetails.Grpc.Metadata = MetadataWithCache(cacheName);
            }
            return new CacheDeleteResponse.Error(exc);
        }
        return new CacheDeleteResponse.Success();
    }
}
