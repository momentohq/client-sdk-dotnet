using Google.Protobuf;
using Momento.Protos.CacheClient;
using MomentoSdk.Exceptions;
using MomentoSdk.Responses;

namespace MomentoSdk.Incubating.Responses;

public class CacheDictionaryGetResponse
{
    public CacheGetStatus Status { get; private set; }
    private readonly ByteString? value;

    public CacheDictionaryGetResponse()
    {
        Status = CacheGetStatus.MISS;
        value = null;
    }

    public CacheDictionaryGetResponse(_DictionaryGetResponse.Types._DictionaryGetResponsePart unaryResponse)
    {
        Status = CacheGetStatusUtil.From(unaryResponse.Result);
        value = (Status == CacheGetStatus.HIT) ? unaryResponse.CacheBody : null;
    }

    public CacheDictionaryGetResponse(_DictionaryGetResponse response)
    {
        if (response.DictionaryCase == _DictionaryGetResponse.DictionaryOneofCase.Found)
        {
            if (response.Found.Items.Count == 0)
            {
                // TODO there are no exception types that cleanly map to this kind of error
                throw new ClientSdkException("_DictionaryGetResponseResponse contained no data but was found");
            }
            Status = CacheGetStatusUtil.From(response.Found.Items[0].Result);
            value = response.Found.Items[0].CacheBody;
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
