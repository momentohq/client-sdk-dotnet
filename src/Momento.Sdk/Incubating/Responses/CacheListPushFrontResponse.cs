using Momento.Protos.CacheClient;

namespace Momento.Sdk.Incubating.Responses;

/// <summary>
/// The result of a <c>ListPushFront</c> command
/// </summary>
public class CacheListPushFrontResponse
{
    /// <summary>
    /// The length of the list post-push (and post-truncate, if that applies)
    /// </summary>
    public int ListLength { get; private set; }

    public CacheListPushFrontResponse(_ListPushFrontResponse response)
    {
        ListLength = checked((int)response.ListLength);
    }
}
