using System;
using Microsoft.Extensions.Logging;

namespace Momento.Sdk.Config.Transport;

/// <summary>
/// This is responsible for configuring network tunables.
/// </summary>
public interface ITransportStrategy
{
    /// <summary>
    /// The maximum number of concurrent requests that the Momento client will
    /// allow onto the wire at a given time.
    /// </summary>
    public int MaxConcurrentRequests { get; }
    
    /// <summary>
    /// If false, the client will only attempt to connect to the server lazily when the first request is executed.
    /// If true, the client will attempt to connect to the server immediately upon construction.
    /// </summary>
    public Boolean EagerConnection { get;  }
    
    /// <summary>
    /// Configures the low-level gRPC settings for the Momento client's communication
    /// with the Momento server.
    /// </summary>
    public IGrpcConfiguration GrpcConfig { get; }

    /// <summary>
    /// Copy constructor to update the maximum number of concurrent requests.
    /// </summary>
    /// <param name="maxConcurrentRequests"></param>
    /// <returns>A new ITransportStrategy with the specified maxConccurrentRequests</returns>
    public ITransportStrategy WithMaxConcurrentRequests(int maxConcurrentRequests);

    /// <summary>
    /// Copy constructor to update the gRPC configuration
    /// </summary>
    /// <param name="grpcConfig"></param>
    /// <returns>A new ITransportStrategy with the specified grpcConfig</returns>
    public ITransportStrategy WithGrpcConfig(IGrpcConfiguration grpcConfig);

    /// <summary>
    /// Copy constructor to update the client timeout
    /// </summary>
    /// <param name="clientTimeout"></param>
    /// <returns>A new ITransportStrategy with the specified client timeout</returns>
    public ITransportStrategy WithClientTimeout(TimeSpan clientTimeout);

    /// <summary>
    /// Copy constructor to enable eager connection to the server
    /// </summary>
    /// <returns>A new ITransportStrategy configured to eagerly connect to the server upon construction</returns>
    public ITransportStrategy WithEagerConnection();
}
