namespace Momento.Sdk.Auth;

using System;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;

public class EnvironmentTokenProvider : ICredentialProvider
{
    public string AuthToken { get; private set; }
    public string ControlEndpoint {get; private set; }
    public string CacheEndpoint { get; private set; }

    /// <summary>
    /// Reads and parses a JWT token stored as an environment variable.
    /// <param name="name">Name of the environment variable that contains the JWT token.</param>
    /// </summary>
    public EnvironmentTokenProvider(string name)
    {
        AuthToken =  Environment.GetEnvironmentVariable(name);
        if (String.IsNullOrEmpty(AuthToken))
        {
            throw new InvalidArgumentException($"Environment variable '{name}' is empty or null.");
        }

        Claims claims;
        try {
            claims = JwtUtils.DecodeJwt(AuthToken);
        } catch (InvalidArgumentException) {
            throw new InvalidArgumentException("The supplied Momento authToken is not valid.");
        }
        ControlEndpoint = claims.ControlEndpoint;
        CacheEndpoint = claims.CacheEndpoint;
    }
}
