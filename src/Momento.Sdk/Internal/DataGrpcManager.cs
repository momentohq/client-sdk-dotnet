﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Momento.Protos.CacheClient;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Internal.Middleware;
using static System.Reflection.Assembly;
using static Grpc.Core.Interceptors.Interceptor;

namespace Momento.Sdk.Internal;

public interface IDataClient
{
    public Task<_GetResponse> GetAsync(_GetRequest request, CallOptions callOptions);
    public Task<_SetResponse> SetAsync(_SetRequest request, CallOptions callOptions);
    public Task<_DeleteResponse> DeleteAsync(_DeleteRequest request, CallOptions callOptions);
    public Task<_DictionarySetResponse> DictionarySetAsync(_DictionarySetRequest request, CallOptions callOptions);
    public Task<_DictionaryIncrementResponse> DictionaryIncrementAsync(_DictionaryIncrementRequest request, CallOptions callOptions);
    public Task<_DictionaryGetResponse> DictionaryGetAsync(_DictionaryGetRequest request, CallOptions callOptions);
    public Task<_DictionaryFetchResponse> DictionaryFetchAsync(_DictionaryFetchRequest request, CallOptions callOptions);
    public Task<_DictionaryDeleteResponse> DictionaryDeleteAsync(_DictionaryDeleteRequest request, CallOptions callOptions);
    public Task<_SetUnionResponse> SetUnionAsync(_SetUnionRequest request, CallOptions callOptions);
    public Task<_SetDifferenceResponse> SetDifferenceAsync(_SetDifferenceRequest request, CallOptions callOptions);
    public Task<_SetFetchResponse> SetFetchAsync(_SetFetchRequest request, CallOptions callOptions);
    public Task<_ListPushFrontResponse> ListPushFrontAsync(_ListPushFrontRequest request, CallOptions callOptions);
    public Task<_ListPushBackResponse> ListPushBackAsync(_ListPushBackRequest request, CallOptions callOptions);
    public Task<_ListPopFrontResponse> ListPopFrontAsync(_ListPopFrontRequest request, CallOptions callOptions);
    public Task<_ListPopBackResponse> ListPopBackAsync(_ListPopBackRequest request, CallOptions callOptions);
    public Task<_ListFetchResponse> ListFetchAsync(_ListFetchRequest request, CallOptions callOptions);
    public Task<_ListRemoveResponse> ListRemoveAsync(_ListRemoveRequest request, CallOptions callOptions);
    public Task<_ListLengthResponse> ListLengthAsync(_ListLengthRequest request, CallOptions callOptions);
}


// Ideally we would implement our middleware based on gRPC Interceptors.  Unfortunately,
// the their method signatures are not asynchronous. Thus, for any middleware that may
// require asynchronous actions (such as our MaxConcurrentRequestsMiddleware), we would
// end up blocking threads to wait for the completion of the async task, which would have
// a big negative impact on performance. Instead, in this commit, we implement a thin
// middleware layer of our own that uses asynchronous signatures throughout.  This has
// the nice side effect of making the user-facing API for writing Middlewares a bit less
// of a learning curve for anyone not super deep on gRPC internals.
internal class DataClientWithMiddleware : IDataClient
{
    private readonly IList<IMiddleware> _middlewares;
    private readonly Scs.ScsClient _generatedClient;

    public DataClientWithMiddleware(Scs.ScsClient generatedClient, IList<IMiddleware> middlewares)
    {
        _generatedClient = generatedClient;
        _middlewares = middlewares;
    }

