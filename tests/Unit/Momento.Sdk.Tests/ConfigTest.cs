using System;
using Momento.Sdk.Config.Transport;
using Xunit;

public class ConfigTest
{
    [Fact]
    public void StaticGrpcConfiguration_BadDeadline_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new StaticGrpcConfiguration(TimeSpan.FromSeconds(-1)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new StaticGrpcConfiguration(TimeSpan.FromSeconds(0)));
    }
}
