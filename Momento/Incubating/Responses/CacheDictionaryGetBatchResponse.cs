using System.Collections.Generic;
using System.Linq;
using CacheClient;
using MomentoSdk.Responses;


namespace MomentoSdk.Incubating.Responses;

public class CacheDictionaryGetBatchResponse
{
    public IEnumerable<CacheDictionaryGetResponse> Responses { get; private set; }

    public CacheDictionaryGetBatchResponse(_DictionaryGetResponse response, int numRequested)
    {
        if (response.DictionaryCase == _DictionaryGetResponse.DictionaryOneofCase.Found)
        {
            Responses = response.Found.Items.Select(item => new CacheDictionaryGetResponse(item));
        }
        else
        {
            Responses = Enumerable.Range(1, numRequested).Select(_ => new CacheDictionaryGetResponse());
        }

    }

    public IEnumerable<CacheGetStatus> Status
    {
        get => Responses.Select(response => response.Status);
    }

    public IEnumerable<string?> Strings()
    {
        return Responses.Select(response => response.String());
    }

    public IEnumerable<byte[]?> ByteArrays
    {
        get => Responses.Select(response => response.ByteArray);
    }
}
