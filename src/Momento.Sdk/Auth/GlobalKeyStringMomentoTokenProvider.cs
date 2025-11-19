namespace Momento.Sdk.Auth;

using Momento.Sdk.Exceptions;
using System;

/// <summary>
/// Reads and parses a global api key stored as a string.
/// </summary>
public class GlobalKeyStringMomentoTokenProvider : ICredentialProvider
{
    // For global api keys, the original endpoint is necessary to reconstruct
    // the provider in WithCacheEndpoint.
    private readonly string origEndpoint;
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
    /// Constructs a GlobalKeyStringMomentoTokenProvider from a global api key and endpoint.
    /// </summary>
    /// <param name="token">The global api key.</param>
    /// <param name="endpoint">The Momento service endpoint.</param>
    public GlobalKeyStringMomentoTokenProvider(string token, string endpoint)
    {
        if (String.IsNullOrEmpty(token))
        {
            throw new InvalidArgumentException($"Auth token is empty or null.");
        }
        if (String.IsNullOrEmpty(endpoint))
        {
            throw new InvalidArgumentException($"Endpoint is empty or null.");
        }

        AuthToken = token;
        ControlEndpoint = "control." + endpoint;
        CacheEndpoint = "cache." + endpoint;
        TokenEndpoint = "token." + endpoint;
        origEndpoint = endpoint;
    }

    /// <inheritdoc />
    public ICredentialProvider WithCacheEndpoint(string cacheEndpoint)
    {
        var updated = new GlobalKeyStringMomentoTokenProvider(this.AuthToken, this.origEndpoint);
        updated.CacheEndpoint = cacheEndpoint;
        return updated;
    }
}
