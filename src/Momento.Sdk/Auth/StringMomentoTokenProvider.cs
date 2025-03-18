namespace Momento.Sdk.Auth;

using System;
using Momento.Sdk.Exceptions;

/// <summary>
/// Reads and parses a JWT token stored as a string.
/// </summary>
public class StringMomentoTokenProvider : ICredentialProvider
{
    // For V1 tokens, the original token is necessary to reconstruct
    // the provider in WithCacheEndpoint.
    private readonly string origAuthToken;
    /// <inheritdoc />
    public string AuthToken { get; private set; }
    /// <inheritdoc />
    public string ControlEndpoint { get; private set; }
    /// <inheritdoc />
    public string CacheEndpoint { get; private set; }
    /// <inheritdoc />
    public string TokenEndpoint { get; private set; }
    /// <inheritdoc />
    public bool SecureEndpoints { get; private set; } = true;

    /// <summary>
    /// Reads and parses a JWT token from a string.
    /// </summary>
    /// <param name="token">The JWT token.</param>
    public StringMomentoTokenProvider(string token)
    {
        origAuthToken = token;
        AuthToken = token;
        if (String.IsNullOrEmpty(AuthToken))
        {
            throw new InvalidArgumentException($"String '{token}' is empty or null.");
        }

        var tokenData = AuthUtils.TryDecodeAuthToken(AuthToken);
        ControlEndpoint = tokenData.ControlEndpoint;
        CacheEndpoint = tokenData.CacheEndpoint;
        TokenEndpoint = tokenData.TokenEndpoint;
        AuthToken = tokenData.AuthToken;
    }

    /// <inheritdoc />
    public ICredentialProvider WithCacheEndpoint(string cacheEndpoint)
    {
        var updated = new StringMomentoTokenProvider(this.origAuthToken);
        updated.CacheEndpoint = cacheEndpoint;
        return updated;
    }
}
