using System;
using Microsoft.Extensions.Logging;

namespace Momento.Sdk.Config.Transport;

/// <summary>
/// The simplest way to configure the transport layer for the Momento Topic client.
/// Provides static values for the gRPC configuration.
/// </summary>
public class StaticTopicTransportStrategy : ITopicTransportStrategy
{
    private readonly ILoggerFactory _loggerFactory;

    /// <inheritdoc />
    public IGrpcConfiguration GrpcConfig { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="grpcConfig">Configures how Momento Topic client interacts with the Momento service via gRPC</param>
    public StaticTopicTransportStrategy(ILoggerFactory loggerFactory, IGrpcConfiguration grpcConfig)
    {
        _loggerFactory = loggerFactory;
        GrpcConfig = grpcConfig;
    }

    /// <inheritdoc/>
    public ITopicTransportStrategy WithGrpcConfig(IGrpcConfiguration grpcConfig)
    {
        return new StaticTopicTransportStrategy(_loggerFactory, grpcConfig);
    }

    /// <inheritdoc/>
    public ITopicTransportStrategy WithClientTimeout(TimeSpan clientTimeout)
    {
        return new StaticTopicTransportStrategy(_loggerFactory, GrpcConfig.WithDeadline(clientTimeout));
    }

    /// <summary>
    /// Test equality by value.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }

        var other = (StaticTransportStrategy)obj;
        return GrpcConfig.Equals(other.GrpcConfig);
    }

    /// <summary>
    /// Trivial hash code implementation.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
