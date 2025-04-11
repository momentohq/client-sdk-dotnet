using Momento.Sdk.Auth.AccessControl;
using Momento.Sdk.Responses;
using System;
using System.Threading.Tasks;

namespace Momento.Sdk;

/// <summary>
/// The auth client used to generate disposable tokens.
/// </summary>
public interface IAuthClient : IDisposable
{
    /// <summary>
    /// Generate a disposable token.
    /// </summary>
    /// <param name="scope">The scope of the token.</param>
    /// <param name="expiresIn">How long the token should be valid for.</param>
    /// <param name="tokenId">The ID of the token.</param>
    /// <returns>A <see cref="GenerateDisposableTokenResponse"/> object.</returns>
    public Task<GenerateDisposableTokenResponse> GenerateDisposableTokenAsync(DisposableTokenScope scope,
        ExpiresIn expiresIn, string? tokenId = null);
}
