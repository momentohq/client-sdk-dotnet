using System;
using Xunit;
using MomentoSdk;
using MomentoSdk.Exceptions;
namespace MomentoIntegrationTest
{
    public class MomentoTest
    {
        private String authKey = Environment.GetEnvironmentVariable("TEST_AUTH_TOKEN");
        private String cacheName = Environment.GetEnvironmentVariable("TEST_CACHE_NAME");
        [Fact]
        public void DeleteCacheThatDoesntExist()
        {
            Momento momento = new Momento(authKey);
            Assert.Throws<CacheNotFoundException>(() => momento.DeleteCache("non existant cache"));
        }
    }
}
