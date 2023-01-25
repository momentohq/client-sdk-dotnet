using System;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Internal;

namespace Momento.Sdk.Config.Transport;

/// <summary>
/// The simplest way to configure gRPC for the Momento client; specifies static values for
/// request deadline and channel options.
/// </summary>
public class StaticGrpcConfiguration : IGrpcConfiguration
{
    /// <summary>
    /// Maximum amount of time before a request will timeout
    /// </summary>
    public TimeSpan Deadline { get; }
    /// <summary>
    /// Customizations to low-level gRPC channel configuration
    /// </summary>
    public GrpcChannelOptions GrpcChannelOptions { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="deadline">Maximum amount of time before a request will timeout</param>
    /// <param name="grpcChannelOptions">Customizations to low-level gRPC channel configuration</param>
    public StaticGrpcConfiguration(TimeSpan deadline, GrpcChannelOptions? grpcChannelOptions = null)
    {
        Utils.ArgumentStrictlyPositive(deadline, nameof(deadline));
        this.Deadline = deadline;
        this.GrpcChannelOptions = grpcChannelOptions ?? new GrpcChannelOptions();
    }

    /// <summary>
    /// Copy constructor for overriding the deadline
    /// </summary>
    /// <param name="deadline"></param>
    /// <returns>A new GrpcConfiguration with the updated deadline</returns>
    public IGrpcConfiguration WithDeadline(TimeSpan deadline)
    {
        return new StaticGrpcConfiguration(deadline, this.GrpcChannelOptions);
    }

    /// <summary>
    /// Copy constructor for overriding the gRPC channel options
    /// </summary>
    /// <param name="grpcChannelOptions"></param>
    /// <returns>A new GrpcConfiguration with the specified channel options</returns>
    public IGrpcConfiguration WithGrpcChannelOptions(GrpcChannelOptions grpcChannelOptions)
    {
        return new StaticGrpcConfiguration(this.Deadline, grpcChannelOptions);
    }
}

/// <summary>
/// The simplest way to configure the transport layer for the Momento client.
/// Provides static values for the maximum number of concurrent requests and the
/// gRPC configuration.
/// </summary>
public class StaticTransportStrategy : ITransportStrategy
{
    private readonly ILoggerFactory _loggerFactory;

    /// <inheritdoc />
    public int MaxConcurrentRequests { get; }

    /// <inheritdoc />
    public TimeSpan? EagerConnectionTimeout { get; }

    /// <inheritdoc />
    public IGrpcConfiguration GrpcConfig { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="maxConcurrentRequests">The maximum number of concurrent requests that the Momento client will allow on the wire at one time.</param>
    /// <param name="grpcConfig">Configures how Momento client interacts with the Momento service via gRPC</param>
    /// <param name="eagerConnectionTimeout">If null, the client will only attempt to connect to the server lazily when the first request is executed.
    /// If provided, the client will attempt to connect to the server immediately upon construction; if the connection
    /// cannot be established within the specified TimeSpan, it will abort the connection attempt, log a warning,
    /// and proceed with execution so that the application doesn't hang.</param>
    public StaticTransportStrategy(ILoggerFactory loggerFactory, int maxConcurrentRequests, IGrpcConfiguration grpcConfig, TimeSpan? eagerConnectionTimeout = null)
    {
        _loggerFactory = loggerFactory;
        MaxConcurrentRequests = maxConcurrentRequests;
        EagerConnectionTimeout = eagerConnectionTimeout;
        GrpcConfig = grpcConfig;
    }

    /// <inheritdoc/>
    public ITransportStrategy WithMaxConcurrentRequests(int maxConcurrentRequests)
    {
        return new StaticTransportStrategy(_loggerFactory, maxConcurrentRequests, GrpcConfig);
    }

    /// <inheritdoc/>
    public ITransportStrategy WithGrpcConfig(IGrpcConfiguration grpcConfig)
    {
        return new StaticTransportStrategy(_loggerFactory, MaxConcurrentRequests, grpcConfig);
    }

    /// <inheritdoc/>
    public ITransportStrategy WithClientTimeout(TimeSpan clientTimeout)
    {
        return new StaticTransportStrategy(_loggerFactory, MaxConcurrentRequests, GrpcConfig.WithDeadline(clientTimeout));
    }

    /// <inheritdoc/>
    public ITransportStrategy WithEagerConnectionTimeout(TimeSpan connectionTimeout)
    {
        return new StaticTransportStrategy(_loggerFactory, MaxConcurrentRequests, GrpcConfig, connectionTimeout);
    }
}
