namespace Momento.Sdk.Auth;

using System;
using Momento.Sdk.Exceptions;

/// <summary>
/// Sets the endpoint and port for connecting to momento-local.
/// </summary>
public class MomentoLocalProvider : ICredentialProvider 
{
    /// <summary>
    /// The port for connecting to momento-local.
    /// </summary>
    public int Port { get; private set; }
    /// <inheritdoc />
    public string AuthToken { get; private set; }
    /// <inheritdoc />
    public string ControlEndpoint { get; private set; }
    /// <inheritdoc />
    public string CacheEndpoint { get; private set; }
    /// <inheritdoc />
    public string TokenEndpoint { get; private set; }
    /// <inheritdoc />
    public bool SecureEndpoints { get; private set; } = false;

    /// <summary>
    /// Default constructor for MomentoLocalProvider. 
    /// Uses localhost and port 8080 to connect to momento-local.
    /// </summary>
    public MomentoLocalProvider()
    {
        AuthToken = "";
        Port = 8080;
        string endpoint = $"127.0.0.1:{Port}";
        ControlEndpoint = endpoint;
        CacheEndpoint = endpoint;
        TokenEndpoint = endpoint;
    }

    /// <summary>
    /// Constructor for MomentoLocalProvider.
    /// Uses the provided hostname and default port (8080) to connect to momento-local.
    /// </summary>
    public MomentoLocalProvider(string hostname)
    {
        AuthToken = "";
        Port = 8080;
        string endpoint = $"{hostname}:{Port}";
        ControlEndpoint = endpoint;
        CacheEndpoint = endpoint;
        TokenEndpoint = endpoint;
        
    }

    /// <summary>
    /// Constructor for MomentoLocalProvider.
    /// Uses the default hostname (localhost) and provided port to connect to momento-local.
    /// </summary>
    public MomentoLocalProvider(int port)
    {
        AuthToken = "";
        Port = port;
        string endpoint = $"127.0.0.1:{Port}";
        ControlEndpoint = endpoint;
        CacheEndpoint = endpoint;
        TokenEndpoint = endpoint;
        
    }

    /// <summary>
    /// Constructor for MomentoLocalProvider.
    /// Uses the provided hostname and provided port to connect to momento-local.
    /// </summary>
    public MomentoLocalProvider(string hostname, int port)
    {
        AuthToken = "";
        Port = port;
        string endpoint = $"{hostname}:{Port}";
        ControlEndpoint = endpoint;
        CacheEndpoint = endpoint;
        TokenEndpoint = endpoint;
    }

    /// <inheritdoc />
    public ICredentialProvider WithCacheEndpoint(string cacheEndpoint)
    {
        return new MomentoLocalProvider(cacheEndpoint, this.Port);
    }
}