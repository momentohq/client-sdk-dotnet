using System;
using System.Linq;
using Xunit;
using MomentoSdk.Exceptions;
using MomentoSdk.Responses;
using System.Collections.Generic;

namespace MomentoIntegrationTest
{
    public class SimpleCacheControlPlaneTests
    {
        private string authKey = Environment.GetEnvironmentVariable("TEST_AUTH_TOKEN");

        [Fact]
        public void SimpleCacheClientConstructor_BadRequestTimeout_ThrowsException()
        {
            uint defaultTtlSeconds = 10;
            uint requestTimeoutMilliseconds = 0;
            Assert.Throws<InvalidArgumentException>(() => new SimpleCacheClient(authKey, defaultTtlSeconds, requestTimeoutMilliseconds));
        }

        [Fact]
        public void SimpleCacheClientConstructor_BadJWT_InvalidJwtException()
        {
            uint defaultTtlSeconds = 10;
            Assert.Throws<InvalidArgumentException>(() => new SimpleCacheClient("eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJpbnRlZ3JhdGlvbiJ9.ZOgkTs", defaultTtlSeconds));
        }

        [Fact]
        public void DeleteCache_CacheDoesntExist_NotFoundException()
        {
            uint defaultTtlSeconds = 10;
            SimpleCacheClient client = new SimpleCacheClient(authKey, defaultTtlSeconds);
            Assert.Throws<NotFoundException>(() => client.DeleteCache("non existant cache"));
        }

        // Tests: creating a cache, listing a cache, and deleting a cache.
        [Fact]
        public void ListCaches_OneCache_HappyPath()
        {
            // Create cache
            SimpleCacheClient client = new SimpleCacheClient(authKey, defaultTtlSeconds: 10);
            string cacheName = Guid.NewGuid().ToString();
            client.CreateCache(cacheName);

            // Test cache exists
            ListCachesResponse result = client.ListCaches();
            List<CacheInfo> caches = result.Caches();
            Assert.True(caches.Contains(new CacheInfo(cacheName)));

            // Test deleting cache
            client.DeleteCache(cacheName);
            result = client.ListCaches();
            caches = result.Caches();
            Assert.False(caches.Contains(new CacheInfo(cacheName)));
        }

        /* TODO: ensure local client supports many caches
        [Fact]
        public void ListCaches_Iteration_HappyPath()
        {
            // Create caches
            SimpleCacheClient client = new SimpleCacheClient(authKey, defaultTtlSeconds: 10);
            List<String> cacheNames = new List<String>();
            foreach (int val in Enumerable.Range(1, 20))
            {
                String cacheName = Guid.NewGuid().ToString();
                cacheNames.Add(cacheName);
                client.CreateCache(cacheName);
            }

            // List caches
            HashSet<String> retrievedCaches = new HashSet<string>();
            ListCachesResponse result = client.ListCaches();
            do
            {
                foreach (CacheInfo cache in result.Caches())
                {
                    retrievedCaches.Add(cache.Name());
                }
            } while (result.NextPageToken != null);

            // Cleanup
            foreach (String cacheName in cacheNames)
            {
                client.DeleteCache(cacheName);
            }
        }
        */

        [Fact]
        public void ListCaches_BadNextToken_NoException()
        {
            // A bad next token does not throw an exception
            SimpleCacheClient client = new SimpleCacheClient(authKey, defaultTtlSeconds: 10);
            client.ListCaches(nextPageToken: "hello world");
        }
    }
}
