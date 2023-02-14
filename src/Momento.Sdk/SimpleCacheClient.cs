using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Auth;
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
    private readonly List<ScsDataClient> dataClients;
    private int nextDataClientIndex = 0;

    /// <inheritdoc cref="Momento.Sdk.Config.IConfiguration" />
    protected readonly IConfiguration config;
    /// <inheritdoc cref="Microsoft.Extensions.Logging.ILogger" />
    protected readonly ILogger _logger;


    /// <summary>
    /// Client to perform operations against the Simple Cache Service.
    /// </summary>
    /// <param name="config">Configuration to use for the transport, retries, middlewares. See <see cref="Configurations"/> for out-of-the-box configuration choices, eg <see cref="Configurations.Laptop.Latest"/></param>
    /// <param name="authProvider">Momento auth provider.</param>
    /// <param name="defaultTtl">Default time to live for the item in cache.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="defaultTtl"/> is zero or negative.</exception>
    public SimpleCacheClient(IConfiguration config, ICredentialProvider authProvider, TimeSpan defaultTtl)
    {
        this.config = config;
        var _loggerFactory = config.LoggerFactory;
        this._logger = _loggerFactory.CreateLogger<SimpleCacheClient>();
        Utils.ArgumentStrictlyPositive(defaultTtl, "defaultTtl");
        this.controlClient = new(_loggerFactory, authProvider.AuthToken, authProvider.ControlEndpoint);
        this.dataClients = new List<ScsDataClient>();
        for (var i = 1; i <= config.TransportStrategy.GrpcConfig.MinNumGrpcChannels; i++)
        {
            this.dataClients.Add(new(config, authProvider.AuthToken, authProvider.CacheEndpoint, defaultTtl));
        }
    }

    /// <inheritdoc />
    public async Task<CreateCacheResponse> CreateCacheAsync(string cacheName)
    {
        return await this.controlClient.CreateCacheAsync(cacheName);
    }

    /// <inheritdoc />
    public async Task<DeleteCacheResponse> DeleteCacheAsync(string cacheName)
    {
        return await this.controlClient.DeleteCacheAsync(cacheName);
    }

    /// <inheritdoc />
    public async Task<FlushCacheResponse> FlushCacheAsync(string cacheName)
    {
        return await this.controlClient.FlushCacheAsync(cacheName);
    }

    /// <inheritdoc />
    public async Task<ListCachesResponse> ListCachesAsync(string? nextPageToken = null)
    {
        return await this.controlClient.ListCachesAsync(nextPageToken);
    }

    /// <inheritdoc />
    public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value, TimeSpan? ttl = null)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(key, nameof(key));
            Utils.ArgumentNotNull(value, nameof(value));
            Utils.ArgumentStrictlyPositive(ttl, nameof(ttl));
        }
        catch (ArgumentNullException e)
        {
            return new CacheSetResponse.Error(new InvalidArgumentException(e.Message));
        }
        catch (ArgumentOutOfRangeException e)
        {
            return new CacheSetResponse.Error(new InvalidArgumentException(e.Message));
        }
        return await this.NextDataClient().SetAsync(cacheName, key, value, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheGetResponse> GetAsync(string cacheName, byte[] key)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(key, nameof(key));
        }
        catch (ArgumentNullException e)
        {
            return new CacheGetResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.NextDataClient().GetAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, byte[] key)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(key, nameof(key));
        }
        catch (ArgumentNullException e)
        {
            return new CacheDeleteResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.NextDataClient().DeleteAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, TimeSpan? ttl = null)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(key, nameof(key));
            Utils.ArgumentNotNull(value, nameof(value));
            Utils.ArgumentStrictlyPositive(ttl, nameof(ttl));
        }
        catch (ArgumentNullException e)
        {
            return new CacheSetResponse.Error(new InvalidArgumentException(e.Message));
        }
        catch (ArgumentOutOfRangeException e)
        {
            return new CacheSetResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.NextDataClient().SetAsync(cacheName, key, value, ttl);
    }

    /// <inheritdoc />
    public async Task<CacheGetResponse> GetAsync(string cacheName, string key)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(key, nameof(key));
        }
        catch (ArgumentNullException e)
        {
            return new CacheGetResponse.Error(new InvalidArgumentException(e.Message));
        }
        return await this.NextDataClient().GetAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, string key)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(key, nameof(key));
        }
        catch (ArgumentNullException e)
        {
            return new CacheDeleteResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.NextDataClient().DeleteAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, byte[] value, TimeSpan? ttl = null)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(key, nameof(key));
            Utils.ArgumentNotNull(value, nameof(value));
            Utils.ArgumentStrictlyPositive(ttl, nameof(ttl));
        }
        catch (ArgumentNullException e)
        {
            return new CacheSetResponse.Error(new InvalidArgumentException(e.Message));
        }
        catch (ArgumentOutOfRangeException e)
        {
            return new CacheSetResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.NextDataClient().SetAsync(cacheName, key, value, ttl);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.controlClient.Dispose();
        foreach (var dataClient in this.dataClients)
        {
            dataClient.Dispose();
        }
        GC.SuppressFinalize(this);
    }


    private ScsDataClient NextDataClient()
    {
        return this.dataClients[Interlocked.Increment(ref this.nextDataClientIndex) % this.dataClients.Count];
    }
}
