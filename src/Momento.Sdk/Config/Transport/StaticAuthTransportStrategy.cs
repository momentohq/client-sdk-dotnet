#if !BUILD_FOR_UNITY
using System;
using Microsoft.Extensions.Logging;

namespace Momento.Sdk.Config.Transport;

/// <summary>
/// The simplest way to configure the transport layer for the Momento Auth client.
/// Provides static values for the gRPC configuration.
/// </summary>
public class StaticAuthTransportStrategy : IAuthTransportStrategy
{
    private readonly ILoggerFactory _loggerFactory;

    /// <inheritdoc />
    public IGrpcConfiguration GrpcConfig { get; }

    /// <summary>
    /// Configures the transport layer for the Momento client.
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="grpcConfig">Configures how Momento Auth client interacts with the Momento service via gRPC</param>
    public StaticAuthTransportStrategy(ILoggerFactory loggerFactory, IGrpcConfiguration grpcConfig)
    {
        _loggerFactory = loggerFactory;
        GrpcConfig = grpcConfig;
    }

    /// <inheritdoc/>
    public IAuthTransportStrategy WithGrpcConfig(IGrpcConfiguration grpcConfig)
    {
        return new StaticAuthTransportStrategy(_loggerFactory, grpcConfig);
    }

    /// <inheritdoc/>
    public IAuthTransportStrategy WithClientTimeout(TimeSpan clientTimeout)
    {
        return new StaticAuthTransportStrategy(_loggerFactory, GrpcConfig.WithDeadline(clientTimeout));
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
#endif
