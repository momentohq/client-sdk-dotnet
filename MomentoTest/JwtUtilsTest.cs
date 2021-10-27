using System;
using Xunit;
using MomentoSdk;

namespace MomentoTest
{
    public class JwtUtilsTest
    {
        [Fact]
        public void TestValidJwt()
        {
            string jwt = "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJzcXVpcnJlbCIsImNwIjoiY29udHJvbCBwbGFuZSBlbmRwb2ludCIsImMiOiJkYXRhIHBsYW5lIGVuZHBvaW50In0.zsTsEXFawetTCZI";
            Claims c = JwtUtils.decodeJwt(jwt);
            Assert.Equal("data plane endpoint", c.cacheEndpoint);
            Assert.Equal("control plane endpoint", c.controlEndpoint);
        }
    }
}
