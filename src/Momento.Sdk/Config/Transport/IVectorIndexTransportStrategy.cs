using System;

namespace Momento.Sdk.Config.Transport;

/// <summary>
/// This is responsible for configuring network tunables for the vector index client.
/// </summary>
public interface IVectorIndexTransportStrategy
{
    /// <summary>
    /// Configures the low-level gRPC settings for the Momento Vector Index client's communication
    /// with the Momento server.
    /// </summary>
    public IGrpcConfiguration GrpcConfig { get; }

    /// <summary>
    /// Copy constructor to update the gRPC configuration
    /// </summary>
    /// <param name="grpcConfig"></param>
    /// <returns>A new IVectorIndexTransportStrategy with the specified grpcConfig</returns>
    public IVectorIndexTransportStrategy WithGrpcConfig(IGrpcConfiguration grpcConfig);

    /// <summary>
    /// Copy constructor to update the client timeout
    /// </summary>
    /// <param name="clientTimeout"></param>
    /// <returns>A new IVectorIndexTransportStrategy with the specified client timeout</returns>
    public IVectorIndexTransportStrategy WithClientTimeout(TimeSpan clientTimeout);
}
