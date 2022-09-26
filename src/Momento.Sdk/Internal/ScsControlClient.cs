using System;
using Microsoft.Extensions.Logging;
using Momento.Protos.ControlClient;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Responses;

namespace Momento.Sdk.Internal;

internal sealed class ScsControlClient : IDisposable
{
    private readonly ControlGrpcManager grpcManager;
    private readonly string authToken;
    private const uint DEADLINE_SECONDS = 60;
    private readonly ILogger _logger;

    public ScsControlClient(string authToken, string endpoint, ILoggerFactory loggerFactory)
    {
        this.grpcManager = new ControlGrpcManager(authToken, endpoint, loggerFactory);
        this.authToken = authToken;
        this._logger = loggerFactory.CreateLogger<ScsControlClient>();
    }

    public CreateCacheResponse CreateCache(string cacheName)
    {
        try
        {
            CheckValidCacheName(cacheName);
            _CreateCacheRequest request = new _CreateCacheRequest() { CacheName = cacheName };
            this.grpcManager.Client.CreateCache(request, deadline: CalculateDeadline());
            return new CreateCacheResponse.Success();
        }
        catch (Exception e)
        {
            return new CreateCacheResponse.Error(CacheExceptionMapper.Convert(e));
        }
    }

    public DeleteCacheResponse DeleteCache(string cacheName)
    {
        try
        {
            CheckValidCacheName(cacheName);
            _DeleteCacheRequest request = new _DeleteCacheRequest() { CacheName = cacheName };
            this.grpcManager.Client.DeleteCache(request, deadline: CalculateDeadline());
            return new DeleteCacheResponse.Success();
        }
        catch (Exception e)
        {
            return new DeleteCacheResponse.Error(CacheExceptionMapper.Convert(e));
        }
    }

    public ListCachesResponse ListCaches(string? nextPageToken = null)
    {
        _ListCachesRequest request = new _ListCachesRequest() { NextToken = nextPageToken == null ? "" : nextPageToken };
        try
        {
            _ListCachesResponse result = this.grpcManager.Client.ListCaches(request, deadline: CalculateDeadline());
            return new ListCachesResponse.Success(result);
        }
        catch (Exception e)
        {
            return new ListCachesResponse.Error(CacheExceptionMapper.Convert(e));
        }
    }

    private bool CheckValidCacheName(string cacheName)
    {
        if (string.IsNullOrWhiteSpace(cacheName))
        {
            throw new InvalidArgumentException("Cache name must be nonempty");
        }
        return true;
    }

    private DateTime CalculateDeadline()
    {
        return DateTime.UtcNow.AddSeconds(DEADLINE_SECONDS);
    }

    public void Dispose()
    {
        this.grpcManager.Dispose();
    }
}
