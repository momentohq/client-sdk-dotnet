using System;
using CacheClient;
using static CacheClient.Scs;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using System.Threading.Tasks;
using Google.Protobuf;

namespace MomentoSdk
{
    public class MomentoCache
    {
        private readonly int defaultTtlSeconds;
        private readonly ScsClient client;
        private readonly string cacheName;
        private readonly uint defaultTtl;
        protected MomentoCache(string authToken, string cacheName, string endpoint, uint defaultTtlSeconds)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions() { Credentials = ChannelCredentials.SecureSsl });
            CallInvoker invoker = channel.Intercept(new AuthHeaderInterceptor(authToken));
            this.client = new ScsClient(invoker);
            this.defaultTtl = defaultTtlSeconds * 1000;
        }

        public static MomentoCache Init(string authToken, string cacheName, string endpoint, uint defaultTtlSeconds)
        {
            MomentoCache cache = new MomentoCache(authToken, cacheName, endpoint, defaultTtlSeconds);
            cache.WaitUntilReady();
            return cache;
        }

        public async Task<SetResponse> SetAsync(ByteString key, ByteString value, uint ttl)
        {
            SetRequest request = new SetRequest() { CacheBody = value, CacheKey = key, TtlMilliseconds = ttl };
            return await this.client.SetAsync(request);
        }

        public async Task<GetResponse> GetAsync(ByteString key)
        {
            GetRequest request = new GetRequest() { CacheKey = key };
            return await this.client.GetAsync(request);
        }

        public SetResponse Set(ByteString key, ByteString value, uint ttl)
        {
            SetRequest request = new SetRequest() { CacheBody = value, CacheKey = key, TtlMilliseconds = ttl };
            return this.client.Set(request);
        }

        public GetResponse Get(ByteString key)
        {
            GetRequest request = new GetRequest() { CacheKey = key };
            return this.client.Get(request);
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
