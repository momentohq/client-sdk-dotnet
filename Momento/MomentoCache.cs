using System;
using System.Threading;
using CacheClient;
using static CacheClient.Scs;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using System.Threading.Tasks;
using Google.Protobuf;
using MomentoSdk.Responses;
using MomentoSdk.Exceptions;

namespace MomentoSdk
{
    public class MomentoCache : IDisposable
    {
        private readonly ScsClient client;
        private readonly uint defaultTtlSeconds;
        private readonly GrpcChannel channel;
        private bool disposedValue;

        protected MomentoCache(string authToken, string cacheName, string endpoint, uint defaultTtlSeconds)
        {
            this.channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions() { Credentials = ChannelCredentials.SecureSsl });
            Header[] headers = { new Header(name: "Authorization", value: authToken), new Header(name: "cache", value: cacheName) };
            CallInvoker invoker = this.channel.Intercept(new HeaderInterceptor(headers));
            this.client = new ScsClient(invoker);
            this.defaultTtlSeconds = defaultTtlSeconds;
        }

        /// <summary>
        /// Intitializes a MomentoCache
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="cacheName"></param>
        /// <param name="endpoint"></param>
        /// <param name="defaultTtlSeconds"></param>
        /// <returns>returns a cache ready for gets and sets</returns>
        internal static MomentoCache Init(string authToken, string cacheName, string endpoint, uint defaultTtlSeconds)
        {
            return new MomentoCache(authToken, cacheName, endpoint, defaultTtlSeconds);
        }

        internal MomentoCache Connect()
        {
            SendGet(ByteString.CopyFromUtf8(Guid.NewGuid().ToString()));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttlSeconds"></param>
        /// <returns></returns>
        public async Task<CacheSetResponse> SetAsync(byte[] key, byte[] value, uint ttlSeconds)
        {
            SetResponse response = await this.SendSetAsync(value: Convert(value), key: Convert(key), ttlSeconds: ttlSeconds);
            return new CacheSetResponse(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<CacheSetResponse> SetAsync(byte[] key, byte[] value)
        {
            return await this.SetAsync(key, value, defaultTtlSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">The key to perform a cache lookup on</param>
        /// <returns></returns>
        public async Task<CacheGetResponse> GetAsync(byte[] key)
        {
            GetResponse resp = await this.SendGetAsync(Convert(key));
            return new CacheGetResponse(resp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttlSeconds"></param>
        /// <returns></returns>
        public async Task<CacheSetResponse> SetAsync(String key, String value, uint ttlSeconds)
        {
            SetResponse response = await this.SendSetAsync(key: Convert(key), value: Convert(value), ttlSeconds: ttlSeconds);
            return new CacheSetResponse(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<CacheSetResponse> SetAsync(String key, String value)
        {
            return await this.SetAsync(key, value, defaultTtlSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<CacheGetResponse> GetAsync(String key)
        {
            GetResponse resp = await this.SendGetAsync(Convert(key));
            return new CacheGetResponse(resp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttlSeconds"></param>
        /// <returns></returns>
        public CacheSetResponse Set(byte[] key, byte[] value, uint ttlSeconds)
        {
            SetResponse resp = this.SendSet(key: Convert(key), value: Convert(value), ttlSeconds: ttlSeconds);
            return new CacheSetResponse(resp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public CacheSetResponse Set(byte[] key, byte[] value)
        {
            return this.Set(key, value, defaultTtlSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CacheGetResponse Get(byte[] key)
        {
            GetResponse resp = this.SendGet(Convert(key));
            return new CacheGetResponse(resp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttlSeconds"></param>
        /// <returns></returns>
        public CacheSetResponse Set(String key, String value, uint ttlSeconds)
        {
            SetResponse response = this.SendSet(key: Convert(key), value: Convert(value), ttlSeconds: ttlSeconds);
            return new CacheSetResponse(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttlSeconds"></param>
        /// <returns></returns>
        public CacheSetResponse Set(String key, String value)
        {
            return this.Set(key, value, defaultTtlSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CacheGetResponse Get(String key)
        {
            GetResponse resp = this.SendGet(Convert(key));
            return new CacheGetResponse(resp);
        }

        private GetResponse SendGet(ByteString key)
        {
            GetRequest request = new GetRequest() { CacheKey = key };
            try
            {
                return this.client.Get(request);
            } catch(Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        private async Task<GetResponse> SendGetAsync(ByteString key)
        {
            GetRequest request = new GetRequest() { CacheKey = key };
            try
            {
                return await this.client.GetAsync(request);
            } catch(Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        private SetResponse SendSet(ByteString key, ByteString value, uint ttlSeconds)
        {
            SetRequest request = new SetRequest() { CacheBody = value, CacheKey = key, TtlMilliseconds = ttlSeconds * 1000 };
            try
            {
                return this.client.Set(request);
            } catch(Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        private async Task<SetResponse> SendSetAsync(ByteString key, ByteString value, uint ttlSeconds)
        {
            SetRequest request = new SetRequest() { CacheBody = value, CacheKey = key, TtlMilliseconds = ttlSeconds * 1000 };
            try
            {
                return await this.client.SetAsync(request);
            } catch(Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        private ByteString Convert(byte[] bytes)
        {
            return ByteString.CopyFrom(bytes);
        }

        private ByteString Convert(String s)
        {
            return ByteString.CopyFromUtf8(s);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.channel.Dispose();
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
