using System;
using System.Threading.Tasks;
using MomentoSdk.Responses;
using MomentoSdk.Exceptions;
using CacheClient;
using Google.Protobuf;
using Grpc.Core;

namespace MomentoSdk
{
    public class ScsDataClient : IDisposable
    {
        private readonly DataGrpcManager grpcManager;
        private readonly uint defaultTtlSeconds;
        private bool disposedValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken">Momento jwt</param>
        /// <param name="endpoint">Control client endpint</param>
        /// <param name="defaultTtlSeconds"></param>
        public ScsDataClient(string authToken, string endpoint, uint defaultTtlSeconds)
        {
            this.grpcManager = new DataGrpcManager(authToken, endpoint);
            this.defaultTtlSeconds = defaultTtlSeconds;
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
            SetResponse response = await this.SendSetAsync(cacheName, value: Convert(value), key: Convert(key), ttlSeconds: ttlSeconds);
            return new CacheSetResponse(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value)
        {
            return await this.SetAsync(cacheName, key, value, defaultTtlSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">The key to perform a cache lookup on</param>
        /// <returns></returns>
        public async Task<CacheGetResponse> GetAsync(string cacheName, byte[] key)
        {
            GetResponse resp = await this.SendGetAsync(cacheName, Convert(key));
            return new CacheGetResponse(resp);
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
            SetResponse response = await this.SendSetAsync(cacheName, key: Convert(key), value: Convert(value), ttlSeconds: ttlSeconds);
            return new CacheSetResponse(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value)
        {
            return await this.SetAsync(cacheName, key, value, defaultTtlSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<CacheGetResponse> GetAsync(string cacheName, string key)
        {
            GetResponse resp = await this.SendGetAsync(cacheName, Convert(key));
            return new CacheGetResponse(resp);
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
            SetResponse resp = this.SendSet(cacheName, key: Convert(key), value: Convert(value), ttlSeconds: ttlSeconds);
            return new CacheSetResponse(resp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public CacheSetResponse Set(string cacheName, byte[] key, byte[] value)
        {
            return this.Set(cacheName, key, value, defaultTtlSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CacheGetResponse Get(string cacheName, byte[] key)
        {
            GetResponse resp = this.SendGet(cacheName, Convert(key));
            return new CacheGetResponse(resp);
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
            SetResponse response = this.SendSet(cacheName, key: Convert(key), value: Convert(value), ttlSeconds: ttlSeconds);
            return new CacheSetResponse(response);
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
            return this.Set(cacheName, key, value, defaultTtlSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CacheGetResponse Get(string cacheName, string key)
        {
            GetResponse resp = this.SendGet(cacheName, Convert(key));
            return new CacheGetResponse(resp);
        }

        private async Task<SetResponse> SendSetAsync(string cacheName, ByteString key, ByteString value, uint ttlSeconds)
        {
            SetRequest request = new SetRequest() { CacheBody = value, CacheKey = key, TtlMilliseconds = ttlSeconds * 1000 };
            try
            {
                return await this.grpcManager.Client().SetAsync(request, new Metadata { { "cache", cacheName } });
            }
            catch (Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        private GetResponse SendGet(string cacheName, ByteString key)
        {
            GetRequest request = new GetRequest() { CacheKey = key };
            try
            {
                return this.grpcManager.Client().Get(request, new Metadata { { "cache", cacheName } });
            }
            catch (Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        private async Task<GetResponse> SendGetAsync(string cacheName, ByteString key)
        {
            GetRequest request = new GetRequest() { CacheKey = key };
            try
            {
                return await this.grpcManager.Client().GetAsync(request, new Metadata { { "cache", cacheName } });
            }
            catch (Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        private SetResponse SendSet(string cacheName, ByteString key, ByteString value, uint ttlSeconds)
        {
            SetRequest request = new SetRequest() { CacheBody = value, CacheKey = key, TtlMilliseconds = ttlSeconds * 1000 };
            try
            {
                return this.grpcManager.Client().Set(request, new Metadata { { "cache", cacheName } });
            }
            catch (Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        private ByteString Convert(byte[] bytes)
        {
            return ByteString.CopyFrom(bytes);
        }

        private ByteString Convert(string s)
        {
            return ByteString.CopyFromUtf8(s);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.grpcManager.Dispose();
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
