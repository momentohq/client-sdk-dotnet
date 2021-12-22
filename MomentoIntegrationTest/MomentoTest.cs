using System;
using Xunit;
using MomentoSdk;
using MomentoSdk.Exceptions;
using MomentoSdk.Responses;
using System.Collections.Generic;
namespace MomentoIntegrationTest
{
    public class MomentoTest
    {
        private String authKey = Environment.GetEnvironmentVariable("TEST_AUTH_TOKEN");

        [Fact]
        public void DeleteCacheThatDoesntExist()
        {
            Momento momento = new(authKey);
            Assert.Throws<CacheNotFoundException>(() => momento.DeleteCache("non existant cache"));
        }


        [Fact]
        public void InvalidJwtException()
        {
            Assert.Throws<InvalidJwtException>(() => new Momento("eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJpbnRlZ3JhdGlvbiJ9.ZOgkTs"));
        }

        [Fact]
        public void GetThrowsNotFoundForNonExistentCache()
        {
            Momento momento = new(authKey);
            Assert.Throws<CacheNotFoundException>(() => momento.GetCache(Guid.NewGuid().ToString(), 60));
        }

        [Fact]
        public void HappyPathListCache()
        {
            Momento momento = new(authKey);
            ListCachesResponse result = momento.ListCaches();
            List<CacheInfo> caches = result.Caches();
            List<string> names = new List<string>(new string[caches.Count]);
            int counter = 0;
            foreach (CacheInfo c in caches)
            {
                names[counter] = c.Name();
                counter++;
            }
            Assert.Contains(Environment.GetEnvironmentVariable("TEST_CACHE_NAME"), names);
        }
    }
}
