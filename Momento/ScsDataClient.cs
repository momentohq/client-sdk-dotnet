using System;
using System.Threading.Tasks;
using MomentoSdk.Responses;
using MomentoSdk.Exceptions;
using CacheClient;
using Google.Protobuf;
using Grpc.Core;
using System.Collections.Generic;

namespace MomentoSdk
{
    internal sealed class ScsDataClient : IDisposable
    {
        private readonly DataGrpcManager grpcManager;
        private readonly uint defaultTtlSeconds;
        private readonly uint dataClientOperationTimeoutMilliseconds;
        private const uint DEFAULT_DEADLINE_MILLISECONDS = 5000;

        public ScsDataClient(string authToken, string endpoint, uint defaultTtlSeconds)
        {
            this.grpcManager = new DataGrpcManager(authToken, endpoint);
            this.defaultTtlSeconds = defaultTtlSeconds;
            this.dataClientOperationTimeoutMilliseconds = DEFAULT_DEADLINE_MILLISECONDS;
        }

        public ScsDataClient(string authToken, string endpoint, uint defaultTtlSeconds, uint dataClientOperationTimeoutMilliseconds)
        {
            this.grpcManager = new DataGrpcManager(authToken, endpoint);
            this.defaultTtlSeconds = defaultTtlSeconds;
            this.dataClientOperationTimeoutMilliseconds = dataClientOperationTimeoutMilliseconds;
        }

        public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value, uint ttlSeconds)
        {
            _SetResponse response = await this.SendSetAsync(cacheName, value: Convert(value), key: Convert(key), ttlSeconds: ttlSeconds, dataClientOperationTimeoutMilliseconds: this.dataClientOperationTimeoutMilliseconds);
            return new CacheSetResponse(response);
        }

