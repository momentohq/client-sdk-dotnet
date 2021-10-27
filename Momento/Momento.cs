using System;
using ControlClient;
using CacheClient;
using static CacheClient.Scs; 
using static ControlClient.ScsControl;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using MomentoSdk.Exceptions;

namespace MomentoSdk
{
    public class Momento
    {
        private readonly string cacheEndpoint;
        private readonly string authToken;
        private readonly ScsControlClient client;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken">Momento jwt</param>
        public Momento(string authToken)
        {
            Claims claims = JwtUtils.decodeJwt(authToken);
            GrpcChannel channel = GrpcChannel.ForAddress("https://" + claims.controlEndpoint + ":443", new GrpcChannelOptions() { Credentials = ChannelCredentials.SecureSsl });
            Header[] headers = { new Header(name: "Authorization", value: authToken) };
            CallInvoker invoker = channel.Intercept(new HeaderInterceptor(headers));
            this.client = new ScsControlClient(invoker);
            this.authToken = authToken;
            this.cacheEndpoint = "https://" + claims.cacheEndpoint + ":443";
        }

        /// <summary>
        /// Creates a cache if it doesnt exist. Returns the cache.
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="defaultTtlSeconds"></param>
        /// <returns>An instance of MomentoCache to perform sets and against against</returns>
        public MomentoCache CreateOrGetCache(String cacheName, uint defaultTtlSeconds)
        {
            try
            {
                CreateCache(cacheName);
                // swallow this error since the cache is already created
            } catch(CacheAlreadyExistsException)
            {
            }
            return GetCache(cacheName, defaultTtlSeconds);

        }

        /// <summary>
        /// Creates a cache with the given name
        /// </summary>
        /// <param name="cacheName">Name of the cache to create</param>
        public Responses.CreateCacheResponse CreateCache (String cacheName)
        {
            CheckValidCacheName(cacheName);
            try
            {
                CreateCacheRequest request = new CreateCacheRequest() { CacheName = cacheName };
                this.client.CreateCache(request);
            }
            catch (Grpc.Core.RpcException e)
            {
                if (e.StatusCode == StatusCode.AlreadyExists)
                {
                    throw new CacheAlreadyExistsException("cache with name " + cacheName + " already exists");
                }
            }

            return new Responses.CreateCacheResponse();
        }

        /// <summary>
        /// Gets an instance of MomentoCache to perform gets and sets on
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="defaultTtlSeconds"></param>
        /// <returns>An instance of MomentoCache to perform sets and against against</returns>
        public MomentoCache GetCache(String cacheName, uint defaultTtlSeconds)
        {
            CheckValidCacheName(cacheName);
            return MomentoCache.Init(this.authToken, cacheName, this.cacheEndpoint, defaultTtlSeconds);
        }

        /// <summary>
        /// Deletes a cache and all of the items within it
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public Responses.DeleteCacheResponse DeleteCache(String cacheName)
        {
            DeleteCacheRequest request = new DeleteCacheRequest() { CacheName = cacheName };
            this.client.DeleteCache(request);
            return new Responses.DeleteCacheResponse();
        }

        private Boolean CheckValidCacheName(String cacheName)
        {
            if (String.IsNullOrWhiteSpace(cacheName))
            {
                throw new InvalidCacheNameException("cache name must be nonempty");
            }
            return true;
        }

    }
}
