using System;
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
    public class MomentoCache
    {
        private readonly ScsClient client;
        private readonly string cacheName;
        private readonly uint defaultTtlMillis;
        protected MomentoCache(string authToken, string cacheName, string endpoint, uint defaultTtlSeconds)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions() { Credentials = ChannelCredentials.SecureSsl });
            Header[] headers = { new Header(name: "Authorization", value: authToken), new Header(name: "cache", value: cacheName) };
            CallInvoker invoker = channel.Intercept(new HeaderInterceptor(headers));
            this.client = new ScsClient(invoker);
            this.defaultTtlMillis = defaultTtlSeconds * 1000;
        }

        /// <summary>
        /// Intitializes a MomentoCache
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="cacheName"></param>
        /// <param name="endpoint"></param>
        /// <param name="defaultTtlSeconds"></param>
        /// <returns>returns a cache ready for gets and sets</returns>
        public static MomentoCache Init(string authToken, string cacheName, string endpoint, uint defaultTtlSeconds)
        {
            MomentoCache cache = new MomentoCache(authToken, cacheName, endpoint, defaultTtlSeconds);
            cache.WaitUntilReady();
            return cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttlSeconds"></param>
        /// <returns></returns>
        public async Task<CacheSetResponse> SetAsync(ByteString key, ByteString value, uint ttlSeconds)
        {
            SetResponse response = await this.SendSetAsync(value: value, key: key, ttlSeconds: ttlSeconds);
            return new CacheSetResponse(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">The key to perform a cache lookup on</param>
        /// <returns></returns>
        public async Task<CacheGetResponse> GetAsync(ByteString key)
        {
            GetResponse resp = await this.SendGetAsync(key);
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
            ByteString byteKey = Google.Protobuf.ByteString.CopyFromUtf8(key);
            ByteString byteValue = Google.Protobuf.ByteString.CopyFromUtf8(value);
            SetResponse response = await this.SendSetAsync(key: byteKey, value: byteValue, ttlSeconds: ttlSeconds);
            return new CacheSetResponse(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<CacheGetResponse> GetAsync(String key)
        {
            ByteString byteKey = Google.Protobuf.ByteString.CopyFromUtf8(key);
            GetResponse resp = await this.SendGetAsync(byteKey);
            return new CacheGetResponse(resp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttlSeconds"></param>
        /// <returns></returns>
        public CacheSetResponse Set(ByteString key, ByteString value, uint ttlSeconds)
        {
            SetResponse resp = this.SendSet(key: key, value: value, ttlSeconds: ttlSeconds);
            return new CacheSetResponse(resp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CacheGetResponse Get(ByteString key)
        {
            GetResponse resp = this.SendGet(key);
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
            ByteString byteKey = Google.Protobuf.ByteString.CopyFromUtf8(key);
            ByteString byteValue = Google.Protobuf.ByteString.CopyFromUtf8(value);
            SetResponse response = this.SendSet(key: byteKey, value: byteValue, ttlSeconds: ttlSeconds);
            return new CacheSetResponse(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CacheGetResponse Get(String key)
        {
            ByteString byteKey = Google.Protobuf.ByteString.CopyFromUtf8(key);
            GetResponse resp = this.SendGet(byteKey);
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

        private async void WaitUntilReady()
        {
            ByteString cacheKey = ByteString.CopyFromUtf8("matt");
            GetRequest request = new GetRequest() { CacheKey = cacheKey };
            Exception lastError = new Exception();
            int backoffMillis = 50;
            int maxWaitDuration = 5000;

            long start = GetUnixTime();

            while(GetUnixTime() - start < maxWaitDuration)
            {
                try
                {
                    this.client.Get(request);
                    return;
                }
                catch (Exception e)
                {
                    lastError = e;
                    await Task.Delay(backoffMillis);
                }
                
            }

            throw lastError;
        }

        private long GetUnixTime()
        {
            DateTime now = DateTime.Now;
            return ((DateTimeOffset)now).ToUnixTimeSeconds();
        }
    }
}
