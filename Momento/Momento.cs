﻿using System;
using ControlClient;
using static ControlClient.ScsControl;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using MomentoSdk.Exceptions;

namespace MomentoSdk
{
    public class Momento : IDisposable
    {
        private readonly string cacheEndpoint;
        private readonly string authToken;
        private readonly ScsControlClient client;
        private readonly GrpcChannel channel;
        private bool disposedValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken">Momento jwt</param>
        public Momento(string authToken)
        {
<<<<<<< Updated upstream
            Claims claims = JwtUtils.decodeJwt(authToken);
            this.channel = GrpcChannel.ForAddress("https://" + claims.controlEndpoint + ":443", new GrpcChannelOptions() { Credentials = ChannelCredentials.SecureSsl });
=======
            Claims claims = JwtUtils.DecodeJwt(authToken);
            GrpcChannel channel = GrpcChannel.ForAddress("https://" + claims.ControlEndpoint + ":443", new GrpcChannelOptions() { Credentials = ChannelCredentials.SecureSsl });
>>>>>>> Stashed changes
            Header[] headers = { new Header(name: "Authorization", value: authToken) };
            CallInvoker invoker = this.channel.Intercept(new HeaderInterceptor(headers));
            this.client = new ScsControlClient(invoker);
            this.authToken = authToken;
            this.cacheEndpoint = "https://" + claims.CacheEndpoint + ":443";
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
                client.CreateCache(request);
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.AlreadyExists)
                {
                    throw new CacheAlreadyExistsException("cache with name " + cacheName + " already exists");
                }
                throw CacheExceptionMapper.Convert(e);
            }
            catch(Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
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
            return MomentoCache.Init(authToken, cacheName, cacheEndpoint, defaultTtlSeconds);
        }

        /// <summary>
        /// Deletes a cache and all of the items within it
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public Responses.DeleteCacheResponse DeleteCache(String cacheName)
        {
            DeleteCacheRequest request = new DeleteCacheRequest() { CacheName = cacheName };
            try
            {
                client.DeleteCache(request);
                return new Responses.DeleteCacheResponse();
            } catch(Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        private Boolean CheckValidCacheName(String cacheName)
        {
            if (String.IsNullOrWhiteSpace(cacheName))
            {
                throw new InvalidCacheNameException("cache name must be nonempty");
            }
            return true;
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
