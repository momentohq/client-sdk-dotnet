using System;
using System.Threading.Tasks;
using Xunit;
using MomentoSdk.Responses;
using MomentoSdk.Exceptions;
using System.Text;
using System.Collections.Generic;

namespace MomentoIntegrationTest
{
    public class CacheTest : IDisposable
    {
        private string authKey = Environment.GetEnvironmentVariable("TEST_AUTH_TOKEN");
        private string cacheName = "client-sdk-csharp";
        private uint defaultTtlSeconds = 10;
        private SimpleCacheClient client;

        // Test initialization
        public CacheTest()
        {
            uint defaultTtlSeconds = 10;
            client = new SimpleCacheClient(authKey, defaultTtlSeconds);
            try
            {
                client.CreateCache(cacheName);
            }
            catch (AlreadyExistsException)
            {
            }
        }

        // Test cleanup
        public void Dispose()
        {
            client.DeleteCache(cacheName);
            client.Dispose();
        }

        [Fact]
        public void HappyPath()
        {
            string cacheKey = "some cache key";
            string cacheValue = "some cache value";
            client.Set(cacheName, cacheKey, cacheValue, defaultTtlSeconds);
            CacheGetResponse result = client.Get(cacheName, cacheKey);
            string stringResult = result.String();
            Assert.Equal(cacheValue, stringResult);
        }

        [Fact]
        public async Task HappyPathMultiGetAsync()
        {
            string cacheKey1 = "key1";
            string cacheValue1 = "value1";
            string cacheKey2 = "key2";
            string cacheValue2 = "value2";
            client.Set(cacheName, cacheKey1, cacheValue1, defaultTtlSeconds);
            client.Set(cacheName, cacheKey2, cacheValue2, defaultTtlSeconds);
            List<string> keys = new() { cacheKey1, cacheKey2 };
            CacheMultiGetResponse result = await client.MultiGetAsync(cacheName, keys);
            string stringResult1 = result.Strings()[0];
            string stringResult2 = result.Strings()[1];
            Assert.Equal(cacheValue1, stringResult1);
            Assert.Equal(cacheValue2, stringResult2);
        }

        [Fact]
        public async Task HappyPathMultiGetAsyncByteKeys()
        {
            string cacheKey1 = "key1";
            string cacheValue1 = "value1";
            string cacheKey2 = "key2";
            string cacheValue2 = "value2";
            client.Set(cacheName, cacheKey1, cacheValue1, defaultTtlSeconds);
            client.Set(cacheName, cacheKey2, cacheValue2, defaultTtlSeconds);
            List<byte[]> keys = new() { Encoding.ASCII.GetBytes(cacheKey1), Encoding.ASCII.GetBytes(cacheKey2) };
            CacheMultiGetResponse result = await client.MultiGetAsync(cacheName, keys);
            string stringResult1 = result.Strings()[0];
            string stringResult2 = result.Strings()[1];
            Assert.Equal(cacheValue1, stringResult1);
            Assert.Equal(cacheValue2, stringResult2);
        }

        [Fact]
        public async Task HappyPathMultiGetAsyncFailureRetry()
        {
            // Set very small timeout for dataClientOperationTimeoutMilliseconds
            SimpleCacheClient simpleCacheClient = new SimpleCacheClient(authKey, defaultTtlSeconds, 1);
            string cacheKey1 = "key1";
            string cacheValue1 = "value1";
            string cacheKey2 = "key2";
            string cacheValue2 = "value2";
            List<string> keys = new() { cacheKey1, cacheKey2 };
            CacheMultiGetResponse failedResult = await simpleCacheClient.MultiGetAsync(cacheName, keys);
            Assert.Equal(2, failedResult.FailedResponses().Count);
            Assert.Empty(failedResult.SuccessfulResponses());

            // Use normal test client and retry
            client.Set(cacheName, cacheKey1, cacheValue1, defaultTtlSeconds);
            client.Set(cacheName, cacheKey2, cacheValue2, defaultTtlSeconds);
            CacheMultiGetResponse result = await client.MultiGetAsync(cacheName, failedResult.FailedResponses());
            string stringResult1 = result.Strings()[0];
            string stringResult2 = result.Strings()[1];
            Assert.Equal(2, result.SuccessfulResponses().Count);
            Assert.Equal(cacheValue1, stringResult1);
            Assert.Equal(cacheValue2, stringResult2);
        }

