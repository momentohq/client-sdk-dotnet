using Momento.Sdk.Auth;
using Momento.Sdk.Auth.AccessControl;
using Momento.Sdk.Config;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;
using Momento.Sdk.Responses;
using System;
using System.Threading.Tasks;

namespace Momento.Sdk;

/// <inheritdoc />
public class AuthClient : IAuthClient
{

    private readonly ScsTokenClient scsTokenClient;

    /// <summary>
    /// Create a new <see cref="AuthClient"/> object.
    /// </summary>
    /// <param name="config">The config.</param>
    /// <param name="authProvider">The auth provider.</param>
    public AuthClient(IAuthConfiguration config, ICredentialProvider authProvider)
    {
        scsTokenClient = new ScsTokenClient(config, authProvider);
    }

    /// <inheritdoc />
    public async Task<GenerateDisposableTokenResponse> GenerateDisposableTokenAsync(DisposableTokenScope scope, ExpiresIn expiresIn, string? tokenId = null)
    {
        try
        {
            Utils.CheckValidDisposableTokenExpiry(expiresIn);
        }
        catch (Exception e)
        {
            return new GenerateDisposableTokenResponse.Error(new InvalidArgumentException(e.Message));
        }
        return await scsTokenClient.GenerateDisposableToken(scope, expiresIn, tokenId);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        scsTokenClient.Dispose();
    }
}
