using System;
using System.Threading.Tasks;
using Xunit;
using MomentoSdk.Responses;
using MomentoSdk.Exceptions;
using System.Text;

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
            try {
                client.CreateCache(cacheName);
            } catch (AlreadyExistsException) {
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
    }
}
