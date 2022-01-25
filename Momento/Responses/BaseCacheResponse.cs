using System;
using CacheClient;
namespace MomentoSdk.Responses
{
    public class BaseCacheResponse
    {
        public BaseCacheResponse()
        {
        }

        protected MomentoCacheResult ResultMapper(ECacheResult result)
        {
            switch (result)
            {
                case ECacheResult.Hit: return MomentoCacheResult.HIT;
                case ECacheResult.Miss: return MomentoCacheResult.MISS;
                default: return MomentoCacheResult.UNKNOWN;
            }
        }
    }
}
