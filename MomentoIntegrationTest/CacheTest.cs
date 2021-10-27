using System;
using Xunit;
using MomentoSdk;

namespace MomentoIntegrationTest
{
    public class CacheTest
    {
        [Fact]
        public void HappyPath()
        {
            String authKey = Environment.GetEnvironmentVariable("TEST_AUTH_TOKEN");
            String cacheName = Environment.GetEnvironmentVariable("TEST_CACHE_NAME");
            String cacheKey = "some cache key";
            String cacheValue = "some cache value";
            uint defaultTtlSeconds = 100;
            Momento momento = new Momento(authKey);
            MomentoCache cache = momento.CreateOrGetCache(cacheName, defaultTtlSeconds);
            cache.Set(cacheKey, cacheValue, defaultTtlSeconds);
            var result = cache.Get(cacheKey);
            String stringResult = result.CacheBody.ToStringUtf8();
            Assert.Equal(cacheValue, stringResult);
        }
    }
}
