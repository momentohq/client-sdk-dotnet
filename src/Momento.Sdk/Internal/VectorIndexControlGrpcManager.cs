#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
#if USE_GRPC_WEB
using System.Net.Http;
using Grpc.Net.Client.Web;
#endif
using Microsoft.Extensions.Logging;
using Momento.Protos.CachePing;
using Momento.Protos.ControlClient;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Retry;
using Momento.Sdk.Internal.Middleware;
using static System.Reflection.Assembly;

namespace Momento.Sdk.Internal;

public interface IVectorIndexControlClient
{
    public Task<_CreateIndexResponse> CreateIndexAsync(_CreateIndexRequest request, CallOptions callOptions);
    public Task<_ListIndexesResponse> ListIndexesAsync(_ListIndexesRequest request, CallOptions callOptions);
    public Task<_DeleteIndexResponse> DeleteIndexAsync(_DeleteIndexRequest request, CallOptions callOptions);
}


// Ideally we would implement our middleware based on gRPC Interceptors.  Unfortunately,
// the their method signatures are not asynchronous. Thus, for any middleware that may
// require asynchronous actions (such as our MaxConcurrentRequestsMiddleware), we would
// end up blocking threads to wait for the completion of the async task, which would have
// a big negative impact on performance. Instead, in this commit, we implement a thin
// middleware layer of our own that uses asynchronous signatures throughout.  This has
// the nice side effect of making the user-facing API for writing Middlewares a bit less
// of a learning curve for anyone not super deep on gRPC internals.
public class VectorIndexControlClientWithMiddleware : IVectorIndexControlClient
{
    private readonly IList<IMiddleware> _middlewares;
    private readonly ScsControl.ScsControlClient _generatedClient;

    public VectorIndexControlClientWithMiddleware(ScsControl.ScsControlClient generatedClient, IList<IMiddleware> middlewares)
    {
        _generatedClient = generatedClient;
        _middlewares = middlewares;
    }

    public async Task<_CreateIndexResponse> CreateIndexAsync(_CreateIndexRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.CreateIndexAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ListIndexesResponse> ListIndexesAsync(_ListIndexesRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.ListIndexesAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_DeleteIndexResponse> DeleteIndexAsync(_DeleteIndexRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.DeleteIndexAsync(r, o));
        return await wrapped.ResponseAsync;
    }
}

public class VectorIndexControlGrpcManager : GrpcManager
{
    public readonly IVectorIndexControlClient Client;

    internal VectorIndexControlGrpcManager(IVectorIndexConfiguration config, string authToken, string endpoint): base(config.TransportStrategy.GrpcConfig, config.LoggerFactory, authToken, endpoint, "VectorIndexControlGrpcManager")
    {
        var middlewares = new List<IMiddleware> {
            new HeaderMiddleware(config.LoggerFactory, this.headers)
        };

        var client = new ScsControl.ScsControlClient(this.invoker);
        Client = new VectorIndexControlClientWithMiddleware(client, middlewares);
    }
}
