using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Momento.Sdk.Config;
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
    private readonly ScsDataClient dataClient;
    protected readonly IConfiguration config;

    /// <summary>
    /// Client to perform operations against the Simple Cache Service.
    /// </summary>
    /// <param name="authToken">Momento JWT.</param>
    /// <param name="defaultTtlSeconds">Default time to live for the item in cache.</param>
    /// <param name="dataClientOperationTimeoutMilliseconds">Deadline (timeout) for communicating to the server. Defaults to 5 seconds.</param>
    /// <param name="config">Configuration to use for the transport, retries, middlewares. Defaults to <see cref="PrebuiltConfigurations.DevConfig"/></param>
    public SimpleCacheClient(string authToken, uint defaultTtlSeconds, uint? dataClientOperationTimeoutMilliseconds = null, IConfiguration? config = null)
    {
        ValidateRequestTimeout(dataClientOperationTimeoutMilliseconds);
        Claims claims = JwtUtils.DecodeJwt(authToken);
        // Default to dev config
        this.config = (config ?? PrebuiltConfigurations.DevConfig);

        this.controlClient = new(authToken, claims.ControlEndpoint);
        this.dataClient = new(authToken, claims.CacheEndpoint, defaultTtlSeconds, dataClientOperationTimeoutMilliseconds);
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

        return await this.dataClient.SetAsync(cacheName, key, value, ttlSeconds);
    }

    /// <inheritdoc />
    public async Task<CacheGetResponse> GetAsync(string cacheName, byte[] key)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(key, nameof(key));

        return await this.dataClient.GetAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, byte[] key)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(key, nameof(key));

        return await this.dataClient.DeleteAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, uint? ttlSeconds = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(key, nameof(key));
        Utils.ArgumentNotNull(value, nameof(value));

        return await this.dataClient.SetAsync(cacheName, key, value, ttlSeconds);
    }

    /// <inheritdoc />
    public async Task<CacheGetResponse> GetAsync(string cacheName, string key)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(key, nameof(key));

        return await this.dataClient.GetAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, string key)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(key, nameof(key));

        return await this.dataClient.DeleteAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, byte[] value, uint? ttlSeconds = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(key, nameof(key));
        Utils.ArgumentNotNull(value, nameof(value));

        return await this.dataClient.SetAsync(cacheName, key, value, ttlSeconds);
    }

    /// <inheritdoc />
    public async Task<CacheGetBatchResponse> GetBatchAsync(string cacheName, IEnumerable<byte[]> keys)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(keys, nameof(keys));
        Utils.ElementsNotNull(keys, nameof(keys));

        return await this.dataClient.GetBatchAsync(cacheName, keys);
    }

    /// <inheritdoc />
    public async Task<CacheGetBatchResponse> GetBatchAsync(string cacheName, IEnumerable<string> keys)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(keys, nameof(keys));
        Utils.ElementsNotNull(keys, nameof(keys));

        return await this.dataClient.GetBatchAsync(cacheName, keys);
    }

    /// <inheritdoc />
    public async Task<CacheSetBatchResponse> SetBatchAsync(string cacheName, IEnumerable<KeyValuePair<byte[], byte[]>> items, uint? ttlSeconds = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(items, nameof(items));
        Utils.KeysAndValuesNotNull(items, nameof(items));

        return await this.dataClient.SetBatchAsync(cacheName, items, ttlSeconds);
    }

    /// <inheritdoc />
    public async Task<CacheSetBatchResponse> SetBatchAsync(string cacheName, IEnumerable<KeyValuePair<string, string>> items, uint? ttlSeconds = null)
    {
        Utils.ArgumentNotNull(cacheName, nameof(cacheName));
        Utils.ArgumentNotNull(items, nameof(items));
        Utils.KeysAndValuesNotNull(items, nameof(items));

        return await this.dataClient.SetBatchAsync(cacheName, items, ttlSeconds);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.controlClient.Dispose();
        this.dataClient.Dispose();
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
}
