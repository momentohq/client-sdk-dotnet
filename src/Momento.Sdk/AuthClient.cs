using System.Threading.Tasks;
using Momento.Sdk.Auth.AccessControl;
using Momento.Sdk.Responses;

namespace Momento.Sdk;

public class AuthClient : IAuthClient
{
    public Task<GenerateDisposableTokenResponse> GenerateDisposableTokenAsync(DisposableTokenScope scope, ExpiresIn expiresIn)
    {
        throw new System.NotImplementedException();
    }
}
