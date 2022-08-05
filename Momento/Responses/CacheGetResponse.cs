using CacheClient;
using Google.Protobuf;

namespace MomentoSdk.Responses;

public class CacheGetResponse
{
    public CacheGetStatus Status { get; }
    private readonly ByteString? cacheBody;

    public CacheGetResponse(_GetResponse response)
    {
        Status = CacheGetStatusUtil.From(response.Result);

        cacheBody = (Status == CacheGetStatus.HIT) ? response.CacheBody : null;
    }

    public byte[]? Bytes
    {
        get => cacheBody != null ? cacheBody.ToByteArray() : null;
    }

    public string? String() => (cacheBody != null) ? cacheBody.ToStringUtf8() : null;
}
