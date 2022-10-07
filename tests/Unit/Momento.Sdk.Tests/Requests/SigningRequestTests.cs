using System;
using Momento.Sdk.Requests;
using Xunit;

namespace Momento.Sdk.Tests.Requests;

public class SigningRequestTest
{
    [Fact]
    public void Constructor_InvalidTTL_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SigningRequest("testCacheName", "testCacheKey", CacheOperation.GET, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new SigningRequest("testCacheName", "testCacheKey", CacheOperation.GET, 0));
    }
}
