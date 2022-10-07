namespace Momento.Sdk.Auth;

using System.Collections.Generic;

/// <summary>
/// Specifies the fields that are required for a Momento client to connect to and authenticate with the Momento service.
/// </summary>
public interface ICredentialProvider
{
    /// <summary>
    /// JWT provided by user
    /// </summary>
    string AuthToken { get; }
    /// <summary>
    /// The host which the Momento client will connect to the Momento control plane
    /// </summary>
    string ControlEndpoint { get; }
    /// <summary>
    /// The host which the Momento client will connect to the Momento data plane
    /// </summary>
    string CacheEndpoint { get; }
}
