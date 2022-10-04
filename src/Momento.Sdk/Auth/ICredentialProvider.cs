namespace Momento.Sdk.Auth;

using System.Collections.Generic;

public interface ICredentialProvider
{
    string AuthToken { get; }
    string ControlEndpoint {get; }
    string CacheEndpoint { get; }
}
