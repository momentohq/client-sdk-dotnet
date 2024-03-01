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
using Momento.Sdk.Config;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Internal.Middleware;
using Vectorindex;
using static System.Reflection.Assembly;

namespace Momento.Sdk.Internal;

public interface IVectorIndexDataClient
{
    public Task<_CountItemsResponse> CountItemsAsync(_CountItemsRequest request, CallOptions callOptions);
    public Task<_UpsertItemBatchResponse> UpsertItemBatchAsync(_UpsertItemBatchRequest request, CallOptions callOptions);
    public Task<_SearchResponse> SearchAsync(_SearchRequest request, CallOptions callOptions);
    public Task<_SearchAndFetchVectorsResponse> SearchAndFetchVectorsAsync(_SearchAndFetchVectorsRequest request, CallOptions callOptions);
    public Task<_GetItemBatchResponse> GetItemBatchAsync(_GetItemBatchRequest request, CallOptions callOptions);
    public Task<_GetItemMetadataBatchResponse> GetItemMetadataBatchAsync(_GetItemMetadataBatchRequest request, CallOptions callOptions);
    public Task<_DeleteItemBatchResponse> DeleteItemBatchAsync(_DeleteItemBatchRequest request, CallOptions callOptions);
}


// Ideally we would implement our middleware based on gRPC Interceptors.  Unfortunately,
// the their method signatures are not asynchronous. Thus, for any middleware that may
// require asynchronous actions (such as our MaxConcurrentRequestsMiddleware), we would
// end up blocking threads to wait for the completion of the async task, which would have
// a big negative impact on performance. Instead, in this commit, we implement a thin
// middleware layer of our own that uses asynchronous signatures throughout.  This has
// the nice side effect of making the user-facing API for writing Middlewares a bit less
// of a learning curve for anyone not super deep on gRPC internals.
public class VectorIndexDataClientWithMiddleware : IVectorIndexDataClient
{
    private readonly IList<IMiddleware> _middlewares;
    private readonly VectorIndex.VectorIndexClient _generatedClient;

    public VectorIndexDataClientWithMiddleware(VectorIndex.VectorIndexClient generatedClient, IList<IMiddleware> middlewares)
    {
        _generatedClient = generatedClient;
        _middlewares = middlewares;
    }

    public async Task<_CountItemsResponse> CountItemsAsync(_CountItemsRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.CountItemsAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_UpsertItemBatchResponse> UpsertItemBatchAsync(_UpsertItemBatchRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.UpsertItemBatchAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_SearchResponse> SearchAsync(_SearchRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.SearchAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_SearchAndFetchVectorsResponse> SearchAndFetchVectorsAsync(_SearchAndFetchVectorsRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.SearchAndFetchVectorsAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_GetItemBatchResponse> GetItemBatchAsync(_GetItemBatchRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.GetItemBatchAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_GetItemMetadataBatchResponse> GetItemMetadataBatchAsync(_GetItemMetadataBatchRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.GetItemMetadataBatchAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_DeleteItemBatchResponse> DeleteItemBatchAsync(_DeleteItemBatchRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.DeleteItemBatchAsync(r, o));
        return await wrapped.ResponseAsync;
    }
}

public class VectorIndexDataGrpcManager : GrpcManager
{
    public readonly IVectorIndexDataClient Client;

    internal VectorIndexDataGrpcManager(IVectorIndexConfiguration config, string authToken, string endpoint): base(config.TransportStrategy.GrpcConfig, config.LoggerFactory, authToken, endpoint, "VectorIndexDataGrpcManager")
    {
        var middlewares = new List<IMiddleware> {
            new HeaderMiddleware(config.LoggerFactory, this.headers)
        };

        var client = new VectorIndex.VectorIndexClient(this.invoker);
        Client = new VectorIndexDataClientWithMiddleware(client, middlewares);
    }
}
