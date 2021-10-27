using System;
using CacheClient;
using Google.Protobuf;
namespace MomentoSdk.Responses
{
    public class CacheGetResponse : BaseCacheResponse
    {
        public MomentoCacheResult result { get; private set; }
        public ByteString body { get; private set; }
        public CacheGetResponse(GetResponse response)
        {
            body = response.CacheBody;
            result = this.ResultMapper(response.Result);
        }
    }
}
