using Momento.Protos.CacheClient;

namespace Momento.Sdk.Incubating.Responses;

public class CacheListPushFrontResponse
{
    public int ListLength { get; private set; }

    public CacheListPushFrontResponse(_ListPushFrontResponse response)
    {
        ListLength = checked((int)response.ListLength);
    }
}
