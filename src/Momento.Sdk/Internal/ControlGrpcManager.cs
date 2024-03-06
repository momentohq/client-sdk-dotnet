﻿#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using System.Net.Http;
#if USE_GRPC_WEB
using Grpc.Net.Client.Web;
#endif
using Microsoft.Extensions.Logging;
using Momento.Protos.ControlClient;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Internal.Middleware;
using static System.Reflection.Assembly;

namespace Momento.Sdk.Internal;


public interface IControlClient
{
    public Task<_CreateCacheResponse> CreateCacheAsync(_CreateCacheRequest request, CallOptions callOptions);
    public Task<_DeleteCacheResponse> DeleteCacheAsync(_DeleteCacheRequest request, CallOptions callOptions);
    public Task<_FlushCacheResponse> FlushCacheAsync(_FlushCacheRequest request, CallOptions callOptions);
    public Task<_ListCachesResponse> ListCachesAsync(_ListCachesRequest request, CallOptions callOptions);
}


// Ideally we would implement our middleware based on gRPC Interceptors.  Unfortunately,
// the their method signatures are not asynchronous. Thus, for any middleware that may
// require asynchronous actions (such as our MaxConcurrentRequestsMiddleware), we would
// end up blocking threads to wait for the completion of the async task, which would have
// a big negative impact on performance. Instead, in this commit, we implement a thin
// middleware layer of our own that uses asynchronous signatures throughout.  This has
// the nice side effect of making the user-facing API for writing Middlewares a bit less
// of a learning curve for anyone not super deep on gRPC internals.
internal class ControlClientWithMiddleware : IControlClient
{
    private readonly IList<IMiddleware> _middlewares;
    private readonly ScsControl.ScsControlClient _generatedClient;

    public ControlClientWithMiddleware(ScsControl.ScsControlClient generatedClient, IList<IMiddleware> middlewares)
    {
        _generatedClient = generatedClient;
        _middlewares = middlewares;
    }

    public async Task<_CreateCacheResponse> CreateCacheAsync(_CreateCacheRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.CreateCacheAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_DeleteCacheResponse> DeleteCacheAsync(_DeleteCacheRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.DeleteCacheAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_FlushCacheResponse> FlushCacheAsync(_FlushCacheRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.FlushCacheAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ListCachesResponse> ListCachesAsync(_ListCachesRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.ListCachesAsync(r, o));
        return await wrapped.ResponseAsync;
    }
}

internal sealed class ControlGrpcManager : GrpcManager
{
    public IControlClient Client { get; }

    public ControlGrpcManager(IConfiguration config, string authToken, string endpoint): base(config.TransportStrategy.GrpcConfig, config.LoggerFactory, authToken, endpoint, "ControlGrpcManager")
    {
        var middlewares = new List<IMiddleware> 
        {
            new HeaderMiddleware(config.LoggerFactory, this.headers)
        };

        var client = new ScsControl.ScsControlClient(this.invoker);
        Client = new ControlClientWithMiddleware(client, middlewares);
    }
}
