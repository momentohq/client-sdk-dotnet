using System;
using ControlClient;
using CacheClient;
using static CacheClient.Scs;
using static ControlClient.ScsControl;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;

namespace MomentoSdk
{
    public class Momento
    {
        private readonly string cacheEndpoint;
        private readonly string authToken;
        private readonly ScsControlClient client;

        public Momento(string authToken)
        {
            Claims claims = JwtUtils.decodeJwt(authToken);
            GrpcChannel channel = GrpcChannel.ForAddress(claims.controlEndpoint, new GrpcChannelOptions() { Credentials = ChannelCredentials.SecureSsl });
            CallInvoker invoker = channel.Intercept(new AuthHeaderInterceptor(authToken));
            this.client = new ScsControlClient(invoker);
            this.authToken = authToken;
            this.cacheEndpoint = claims.cacheEndpoint;
        }

        void CreateCache (String cacheName)
        {
            CreateCacheRequest request = new CreateCacheRequest() { CacheName = cacheName };
            this.client.CreateCache(request);
        }

        MomentoCache GetCache(String cacheName, uint defaultTtlSeconds)
        {
            return MomentoCache.Init(this.authToken, cacheName, this.cacheEndpoint, defaultTtlSeconds);
        }

        DeleteCacheResponse DeleteCache(String cacheName)
        {
            DeleteCacheRequest request = new DeleteCacheRequest() { CacheName = cacheName };
            return this.client.DeleteCache(request);
        }

    }
}
