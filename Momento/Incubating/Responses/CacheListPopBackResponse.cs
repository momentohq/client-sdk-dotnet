using Google.Protobuf;
using Momento.Protos.CacheClient;
using Momento.Sdk.Responses;

namespace Momento.Sdk.Incubating.Responses;

public class CacheListPopBackResponse
{
    public CacheGetStatus Status { get; private set; }
    private readonly ByteString? value;

    public CacheListPopBackResponse(_ListPopBackResponse response)
    {
        if (response.ListCase == _ListPopBackResponse.ListOneofCase.Found)
        {
            Status = CacheGetStatus.HIT;
            value = response.Found.Back;
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
