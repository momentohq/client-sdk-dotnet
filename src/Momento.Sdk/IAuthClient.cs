#if !BUILD_FOR_UNITY
using System;
using System.Threading.Tasks;
using Momento.Sdk.Auth.AccessControl;
using Momento.Sdk.Responses;

namespace Momento.Sdk;

public interface IAuthClient : IDisposable
{
    public Task<GenerateDisposableTokenResponse> GenerateDisposableTokenAsync(DisposableTokenScope scope,
        ExpiresIn expiresIn);
}
#endif
