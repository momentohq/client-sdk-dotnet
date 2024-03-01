#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Internal.Middleware;
using Momento.Sdk.Config.Transport;
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
    /// The default value for max_send_message_length is 4mb.  We need to increase this to 5mb in order to support cases where users have requested a limit increase up to our maximum item size of 5mb.
    /// </summary>
    protected const int DEFAULT_MAX_MESSAGE_SIZE = 5_243_000;

    /// <summary>
    /// Constructor for GrpcManager, establishes the GRPC connection and creates the logger and invoker.
    /// </summary>
    /// <param name="grpcConfig"></param>
    /// <param name="loggerFactory"></param>
    /// <param name="authToken"></param>
    /// <param name="endpoint"></param>
    /// <param name="loggerName"></param>
    internal GrpcManager(IGrpcConfiguration grpcConfig, ILoggerFactory loggerFactory, string authToken, string endpoint, string loggerName)
    {
#if USE_GRPC_WEB
        // Note: all web SDK requests are routed to a `web.` subdomain to allow us flexibility on the server
        endpoint = $"web.{endpoint}";
#endif
        var uri = $"https://{endpoint}";
        var channelOptions = this.GrpcChannelOptionsFromGrpcConfig(grpcConfig, loggerFactory);
        this.channel = GrpcChannel.ForAddress(uri, channelOptions);
        this.invoker = this.channel.CreateCallInvoker();
        this._logger = loggerFactory.CreateLogger(loggerName);

        this.headerTuples = new List<Tuple<string, string>>
        {
            new(Header.AuthorizationKey, authToken),
            new(Header.AgentKey, version),
            new(Header.RuntimeVersionKey, runtimeVersion)
        };
        this.headers = headerTuples.Select(tuple => new Header(name: tuple.Item1, value: tuple.Item2)).ToList();
    }

    /// <summary>
    /// Create a GrpcChannelOptions object from an IGrpcConfiguration object.
    /// </summary>
    /// <param name="grpcConfig">The IGrpcConfiguration object specifying underlying grpc options</param>
    /// <param name="loggerFactory"></param>
    /// <returns></returns>
    public GrpcChannelOptions GrpcChannelOptionsFromGrpcConfig(IGrpcConfiguration grpcConfig, ILoggerFactory loggerFactory) {
        var channelOptions = grpcConfig.GrpcChannelOptions ?? new GrpcChannelOptions();
        channelOptions.Credentials = ChannelCredentials.SecureSsl;
        channelOptions.LoggerFactory ??= loggerFactory;
        channelOptions.MaxReceiveMessageSize = grpcConfig.GrpcChannelOptions?.MaxReceiveMessageSize ?? DEFAULT_MAX_MESSAGE_SIZE;
        channelOptions.MaxSendMessageSize = grpcConfig.GrpcChannelOptions?.MaxSendMessageSize ?? DEFAULT_MAX_MESSAGE_SIZE;
#if NET5_0_OR_GREATER
        var keepAliveWithoutCalls = System.Net.Http.HttpKeepAlivePingPolicy.WithActiveRequests;
        if (grpcConfig.KeepAlivePermitWithoutCalls == true)
        {
            keepAliveWithoutCalls = System.Net.Http.HttpKeepAlivePingPolicy.Always;
        }

        if (SocketsHttpHandler.IsSupported) // see: https://github.com/grpc/grpc-dotnet/blob/098dca892a3949ade411c3f2f66003f7b330dfd2/src/Shared/HttpHandlerFactory.cs#L28-L30
        {
            channelOptions.HttpHandler = new SocketsHttpHandler
            {
                EnableMultipleHttp2Connections = grpcConfig.SocketsHttpHandlerOptions.EnableMultipleHttp2Connections,
                PooledConnectionIdleTimeout = grpcConfig.SocketsHttpHandlerOptions.PooledConnectionIdleTimeout,
                KeepAlivePingTimeout = grpcConfig.KeepAlivePingTimeout,
                KeepAlivePingDelay = grpcConfig.KeepAlivePingDelay,
                KeepAlivePingPolicy = keepAliveWithoutCalls
            };
        }
#elif USE_GRPC_WEB
        channelOptions.HttpHandler = new GrpcWebHandler(new HttpClientHandler());
#endif
        return channelOptions;
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