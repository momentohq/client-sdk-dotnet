using System.Collections.Generic;

namespace MomentoSdk.Responses;

public class ListCachesResponse
{
    public List<CacheInfo> Caches { get; }
    public string? NextPageToken { get; }

    public ListCachesResponse(ControlClient._ListCachesResponse result)
    {
        NextPageToken = result.NextToken == "" ? null : result.NextToken;
        Caches = new List<CacheInfo>();
        foreach (ControlClient._Cache c in result.Cache)
        {
            Caches.Add(new CacheInfo(c.CacheName));
        }
    }
}
