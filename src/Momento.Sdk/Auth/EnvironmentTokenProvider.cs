namespace Momento.Sdk.Auth;

using System;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;

public class EnvironmentTokenProvider : ICredentialProvider
{

    private string _authToken;
    private string _controlEndpoint;
    private string _cacheEndpoint;
    public string AuthToken { get => _authToken; }
    public string ControlEndpoint {get => _controlEndpoint; }
    public string CacheEndpoint { get => _cacheEndpoint; }

    /// <summary>
    /// Reads and parses a JWT token stored as an environment variable.
    /// <param name="name">Name of the environment variable that contains the JWT token.</param>
    /// </summary>
    public EnvironmentTokenProvider(string name)
    {
        _authToken =  Environment.GetEnvironmentVariable(name);
        if (String.IsNullOrEmpty(AuthToken))
        {
            throw new InvalidArgumentException($"Environment variable '{name}' is empty or null.");
        }

        Claims claims;
        try {
            claims = JwtUtils.DecodeJwt(_authToken);
        } catch (InvalidArgumentException) {
            throw new InvalidArgumentException("The supplied Momento authToken is not valid.");
        }
        _controlEndpoint = claims.ControlEndpoint;
        _cacheEndpoint = claims.CacheEndpoint;
    }
}