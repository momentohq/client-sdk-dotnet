using System;
using System.Threading.Tasks;
using Momento.Sdk.Auth;
using Momento.Sdk.Auth.AccessControl;
using Momento.Sdk.Config;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;
using Momento.Sdk.Responses;

namespace Momento.Sdk;

public class AuthClient : IAuthClient
{

    private readonly ScsTokenClient scsTokenClient;

    public AuthClient(IAuthConfiguration config, ICredentialProvider authProvider)
    {
        scsTokenClient = new ScsTokenClient(config, authProvider.AuthToken, authProvider.TokenEndpoint);
    }

    public async Task<GenerateDisposableTokenResponse> GenerateDisposableTokenAsync(DisposableTokenScope scope, ExpiresIn expiresIn)
    {
        try {
            Utils.CheckValidDisposableTokenExpiry(expiresIn);
        }
        catch (Exception e)
        {
            return new GenerateDisposableTokenResponse.Error(new InvalidArgumentException(e.Message));
        }
        return await scsTokenClient.GenerateDisposableToken(scope, expiresIn);
    }
}
