namespace Momento.Sdk.Auth;

using System.Collections.Generic;

public interface ICredentialProvider
{
    public string AuthToken { get; }
    public string ControlEndpoint {get; }
    public string CacheEndpoint { get; }
}
