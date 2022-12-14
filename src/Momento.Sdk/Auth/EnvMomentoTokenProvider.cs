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

    /// <summary>
    /// Reads and parses a JWT token stored as an environment variable.
    /// </summary>
    /// <param name="name">Name of the environment variable that contains the JWT token.</param>
    public EnvMomentoTokenProvider(string name)
    {
        this.envVarName = name;
        
        AuthToken = Environment.GetEnvironmentVariable(name);
        if (String.IsNullOrEmpty(AuthToken))
        {
            throw new InvalidArgumentException($"Environment variable '{name}' is empty or null.");
        }

        var claims = AuthUtils.TryDecodeAuthToken(AuthToken);
        ControlEndpoint = claims.ControlEndpoint;
        CacheEndpoint = claims.CacheEndpoint;
    }

    public ICredentialProvider WithCacheEndpoint(string cacheEndpoint)
    {
        var updated = new EnvMomentoTokenProvider(this.envVarName);
        updated.CacheEndpoint = cacheEndpoint;
        return updated;
    }
}
