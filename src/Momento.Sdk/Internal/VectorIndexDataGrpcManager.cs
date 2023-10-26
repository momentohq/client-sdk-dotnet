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
    public Task<_UpsertItemBatchResponse> UpsertItemBatchAsync(_UpsertItemBatchRequest request, CallOptions callOptions);
    public Task<_SearchResponse> SearchAsync(_SearchRequest request, CallOptions callOptions);
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

    public async Task<_DeleteItemBatchResponse> DeleteItemBatchAsync(_DeleteItemBatchRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.DeleteItemBatchAsync(r, o));
        return await wrapped.ResponseAsync;
    }
}

public class VectorIndexDataGrpcManager : IDisposable
{
    private readonly GrpcChannel channel;

    public readonly IVectorIndexDataClient Client;

#if USE_GRPC_WEB
    private const string Moniker = "dotnet-web";
#else
    private const string Moniker = "dotnet";
#endif
    private readonly string version = $"{Moniker}:{GetAssembly(typeof(Responses.CacheGetResponse)).GetName().Version}";
    // Some System.Environment.Version remarks to be aware of
    // https://learn.microsoft.com/en-us/dotnet/api/system.environment.version?view=netstandard-2.0#remarks
    private readonly string runtimeVersion = $"{Moniker}:{Environment.Version}";
    private readonly ILogger _logger;

    internal VectorIndexDataGrpcManager(IVectorIndexConfiguration config, string authToken, string endpoint)
    {
#if USE_GRPC_WEB
        // Note: all web SDK requests are routed to a `web.` subdomain to allow us flexibility on the server
        endpoint = $"web.{endpoint}";
#endif
        var uri = $"https://{endpoint}";
        var channelOptions = config.TransportStrategy.GrpcConfig.GrpcChannelOptions;
        channelOptions.LoggerFactory ??= config.LoggerFactory;
        channelOptions.Credentials = ChannelCredentials.SecureSsl;
#if USE_GRPC_WEB
        channelOptions.HttpHandler = new GrpcWebHandler(new HttpClientHandler());
#endif

        channel = GrpcChannel.ForAddress(uri, channelOptions);
        var headers = new List<Header> { new(name: Header.AuthorizationKey, value: authToken), new(name: Header.AgentKey, value: version), new(name: Header.RuntimeVersionKey, value: runtimeVersion) };

        _logger = config.LoggerFactory.CreateLogger<DataGrpcManager>();

        var invoker = channel.CreateCallInvoker();

        var middlewares = new List<IMiddleware> {
            new HeaderMiddleware(config.LoggerFactory, headers)
        };

        var client = new VectorIndex.VectorIndexClient(invoker);



        Client = new VectorIndexDataClientWithMiddleware(client, middlewares);
    }

    public void Dispose()
    {
        channel.Dispose();
        GC.SuppressFinalize(this);
    }
}
