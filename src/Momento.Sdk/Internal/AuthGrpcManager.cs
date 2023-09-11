#if !BUILD_FOR_UNITY
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
using Momento.Protos.TokenClient;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Internal.Middleware;
using static System.Reflection.Assembly;

namespace Momento.Sdk.Internal;

public interface IAuthClient
{
    public Task<_GenerateDisposableTokenResponse> generateDisposableToken(_GenerateDisposableTokenRequest request, CallOptions callOptions);
}

internal class AuthClientWithMiddleware : IAuthClient
{
    private readonly IList<IMiddleware> _middlewares;
    private readonly Token.TokenClient _generatedClient;
    private readonly IList<Tuple<string, string>> _headers;

    public AuthClientWithMiddleware(Token.TokenClient generatedClient, IList<IMiddleware> middlewares, IList<Tuple<string, string>> headers)
    {
        _generatedClient = generatedClient;
        _middlewares = middlewares;
        _headers = headers;
    }

    public async Task<_GenerateDisposableTokenResponse> generateDisposableToken(_GenerateDisposableTokenRequest request, CallOptions callOptions)
    {
        var wrapped = await _middlewares.WrapRequest(request, callOptions, (r, o) => _generatedClient.GenerateDisposableTokenAsync(r, o));
        return await wrapped.ResponseAsync;
    }

}

public class AuthGrpcManager : IDisposable
{
    private readonly GrpcChannel channel;
    public IAuthClient Client { get; }

#if USE_GRPC_WEB
    private readonly static string moniker = "dotnet-web";
#else
    private readonly static string moniker = "dotnet";
#endif
    private readonly string version = $"{moniker}:{GetAssembly(typeof(Momento.Sdk.Responses.CacheGetResponse)).GetName().Version.ToString()}";
    // Some System.Environment.Version remarks to be aware of
    // https://learn.microsoft.com/en-us/dotnet/api/system.environment.version?view=netstandard-2.0#remarks
    private readonly string runtimeVersion = $"{moniker}:{System.Environment.Version}";
    private readonly ILogger _logger;

    public AuthGrpcManager(IAuthConfiguration config, string authToken, string endpoint)
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
            new(Header.AuthorizationKey, authToken),
            new(Header.AgentKey, version),
            new(Header.RuntimeVersionKey, runtimeVersion)
        };
        var headers = headerTuples.Select(tuple => new Header(name: tuple.Item1, value: tuple.Item2)).ToList();

        CallInvoker invoker = this.channel.CreateCallInvoker();

        var middlewares = new List<IMiddleware> {
            new HeaderMiddleware(config.LoggerFactory, headers)
        };

        var client = new Token.TokenClient(invoker);
        Client = new AuthClientWithMiddleware(client, middlewares, headerTuples);
    }

    public void Dispose()
    {
        this.channel.Dispose();
        GC.SuppressFinalize(this);
    }
}
#endif
