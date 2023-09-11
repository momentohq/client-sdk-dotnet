#if !BUILD_FOR_UNITY
using System;

namespace Momento.Sdk.Config.Transport;

/// <summary>
/// This is responsible for configuring network tunables for auth client.
/// </summary>
public interface IAuthTransportStrategy
{
    /// <summary>
    /// Configures the low-level gRPC settings for the Momento Auth client's communication
    /// with the Momento server.
    /// </summary>
    public IGrpcConfiguration GrpcConfig { get; }

    /// <summary>
    /// Copy constructor to update the gRPC configuration
    /// </summary>
    /// <param name="grpcConfig"></param>
    /// <returns>A new IAuthTransportStrategy with the specified grpcConfig</returns>
    public IAuthTransportStrategy WithGrpcConfig(IGrpcConfiguration grpcConfig);

    /// <summary>
    /// Copy constructor to update the client timeout
    /// </summary>
    /// <param name="clientTimeout"></param>
    /// <returns>A new IAuthTransportStrategy with the specified client timeout</returns>
    public IAuthTransportStrategy WithClientTimeout(TimeSpan clientTimeout);
}
#endif
