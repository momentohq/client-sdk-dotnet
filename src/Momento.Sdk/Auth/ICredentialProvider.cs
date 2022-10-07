namespace Momento.Sdk.Auth;

using System.Collections.Generic;

/// <summary>
/// Interface for credentials
/// </summary>
public interface ICredentialProvider
{
    /// <summary>
    /// JWT provided by user
    /// </summary>
    string AuthToken { get; }
    /// <summary>
    /// Control endpoint
    /// </summary>
    string ControlEndpoint { get; }
    /// <summary>
    /// Cache endpoint
    /// </summary>
    string CacheEndpoint { get; }
}
