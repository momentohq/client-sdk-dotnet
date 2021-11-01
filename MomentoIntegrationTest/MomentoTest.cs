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
            Momento momento = new Momento(authKey);
            Assert.Throws<CacheNotFoundException>(() => momento.DeleteCache("non existant cache"));
        }


        [Fact]
        public void InvalidJwtException()
        {
            Assert.Throws<InvalidJwtException>( () => new Momento("eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJpbnRlZ3JhdGlvbiJ9.ZOgkTs"));
        }
    }
}
