using System;
using System.Threading.Tasks;
using Xunit;
using MomentoSdk.Responses;

namespace MomentoIntegrationTest
{
    public class CacheTest
    {
        private string authKey = Environment.GetEnvironmentVariable("TEST_AUTH_TOKEN");
        private string cacheName = Environment.GetEnvironmentVariable("TEST_CACHE_NAME");
        [Fact]
        public void HappyPath()
        {
            string cacheKey = "some cache key";
            string cacheValue = "some cache value";
            uint defaultTtlSeconds = 10;
            SimpleCacheClient client = new SimpleCacheClient(authKey, defaultTtlSeconds);
            client.CreateCache(cacheName);
            client.Set(cacheName, cacheKey, cacheValue, defaultTtlSeconds);
            CacheGetResponse result = client.Get(cacheName, cacheKey);
            string stringResult = result.String();
            Assert.Equal(cacheValue, stringResult);
        }

        [Fact]
        public async void HappyPathExpiredTtl()
        {
            string cacheKey = "some cache key";
            string cacheValue = "some cache value";
            uint defaultTtlSeconds = 10;
            SimpleCacheClient client = new SimpleCacheClient(authKey, defaultTtlSeconds);
            client.CreateCache(cacheName);
            client.Set(cacheName, cacheKey, cacheValue, 1);
            await Task.Delay(1100);
            CacheGetResponse result = client.Get(cacheName, cacheKey);
            Assert.Equal(CacheGetStatus.MISS, result.Status);
        }

        [Fact]
        public async void HappyPathAsync()
        {
            string cacheKey = "async cache key";
            string cacheValue = "async cache value";
            uint defaultTtlSeconds = 10\;
            SimpleCacheClient client = new SimpleCacheClient(authKey, defaultTtlSeconds);
            client.CreateCache(cacheName);
            await client.SetAsync(cacheName, cacheKey, cacheValue, defaultTtlSeconds);
            CacheGetResponse result = await client.GetAsync(cacheName, cacheKey);
            Assert.Equal(CacheGetStatus.HIT, result.Status);
            Assert.Equal(cacheValue, result.String());
        }

        [Fact]
        public void HappyPathMiss()
        {
            uint defaultTtlSeconds = 10;
            SimpleCacheClient client = new SimpleCacheClient(authKey, defaultTtlSeconds);
            client.CreateCache(cacheName);
            CacheGetResponse result = client.Get(cacheName, Guid.NewGuid().ToString());
            Assert.Equal(CacheGetStatus.MISS, result.Status);
            Assert.Null(result.String());
            Assert.Null(result.Bytes());
        }
    }
}
