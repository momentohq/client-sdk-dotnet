using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

public enum CacheGetStatus
{
    HIT,
    MISS,
}

public static class CacheGetStatusUtil
{
    public static CacheGetStatus From(ECacheResult result)
    {
        switch (result)
        {
            case ECacheResult.Hit:
                return CacheGetStatus.HIT;
            case ECacheResult.Miss:
                return CacheGetStatus.MISS;
            default:
                throw new InternalServerException("Invalid Cache Status.");
        }
    }
}