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
    /// <inheritdoc/>
    public TimeSpan Deadline { get; }
    /// <inheritdoc/>
    public int MinNumGrpcChannels { get; }
    /// <inheritdoc/>
    public GrpcChannelOptions GrpcChannelOptions { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="deadline">Maximum amount of time before a request will timeout</param>
    /// <param name="grpcChannelOptions">Customizations to low-level gRPC channel configuration</param>
    /// <param name="minNumGrpcChannels">minimum number of gRPC channels to open</param>
    public StaticGrpcConfiguration(TimeSpan deadline, GrpcChannelOptions? grpcChannelOptions = null, int minNumGrpcChannels = 1)
    {
        Utils.ArgumentStrictlyPositive(deadline, nameof(deadline));
        this.Deadline = deadline;
        this.MinNumGrpcChannels = minNumGrpcChannels;
        this.GrpcChannelOptions = grpcChannelOptions ?? new GrpcChannelOptions();
    }

    /// <inheritdoc/>
    public IGrpcConfiguration WithDeadline(TimeSpan deadline)
    {
        return new StaticGrpcConfiguration(deadline, this.GrpcChannelOptions, this.MinNumGrpcChannels);
    }

    /// <inheritdoc/>
    public IGrpcConfiguration WithMinNumGrpcChannels(int minNumGrpcChannels)
    {
        return new StaticGrpcConfiguration(this.Deadline, this.GrpcChannelOptions, minNumGrpcChannels);
    }

    /// <inheritdoc/>
    public IGrpcConfiguration WithGrpcChannelOptions(GrpcChannelOptions grpcChannelOptions)
    {
        return new StaticGrpcConfiguration(this.Deadline, grpcChannelOptions, this.MinNumGrpcChannels);
    }

    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }

        var other = (StaticGrpcConfiguration)obj;

        return Deadline.Equals(other.Deadline) &&
                MinNumGrpcChannels == other.MinNumGrpcChannels;
        // TODO: gRPC doesn't implement a to equals for this
        //GrpcChannelOptions.Equals(other.GrpcChannelOptions);
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

    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }

        var other = (StaticTransportStrategy)obj;
        return MaxConcurrentRequests == other.MaxConcurrentRequests &&
            ((EagerConnectionTimeout == null && other.EagerConnectionTimeout == null) ||
                EagerConnectionTimeout.Equals(other.EagerConnectionTimeout)) &&
            GrpcConfig.Equals(other.GrpcConfig);
    }
}
