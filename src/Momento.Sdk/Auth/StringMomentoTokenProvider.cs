namespace Momento.Sdk.Auth;

using System;
using Momento.Sdk.Exceptions;

/// <summary>
/// Reads and parses a JWT token stored as a string.
/// </summary>
public class StringMomentoTokenProvider : ICredentialProvider
{
    /// <inheritdoc />
    public string AuthToken { get; private set; }
    /// <inheritdoc />
    public string ControlEndpoint { get; private set; }
    /// <inheritdoc />
    public string CacheEndpoint { get; private set; }

    /// <summary>
    /// Reads and parses a JWT token from a string.
    /// </summary>
    /// <param name="token">The JWT token.</param>
    public StringMomentoTokenProvider(string token)
    {
        AuthToken = token;
        if (String.IsNullOrEmpty(AuthToken))
        {
            throw new InvalidArgumentException($"String '{token}' is empty or null.");
        }

        var claims = AuthUtils.TryDecodeAuthToken(AuthToken);
        ControlEndpoint = claims.ControlEndpoint;
        CacheEndpoint = claims.CacheEndpoint;
    }

    /// <inheritdoc />
    public ICredentialProvider WithCacheEndpoint(string cacheEndpoint)
    {
        var updated = new StringMomentoTokenProvider(this.AuthToken);
        updated.CacheEndpoint = cacheEndpoint;
        return updated;
    }
}
