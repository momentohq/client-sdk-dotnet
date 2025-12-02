namespace Momento.Sdk.Auth;

using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;
using System;

/// <summary>
/// Reads and parses a global api key stored as an environment variable.
/// </summary>
public class GlobalKeyEnvMomentoTokenProvider : ICredentialProvider
{
    private readonly string envVarName;
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
    /// Constructs a GlobalKeyEnvMomentoTokenProvider from an endpoint and a global api key stored as an environment variable.
    /// </summary>
    /// <param name="name">Name of the environment variable that contains the global api key.</param>
    /// <param name="endpoint">The Momento service endpoint.</param>
    public GlobalKeyEnvMomentoTokenProvider(string name, string endpoint)
    {
        if (String.IsNullOrEmpty(endpoint))
        {
            throw new InvalidArgumentException($"Endpoint is empty or null.");
        }
        this.origEndpoint = endpoint;

        this.envVarName = name;
        if (String.IsNullOrEmpty(name))
        {
            throw new InvalidArgumentException($"Environment variable '{name}' is empty or null.");
        }

        AuthToken = Environment.GetEnvironmentVariable(name);
        if (String.IsNullOrEmpty(AuthToken))
        {
            throw new InvalidArgumentException($"Environment variable '{name}' is empty or null.");
        }
        if (AuthUtils.IsBase64String(AuthToken))
        {
            throw new InvalidArgumentException("Did not expect global API key to be base64 encoded. Are you using the correct key? Or did you mean to use `StringMomentoTokenProvider()` instead?");
        }
        if (!AuthUtils.IsGlobalApiKey(AuthToken))
        {
            throw new InvalidArgumentException("Provided API key is not a global API key. Are you using the correct key? Or did you mean to use `StringMomentoTokenProvider()` instead?");
        }

        ControlEndpoint = "control." + endpoint;
        CacheEndpoint = "cache." + endpoint;
        TokenEndpoint = "token." + endpoint;
    }

    /// <inheritdoc />
    public ICredentialProvider WithCacheEndpoint(string cacheEndpoint)
    {
        var updated = new GlobalKeyEnvMomentoTokenProvider(this.envVarName, this.origEndpoint);
        updated.CacheEndpoint = cacheEndpoint;
        return updated;
    }
}
