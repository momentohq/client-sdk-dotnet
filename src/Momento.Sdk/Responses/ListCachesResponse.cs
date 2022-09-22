using System.Collections.Generic;
using Momento.Protos.ControlClient;
using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

public class ListCachesResponse
{
    public class Success : ListCachesResponse
    {
        public List<CacheInfo> Caches { get; }
        public string? NextPageToken { get; }

        public Success(_ListCachesResponse result)
        {
            NextPageToken = result.NextToken == "" ? null : result.NextToken;
            Caches = new List<CacheInfo>();
            foreach (_Cache c in result.Cache)
            {
                Caches.Add(new CacheInfo(c.CacheName));
            }
        }
    }

    public class Error : ListCachesResponse
    {
        private readonly SdkException error;
        public Error(SdkException e)
        {
            error = e;
        }

        public SdkException Exception
        {
            get => error;
        }

    }
}
