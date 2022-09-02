using Momento.Protos.CacheClient;

namespace Momento.Sdk.Incubating.Responses;

public class CacheListLengthResponse
{
    public int Length { get; private set; } = 0;

    public CacheListLengthResponse(_ListLengthResponse response)
    {
        if (response.ListCase == _ListLengthResponse.ListOneofCase.Found)
        {
            Length = checked((int)response.Found.Length);
        }
    }
}
