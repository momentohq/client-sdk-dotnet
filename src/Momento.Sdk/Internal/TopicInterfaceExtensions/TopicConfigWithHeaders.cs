using System.Collections.Generic;

namespace Momento.Sdk.Internal;

/// <summary>
/// Interface for defining a topic configuration with headers.
/// This is currently used by the MomentoLocalTestTopicConfiguration class, which is an internal wrapper used for
/// testing topics retry logic against momento-local.
/// </summary>
public interface ITopicConfigWithHeaders
{
    /// <summary>
    /// List of headers to be sent with each publish or subscribe request.
    /// </summary>
    public IList<KeyValuePair<string, string>> Headers { get; }
}
