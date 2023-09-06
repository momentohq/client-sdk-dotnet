using System;

namespace Momento.Sdk.Config.Transport;

/// <summary>
/// This is responsible for configuring network tunables for topics.
/// </summary>
public interface ITopicTransportStrategy
{
    /// <summary>
    /// Configures the low-level gRPC settings for the Momento Topic client's communication
    /// with the Momento server.
    /// </summary>
    public IGrpcConfiguration GrpcConfig { get; }

    /// <summary>
    /// Copy constructor to update the gRPC configuration
    /// </summary>
    /// <param name="grpcConfig"></param>
    /// <returns>A new ITransportStrategy with the specified grpcConfig</returns>
    public ITopicTransportStrategy WithGrpcConfig(IGrpcConfiguration grpcConfig);

    /// <summary>
    /// Copy constructor to update the client timeout for publishing
    /// </summary>
    /// <param name="clientTimeout"></param>
    /// <returns>A new ITransportStrategy with the specified client timeout</returns>
    public ITopicTransportStrategy WithClientTimeout(TimeSpan clientTimeout);
}
