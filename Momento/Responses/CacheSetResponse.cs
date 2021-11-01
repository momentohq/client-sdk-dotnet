using System;
using CacheClient;
namespace MomentoSdk.Responses
{
    public class CacheSetResponse : BaseCacheResponse
    {
        public MomentoCacheResult Result { get; private set; }

        public CacheSetResponse(SetResponse response)
        {
            Result = this.ResultMapper(response.Result);
        }
    }
}
