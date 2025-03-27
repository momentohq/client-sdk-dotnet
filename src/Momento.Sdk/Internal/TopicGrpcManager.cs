#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
#if USE_GRPC_WEB
using System.Net.Http;
using Grpc.Net.Client.Web;
#endif
using Microsoft.Extensions.Logging;
using Momento.Protos.CacheClient.Pubsub;
using Momento.Protos.Common;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Retry;
using Momento.Sdk.Internal.Middleware;
using static System.Reflection.Assembly;

namespace Momento.Sdk.Internal;

public interface IPubsubClient
{
    public Task<_Empty> publish(_PublishRequest request, CallOptions callOptions);
    public AsyncServerStreamingCall<_SubscriptionItem> subscribe(_SubscriptionRequest request, CallOptions callOptions);
}

public class PubsubClientWithMiddleware : IPubsubClient
{
    private readonly Pubsub.PubsubClient _generatedClient;
    private readonly IList<IMiddleware> _middlewares;
    private readonly IList<Tuple<string, string>> _headers;

    public PubsubClientWithMiddleware(Pubsub.PubsubClient generatedClient, IList<IMiddleware> middlewares, IList<Tuple<string, string>> headers)
    {
        _generatedClient = generatedClient;
        _middlewares = middlewares;
        _headers = headers;
    }

    public async Task<_Empty> publish(_PublishRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.PublishAsync(r, o));
        return await wrapped.ResponseAsync;
    }

    public AsyncServerStreamingCall<_SubscriptionItem> subscribe(_SubscriptionRequest request, CallOptions callOptions)
    {
        // Middleware is not currently compatible with gRPC streaming calls,
        // so we manually add the headers to ensure the call has the auth token
        // and any other headers that were added.
        var callHeaders = callOptions.Headers ?? new Metadata();
        if (callOptions.Headers == null)
        {
            callOptions = callOptions.WithHeaders(new Metadata());
        }

        // The _headers are from the base GrpcManager, includes authorization, agent, and runtime version.
        foreach (var header in _headers)
        {
            if (callHeaders.Get(header.Item1) == null)
            {
                callHeaders.Add(header.Item1, header.Item2);
            }
        }

        // We also add any stream headers configured on the middlewares.
        foreach (var middleware in _middlewares)
        {
            foreach (var header in middleware.AddStreamRequestHeaders())
            {
                if (callHeaders.Get(header.Item1) == null) 
                {
                    callHeaders.Add(header.Item1, header.Item2);
                }
            }
        }

        return _generatedClient.Subscribe(request, callOptions.WithHeaders(callHeaders));
    }
}

public class TopicGrpcManager : GrpcManager
{
    public readonly IPubsubClient Client;

    internal TopicGrpcManager(ITopicConfiguration config, ICredentialProvider authProvider) : base(config.TransportStrategy.GrpcConfig, config.LoggerFactory, authProvider, authProvider.CacheEndpoint, "TopicGrpcManager")
    {
        var middlewares = config.Middlewares.Concat(
            new List<IMiddleware>
            {
                new HeaderMiddleware(config.LoggerFactory, this.headers),
            }
        ).ToList();

        var client = new Pubsub.PubsubClient(this.invoker);
        Client = new PubsubClientWithMiddleware(client, middlewares, this.headerTuples);
    }
}
