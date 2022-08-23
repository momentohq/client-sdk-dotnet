using Google.Protobuf;
using Momento.Protos.CacheClient;

namespace Momento.Sdk.Responses;

public class CacheGetResponse : CacheGetResponseBase
{
    public CacheGetResponse(_GetResponse response) : base(response.Result, response.CacheBody)
    {
    }
}
