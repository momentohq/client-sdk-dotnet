using System.Text;
using CacheClient;
using MomentoSdk.Responses;


namespace MomentoSdk.Incubating.Responses;

public class CacheDictionaryGetResponse
{
    public CacheGetStatus Status { get; private set; }
    public byte[]? Bytes { get; private set; }

    public CacheDictionaryGetResponse(_DictionaryGetResponse.Types._DictionaryGetResponsePart response)
    {
        this.Status = CacheGetStatusUtil.From(response.Result);
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
}
