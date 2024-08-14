using System;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Transport;
using Xunit;

namespace Momento.Sdk.Tests.Unit;

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

    [Fact]
    public void LambdaConfigDisablesKeepAlive()
    {
        var config = Configurations.InRegion.Lambda.Latest();
        var grpcConfig = config.TransportStrategy.GrpcConfig;
        Assert.Equal(System.Threading.Timeout.InfiniteTimeSpan, grpcConfig.SocketsHttpHandlerOptions.KeepAlivePingTimeout);
        Assert.Equal(System.Threading.Timeout.InfiniteTimeSpan, grpcConfig.SocketsHttpHandlerOptions.KeepAlivePingDelay);
        Assert.False(grpcConfig.SocketsHttpHandlerOptions.KeepAlivePermitWithoutCalls);
    }

    [Fact]
    public void LaptopConfigEnablesKeepAlive()
    {
        var config = Configurations.Laptop.Latest();
        var grpcConfig = config.TransportStrategy.GrpcConfig;
        Assert.Equal(TimeSpan.FromMilliseconds(1000), grpcConfig.SocketsHttpHandlerOptions.KeepAlivePingTimeout);
        Assert.Equal(TimeSpan.FromMilliseconds(5000), grpcConfig.SocketsHttpHandlerOptions.KeepAlivePingDelay);
        Assert.True(grpcConfig.SocketsHttpHandlerOptions.KeepAlivePermitWithoutCalls);
    }
}
