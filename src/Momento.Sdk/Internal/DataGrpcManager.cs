#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Protos.CacheClient;
using Momento.Protos.CachePing;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Retry;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal.Middleware;

namespace Momento.Sdk.Internal;

public interface IDataClient
{
    public Task<_KeysExistResponse> KeysExistAsync(_KeysExistRequest request, CallOptions callOptions);
    public Task<_UpdateTtlResponse> UpdateTtlAsync(_UpdateTtlRequest request, CallOptions callOptions);
    public Task<_ItemGetTtlResponse> ItemGetTtlAsync(_ItemGetTtlRequest request, CallOptions callOptions);
    public Task<_GetResponse> GetAsync(_GetRequest request, CallOptions callOptions);
    public Task<_SetResponse> SetAsync(_SetRequest request, CallOptions callOptions);
    public Task<_DeleteResponse> DeleteAsync(_DeleteRequest request, CallOptions callOptions);
    public Task<_SetIfNotExistsResponse> SetIfNotExistsAsync(_SetIfNotExistsRequest request, CallOptions callOptions);
    public Task<_IncrementResponse> IncrementAsync(_IncrementRequest request, CallOptions callOptions);
    public Task<_DictionarySetResponse> DictionarySetAsync(_DictionarySetRequest request, CallOptions callOptions);
    public Task<_DictionaryIncrementResponse> DictionaryIncrementAsync(_DictionaryIncrementRequest request, CallOptions callOptions);
    public Task<_DictionaryGetResponse> DictionaryGetAsync(_DictionaryGetRequest request, CallOptions callOptions);
    public Task<_DictionaryFetchResponse> DictionaryFetchAsync(_DictionaryFetchRequest request, CallOptions callOptions);
    public Task<_DictionaryDeleteResponse> DictionaryDeleteAsync(_DictionaryDeleteRequest request, CallOptions callOptions);
    public Task<_DictionaryLengthResponse> DictionaryLengthAsync(_DictionaryLengthRequest request, CallOptions callOptions);

    public Task<_SetUnionResponse> SetUnionAsync(_SetUnionRequest request, CallOptions callOptions);
    public Task<_SetDifferenceResponse> SetDifferenceAsync(_SetDifferenceRequest request, CallOptions callOptions);
    public Task<_SetFetchResponse> SetFetchAsync(_SetFetchRequest request, CallOptions callOptions);
    public Task<_SetSampleResponse> SetSampleAsync(_SetSampleRequest request, CallOptions callOptions);
    public Task<_SetLengthResponse> SetLengthAsync(_SetLengthRequest request, CallOptions callOptions);
    public Task<_ListPushFrontResponse> ListPushFrontAsync(_ListPushFrontRequest request, CallOptions callOptions);
    public Task<_ListPushBackResponse> ListPushBackAsync(_ListPushBackRequest request, CallOptions callOptions);
    public Task<_ListPopFrontResponse> ListPopFrontAsync(_ListPopFrontRequest request, CallOptions callOptions);
    public Task<_ListPopBackResponse> ListPopBackAsync(_ListPopBackRequest request, CallOptions callOptions);
    public Task<_ListFetchResponse> ListFetchAsync(_ListFetchRequest request, CallOptions callOptions);
    public Task<_ListRetainResponse> ListRetainAsync(_ListRetainRequest request, CallOptions callOptions);
    public Task<_ListRemoveResponse> ListRemoveAsync(_ListRemoveRequest request, CallOptions callOptions);
    public Task<_ListLengthResponse> ListLengthAsync(_ListLengthRequest request, CallOptions callOptions);
    public Task<_ListConcatenateFrontResponse> ListConcatenateFrontAsync(_ListConcatenateFrontRequest request, CallOptions callOptions);
    public Task<_ListConcatenateBackResponse> ListConcatenateBackAsync(_ListConcatenateBackRequest request, CallOptions callOptions);
}


// Ideally we would implement our middleware based on gRPC Interceptors.  Unfortunately,
// the their method signatures are not asynchronous. Thus, for any middleware that may
// require asynchronous actions (such as our MaxConcurrentRequestsMiddleware), we would
// end up blocking threads to wait for the completion of the async task, which would have
// a big negative impact on performance. Instead, in this commit, we implement a thin
// middleware layer of our own that uses asynchronous signatures throughout.  This has
// the nice side effect of making the user-facing API for writing Middlewares a bit less
// of a learning curve for anyone not super deep on gRPC internals.
public class DataClientWithMiddleware : IDataClient
{
    private readonly IList<IMiddleware> _middlewares;
    private readonly Scs.ScsClient _generatedClient;

    public DataClientWithMiddleware(Scs.ScsClient generatedClient, IList<IMiddleware> middlewares)
    {
        _generatedClient = generatedClient;
        _middlewares = middlewares;
    }

