namespace Momento.Sdk.Auth;

using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;
using System;

/// <summary>
/// Reads v2 api key and Momento service endpoint stored as environment variables.
/// </summary>
public class EnvVarV2TokenProvider : ICredentialProvider
{
    // The original environment variable names are required to reconstruct
    // the provider when using WithCacheEndpoint.
    private readonly string endpointEnvVarName;
    private readonly string apiKeyEnvVarName;

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
    /// Constructs a EnvVarV2TokenProvider from a Momento service endpoint and a v2 api key stored as environment variables.
    /// </summary>
    /// <param name="apiKeyEnvVar">Name of the environment variable that contains the v2 api key.</param>
    /// <param name="endpointEnvVar">Name of the environment variable that contains the Momento service endpoint.</param>
    public EnvVarV2TokenProvider(string apiKeyEnvVar, string endpointEnvVar)
    {
        if (String.IsNullOrEmpty(endpointEnvVar))
        {
            throw new InvalidArgumentException($"Endpoint environment variable name is empty or null.");
        }
        var endpoint = Environment.GetEnvironmentVariable(endpointEnvVar);
        if (String.IsNullOrEmpty(endpoint))
        {
            throw new InvalidArgumentException($"Endpoint is empty or null.");
        }
        this.endpointEnvVarName = endpointEnvVar;

        if (String.IsNullOrEmpty(apiKeyEnvVar))
        {
            throw new InvalidArgumentException($"API key environment variable name is empty or null.");
        }
        var apiKey = Environment.GetEnvironmentVariable(apiKeyEnvVar);
        if (String.IsNullOrEmpty(apiKey))
        {
            throw new InvalidArgumentException($"Environment variable '{apiKeyEnvVar}' is empty or null.");
        }
        this.apiKeyEnvVarName = apiKeyEnvVar;

        AuthToken = apiKey;
        if (!AuthUtils.IsV2ApiKey(AuthToken))
        {
            throw new InvalidArgumentException("Received an invalid v2 API key. Are you using the correct key? Or did you mean to use `StringMomentoTokenProvider()` with a legacy key instead?");
        }

        ControlEndpoint = "control." + endpoint;
        CacheEndpoint = "cache." + endpoint;
        TokenEndpoint = "token." + endpoint;
    }

    /// <inheritdoc />
    public ICredentialProvider WithCacheEndpoint(string cacheEndpoint)
    {
        var updated = new EnvVarV2TokenProvider(this.apiKeyEnvVarName, this.endpointEnvVarName);
        updated.CacheEndpoint = cacheEndpoint;
        return updated;
    }
}
