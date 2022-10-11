#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Momento.Protos.ControlClient;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Internal.Middleware;
using static System.Reflection.Assembly;

namespace Momento.Sdk.Internal;


public interface IControlClient
{
    public Task<_CreateCacheResponse> CreateCacheAsync(_CreateCacheRequest request, CallOptions callOptions);
    public Task<_DeleteCacheResponse> DeleteCacheAsync(_DeleteCacheRequest request, CallOptions callOptions);
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

    public async Task<_ListCachesResponse> ListCachesAsync(_ListCachesRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.ListCachesAsync(r, o));
        return await wrapped.ResponseAsync;
    }
}

internal sealed class ControlGrpcManager : IDisposable
{
    private readonly GrpcChannel channel;
    public IControlClient Client { get; }
    private readonly string version = "dotnet:" + GetAssembly(typeof(Momento.Sdk.Responses.CacheGetResponse)).GetName().Version.ToString();
    // Some System.Environment.Version remarks to be aware of
    // https://learn.microsoft.com/en-us/dotnet/api/system.environment.version?view=netstandard-2.0#remarks
    private readonly string runtimeVersion = "dotnet:" + System.Environment.Version;
    private readonly ILogger _logger;

    public ControlGrpcManager(ILoggerFactory loggerFactory, string authToken, string endpoint)
    {
        var uri = $"https://{endpoint}";
        this.channel = GrpcChannel.ForAddress(uri, new GrpcChannelOptions() { Credentials = ChannelCredentials.SecureSsl });
        List<Header> headers = new List<Header> { new Header(name: Header.AuthorizationKey, value: authToken), new Header(name: Header.AgentKey, value: version), new Header(name: Header.RuntimeVersionKey, value: runtimeVersion) };
        CallInvoker invoker = this.channel.CreateCallInvoker();

        var middlewares = new List<IMiddleware> {
            new HeaderMiddleware(loggerFactory, headers)
        };

        Client = new ControlClientWithMiddleware(new ScsControl.ScsControlClient(invoker), middlewares);

        this._logger = loggerFactory.CreateLogger<ControlGrpcManager>();
    }

    public void Dispose()
    {
        this.channel.Dispose();
        GC.SuppressFinalize(this);
    }
}
