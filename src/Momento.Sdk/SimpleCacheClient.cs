using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
    private readonly ScsDataClient dataClient;
    protected readonly IConfiguration config;
    protected readonly ILogger _logger;


    /// <summary>
    /// Client to perform operations against the Simple Cache Service.
    /// </summary>
    /// <param name="config">Configuration to use for the transport, retries, middlewares. See <see cref="Configurations"/> for out-of-the-box configuration choices, eg <see cref="Configurations.Laptop.Latest"/></param>
    /// <param name="authProvider">Momento auth provider.</param>
    /// <param name="defaultTtlSeconds">Default time to live for the item in cache.</param>
    /// <param name="loggerFactory">Logger factory to create loggers for contained instances.</param>
    public SimpleCacheClient(IConfiguration config, ICredentialProvider authProvider, uint defaultTtlSeconds, ILoggerFactory? loggerFactory = null)
    {
        this.config = config;
        var _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        this._logger = _loggerFactory.CreateLogger<SimpleCacheClient>();
        ValidateRequestTimeout(config.TransportStrategy.GrpcConfig.DeadlineMilliseconds);
        this.controlClient = new(authProvider.AuthToken, authProvider.ControlEndpoint, _loggerFactory);
        this.dataClient = new(config, authProvider.AuthToken, authProvider.CacheEndpoint, defaultTtlSeconds, _loggerFactory);
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
    public async Task<ListCachesResponse> ListCachesAsync(string? nextPageToken = null)
    {
        return await this.controlClient.ListCachesAsync(nextPageToken);
    }

    /// <inheritdoc />
    public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value, uint? ttlSeconds = null)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(key, nameof(key));
            Utils.ArgumentNotNull(value, nameof(value));
        }
        catch (ArgumentNullException e)
        {
            return new CacheSetResponse.Error(new InvalidArgumentException(e.Message));
        }
        return await this.dataClient.SetAsync(cacheName, key, value, ttlSeconds);
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

        return await this.dataClient.GetAsync(cacheName, key);
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

        return await this.dataClient.DeleteAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, uint? ttlSeconds = null)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(key, nameof(key));
            Utils.ArgumentNotNull(value, nameof(value));
        }
        catch (ArgumentNullException e)
        {
            return new CacheSetResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.SetAsync(cacheName, key, value, ttlSeconds);
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
        return await this.dataClient.GetAsync(cacheName, key);
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

        return await this.dataClient.DeleteAsync(cacheName, key);
    }

    /// <inheritdoc />
    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, byte[] value, uint? ttlSeconds = null)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(key, nameof(key));
            Utils.ArgumentNotNull(value, nameof(value));
        }
        catch (ArgumentNullException e)
        {
            return new CacheSetResponse.Error(new InvalidArgumentException(e.Message));
        }

        return await this.dataClient.SetAsync(cacheName, key, value, ttlSeconds);
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
