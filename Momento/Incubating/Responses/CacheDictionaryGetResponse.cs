using Google.Protobuf;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Responses;

namespace Momento.Sdk.Incubating.Responses;

public class CacheDictionaryGetResponse : CacheGetResponseBase
{
    public CacheDictionaryGetResponse() : base(CacheGetStatus.MISS, null)
    {
    }

    public CacheDictionaryGetResponse(ECacheResult status, ByteString value) : base(status, value)
    {
    }

    public CacheDictionaryGetResponse(_DictionaryGetResponse.Types._DictionaryGetResponsePart unaryResponse)
        : base(unaryResponse.Result, unaryResponse.CacheBody)
    {
    }

    public static CacheDictionaryGetResponse From(_DictionaryGetResponse response)
    {
        if (response.DictionaryCase == _DictionaryGetResponse.DictionaryOneofCase.Missing)
        {
            return new CacheDictionaryGetResponse();
        }

        if (response.Found.Items.Count == 0)
        {
            // TODO there are no exception types that cleanly map to this kind of error
            throw new ClientSdkException("_DictionaryGetResponseResponse contained no data but was found");
        }
        return new CacheDictionaryGetResponse(response.Found.Items[0].Result, response.Found.Items[0].CacheBody);
    }
}
