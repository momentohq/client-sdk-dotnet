using System.Collections.Generic;
namespace MomentoSdk.Responses
{
    public class ListCachesResponse
    {
        private readonly List<CacheInfo> caches;
        private readonly string nextPageToken;

        public ListCachesResponse(ControlClient.ListCachesResponse result)
        {
            this.nextPageToken = result.NextToken;
            this.caches = new List<CacheInfo>(new CacheInfo[result.Cache.Count]);
            int counter = 0;
            foreach (ControlClient.Cache c in result.Cache)
            {
                this.caches[counter] = new CacheInfo(c.CacheName);
                counter++;
            }
        }

        public List<CacheInfo> Caches()
        {
            return this.caches;
        }

        public string NextPageToken()
        {
            return this.nextPageToken;
        }
    }
}