using System;
using Xunit;
using MomentoSdk.Exceptions;
using MomentoSdk.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MomentoIntegrationTest
{
    public class SimpleCacheControlPlaneTests
    {
        private string authKey = Environment.GetEnvironmentVariable("TEST_AUTH_TOKEN");

        [Fact]
        public void DeleteCacheThatDoesntExist()
        {
            uint defaultTtlSeconds = 10;
            SimpleCacheClient client = new SimpleCacheClient(authKey, defaultTtlSeconds);
            Assert.Throws<NotFoundException>(() => client.DeleteCache("non existant cache"));
        }

        [Fact]
        public void InvalidJwtException()
        {
            uint defaultTtlSeconds = 10;
            Assert.Throws<InvalidArgumentException>(() => new SimpleCacheClient("eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJpbnRlZ3JhdGlvbiJ9.ZOgkTs", defaultTtlSeconds));
        }


        [Fact]
        public void HappyPathListCache()
        {
            uint defaultTtlSeconds = 10;
            SimpleCacheClient client = new SimpleCacheClient(authKey, defaultTtlSeconds);
            string cacheName = Guid.NewGuid().ToString();
            client.CreateCache(cacheName);
            ListCachesResponse result = client.ListCaches();
            List<CacheInfo> caches = result.Caches();
            Assert.True(caches.Exists(item => item.Name().Equals(cacheName)));
            client.DeleteCache(cacheName);
        }

        [Fact]
        public void InvalidRequestTimeout()
        {
            uint defaultTtlSeconds = 10;
            uint requestTimeoutSeconds = 0;
            Assert.Throws<InvalidArgumentException>(() => new SimpleCacheClient(authKey, defaultTtlSeconds, requestTimeoutSeconds));
        }
    }
}
