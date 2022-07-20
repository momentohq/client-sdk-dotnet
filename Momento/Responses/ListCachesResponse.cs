using System.Collections.Generic;

namespace MomentoSdk.Responses;

public class ListCachesResponse
{
    private readonly List<CacheInfo> caches;
    private readonly string? nextPageToken;

    public ListCachesResponse(ControlClient._ListCachesResponse result)
    {
        nextPageToken = result.NextToken == "" ? null : result.NextToken;
        caches = new List<CacheInfo>();
        foreach (ControlClient._Cache c in result.Cache)
        {
            caches.Add(new CacheInfo(c.CacheName));
        }
    }

    public List<CacheInfo> Caches()
    {
        return caches;
    }

    public string? NextPageToken()
    {
        return nextPageToken;
    }
}
