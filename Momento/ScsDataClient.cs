using System;
using System.Threading.Tasks;
using MomentoSdk.Responses;
using MomentoSdk.Exceptions;
using CacheClient;
using Google.Protobuf;
using Grpc.Core;

namespace MomentoSdk
{
    internal sealed class ScsDataClient : IDisposable
    {
        private readonly DataGrpcManager grpcManager;
        private readonly uint defaultTtlSeconds;
        private readonly uint dataClientOperationTimeoutSeconds;
        private const uint DEFAULT_DEADLINE_SECONDS = 5;

        public ScsDataClient(string authToken, string endpoint, uint defaultTtlSeconds)
        {
            this.grpcManager = new DataGrpcManager(authToken, endpoint);
            this.defaultTtlSeconds = defaultTtlSeconds;
            this.dataClientOperationTimeoutSeconds = DEFAULT_DEADLINE_SECONDS;
        }

        public ScsDataClient(string authToken, string endpoint, uint defaultTtlSeconds, uint dataClientOperationTimeoutSeconds)
        {
            this.grpcManager = new DataGrpcManager(authToken, endpoint);
            this.defaultTtlSeconds = defaultTtlSeconds;
            this.dataClientOperationTimeoutSeconds = dataClientOperationTimeoutSeconds;
        }

        public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value, uint ttlSeconds)
        {
            _SetResponse response = await this.SendSetAsync(cacheName, value: Convert(value), key: Convert(key), ttlSeconds: ttlSeconds, dataClientOperationTimeoutSeconds: this.dataClientOperationTimeoutSeconds);
            return new CacheSetResponse(response);
        }

        public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value)
        {
            return await this.SetAsync(cacheName, key, value, defaultTtlSeconds);
        }

        public async Task<CacheGetResponse> GetAsync(string cacheName, byte[] key)
        {
            _GetResponse resp = await this.SendGetAsync(cacheName, Convert(key), dataClientOperationTimeoutSeconds: this.dataClientOperationTimeoutSeconds);
            return new CacheGetResponse(resp);
        }

        public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, uint ttlSeconds)
        {
            _SetResponse response = await this.SendSetAsync(cacheName, key: Convert(key), value: Convert(value), ttlSeconds: ttlSeconds, dataClientOperationTimeoutSeconds: this.dataClientOperationTimeoutSeconds);
            return new CacheSetResponse(response);
        }

        public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value)
        {
            return await this.SetAsync(cacheName, key, value, defaultTtlSeconds);
        }

        public async Task<CacheGetResponse> GetAsync(string cacheName, string key)
        {
            _GetResponse resp = await this.SendGetAsync(cacheName, Convert(key), dataClientOperationTimeoutSeconds: this.dataClientOperationTimeoutSeconds);
            return new CacheGetResponse(resp);
        }

        public CacheSetResponse Set(string cacheName, byte[] key, byte[] value, uint ttlSeconds)
        {
            _SetResponse resp = this.SendSet(cacheName, key: Convert(key), value: Convert(value), ttlSeconds: ttlSeconds);
            return new CacheSetResponse(resp);
        }

        public CacheSetResponse Set(string cacheName, byte[] key, byte[] value)
        {
            return this.Set(cacheName, key, value, defaultTtlSeconds);
        }

        public CacheGetResponse Get(string cacheName, byte[] key)
        {
            _GetResponse resp = this.SendGet(cacheName, Convert(key));
            return new CacheGetResponse(resp);
        }

        public CacheSetResponse Set(string cacheName, string key, string value, uint ttlSeconds)
        {
            _SetResponse response = this.SendSet(cacheName, key: Convert(key), value: Convert(value), ttlSeconds: ttlSeconds);
            return new CacheSetResponse(response);
        }

        public CacheSetResponse Set(string cacheName, string key, string value)
        {
            return this.Set(cacheName, key, value, defaultTtlSeconds);
        }

        public CacheGetResponse Get(string cacheName, string key)
        {
            _GetResponse resp = this.SendGet(cacheName, Convert(key));
            return new CacheGetResponse(resp);
        }

        private async Task<_SetResponse> SendSetAsync(string cacheName, ByteString key, ByteString value, uint ttlSeconds, uint dataClientOperationTimeoutSeconds)
        {
            _SetRequest request = new _SetRequest() { CacheBody = value, CacheKey = key, TtlMilliseconds = ttlSeconds * 1000 };
            DateTime deadline = DateTime.UtcNow.AddSeconds(dataClientOperationTimeoutSeconds);
            try
            {
                return await this.grpcManager.Client().SetAsync(request, new Metadata { { "cache", cacheName } }, deadline: deadline);
            }
            catch (Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        private _GetResponse SendGet(string cacheName, ByteString key)
        {
            _GetRequest request = new _GetRequest() { CacheKey = key };
            try
            {
                return this.grpcManager.Client().Get(request, new Metadata { { "cache", cacheName } });
            }
            catch (Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        private async Task<_GetResponse> SendGetAsync(string cacheName, ByteString key, uint dataClientOperationTimeoutSeconds)
        {
            _GetRequest request = new _GetRequest() { CacheKey = key };
            DateTime deadline = DateTime.UtcNow.AddSeconds(dataClientOperationTimeoutSeconds);
            try
            {
                return await this.grpcManager.Client().GetAsync(request, new Metadata { { "cache", cacheName } }, deadline: deadline);
            }
            catch (Exception e)
            {
                throw CacheExceptionMapper.Convert(e);
            }
        }

        private _SetResponse SendSet(string cacheName, ByteString key, ByteString value, uint ttlSeconds)
        {
            _SetRequest request = new _SetRequest() { CacheBody = value, CacheKey = key, TtlMilliseconds = ttlSeconds * 1000 };
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

        public void Dispose()
        {
            this.grpcManager.Dispose();
        }
    }
}
