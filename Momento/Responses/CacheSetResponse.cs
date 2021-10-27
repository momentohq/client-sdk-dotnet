using System;
using CacheClient;
namespace MomentoSdk.Responses
{
    public class CacheSetResponse : BaseCacheResponse
    {
        public MomentoCacheResult result { get; private set; }

        public CacheSetResponse(SetResponse response)
        {
            result = this.ResultMapper(response.Result);
        }
    }
}
