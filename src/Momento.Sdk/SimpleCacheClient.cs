﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;
using Momento.Sdk.Responses;

namespace Momento.Sdk;

/// <summary>
/// Client to perform control and data operations against the Simple Cache Service.
/// 
/// See <see href="https://github.com/momentohq/client-sdk-examples/tree/main/dotnet/MomentoExamples">the examples repo</see> for complete workflows.
/// </summary>
public class SimpleCacheClient : ISimpleCacheClient
{
    private readonly ScsControlClient controlClient;
    private readonly List<ScsDataClient> dataClients;
    private int NextDataClientIndex;
    private readonly Claims _claims;

    public static async Task<SimpleCacheClient> ConstructSimpleCacheClientAsync(string authToken, uint defaultTtlSeconds, uint numGrpcChannels, int maxConcurrentRequests, uint? dataClientOperationTimeoutMilliseconds = null)
    {
        var client = new SimpleCacheClient(authToken, defaultTtlSeconds, numGrpcChannels, maxConcurrentRequests, dataClientOperationTimeoutMilliseconds);
        await client.Initialize(authToken, numGrpcChannels, maxConcurrentRequests, defaultTtlSeconds, dataClientOperationTimeoutMilliseconds);
        return client;
    }



    /// <summary>
    /// Client to perform operations against the Simple Cache Service.
    /// </summary>
    /// <param name="authToken">Momento JWT.</param>
    /// <param name="defaultTtlSeconds">Default time to live for the item in cache.</param>
    /// <param name="dataClientOperationTimeoutMilliseconds">Deadline (timeout) for communicating to the server. Defaults to 5 seconds.</param>
    private SimpleCacheClient(string authToken, uint defaultTtlSeconds, uint numGrpcChannels, int maxConcurrentRequests, uint? dataClientOperationTimeoutMilliseconds = null)
    {
        Console.WriteLine("\n\n\n\nWELCOME TO THE SIMPLE CACHE CLIENT!\n\n\n\n");
        ValidateRequestTimeout(dataClientOperationTimeoutMilliseconds);
        _claims = JwtUtils.DecodeJwt(authToken);

        this.controlClient = new(authToken, _claims.ControlEndpoint);
        this.dataClients = new List<ScsDataClient>();
        this.NextDataClientIndex = 0;
        //for (var i = 0; i < numGrpcChannels; i++)
        //{
        //    this.dataClients.Add(new ScsDataClient(authToken, claims.CacheEndpoint,
        //        maxConcurrentRequests: maxConcurrentRequests,
        //        defaultTtlSeconds, dataClientOperationTimeoutMilliseconds));
        //}
        //new(authToken, claims.CacheEndpoint, defaultTtlSeconds, dataClientOperationTimeoutMilliseconds);
    }

    private async Task Initialize(string authToken, uint numGrpcChannels, int maxConcurrentRequests, uint defaultTtlSeconds, uint? dataClientOperationTimeoutMilliseconds = null)
    {
        for (var i = 0; i < numGrpcChannels; i++)
        {
            this.dataClients.Add(await ScsDataClient.ConstructScsDataClient(authToken, _claims.CacheEndpoint,
                maxConcurrentRequests: maxConcurrentRequests,
                defaultTtlSeconds, dataClientOperationTimeoutMilliseconds));
        }
    }

    /// <inheritdoc />
    public CreateCacheResponse CreateCache(string cacheName)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        return this.controlClient.CreateCache(cacheName);
    }

    /// <inheritdoc />
    public DeleteCacheResponse DeleteCache(string cacheName)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        return this.controlClient.DeleteCache(cacheName);
    }

    /// <inheritdoc />
    public ListCachesResponse ListCaches(string? nextPageToken = null)
    {
        return this.controlClient.ListCaches(nextPageToken);
    }

    /// <inheritdoc />
    public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value, uint? ttlSeconds = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(key, nameof(key));
        Utils.ArgumentNotNull(value, nameof(value));

        return await this.GetNextDataClient().SetAsync(cacheName, key, value, ttlSeconds);
    }

    /// <inheritdoc />
    public async Task<CacheGetResponse> GetAsync(string cacheName, byte[] key)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(key, nameof(key));

        return await this.GetNextDataClient().GetAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, byte[] key)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(key, nameof(key));

        return await this.GetNextDataClient().DeleteAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, uint? ttlSeconds = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(key, nameof(key));
        Utils.ArgumentNotNull(value, nameof(value));

        return await this.GetNextDataClient().SetAsync(cacheName, key, value, ttlSeconds);
    }

    /// <inheritdoc />
    public async Task<CacheGetResponse> GetAsync(string cacheName, string key)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(key, nameof(key));

        return await this.GetNextDataClient().GetAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, string key)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(key, nameof(key));

        return await this.GetNextDataClient().DeleteAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, byte[] value, uint? ttlSeconds = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(key, nameof(key));
        Utils.ArgumentNotNull(value, nameof(value));

        return await this.GetNextDataClient().SetAsync(cacheName, key, value, ttlSeconds);
    }

    /// <inheritdoc />
    public async Task<CacheGetBatchResponse> GetBatchAsync(string cacheName, IEnumerable<byte[]> keys)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(keys, nameof(keys));
        Utils.ElementsNotNull(keys, nameof(keys));

        return await this.GetNextDataClient().GetBatchAsync(cacheName, keys);
    }

    /// <inheritdoc />
    public async Task<CacheGetBatchResponse> GetBatchAsync(string cacheName, IEnumerable<string> keys)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(keys, nameof(keys));
        Utils.ElementsNotNull(keys, nameof(keys));

        return await this.GetNextDataClient().GetBatchAsync(cacheName, keys);
    }

    /// <inheritdoc />
    public async Task<CacheSetBatchResponse> SetBatchAsync(string cacheName, IEnumerable<KeyValuePair<byte[], byte[]>> items, uint? ttlSeconds = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(items, nameof(items));
        Utils.KeysAndValuesNotNull(items, nameof(items));

        return await this.GetNextDataClient().SetBatchAsync(cacheName, items, ttlSeconds);
    }

    /// <inheritdoc />
    public async Task<CacheSetBatchResponse> SetBatchAsync(string cacheName, IEnumerable<KeyValuePair<string, string>> items, uint? ttlSeconds = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(items, nameof(items));
        Utils.KeysAndValuesNotNull(items, nameof(items));

        return await this.GetNextDataClient().SetBatchAsync(cacheName, items, ttlSeconds);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.controlClient.Dispose();
        foreach (ScsDataClient client in this.dataClients)
        {
            client.Dispose();
        }
        GC.SuppressFinalize(this);
    }

    private void ValidateRequestTimeout(uint? requestTimeoutMilliseconds = null)
    {
        if (requestTimeoutMilliseconds == null)
        {
            return;
        }
        if (requestTimeoutMilliseconds == 0)
        {
            throw new InvalidArgumentException("Request timeout must be greater than zero.");
        }
    }

    private ScsDataClient GetNextDataClient()
    {
        var nextCount = (uint) Interlocked.Increment(ref this.NextDataClientIndex);
        //Console.WriteLine($"NEXT COUNT IS: {nextCount}");
        int nextIndex = (int)(nextCount % this.dataClients.Count);
        //Console.WriteLine($"Using data client: {nextIndex}");
        return this.dataClients[nextIndex];
    }
}
