using Google.Protobuf;
using Momento.Protos.CacheClient;
using Momento.Sdk.Responses;

namespace Momento.Sdk.Incubating.Responses;

public class CacheListPopFrontResponse
{
    public CacheGetStatus Status { get; private set; }
    private readonly ByteString? value;

    public CacheListPopFrontResponse(_ListPopFrontResponse response)
    {
        if (response.ListCase == _ListPopFrontResponse.ListOneofCase.Found)
        {
            Status = CacheGetStatus.HIT;
            value = response.Found.Front;
        }
        else
        {
            Status = CacheGetStatus.MISS;
            value = null;
        }
    }

    public byte[]? ByteArray
    {
        get => (value != null) ? value.ToByteArray() : null;
    }

    public string? String() => (value != null) ? value.ToStringUtf8() : null;
}
