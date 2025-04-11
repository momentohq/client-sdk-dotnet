using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using System;
using Xunit;

namespace Momento.Sdk.Tests.Unit;

public class CacheClientConstructorTest
{
    // this token can be parsed by the JWT parser, so it works for validating the constructors.  It is not a valid token, though, and cannot be used to make actual requests.
    const String INVALID_AUTH_TOKEN = "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJmb29Abm90LmEuZG9tYWluIiwiY3AiOiJjb250cm9sLXBsYW5lLWVuZHBvaW50Lm5vdC5hLmRvbWFpbiIsImMiOiJjYWNoZS1lbmRwb2ludC5ub3QuYS5kb21haW4ifQo.rtxfu4miBHQ1uptWJ9x3URAwwJYvMeYIkkpXxUno_wIavg4h6YJStcbxk32NDBbmJkJS7mUw6MsvJNWaxfdPOw";


    [Fact]
    public void CanConstruct_CacheClient()
    {
        new CacheClient(Configurations.Laptop.Latest(), new StringMomentoTokenProvider(INVALID_AUTH_TOKEN), defaultTtl: TimeSpan.FromSeconds(60));
    }

    [Fact]
    public void CanConstruct_Legacy_SimpleCacheClient()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        new SimpleCacheClient(Configurations.Laptop.Latest(), new StringMomentoTokenProvider(INVALID_AUTH_TOKEN), defaultTtl: TimeSpan.FromSeconds(60));
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
