using Google.Protobuf;
using Momento.Protos.CacheClient;
using Momento.Sdk.Responses;

namespace Momento.Sdk.Incubating.Responses;

public class CacheListPopBackResponse : CacheGetResponseBase
{
    public CacheListPopBackResponse(CacheGetStatus status, ByteString? value) : base(status, value)
    {
    }

    public static CacheListPopBackResponse From_ListPopBackResponse(_ListPopBackResponse response)
    {
        if (response.ListCase == _ListPopBackResponse.ListOneofCase.Missing)
        {
            return new CacheListPopBackResponse(CacheGetStatus.MISS, null);
        }
        return new CacheListPopBackResponse(CacheGetStatus.HIT, response.Found.Back);
    }
}
