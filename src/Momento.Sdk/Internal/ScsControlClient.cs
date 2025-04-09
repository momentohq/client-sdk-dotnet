using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Protos.ControlClient;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Transport;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Responses;
using System;
using System.Threading.Tasks;

namespace Momento.Sdk.Internal;

internal sealed class ScsControlClient : IDisposable
{
    private readonly ControlGrpcManager grpcManager;
    private readonly string authToken;
    private readonly TimeSpan deadline = TimeSpan.FromSeconds(60);

    private readonly ILogger _logger;
    private readonly CacheExceptionMapper _exceptionMapper;

    public ScsControlClient(IConfiguration config, ICredentialProvider authProvider)
    {
        // Override the sockets http handler options to disable keepalive
        var overrideKeepalive = SocketsHttpHandlerOptions.Of(
            pooledConnectionIdleTimeout: config.TransportStrategy.GrpcConfig.SocketsHttpHandlerOptions.PooledConnectionIdleTimeout,
            enableMultipleHttp2Connections: config.TransportStrategy.GrpcConfig.SocketsHttpHandlerOptions.EnableMultipleHttp2Connections,
            keepAlivePingTimeout: System.Threading.Timeout.InfiniteTimeSpan,
            keepAlivePingDelay: System.Threading.Timeout.InfiniteTimeSpan,
            keepAlivePermitWithoutCalls: false
        );
        var controlConfig = config.WithTransportStrategy(config.TransportStrategy.WithSocketsHttpHandlerOptions(overrideKeepalive));

        this.grpcManager = new ControlGrpcManager(controlConfig, authProvider);
        this.authToken = authProvider.AuthToken;
        this._logger = config.LoggerFactory.CreateLogger<ScsControlClient>();
        this._exceptionMapper = new CacheExceptionMapper(config.LoggerFactory);
    }

    public async Task<CreateCacheResponse> CreateCacheAsync(string cacheName)
    {
        try
        {
            CheckValidCacheName(cacheName);
            _CreateCacheRequest request = new _CreateCacheRequest() { CacheName = cacheName };
            await this.grpcManager.Client.CreateCacheAsync(request, new CallOptions(deadline: Utils.CalculateDeadline(deadline)));
            return new CreateCacheResponse.Success();
        }
        catch (Exception e)
        {
            if (e is RpcException ex && ex.StatusCode == StatusCode.AlreadyExists)
            {
                return new CreateCacheResponse.CacheAlreadyExists();
            }
            return new CreateCacheResponse.Error(_exceptionMapper.Convert(e));
        }
    }

    public async Task<DeleteCacheResponse> DeleteCacheAsync(string cacheName)
    {
        try
        {
            CheckValidCacheName(cacheName);
            _DeleteCacheRequest request = new _DeleteCacheRequest() { CacheName = cacheName };
            await this.grpcManager.Client.DeleteCacheAsync(request, new CallOptions(deadline: Utils.CalculateDeadline(deadline)));
            return new DeleteCacheResponse.Success();
        }
        catch (Exception e)
        {
            return new DeleteCacheResponse.Error(_exceptionMapper.Convert(e));
        }
    }

    public async Task<FlushCacheResponse> FlushCacheAsync(string cacheName)
    {
        try
        {
            CheckValidCacheName(cacheName);
            _FlushCacheRequest request = new() { CacheName = cacheName };
            await this.grpcManager.Client.FlushCacheAsync(request, new CallOptions(deadline: Utils.CalculateDeadline(deadline)));
            return new FlushCacheResponse.Success();
        }
        catch (Exception e)
        {
            return new FlushCacheResponse.Error(_exceptionMapper.Convert(e));
        }
    }

    public async Task<ListCachesResponse> ListCachesAsync(string? nextPageToken = null)
    {
        _ListCachesRequest request = new _ListCachesRequest() { NextToken = nextPageToken == null ? "" : nextPageToken };
        try
        {
            _ListCachesResponse result = await this.grpcManager.Client.ListCachesAsync(request, new CallOptions(deadline: Utils.CalculateDeadline(deadline)));
            return new ListCachesResponse.Success(result);
        }
        catch (Exception e)
        {
            return new ListCachesResponse.Error(_exceptionMapper.Convert(e));
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

    public void Dispose()
    {
        this.grpcManager.Dispose();
    }
}
