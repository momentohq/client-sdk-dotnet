using Google.Protobuf;
using Momento.Protos.CacheClient;
using Momento.Sdk.Responses;

namespace Momento.Sdk.Incubating.Responses;

public class CacheListPopFrontResponse : CacheGetResponseBase
{
    public CacheListPopFrontResponse(CacheGetStatus status, ByteString? value) : base(status, value)
    {
    }

    public static CacheListPopFrontResponse From(_ListPopFrontResponse response)
    {
        if (response.ListCase == _ListPopFrontResponse.ListOneofCase.Missing)
        {
            return new CacheListPopFrontResponse(CacheGetStatus.MISS, null);
        }
        return new CacheListPopFrontResponse(CacheGetStatus.HIT, response.Found.Front);
    }
}
