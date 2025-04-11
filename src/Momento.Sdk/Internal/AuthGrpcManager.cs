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
using Momento.Protos.TokenClient;
using Momento.Sdk.Auth;
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
    private readonly IList<KeyValuePair<string, string>> _headers;

    public AuthClientWithMiddleware(Token.TokenClient generatedClient, IList<IMiddleware> middlewares, IList<KeyValuePair<string, string>> headers)
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

public class AuthGrpcManager : GrpcManager
{
    public IAuthClient Client { get; }

    public AuthGrpcManager(IAuthConfiguration config, ICredentialProvider authProvider) : base(config.TransportStrategy.GrpcConfig, config.LoggerFactory, authProvider, authProvider.TokenEndpoint, "AuthGrpcManager")
    {
        var middlewares = new List<IMiddleware> {
            new HeaderMiddleware(config.LoggerFactory, this.headers)
        };

        var client = new Token.TokenClient(this.invoker);
        Client = new AuthClientWithMiddleware(client, middlewares, this.headerTuples);
    }
}
