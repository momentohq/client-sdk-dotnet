using System.Text;
using CacheClient;
using Google.Protobuf;
using MomentoSdk.Exceptions;

namespace MomentoSdk.Responses;

public class CacheGetResponse
{
    public CacheGetStatus Status { get; }
    public byte[]? Bytes { get; }

    public CacheGetResponse(_GetResponse response)
    {
        Status = From(response.Result);
        Bytes = (Status == CacheGetStatus.HIT) ? response.CacheBody.ToByteArray() : null;
    }

    public string? String()
    {
        if (Bytes == null)
        {
            return null;
        }
        return Encoding.UTF8.GetString(Bytes);
    }

    private static CacheGetStatus From(ECacheResult result)
    {
        switch (result)
        {
            case ECacheResult.Hit: return CacheGetStatus.HIT;
            case ECacheResult.Miss: return CacheGetStatus.MISS;
            default: throw new InternalServerException("Invalid Cache Status.");
        }
    }

}
