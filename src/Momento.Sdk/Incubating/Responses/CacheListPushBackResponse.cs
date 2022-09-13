using Momento.Protos.CacheClient;

namespace Momento.Sdk.Incubating.Responses;

public class CacheListPushBackResponse
{
    public int ListLength { get; private set; }

    public CacheListPushBackResponse(_ListPushBackResponse response)
    {
        ListLength = checked((int)response.ListLength);
    }
}