    public async Task<_KeysExistResponse> KeysExistAsync(_KeysExistRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.KeysExistAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_UpdateTtlResponse> UpdateTtlAsync(_UpdateTtlRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.UpdateTtlAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ItemGetTtlResponse> ItemGetTtlAsync(_ItemGetTtlRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.ItemGetTtlAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_DeleteResponse> DeleteAsync(_DeleteRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.DeleteAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_GetResponse> GetAsync(_GetRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.GetAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_SetResponse> SetAsync(_SetRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.SetAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_SetIfNotExistsResponse> SetIfNotExistsAsync(_SetIfNotExistsRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.SetIfNotExistsAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_IncrementResponse> IncrementAsync(_IncrementRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.IncrementAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_DictionarySetResponse> DictionarySetAsync(_DictionarySetRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.DictionarySetAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_DictionaryIncrementResponse> DictionaryIncrementAsync(_DictionaryIncrementRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.DictionaryIncrementAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_DictionaryGetResponse> DictionaryGetAsync(_DictionaryGetRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.DictionaryGetAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_DictionaryFetchResponse> DictionaryFetchAsync(_DictionaryFetchRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.DictionaryFetchAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_DictionaryDeleteResponse> DictionaryDeleteAsync(_DictionaryDeleteRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.DictionaryDeleteAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_DictionaryLengthResponse> DictionaryLengthAsync(_DictionaryLengthRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.DictionaryLengthAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_SetUnionResponse> SetUnionAsync(_SetUnionRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.SetUnionAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_SetDifferenceResponse> SetDifferenceAsync(_SetDifferenceRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.SetDifferenceAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_SetFetchResponse> SetFetchAsync(_SetFetchRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.SetFetchAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_SetSampleResponse> SetSampleAsync(_SetSampleRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.SetSampleAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_SetLengthResponse> SetLengthAsync(_SetLengthRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.SetLengthAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ListPushFrontResponse> ListPushFrontAsync(_ListPushFrontRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.ListPushFrontAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ListPushBackResponse> ListPushBackAsync(_ListPushBackRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.ListPushBackAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ListPopFrontResponse> ListPopFrontAsync(_ListPopFrontRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.ListPopFrontAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ListPopBackResponse> ListPopBackAsync(_ListPopBackRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.ListPopBackAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ListFetchResponse> ListFetchAsync(_ListFetchRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.ListFetchAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ListRetainResponse> ListRetainAsync(_ListRetainRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.ListRetainAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ListRemoveResponse> ListRemoveAsync(_ListRemoveRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.ListRemoveAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ListLengthResponse> ListLengthAsync(_ListLengthRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.ListLengthAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ListConcatenateFrontResponse> ListConcatenateFrontAsync(_ListConcatenateFrontRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.ListConcatenateFrontAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ListConcatenateBackResponse> ListConcatenateBackAsync(_ListConcatenateBackRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.ListConcatenateBackAsync(r, o));
        return await wrapped.ResponseAsync;
    }
}

public class DataGrpcManager : GrpcManager
{
    public readonly IDataClient Client;

    internal DataGrpcManager(IConfiguration config, string authToken, string endpoint): base(config.TransportStrategy.GrpcConfig, config.LoggerFactory, authToken, endpoint, "DataGrpcManager")
    {
        var readConcernHeader = new Header(Header.ReadConcern, config.ReadConcern.ToStringValue());
        headers.Add(readConcernHeader);
        
        var middlewares = config.Middlewares.Concat(
            new List<IMiddleware> {
                new RetryMiddleware(config.LoggerFactory, config.RetryStrategy),
                new HeaderMiddleware(config.LoggerFactory, headers),
                new MaxConcurrentRequestsMiddleware(config.LoggerFactory, config.TransportStrategy.MaxConcurrentRequests)
            }
        ).ToList();

        var client = new Scs.ScsClient(this.invoker);
        Client = new DataClientWithMiddleware(client, middlewares);
    }

    internal async Task EagerConnectAsync(TimeSpan eagerConnectionTimeout)
    {
        _logger.LogDebug("Attempting eager connection to server");
        var pingClient = new Ping.PingClient(this.channel);
        try
        {
            await pingClient.PingAsync(new _PingRequest(),
                new CallOptions(deadline: DateTime.UtcNow.Add(eagerConnectionTimeout)));
        }
        catch (RpcException ex)
        {
            MomentoErrorTransportDetails transportDetails = new MomentoErrorTransportDetails(
                new MomentoGrpcErrorDetails(ex.StatusCode, ex.Message)
            );
            throw new ConnectionException("Eager connection to server failed", transportDetails, ex);
        }
    }
}
