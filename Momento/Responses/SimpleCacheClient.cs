using System;
using System.Threading.Tasks;

namespace MomentoSdk.Responses
{
    public class SimpleCacheClient : IDisposable
    {
        private readonly ScsControlClient controlClient;
        private readonly ScsDataClient dataClient;
        private bool disposedValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken">Momento jwt</param>
        /// <param name="defaultTtlSeconds"></param>
        public SimpleCacheClient(string authToken, uint defaultTtlSecond)
        {
            Claims claims = JwtUtils.DecodeJwt(authToken);
            string controlEndpoint = "https://" + claims.ControlEndpoint + ":443";
            string cacheEndpoint = "https://" + claims.CacheEndpoint + ":443";
            ScsControlClient controlClient = new ScsControlClient(authToken, controlEndpoint);
            ScsDataClient dataClient = new ScsDataClient(authToken, cacheEndpoint, defaultTtlSecond);
            this.controlClient = controlClient;
            this.dataClient = dataClient;
        }

        /// <summary>
        /// Creates a cache if it doesnt exist. Returns the cache.
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public Responses.CreateCacheResponse CreateCache(string cacheName)
        {
            return this.controlClient.CreateCache(cacheName);
        }

        /// <summary>
        /// Deletes a cache and all of the items within it
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public Responses.DeleteCacheResponse DeleteCache(string cacheName)
        {
            return this.controlClient.DeleteCache(cacheName);
        }

        /// <summary>
        /// List all caches
        /// </summary>
        /// <param name="nextPageToken"></param>
        /// <returns></returns>
        public Responses.ListCachesResponse ListCaches(string nextPageToken = null)
        {
            return this.controlClient.ListCaches(nextPageToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttlSeconds"></param>
        /// <returns></returns>
        public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value, uint ttlSeconds)
        {
            return await this.dataClient.SetAsync(cacheName, key, value, ttlSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value)
        {
            return await this.dataClient.SetAsync(cacheName, key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">The key to perform a cache lookup on</param>
        /// <returns></returns>
        public async Task<CacheGetResponse> GetAsync(string cacheName, byte[] key)
        {
            return await this.dataClient.GetAsync(cacheName, key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttlSeconds"></param>
        /// <returns></returns>
        public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, uint ttlSeconds)
        {
            return await this.dataClient.SetAsync(cacheName, key, value, ttlSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value)
        {
            return await this.dataClient.SetAsync(cacheName, key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<CacheGetResponse> GetAsync(string cacheName, string key)
        {
            return await this.dataClient.GetAsync(cacheName, key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttlSeconds"></param>
        /// <returns></returns>
        public CacheSetResponse Set(string cacheName, byte[] key, byte[] value, uint ttlSeconds)
        {
            return this.dataClient.Set(cacheName, key, key, ttlSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public CacheSetResponse Set(string cacheName, byte[] key, byte[] value)
        {
            return this.dataClient.Set(cacheName, key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CacheGetResponse Get(string cacheName, byte[] key)
        {
            return this.dataClient.Get(cacheName, key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttlSeconds"></param>
        /// <returns></returns>
        public CacheSetResponse Set(string cacheName, string key, string value, uint ttlSeconds)
        {
            return this.dataClient.Set(cacheName, key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttlSeconds"></param>
        /// <returns></returns>
        public CacheSetResponse Set(string cacheName, string key, string value)
        {
            return this.dataClient.Set(cacheName, key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CacheGetResponse Get(string cacheName, string key)
        {
            return this.dataClient.Get(cacheName, key);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.controlClient.Dispose();
                    this.dataClient.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
