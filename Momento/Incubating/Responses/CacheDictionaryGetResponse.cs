using System;
using System.Text;
using CacheClient;
using MomentoSdk.Exceptions;
using MomentoSdk.Responses;


namespace MomentoSdk.Incubating.Responses;

public class CacheDictionaryGetResponse
{
    public CacheGetStatus Status { get; private set; }
    public byte[]? Bytes { get; private set; }

    public CacheDictionaryGetResponse(_DictionaryGetResponse.Types._DictionaryGetResponsePart response)
    {
        this.Status = From(response.Result);
        this.Bytes = (Status == CacheGetStatus.HIT) ? response.CacheBody.ToByteArray() : null;
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
