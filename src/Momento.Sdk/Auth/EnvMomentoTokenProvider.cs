namespace Momento.Sdk.Auth;

using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;
using System;

/// <summary>
/// Reads and parses a JWT token stored as an environment variable.
/// </summary>
[Obsolete("EnvMomentoTokenProvider is deprecated, please use EnvMomentoV2TokenProvider instead.")]
public class EnvMomentoTokenProvider : ICredentialProvider
{
    private readonly string envVarName;

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
    /// Reads and parses a JWT token stored as an environment variable.
    /// </summary>
    /// <param name="name">Name of the environment variable that contains the JWT token.</param>
    [Obsolete("EnvMomentoTokenProvider is deprecated, please use EnvMomentoV2TokenProvider instead.")]
    public EnvMomentoTokenProvider(string name)
    {
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

        var tokenData = AuthUtils.TryDecodeAuthToken(AuthToken);
        ControlEndpoint = tokenData.ControlEndpoint;
        CacheEndpoint = tokenData.CacheEndpoint;
        TokenEndpoint = tokenData.TokenEndpoint;
        AuthToken = tokenData.AuthToken;
    }

    /// <inheritdoc />
    public ICredentialProvider WithCacheEndpoint(string cacheEndpoint)
    {
        var updated = new EnvMomentoTokenProvider(this.envVarName);
        updated.CacheEndpoint = cacheEndpoint;
        return updated;
    }
}
