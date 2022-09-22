using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal.ExtensionMethods;
using Momento.Sdk.Responses;
using Newtonsoft.Json.Linq;

namespace Momento.Sdk.Internal;

internal class ScsDataClientBase : IDisposable
{
    protected readonly DataGrpcManager grpcManager;
    protected readonly uint defaultTtlSeconds;
    protected readonly uint dataClientOperationTimeoutMilliseconds;
    protected const uint DEFAULT_DEADLINE_MILLISECONDS = 5000;

    internal ScsDataClientBase(string authToken, string endpoint, uint defaultTtlSeconds, uint? dataClientOperationTimeoutMilliseconds = null)
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
    //private readonly Semaphore _maxConcurrentRequestSemapore;
    private readonly System.Threading.Channels.Channel<bool> _requestTokenChannel;
    private readonly int _maxConcurrentRequests;

    public static async Task<ScsDataClient> ConstructScsDataClient(string authToken, string endpoint, int maxConcurrentRequests, uint defaultTtlSeconds, uint? dataClientOperationTimeoutMilliseconds = null)
    {
        var client = new ScsDataClient(authToken, endpoint, maxConcurrentRequests, defaultTtlSeconds, dataClientOperationTimeoutMilliseconds);
        await client.Initialize();
        return client;
    }

    private ScsDataClient(string authToken, string endpoint, int maxConcurrentRequests, uint defaultTtlSeconds, uint? dataClientOperationTimeoutMilliseconds = null)
        : base(authToken, endpoint, defaultTtlSeconds, dataClientOperationTimeoutMilliseconds)
    {
        Console.WriteLine($"\n\n\n\n\nCREATING SEMAPHORE WITH COUNT: {maxConcurrentRequests}\n\n\n\n\n\n");
        //_maxConcurrentRequestSemapore = new Semaphore(initialCount: maxConcurrentRequests, maximumCount: maxConcurrentRequests);
        _requestTokenChannel = System.Threading.Channels.Channel.CreateBounded<bool>(maxConcurrentRequests);
        _maxConcurrentRequests = maxConcurrentRequests;
    }
    internal async Task Initialize()
    {
        for (var i = 0; i < _maxConcurrentRequests; i++)
        {
            await _requestTokenChannel.Writer.WriteAsync(true);
        }
    }

    public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value, uint? ttlSeconds = null)
    {
        Console.WriteLine("WAITING FOR SEMAPHORE");
        //_maxConcurrentRequestSemapore.WaitOne();
        //try
        //{
            return await this.SendSetAsync(cacheName, value: value.ToByteString(), key: key.ToByteString(), ttlSeconds: ttlSeconds);
        //} finally
        //{
            //Console.WriteLine("DELAYING BEFORE RELEASING SEMAPHORE");
            //await Task.Delay(100);
            //Console.WriteLine("RELEASING SEMAPHORE");
            //_maxConcurrentRequestSemapore.Release();
        //}
    }

    public async Task<CacheGetResponse> GetAsync(string cacheName, byte[] key)
    {
        //_maxConcurrentRequestSemapore.WaitOne();
        //try
        //{
            return await this.SendGetAsync(cacheName, key.ToByteString());
        //} finally
        //{
            //_maxConcurrentRequestSemapore.Release();
        //}
    }

    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, byte[] key)
    {
        return await this.SendDeleteAsync(cacheName, key.ToByteString());
    }

    // CHRIS
    // CHRIS THESE ARE THE ONES THAT MATTER
    // CHRIS

    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, uint? ttlSeconds = null)
    {
        //Console.WriteLine("WAITING FOR SEMAPHORE");
        //_maxConcurrentRequestSemapore.WaitOne();
        var token = await _requestTokenChannel.Reader.ReadAsync();
        try
        {
            return await this.SendSetAsync(cacheName, key: key.ToByteString(), value: value.ToByteString(), ttlSeconds: ttlSeconds);
        }
        finally
        {
            //Console.WriteLine("DELAYING BEFORE RELEASING SEMAPHORE");
            //await Task.Delay(100);
            //Console.WriteLine("RELEASING SEMAPHORE");
            //_maxConcurrentRequestSemapore.Release();
            await _requestTokenChannel.Writer.WriteAsync(token);
        }
    }

    public async Task<CacheGetResponse> GetAsync(string cacheName, string key)
    {
        //Console.WriteLine("WAITING FOR SEMAPHORE");
        //_maxConcurrentRequestSemapore.WaitOne();
        var token = await _requestTokenChannel.Reader.ReadAsync();
        try
        {
            return await this.SendGetAsync(cacheName, key.ToByteString());
        }
        finally
        {
            //Console.WriteLine("DELAYING BEFORE RELEASING SEMAPHORE");
            //await Task.Delay(100);
            //Console.WriteLine("RELEASING SEMAPHORE");
            //_maxConcurrentRequestSemapore.Release();
            await _requestTokenChannel.Writer.WriteAsync(token);
        }
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
        return new CacheGetBatchResponse(continuation.Result);
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
        return new CacheSetBatchResponse();
    }

    private async Task<CacheSetResponse> SendSetAsync(string cacheName, ByteString key, ByteString value, uint? ttlSeconds = null)
    {
        _SetRequest request = new _SetRequest() { CacheBody = value, CacheKey = key, TtlMilliseconds = TtlSecondsToMilliseconds(ttlSeconds) };
        try
        {
            await this.grpcManager.Client.SetAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheSetResponse();
    }

    private async Task<CacheGetResponse> SendGetAsync(string cacheName, ByteString key)
    {
        _GetRequest request = new _GetRequest() { CacheKey = key };
        _GetResponse response;
        try
        {
            response = await this.grpcManager.Client.GetAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheGetResponse(response);
    }

    private async Task<CacheDeleteResponse> SendDeleteAsync(string cacheName, ByteString key)
    {
        _DeleteRequest request = new _DeleteRequest() { CacheKey = key };
        try
        {
            await this.grpcManager.Client.DeleteAsync(request, MetadataWithCache(cacheName), deadline: CalculateDeadline());
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
        return new CacheDeleteResponse();
    }
}
