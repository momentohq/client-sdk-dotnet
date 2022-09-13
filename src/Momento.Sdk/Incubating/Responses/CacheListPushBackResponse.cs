using Momento.Protos.CacheClient;

namespace Momento.Sdk.Incubating.Responses;

/// <summary>
/// The result of a <c>ListPushBack</c> command
/// </summary>
public class CacheListPushBackResponse
{
    /// <summary>
    /// The length of the list post-push (and post-truncate, if that applies)
    /// </summary>
    public int ListLength { get; private set; }

    public CacheListPushBackResponse(_ListPushBackResponse response)
    {
        ListLength = checked((int)response.ListLength);
    }
}
