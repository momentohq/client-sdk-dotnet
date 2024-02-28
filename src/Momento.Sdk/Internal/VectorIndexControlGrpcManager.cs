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

public class VectorIndexControlGrpcManager : IDisposable
{
    private readonly GrpcChannel channel;

    public readonly IVectorIndexControlClient Client;

#if USE_GRPC_WEB
    private const string Moniker = "dotnet-web";
#else
    private const string Moniker = "dotnet";
#endif
    private readonly string version = $"{Moniker}:{GetAssembly(typeof(Responses.CacheGetResponse)).GetName().Version.ToString()}";
    // Some System.Environment.Version remarks to be aware of
    // https://learn.microsoft.com/en-us/dotnet/api/system.environment.version?view=netstandard-2.0#remarks
    private readonly string runtimeVersion = $"{Moniker}:{Environment.Version}";
    private readonly ILogger _logger;

    internal VectorIndexControlGrpcManager(ILoggerFactory loggerFactory, string authToken, string endpoint)
    {
#if USE_GRPC_WEB
        // Note: all web SDK requests are routed to a `web.` subdomain to allow us flexibility on the server
        endpoint = $"web.{endpoint}";
#endif
        var uri = $"https://{endpoint}";
        var channelOptions = Utils.GrpcChannelOptionsFromGrpcConfig(null, loggerFactory);
        channel = GrpcChannel.ForAddress(uri, channelOptions);
        var headers = new List<Header> { new(name: Header.AuthorizationKey, value: authToken), new(name: Header.AgentKey, value: version), new(name: Header.RuntimeVersionKey, value: runtimeVersion) };

        _logger = loggerFactory.CreateLogger<VectorIndexControlGrpcManager>();

        var invoker = channel.CreateCallInvoker();

        var middlewares = new List<IMiddleware> {
            new HeaderMiddleware(loggerFactory, headers)
        };

        var client = new ScsControl.ScsControlClient(invoker);
        Client = new VectorIndexControlClientWithMiddleware(client, middlewares);
    }

    public void Dispose()
    {
        channel.Dispose();
        GC.SuppressFinalize(this);
    }
}
