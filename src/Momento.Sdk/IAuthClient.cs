using System;
using System.Threading.Tasks;
using Momento.Sdk.Auth.AccessControl;
using Momento.Sdk.Responses;

namespace Momento.Sdk;

public interface IAuthClient
{
    public Task<GenerateDisposableTokenResponse> GenerateDisposableTokenAsync(DisposableTokenScope scope,
        ExpiresIn expiresIn);
}
