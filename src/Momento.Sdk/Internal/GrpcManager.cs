#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Internal.Middleware;
using Momento.Sdk.Config.Transport;
using Momento.Sdk.Auth;
using Grpc.Core;
using Grpc.Net.Client;
#if USE_GRPC_WEB
using Grpc.Net.Client.Web;
#endif
using static System.Reflection.Assembly;

namespace Momento.Sdk.Internal;

/// <summary>
/// Base class for the various GrpcManager classes.
/// </summary>
public class GrpcManager : IDisposable
{
    protected GrpcChannel channel;

    protected ILogger _logger;

#if USE_GRPC_WEB
    protected readonly static string moniker = "dotnet-web";
#else
    protected readonly static string moniker = "dotnet";
#endif
    protected readonly string version = $"{moniker}:{GetAssembly(typeof(Momento.Sdk.Responses.CacheGetResponse)).GetName().Version.ToString()}";
    // Some System.Environment.Version remarks to be aware of
    // https://learn.microsoft.com/en-us/dotnet/api/system.environment.version?view=netstandard-2.0#remarks

    protected readonly string runtimeVersion = $"{moniker}:{System.Environment.Version}";

    internal List<Header> headers;

    internal List<Tuple<string, string>> headerTuples;

    protected CallInvoker invoker;

    /// <summary>
    /// Constructor for GrpcManager, establishes the GRPC connection and creates the logger and invoker.
    /// </summary>
    /// <param name="grpcConfig"></param>
    /// <param name="loggerFactory"></param>
    /// <param name="authProvider"></param>
    /// <param name="endpoint"></param>
    /// <param name="loggerName"></param>
    internal GrpcManager(IGrpcConfiguration grpcConfig, ILoggerFactory loggerFactory, ICredentialProvider authProvider, string endpoint, string loggerName)
    {
        this._logger = loggerFactory.CreateLogger(loggerName);

        this.headerTuples = new List<Tuple<string, string>>
        {
            new(Header.AuthorizationKey, authProvider.AuthToken),
            new(Header.AgentKey, version),
            new(Header.RuntimeVersionKey, runtimeVersion)
        };
        this.headers = headerTuples.Select(tuple => new Header(name: tuple.Item1, value: tuple.Item2)).ToList();

        // Set all channel opens and create the grpc connection
        var channelOptions = grpcConfig.GrpcChannelOptions;
        channelOptions.LoggerFactory ??= loggerFactory;
        // Always disable gRPC service config 
        channelOptions.ServiceConfig = null;

        // If connecting to momento-local, must use http and Insecure channel credentials.
        if (authProvider.SecureEndpoints)
        {
            channelOptions.Credentials = ChannelCredentials.SecureSsl;
        }
        else
        {
            channelOptions.Credentials = ChannelCredentials.Insecure;
        }

#if NET5_0_OR_GREATER
        if (SocketsHttpHandler.IsSupported) // see: https://github.com/grpc/grpc-dotnet/blob/098dca892a3949ade411c3f2f66003f7b330dfd2/src/Shared/HttpHandlerFactory.cs#L28-L30
        {
            channelOptions.HttpHandler = new SocketsHttpHandler
            {
                EnableMultipleHttp2Connections = grpcConfig.SocketsHttpHandlerOptions.EnableMultipleHttp2Connections,
                PooledConnectionIdleTimeout = grpcConfig.SocketsHttpHandlerOptions.PooledConnectionIdleTimeout,
                KeepAlivePingTimeout = grpcConfig.SocketsHttpHandlerOptions.KeepAlivePingTimeout,
                KeepAlivePingDelay = grpcConfig.SocketsHttpHandlerOptions.KeepAlivePingDelay,
                KeepAlivePingPolicy = grpcConfig.SocketsHttpHandlerOptions.KeepAlivePermitWithoutCalls ? System.Net.Http.HttpKeepAlivePingPolicy.Always : System.Net.Http.HttpKeepAlivePingPolicy.WithActiveRequests,
            };
        }
#elif USE_GRPC_WEB
        channelOptions.HttpHandler = new GrpcWebHandler(new HttpClientHandler());
        // Note: all web SDK requests are routed to a `web.` subdomain to allow us flexibility on the server
        endpoint = $"web.{endpoint}";
#endif
        // If connecting to momento-local, must use http and Insecure channel credentials.
        var uri = $"https://{endpoint}";
        if (!authProvider.SecureEndpoints)
        {
            uri = $"http://{endpoint}";
        }
        this.channel = GrpcChannel.ForAddress(uri, channelOptions);
        this.invoker = this.channel.CreateCallInvoker();
    }

    /// <summary>
    /// Implement IDisposable.
    /// </summary>
    public void Dispose()
    {
        this.channel.Dispose();
        GC.SuppressFinalize(this);
    }
}
