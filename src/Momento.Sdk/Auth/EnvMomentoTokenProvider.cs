namespace Momento.Sdk.Auth;

using System;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;

/// <summary>
/// Reads and parses a JWT token stored as an environment variable.
/// </summary>
public class EnvMomentoTokenProvider : ICredentialProvider
{
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
        AuthToken = Environment.GetEnvironmentVariable(name);
        if (String.IsNullOrEmpty(AuthToken))
        {
            throw new InvalidArgumentException($"Environment variable '{name}' is empty or null.");
        }

        Claims claims;
        try
        {
            claims = JwtUtils.DecodeJwt(AuthToken);
        }
        catch (InvalidArgumentException)
        {
            throw new InvalidArgumentException("The supplied Momento authToken is not valid.");
        }
        ControlEndpoint = claims.ControlEndpoint;
        CacheEndpoint = claims.CacheEndpoint;
    }
}
