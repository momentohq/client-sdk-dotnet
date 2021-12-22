using System;
namespace MomentoSdk.Responses
{
    public class ListCacheResponse
    {
        private readonly ControlClient.ListCachesResponse result;
        private readonly string[] caches;
        private readonly string nextPageToken;

        public ListCacheResponse(ControlClient.ListCachesResponse result)
        {
            this.nextPageToken = result.NextToken;
            this.caches = new string[0];
            int counter = 0;
            foreach (ControlClient.Cache c in result.Cache)
            {
                this.caches[counter] = c.CacheName;
                counter++;
            }
        }

        public string[] Caches()
        {
            return this.caches;
        }

        public string NextPageToken()
        {
            return this.nextPageToken;
        }
    }
}