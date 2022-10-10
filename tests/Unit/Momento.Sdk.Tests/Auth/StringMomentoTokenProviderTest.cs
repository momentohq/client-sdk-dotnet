using Momento.Sdk.Auth;
using Momento.Sdk.Exceptions;
using Xunit;

namespace Momento.Sdk.Tests;

public class StringMomentoTokenProviderTest
{
    [Fact]
    public void StringMomentoTokenProvider_EmptyOrNull_ThrowsException()
    {
        Assert.Throws<InvalidArgumentException>(
            () => new StringMomentoTokenProvider("eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJpbnRlZ3JhdGlvbiJ9.ZOgkTs")
        );
        Assert.Throws<InvalidArgumentException>(() => new StringMomentoTokenProvider(null!));
    }
}
