namespace Momento.Sdk.Auth;

using System;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;

/// <summary>
/// Reads and parses a JWT token stored as an environment variable.
/// </summary>
public class EnvMomentoTokenProvider : ICredentialProvider
{
    private readonly string envVarName;

    /// <inheritdoc />
    public string AuthToken { get; private set; }
    /// <inheritdoc />
    public string ControlEndpoint { get; private set; }
    /// <inheritdoc />
    public string CacheEndpoint { get; private set; }

    #if !BUILD_FOR_UNITY
    /// <inheritdoc />
    public string TokenEndpoint { get; private set; }
    #endif

    /// <summary>
    /// Reads and parses a JWT token stored as an environment variable.
    /// </summary>
    /// <param name="name">Name of the environment variable that contains the JWT token.</param>
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
        #if !BUILD_FOR_UNITY
        TokenEndpoint = tokenData.TokenEndpoint;
        #endif
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
