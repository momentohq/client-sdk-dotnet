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
    /// If null, the client will only attempt to connect to the server lazily when the first request is executed.
    /// If provided, the client will attempt to connect to the server immediately upon construction; if the connection
    /// cannot be established within the specified TimeSpan, it will abort the connection attempt, log a warning,
    /// and proceed with execution so that the application doesn't hang.
    /// </summary>
    public TimeSpan? EagerConnectionTimeout { get; }

    /// <summary>
    /// Configures the low-level gRPC settings for the Momento client's communication
    /// with the Momento server.
    /// </summary>
    public IGrpcConfiguration GrpcConfig { get; }

    /// <summary>
    /// Copy constructor to update the maximum number of concurrent requests.
    /// For every 100 concurrent requests, a new gRPC connection gets created. This value essentially limits
    /// the number of connections you will get for your gRPC client.
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
    /// <param name="connectionTimeout">A timeout for attempting an eager connection to the server.  When the client
    /// is constructed, it will attempt to connect to the server immediately.  If the connection
    /// cannot be established within the specified TimeSpan, it will abort the connection attempt, log a warning,
    /// and proceed with execution so that the application doesn't hang.
    /// </param>
    /// <returns>A new ITransportStrategy configured to eagerly connect to the server upon construction</returns>
    public ITransportStrategy WithEagerConnectionTimeout(TimeSpan connectionTimeout);
}
