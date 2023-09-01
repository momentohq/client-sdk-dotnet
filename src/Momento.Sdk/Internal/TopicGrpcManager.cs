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
using Momento.Protos.CacheClient;
using Momento.Protos.CacheClient.Pubsub;
using Momento.Protos.CachePing;
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

    public PubsubClientWithMiddleware(Pubsub.PubsubClient generatedClient, IList<IMiddleware> middlewares,
        IList<Tuple<string, string>> headers)
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
        // so we manually add the headers to ensure the call has the auth token.
        var callHeaders = callOptions.Headers ?? new Metadata();
        if (callOptions.Headers == null)
        {
            callOptions = callOptions.WithHeaders(new Metadata());
        }
        
        foreach (var header in _headers)
        {
            callHeaders.Add(header.Item1, header.Item2);
        }

        return _generatedClient.Subscribe(request, callOptions.WithHeaders(callHeaders));
    }
}

public class TopicGrpcManager : IDisposable
{
    private readonly GrpcChannel channel;

    public readonly IPubsubClient Client;

#if USE_GRPC_WEB
    private static readonly string Moniker = "dotnet-web";
#else
    private static readonly string Moniker = "dotnet";
#endif
    private readonly string version =
        $"{Moniker}:{GetAssembly(typeof(Responses.CacheGetResponse)).GetName().Version.ToString()}";

    // Some System.Environment.Version remarks to be aware of
    // https://learn.microsoft.com/en-us/dotnet/api/system.environment.version?view=netstandard-2.0#remarks
    private readonly string runtimeVersion = $"{Moniker}:{Environment.Version}";
    private readonly ILogger _logger;

    internal TopicGrpcManager(IConfiguration config, string authToken, string endpoint)
    {
#if USE_GRPC_WEB
        // Note: all web SDK requests are routed to a `web.` subdomain to allow us flexibility on the server
        endpoint = $"web.{endpoint}";
#endif
        var uri = $"https://{endpoint}";
        var channelOptions = config.TransportStrategy.GrpcConfig.GrpcChannelOptions;
        if (channelOptions.LoggerFactory == null)
        {
            channelOptions.LoggerFactory = config.LoggerFactory;
        }

        channelOptions.Credentials = ChannelCredentials.SecureSsl;
#if USE_GRPC_WEB
        channelOptions.HttpHandler = new GrpcWebHandler(new HttpClientHandler());
#endif

        channel = GrpcChannel.ForAddress(uri, channelOptions);
        var headerTuples = new List<Tuple<string, string>>
        {
            new(Header.AuthorizationKey, authToken), new(Header.AgentKey, version),
            new(Header.RuntimeVersionKey, runtimeVersion)
        };
        var headers = headerTuples.Select(tuple => new Header(name: tuple.Item1, value: tuple.Item2)).ToList();

        _logger = config.LoggerFactory.CreateLogger<TopicGrpcManager>();

        var invoker = channel.CreateCallInvoker();
        
        var middlewares = config.Middlewares.Concat(
            new List<IMiddleware> {
                new RetryMiddleware(config.LoggerFactory, config.RetryStrategy),
                new HeaderMiddleware(config.LoggerFactory, headers),
                new MaxConcurrentRequestsMiddleware(config.LoggerFactory, config.TransportStrategy.MaxConcurrentRequests)
            }
        ).ToList();

        var client = new Pubsub.PubsubClient(invoker);

        if (config.TransportStrategy.EagerConnectionTimeout != null)
        {
            var eagerConnectionTimeout = config.TransportStrategy.EagerConnectionTimeout.Value;
            _logger.LogDebug("TransportStrategy EagerConnection is enabled; attempting to connect to server");
            var pingClient = new Ping.PingClient(channel);
            try
            {
                pingClient.Ping(new _PingRequest(),
                    new CallOptions(deadline: DateTime.UtcNow.Add(eagerConnectionTimeout)));
            }
            catch (RpcException)
            {
                _logger.LogWarning(
                    "Failed to eagerly connect to the server; continuing with execution in case failure is recoverable later.");
            }
        }

        Client = new PubsubClientWithMiddleware(client, middlewares, headerTuples);
    }

    public void Dispose()
    {
        channel.Dispose();
        GC.SuppressFinalize(this);
    }
}