    public async Task<_DeleteResponse> DeleteAsync(_DeleteRequest request, CallOptions callOptions)
    {
        var wrapped = await WrapWithMiddleware(request, callOptions, (r, o) => _generatedClient.DeleteAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_GetResponse> GetAsync(_GetRequest request, CallOptions callOptions)
    {
        var wrapped = await WrapWithMiddleware(request, callOptions, (r, o) => _generatedClient.GetAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_SetResponse> SetAsync(_SetRequest request, CallOptions callOptions)
    {
        var wrapped = await WrapWithMiddleware(request, callOptions, (r, o) => _generatedClient.SetAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_DictionarySetResponse> DictionarySetAsync(_DictionarySetRequest request, CallOptions callOptions)
    {
        var wrapped = await WrapWithMiddleware(request, callOptions, (r, o) => _generatedClient.DictionarySetAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_DictionaryIncrementResponse> DictionaryIncrementAsync(_DictionaryIncrementRequest request, CallOptions callOptions)
    {
        var wrapped = await WrapWithMiddleware(request, callOptions, (r, o) => _generatedClient.DictionaryIncrementAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_DictionaryGetResponse> DictionaryGetAsync(_DictionaryGetRequest request, CallOptions callOptions)
    {
        var wrapped = await WrapWithMiddleware(request, callOptions, (r, o) => _generatedClient.DictionaryGetAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_DictionaryFetchResponse> DictionaryFetchAsync(_DictionaryFetchRequest request, CallOptions callOptions)
    {
        var wrapped = await WrapWithMiddleware(request, callOptions, (r, o) => _generatedClient.DictionaryFetchAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_DictionaryDeleteResponse> DictionaryDeleteAsync(_DictionaryDeleteRequest request, CallOptions callOptions)
    {
        var wrapped = await WrapWithMiddleware(request, callOptions, (r, o) => _generatedClient.DictionaryDeleteAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_SetUnionResponse> SetUnionAsync(_SetUnionRequest request, CallOptions callOptions)
    {
        var wrapped = await WrapWithMiddleware(request, callOptions, (r, o) => _generatedClient.SetUnionAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_SetDifferenceResponse> SetDifferenceAsync(_SetDifferenceRequest request, CallOptions callOptions)
    {
        var wrapped = await WrapWithMiddleware(request, callOptions, (r, o) => _generatedClient.SetDifferenceAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_SetFetchResponse> SetFetchAsync(_SetFetchRequest request, CallOptions callOptions)
    {
        var wrapped = await WrapWithMiddleware(request, callOptions, (r, o) => _generatedClient.SetFetchAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ListPushFrontResponse> ListPushFrontAsync(_ListPushFrontRequest request, CallOptions callOptions)
    {
        var wrapped = await WrapWithMiddleware(request, callOptions, (r, o) => _generatedClient.ListPushFrontAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ListPushBackResponse> ListPushBackAsync(_ListPushBackRequest request, CallOptions callOptions)
    {
        var wrapped = await WrapWithMiddleware(request, callOptions, (r, o) => _generatedClient.ListPushBackAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ListPopFrontResponse> ListPopFrontAsync(_ListPopFrontRequest request, CallOptions callOptions)
    {
        var wrapped = await WrapWithMiddleware(request, callOptions, (r, o) => _generatedClient.ListPopFrontAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ListPopBackResponse> ListPopBackAsync(_ListPopBackRequest request, CallOptions callOptions)
    {
        var wrapped = await WrapWithMiddleware(request, callOptions, (r, o) => _generatedClient.ListPopBackAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ListFetchResponse> ListFetchAsync(_ListFetchRequest request, CallOptions callOptions)
    {
        var wrapped = await WrapWithMiddleware(request, callOptions, (r, o) => _generatedClient.ListFetchAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ListRemoveResponse> ListRemoveAsync(_ListRemoveRequest request, CallOptions callOptions)
    {
        var wrapped = await WrapWithMiddleware(request, callOptions, (r, o) => _generatedClient.ListRemoveAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public async Task<_ListLengthResponse> ListLengthAsync(_ListLengthRequest request, CallOptions callOptions)
    {
        var wrapped = await WrapWithMiddleware(request, callOptions, (r, o) => _generatedClient.ListLengthAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    private async Task<MiddlewareResponseState<TResponse>> WrapWithMiddleware<TRequest, TResponse>(
        TRequest request,
        CallOptions callOptions,
        Func<TRequest, CallOptions, AsyncUnaryCall<TResponse>> continuation
    )
    {
        Func<TRequest, CallOptions, Task<MiddlewareResponseState<TResponse>>> continuationWithMiddlewareResponseState = (r, o) =>
        {
            var result = continuation(r, o);
            return Task.FromResult(new MiddlewareResponseState<TResponse>(
                ResponseAsync: result.ResponseAsync,
                ResponseHeadersAsync: result.ResponseHeadersAsync,
                GetStatus: result.GetStatus,
                GetTrailers: result.GetTrailers
            ));
        };

        var wrapped = _middlewares.Aggregate(continuationWithMiddlewareResponseState, (acc, middleware) =>
        {
            return (r, o) => middleware.WrapRequest(r, o, acc);
        });
        return await wrapped(request, callOptions);
    }
}

public class DataGrpcManager : IDisposable
{
    private readonly GrpcChannel channel;

    public readonly IDataClient Client;

    private readonly string version = "dotnet:" + GetAssembly(typeof(Responses.CacheGetResponse)).GetName().Version.ToString();
    // Some System.Environment.Version remarks to be aware of
    // https://learn.microsoft.com/en-us/dotnet/api/system.environment.version?view=netstandard-2.0#remarks
    private readonly string runtimeVersion = "dotnet:" + Environment.Version;
    private readonly ILogger _logger;

    internal DataGrpcManager(IConfiguration config, string authToken, string host)
    {
        var url = $"https://{host}";
        var channelOptions = config.TransportStrategy.GrpcConfig.GrpcChannelOptions;
        if (channelOptions.LoggerFactory == null)
        {
            channelOptions.LoggerFactory = config.LoggerFactory;
        }
        channelOptions.Credentials = ChannelCredentials.SecureSsl;

        this.channel = GrpcChannel.ForAddress(url, channelOptions);
        List<Header> headers = new List<Header> { new Header(name: Header.AuthorizationKey, value: authToken), new Header(name: Header.AgentKey, value: version), new Header(name: Header.RuntimeVersionKey, value: runtimeVersion) };
        this._logger = config.LoggerFactory.CreateLogger<DataGrpcManager>();
        CallInvoker invoker = this.channel.Intercept(new HeaderInterceptor(headers));

        var middlewares = config.Middlewares.Concat(
            new List<IMiddleware> {
                new MaxConcurrentRequestsMiddleware(config.LoggerFactory, config.TransportStrategy.MaxConcurrentRequests)
            }
        ).ToList();

        Client = new DataClientWithMiddleware(new Scs.ScsClient(invoker), middlewares);
    }

    public void Dispose()
    {
        this.channel.Dispose();
        GC.SuppressFinalize(this);
    }
}