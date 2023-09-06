using System.Threading.Tasks;
using Momento.Sdk.Auth;
using Momento.Sdk.Auth.AccessControl;
using Momento.Sdk.Config;
using Momento.Sdk.Internal;
using Momento.Sdk.Responses;

namespace Momento.Sdk;

public class AuthClient : IAuthClient
{

    private readonly ScsTokenClient scsTokenClient;

    public AuthClient(IAuthConfiguration config, ICredentialProvider authProvider)
    {
        // TODO: do we need to adjust the endpoint in the `ScsTokenClient` constructor?
        scsTokenClient = new ScsTokenClient(config, authProvider);
    }

    public Task<GenerateDisposableTokenResponse> GenerateDisposableTokenAsync(DisposableTokenScope scope, ExpiresIn expiresIn)
    {
        throw new System.NotImplementedException();
    }
}
