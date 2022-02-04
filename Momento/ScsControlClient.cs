﻿using System;
using MomentoSdk.Exceptions;
using ControlClient;

namespace MomentoSdk
{
    public class ScsControlClient
    {
        private readonly ControlGrpcManager grpcManager;
        private readonly string authToken;
        private readonly string endpoint;
        private bool disposedValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken">Momento jwt</param>
        /// <param name="endpoint">Control client endpint</param>
        public ScsControlClient(string authToken, string endpoint)
        {
            this.grpcManager = new ControlGrpcManager(authToken, endpoint);
            this.authToken = authToken;
            this.endpoint = endpoint;
        }

        /// <summary>
        /// Creates a cache if it doesnt exist. Returns the cache.
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public Responses.CreateCacheResponse CreateCache(string cacheName)
        {
            CheckValidCacheName(cacheName);
            try
            {
                CreateCacheRequest request = new CreateCacheRequest() { CacheName = cacheName };
                this.grpcManager.Client().CreateCache(request);
                return new Responses.CreateCacheResponse();
            }
            catch (AlreadyExistsException)
            {
                return new Responses.CreateCacheResponse();
            }
            catch (Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        /// <summary>
        /// Deletes a cache and all of the items within it
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public Responses.DeleteCacheResponse DeleteCache(string cacheName)
        {
            DeleteCacheRequest request = new DeleteCacheRequest() { CacheName = cacheName };
            try
            {
                this.grpcManager.Client().DeleteCache(request);
                return new Responses.DeleteCacheResponse();
            }
            catch (Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        /// <summary>
        /// List all caches
        /// </summary>
        /// <param name="nextPageToken"></param>
        /// <returns></returns>
        public Responses.ListCachesResponse ListCaches(string nextPageToken = null)
        {
            ListCachesRequest request = new ListCachesRequest() { NextToken = nextPageToken == null ? "" : nextPageToken };
            try
            {
                ControlClient.ListCachesResponse result = this.grpcManager.Client().ListCaches(request);
                return new Responses.ListCachesResponse(result);
            }
            catch (Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        private bool CheckValidCacheName(string cacheName)
        {
            if (string.IsNullOrWhiteSpace(cacheName))
            {
                throw new InvalidArgumentException("Cache name must be nonempty");
            }
            return true;
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