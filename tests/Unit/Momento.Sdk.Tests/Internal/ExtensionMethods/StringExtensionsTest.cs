using System;
using Momento.Sdk.Internal.ExtensionMethods;
using Xunit;

public class StringExtensionsTest
{
    [Fact]
    public void Truncate_InvalidArguments_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => "hello".Truncate(totalLength: 2, fillValue: "..."));
    }

    [Theory]
    [InlineData("hello", 20, "...", "hello")]
    [InlineData("hello", 4, "...", "...o")]
    [InlineData("hellos", 5, "...", "h...s")]
    [InlineData("hellos", 4, "..", "h..s")]
    [InlineData("hellos", 5, "..", "h..os")]
    [InlineData("hello world", 10, "...", "hel...orld")]
    public void Truncate_StringSmall_ReturnsIdentical(string input, int totalLength, string fillValue, string expected)
    {
        var truncated = input.Truncate(totalLength, fillValue);
        Assert.Equal(expected, truncated);
        Assert.True(truncated.Length <= totalLength);
    }
}
