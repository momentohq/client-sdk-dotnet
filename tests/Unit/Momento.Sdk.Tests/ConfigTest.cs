using System;
using Momento.Sdk.Config;
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

    [Fact]
    public void V1VConfigs_EqualLatest_HappyPath()
    {
        Assert.Equal(Configurations.Laptop.Latest(), Configurations.Laptop.V1());
        Assert.Equal(Configurations.InRegion.Default.Latest(), Configurations.InRegion.Default.V1());
        Assert.Equal(Configurations.InRegion.LowLatency.Latest(), Configurations.InRegion.LowLatency.V1());
    }
}