        public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value)
        {
            return await this.SetAsync(cacheName, key, value, defaultTtlSeconds);
        }

        public async Task<CacheGetResponse> GetAsync(string cacheName, byte[] key)
        {
            _GetResponse resp = await this.SendGetAsync(cacheName, Convert(key), dataClientOperationTimeoutMilliseconds: this.dataClientOperationTimeoutMilliseconds);
            return new CacheGetResponse(resp);
        }

        public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, byte[] key)
        {
            await this.SendDeleteAsync(cacheName, Convert(key));
            return new CacheDeleteResponse();
        }

        public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, uint ttlSeconds)
        {
            _SetResponse response = await this.SendSetAsync(cacheName, key: Convert(key), value: Convert(value), ttlSeconds: ttlSeconds, dataClientOperationTimeoutMilliseconds: this.dataClientOperationTimeoutMilliseconds);
            return new CacheSetResponse(response);
        }

        public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value)
        {
            return await this.SetAsync(cacheName, key, value, defaultTtlSeconds);
        }

        public async Task<CacheGetResponse> GetAsync(string cacheName, string key)
        {
            _GetResponse resp = await this.SendGetAsync(cacheName, Convert(key), dataClientOperationTimeoutMilliseconds: this.dataClientOperationTimeoutMilliseconds);
            return new CacheGetResponse(resp);
        }

        public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, string key)
        {
            await this.SendDeleteAsync(cacheName, Convert(key));
            return new CacheDeleteResponse();
        }

        public async Task<CacheSetResponse> SetAsync(string cacheName, string key, byte[] value, uint ttlSeconds)
        {
            _SetResponse response = await this.SendSetAsync(cacheName, value: Convert(value), key: Convert(key), ttlSeconds: ttlSeconds, dataClientOperationTimeoutMilliseconds: this.dataClientOperationTimeoutMilliseconds);
            return new CacheSetResponse(response);
        }

        public async Task<CacheSetResponse> SetAsync(string cacheName, string key, byte[] value)
        {
            return await this.SetAsync(cacheName, key, value, defaultTtlSeconds);
        }

        public async Task<CacheMultiGetResponse> MultiGetAsync(string cacheName, List<string> keys)
        {
            List<Task<CacheMultiGetResponse>> tasks = new();
            List<CacheGetResponse> successResponses = new();
            List<CacheMultiGetFailureResponse> failedResponses = new();
            foreach (string key in keys)
            {
                tasks.Add(SendMultiGetAsync(cacheName, Convert(key)));
            }

            await Task.WhenAll(tasks);
            ProcessCacheMultiGetResponseTaskResult(tasks, successResponses, failedResponses);
            return new CacheMultiGetResponse(successResponses, failedResponses);
        }

        public async Task<CacheMultiGetResponse> MultiGetAsync(string cacheName, List<byte[]> keys)
        {
            List<Task<CacheMultiGetResponse>> tasks = new();
            List<CacheGetResponse> successResponses = new();
            List<CacheMultiGetFailureResponse> failedResponses = new();
            foreach (byte[] key in keys)
            {
                tasks.Add(SendMultiGetAsync(cacheName, Convert(key)));
            }

            await Task.WhenAll(tasks);
            ProcessCacheMultiGetResponseTaskResult(tasks, successResponses, failedResponses);
            return new CacheMultiGetResponse(successResponses, failedResponses);
        }

        public async Task<CacheMultiGetResponse> MultiGetAsync(string cacheName, List<CacheMultiGetFailureResponse> responses)
        {
            List<Task<CacheMultiGetResponse>> tasks = new();
            List<CacheGetResponse> successResponses = new();
            List<CacheMultiGetFailureResponse> failedResponses = new();
            foreach (CacheMultiGetFailureResponse response in responses)
            {
                tasks.Add(SendMultiGetAsync(cacheName, Convert(response.Key)));
            }

            await Task.WhenAll(tasks);
            ProcessCacheMultiGetResponseTaskResult(tasks, successResponses, failedResponses);
            return new CacheMultiGetResponse(successResponses, failedResponses);
        }

        public CacheSetResponse Set(string cacheName, byte[] key, byte[] value, uint ttlSeconds)
        {
            _SetResponse resp = this.SendSet(cacheName, key: Convert(key), value: Convert(value), ttlSeconds: ttlSeconds, dataClientOperationTimeoutMilliseconds: this.dataClientOperationTimeoutMilliseconds);
            return new CacheSetResponse(resp);
        }

        public CacheSetResponse Set(string cacheName, byte[] key, byte[] value)
        {
            return this.Set(cacheName, key, value, defaultTtlSeconds);
        }

        public CacheGetResponse Get(string cacheName, byte[] key)
        {
            _GetResponse resp = this.SendGet(cacheName, Convert(key), dataClientOperationTimeoutMilliseconds: this.dataClientOperationTimeoutMilliseconds);
            return new CacheGetResponse(resp);
        }

        public CacheDeleteResponse Delete(string cacheName, byte[] key)
        {
            this.SendDelete(cacheName, Convert(key));
            return new CacheDeleteResponse();
        }

        public CacheSetResponse Set(string cacheName, string key, string value, uint ttlSeconds)
        {
            _SetResponse response = this.SendSet(cacheName, key: Convert(key), value: Convert(value), ttlSeconds: ttlSeconds, dataClientOperationTimeoutMilliseconds: this.dataClientOperationTimeoutMilliseconds);
            return new CacheSetResponse(response);
        }

        public CacheSetResponse Set(string cacheName, string key, string value)
        {
            return this.Set(cacheName, key, value, defaultTtlSeconds);
        }

        public CacheGetResponse Get(string cacheName, string key)
        {
            _GetResponse resp = this.SendGet(cacheName, Convert(key), dataClientOperationTimeoutMilliseconds: this.dataClientOperationTimeoutMilliseconds);
            return new CacheGetResponse(resp);
        }

        public CacheDeleteResponse Delete(string cacheName, string key)
        {
            this.SendDelete(cacheName, Convert(key));
            return new CacheDeleteResponse();
        }

        public CacheSetResponse Set(string cacheName, string key, byte[] value, uint ttlSeconds)
        {
            _SetResponse response = this.SendSet(cacheName, key: Convert(key), value: Convert(value), ttlSeconds: ttlSeconds, dataClientOperationTimeoutMilliseconds: this.dataClientOperationTimeoutMilliseconds);
            return new CacheSetResponse(response);
        }

        public CacheSetResponse Set(string cacheName, string key, byte[] value)
        {
            return this.Set(cacheName, key, value, defaultTtlSeconds);
        }

        private async Task<_SetResponse> SendSetAsync(string cacheName, ByteString key, ByteString value, uint ttlSeconds, uint dataClientOperationTimeoutMilliseconds)
        {
            _SetRequest request = new _SetRequest() { CacheBody = value, CacheKey = key, TtlMilliseconds = ttlSeconds * 1000 };
            DateTime deadline = DateTime.UtcNow.AddMilliseconds(dataClientOperationTimeoutMilliseconds);
            try
            {
                return await this.grpcManager.Client().SetAsync(request, new Metadata { { "cache", cacheName } }, deadline: deadline);
            }
            catch (Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        private _GetResponse SendGet(string cacheName, ByteString key, uint dataClientOperationTimeoutMilliseconds)
        {
            _GetRequest request = new _GetRequest() { CacheKey = key };
            DateTime deadline = DateTime.UtcNow.AddMilliseconds(dataClientOperationTimeoutMilliseconds);
            try
            {
                return this.grpcManager.Client().Get(request, new Metadata { { "cache", cacheName } }, deadline: deadline);
            }
            catch (Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        private async Task<_GetResponse> SendGetAsync(string cacheName, ByteString key, uint dataClientOperationTimeoutMilliseconds)
        {
            _GetRequest request = new _GetRequest() { CacheKey = key };
            DateTime deadline = DateTime.UtcNow.AddMilliseconds(dataClientOperationTimeoutMilliseconds);
            try
            {
                return await this.grpcManager.Client().GetAsync(request, new Metadata { { "cache", cacheName } }, deadline: deadline);
            }
            catch (Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        private _SetResponse SendSet(string cacheName, ByteString key, ByteString value, uint ttlSeconds, uint dataClientOperationTimeoutMilliseconds)
        {
            _SetRequest request = new _SetRequest() { CacheBody = value, CacheKey = key, TtlMilliseconds = ttlSeconds * 1000 };
            DateTime deadline = DateTime.UtcNow.AddMilliseconds(dataClientOperationTimeoutMilliseconds);
            try
            {
                return this.grpcManager.Client().Set(request, new Metadata { { "cache", cacheName } }, deadline: deadline);
            }
            catch (Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        private _DeleteResponse SendDelete(string cacheName, ByteString key)
        {
            _DeleteRequest request = new _DeleteRequest() { CacheKey = key };
            DateTime deadline = DateTime.UtcNow.AddMilliseconds(dataClientOperationTimeoutMilliseconds);
            try
            {
                return this.grpcManager.Client().Delete(request, new Metadata { { "cache", cacheName } }, deadline: deadline);
            }
            catch (Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        private async Task<_DeleteResponse> SendDeleteAsync(string cacheName, ByteString key)
        {
            _DeleteRequest request = new _DeleteRequest() { CacheKey = key };
            DateTime deadline = DateTime.UtcNow.AddMilliseconds(dataClientOperationTimeoutMilliseconds);
            try
            {
                return await this.grpcManager.Client().DeleteAsync(request, new Metadata { { "cache", cacheName } }, deadline: deadline);
            }
            catch (Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        private async Task<CacheMultiGetResponse> SendMultiGetAsync(string cacheName, ByteString key)
        {
            _GetRequest request = new _GetRequest() { CacheKey = key };
            DateTime deadline = DateTime.UtcNow.AddMilliseconds(this.dataClientOperationTimeoutMilliseconds);
            try
            {
                _GetResponse resp = await this.grpcManager.Client().GetAsync(request, new Metadata { { "cache", cacheName } }, deadline: deadline);
                return new CacheMultiGetResponse(new CacheGetResponse(resp));

            }
            catch (Exception e)
            {
                return new CacheMultiGetResponse(new CacheMultiGetFailureResponse(key.ToByteArray(), CacheExceptionMapper.Convert(e)));
            }

        }

        private void ProcessCacheMultiGetResponseTaskResult(List<Task<CacheMultiGetResponse>> tasks, List<CacheGetResponse> successResponses, List<CacheMultiGetFailureResponse> failedResponses)
        {
            foreach (Task<CacheMultiGetResponse> t in tasks)
            {
                if (t.Result.SuccessfulResponse() is not null)
                {
                    successResponses.Add(t.Result.SuccessfulResponse());
                }
                if (t.Result.FailedResponse() is not null)
                {
                    failedResponses.Add(t.Result.FailedResponse());
                }
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

        public void Dispose()
        {
            this.grpcManager.Dispose();
        }
    }
}
