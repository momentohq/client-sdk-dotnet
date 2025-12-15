namespace Momento.Sdk.Auth;

using Momento.Sdk.Exceptions;
using System;

/// <summary>
/// Reads a v2 api key and Momento service endpoint stored as strings.
/// </summary>
public class ApiKeyV2TokenProvider : ICredentialProvider
{
    // For v2 api keys, the original endpoint is necessary to reconstruct
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
    /// Constructs a ApiKeyV2TokenProvider from a v2 api key and endpoint.
    /// </summary>
    /// <param name="token">The v2 api key.</param>
    /// <param name="endpoint">The Momento service endpoint.</param>
    public ApiKeyV2TokenProvider(string token, string endpoint)
    {
        if (String.IsNullOrEmpty(endpoint))
        {
            throw new InvalidArgumentException($"Endpoint is empty or null.");
        }
        if (String.IsNullOrEmpty(token))
        {
            throw new InvalidArgumentException($"Auth token is empty or null.");
        }
        if (!AuthUtils.IsV2ApiKey(token))
        {
            throw new InvalidArgumentException("Received an invalid v2 API key. Are you using the correct key? Or did you mean to use `StringMomentoTokenProvider()` with a legacy key instead?");
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
        var updated = new ApiKeyV2TokenProvider(this.AuthToken, this.origEndpoint);
        updated.CacheEndpoint = cacheEndpoint;
        return updated;
    }
}