        [Fact]
        public void HappyPathStringKeyByteValue()
        {
            string cacheKey = "some cache key";
            byte[] cacheValue = Encoding.ASCII.GetBytes("some cache value");
            client.Set(cacheName, cacheKey, cacheValue, defaultTtlSeconds);
            CacheGetResponse result = client.Get(cacheName, cacheKey);
            string stringResult = result.String();
            Assert.Equal(Encoding.ASCII.GetString(cacheValue), stringResult);
        }

        [Fact]
        public async void HappyPathExpiredTtl()
        {
            string cacheKey = "some cache key";
            string cacheValue = "some cache value";
            client.Set(cacheName, cacheKey, cacheValue, 1);
            await Task.Delay(3000);
            CacheGetResponse result = client.Get(cacheName, cacheKey);
            Assert.Equal(CacheGetStatus.MISS, result.Status);
        }

        [Fact]
        public async void HappyPathAsync()
        {
            string cacheKey = "async cache key";
            string cacheValue = "async cache value";
            await client.SetAsync(cacheName, cacheKey, cacheValue, defaultTtlSeconds);
            CacheGetResponse result = await client.GetAsync(cacheName, cacheKey);
            Assert.Equal(CacheGetStatus.HIT, result.Status);
            Assert.Equal(cacheValue, result.String());
        }

        [Fact]
        public void HappyPathMiss()
        {
            CacheGetResponse result = client.Get(cacheName, Guid.NewGuid().ToString());
            Assert.Equal(CacheGetStatus.MISS, result.Status);
            Assert.Null(result.String());
            Assert.Null(result.Bytes());
        }

        [Fact]
        public void GetThrowsNotFoundExceptionNonExistentCache()
        {
            uint defaultTtlSeconds = 10;
            SimpleCacheClient client = new SimpleCacheClient(authKey, defaultTtlSeconds);
            Assert.Throws<NotFoundException>(() => client.Get("non-existent-cache", Guid.NewGuid().ToString()));
        }

        [Fact]
        public void SetThrowsNotFoundExceptionNonExistentCache()
        {
            uint defaultTtlSeconds = 10;
            SimpleCacheClient client = new SimpleCacheClient(authKey, defaultTtlSeconds);
            Assert.Throws<NotFoundException>(() => client.Set("non-existent-cache", Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
        }

        [Fact]
        public void HappyPathDelete_KeyIsByteArray()
        {
            // Set a key to then delete
            byte[] key = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            byte[] value = new byte[] { 0x05, 0x06, 0x07, 0x08 };
            client.Set(cacheName, key, value, ttlSeconds: 60);
            CacheGetResponse getResponse = client.Get(cacheName, key);
            Assert.Equal(CacheGetStatus.HIT, getResponse.Status);

            // Delete
            client.Delete(cacheName, key);

            // Check deleted
            getResponse = client.Get(cacheName, key);
            Assert.Equal(CacheGetStatus.MISS, getResponse.Status);
        }

        [Fact]
        public async Task HappyPathDeleteAsync_KeyIsByteArray()
        {
            // Set a key to then delete
            byte[] key = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            byte[] value = new byte[] { 0x05, 0x06, 0x07, 0x08 };
            await client.SetAsync(cacheName, key, value, ttlSeconds: 60);
            CacheGetResponse getResponse = await client.GetAsync(cacheName, key);
            Assert.Equal(CacheGetStatus.HIT, getResponse.Status);

            // Delete
            await client.DeleteAsync(cacheName, key);

            // Check deleted
            getResponse = await client.GetAsync(cacheName, key);
            Assert.Equal(CacheGetStatus.MISS, getResponse.Status);
        }

        [Fact]
        public void HappyPathDelete_KeyIsString()
        {
            // Set a key to then delete
            string key = "key";
            string value = "value";
            client.Set(cacheName, key, value, ttlSeconds: 60);
            CacheGetResponse getResponse = client.Get(cacheName, key);
            Assert.Equal(CacheGetStatus.HIT, getResponse.Status);

            // Delete
            client.Delete(cacheName, key);

            // Check deleted
            getResponse = client.Get(cacheName, key);
            Assert.Equal(CacheGetStatus.MISS, getResponse.Status);
        }

        [Fact]
        public async Task HappyPathDeleteAsync_KeyIsString()
        {
            // Set a key to then delete
            string key = "key";
            string value = "value";
            await client.SetAsync(cacheName, key, value, ttlSeconds: 60);
            CacheGetResponse getResponse = await client.GetAsync(cacheName, key);
            Assert.Equal(CacheGetStatus.HIT, getResponse.Status);

            // Delete
            await client.DeleteAsync(cacheName, key);

            // Check deleted
            getResponse = await client.GetAsync(cacheName, key);
            Assert.Equal(CacheGetStatus.MISS, getResponse.Status);
        }
    }
}
