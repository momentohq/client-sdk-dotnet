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
        public async Task DeleteCacheThatDoesntExist()
        {
            uint defaultTtlSeconds = 10;
            SimpleCacheClient client = new SimpleCacheClient(authKey, defaultTtlSeconds);
            await Assert.ThrowsAsync<NotFoundException>(() => client.DeleteCache("non existant cache"));
        }


        [Fact]
        public void InvalidJwtException()
        {
            uint defaultTtlSeconds = 10;
            Assert.Throws<InvalidArgumentException>(() => new SimpleCacheClient("eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJpbnRlZ3JhdGlvbiJ9.ZOgkTs", defaultTtlSeconds));
        }


        [Fact]
        public async void HappyPathListCache()
        {
            uint defaultTtlSeconds = 10;
            SimpleCacheClient client = new SimpleCacheClient(authKey, defaultTtlSeconds);
            string cacheName = Guid.NewGuid().ToString();
            client.CreateCache(cacheName);
            Task<ListCachesResponse> result = client.ListCaches();
            result.Wait();
            List<CacheInfo> caches = result.Result.Caches();
            Assert.True(caches.Exists(item => item.Name().Equals(cacheName)));
            await client.DeleteCache(cacheName);
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
