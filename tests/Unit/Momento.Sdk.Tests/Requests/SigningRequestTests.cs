using System;
using Momento.Sdk.Requests;
using Xunit;

namespace Momento.Sdk.Tests.Requests;

public class SigningRequestTest
{
    [Fact]
    public void Constructor_InvalidTTL_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SigningRequest("testCacheName", "testCacheKey", CacheOperation.GET, TimeSpan.FromSeconds(-1)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new SigningRequest("testCacheName", "testCacheKey", CacheOperation.GET, TimeSpan.FromSeconds(0)));
    }
}
