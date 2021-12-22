using System;
using Xunit;
using MomentoSdk;
using MomentoSdk.Exceptions;
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
            string cacheName = "listCacheName";
            Momento momento = new Momento(authKey);
            momento.CreateCache(cacheName);
            string[] returnedCaches = momento.ListCache().Caches();
            Assert.Contains(cacheName, returnedCaches);
            momento.DeleteCache(cacheName);
        }
    }
}
