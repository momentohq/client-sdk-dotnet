using System;
using Momento.Sdk.Config.Transport;
using Xunit;

public class ConfigTest
{
    [Fact]
    public void StaticGrpcConfiguration_BadDeadline_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new StaticGrpcConfiguration(5, 128, true, 0));
    }
}
