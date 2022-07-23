using System;
using MomentoSdk.Responses;
using MomentoSdk.Exceptions;
using ControlClient;
using System.Threading.Tasks;

namespace MomentoSdk;

internal sealed class ScsControlClient : IDisposable
{
    private readonly ControlGrpcManager grpcManager;
    private readonly string authToken;
    private const uint DEADLINE_SECONDS = 60;

    public ScsControlClient(string authToken, string host)
    {
        this.grpcManager = new ControlGrpcManager(authToken, host);
        this.authToken = authToken;
    }

    public CreateCacheResponse CreateCache(string cacheName)
    {
        CheckValidCacheName(cacheName);
        try
        {
            _CreateCacheRequest request = new _CreateCacheRequest() { CacheName = cacheName };
            this.grpcManager.Client.CreateCache(request, deadline: CalculateDeadline());
            return new CreateCacheResponse();
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public DeleteCacheResponse DeleteCache(string cacheName)
    {
        _DeleteCacheRequest request = new _DeleteCacheRequest() { CacheName = cacheName };
        try
        {
            this.grpcManager.Client.DeleteCache(request, deadline: CalculateDeadline());
            return new DeleteCacheResponse();
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
        }
    }

    public ListCachesResponse ListCaches(string? nextPageToken = null)
    {
        _ListCachesRequest request = new _ListCachesRequest() { NextToken = nextPageToken == null ? "" : nextPageToken };
        try
        {
            ControlClient._ListCachesResponse result = this.grpcManager.Client.ListCaches(request, deadline: CalculateDeadline());
            return new ListCachesResponse(result);
        }
        catch (Exception e)
        {
            throw CacheExceptionMapper.Convert(e);
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
