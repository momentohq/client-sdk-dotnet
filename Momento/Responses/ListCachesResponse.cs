using System.Collections.Generic;
using Momento.Protos.ControlClient;

namespace Momento.Sdk.Responses;

public class ListCachesResponse
{
    public List<CacheInfo> Caches { get; }
    public string? NextPageToken { get; }

    public ListCachesResponse(_ListCachesResponse result)
    {
        NextPageToken = result.NextToken == "" ? null : result.NextToken;
        Caches = new List<CacheInfo>();
        foreach (_Cache c in result.Cache)
        {
            Caches.Add(new CacheInfo(c.CacheName));
        }
    }
}
