using System.Collections.Generic;
using System.Linq;

namespace MomentoSdk.Responses;

public class CacheGetBatchResponse
{
    public List<CacheGetResponse> Responses { get; }

    public CacheGetBatchResponse(IEnumerable<CacheGetResponse> responses)
    {
        this.Responses = new(responses);
    }

    public IEnumerable<CacheGetStatus> Status
    {
        get => Responses.Select(response => response.Status);
    }

    public IEnumerable<string?> Strings()
    {
        return Responses.Select(response => response.String());
    }

    public IEnumerable<byte[]?> Bytes
    {
        get => Responses.Select(response => response.Bytes);
    }
}
