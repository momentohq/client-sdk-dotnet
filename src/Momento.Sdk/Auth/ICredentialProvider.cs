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
    /// Control endpoint extracted from JWT
    /// </summary>
    string ControlEndpoint { get; }
    /// <summary>
    /// Cache endpoint extracted from JWT
    /// </summary>
    string CacheEndpoint { get; }
}
