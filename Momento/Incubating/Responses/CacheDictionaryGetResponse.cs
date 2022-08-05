using CacheClient;
using Google.Protobuf;
using MomentoSdk.Responses;


namespace MomentoSdk.Incubating.Responses;

public class CacheDictionaryGetResponse
{
    public CacheGetStatus Status { get; private set; }
    private readonly ByteString? cacheBody;

    public CacheDictionaryGetResponse(_DictionaryGetResponse.Types._DictionaryGetResponsePart response)
    {
        this.Status = CacheGetStatusUtil.From(response.Result);
        this.cacheBody = (Status == CacheGetStatus.HIT) ? response.CacheBody : null;
    }

    public byte[]? Bytes
    {
        get => (cacheBody != null) ? cacheBody.ToByteArray() : null;
    }

    public string? String() => (cacheBody != null) ? cacheBody.ToStringUtf8() : null;
}
