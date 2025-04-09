using Momento.Sdk.Internal.ExtensionMethods;
using System;
using Xunit;

namespace Momento.Sdk.Tests.Unit;

public class ByteArrayExtensionsTest
{
    [Fact]
    public void ToPrettyHexString_HappyPath()
    {
        var actual = new byte[] { 0x00, 0x01 }.ToPrettyHexString();
        Assert.Equal("00-01", actual);

        actual = new byte[] { 0xFF, 0xAB }.ToPrettyHexString();
        Assert.Equal("FF-AB", actual);
    }
}
