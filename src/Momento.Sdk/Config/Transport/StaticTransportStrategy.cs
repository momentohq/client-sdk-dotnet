using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Internal;
using System;
using System.Net.Http;

namespace Momento.Sdk.Config.Transport;

/// <summary>
/// The simplest way to configure gRPC for the Momento client; specifies static values for
/// request deadline and channel options.
/// </summary>
public class StaticGrpcConfiguration : IGrpcConfiguration
{
    private const int DEFAULT_STREAM_CHANNELS = 4;
    private const int DEFAULT_UNARY_CHANNELS = 4;
    private const int DEFAULT_MIN_CHANNELS = 1;
    
    /// <inheritdoc/>
    public TimeSpan Deadline { get; }
    /// <inheritdoc/>
    public int MinNumGrpcChannels { get; }
    
    /// <inheritdoc/>
    public int NumStreamGrpcChannels { get;  }
    
    /// <inheritdoc/>
    public int NumUnaryGrpcChannels { get;  }
    
    /// <inheritdoc/>
    public GrpcChannelOptions GrpcChannelOptions { get; }
    /// <inheritdoc/>
    public SocketsHttpHandlerOptions SocketsHttpHandlerOptions { get; }
    /// <inheritdoc/>
    public TimeSpan KeepAlivePingTimeout { get; }
    /// <inheritdoc/>
    public TimeSpan KeepAlivePingDelay { get; }
    /// <inheritdoc/>
    public bool KeepAlivePermitWithoutCalls { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="deadline">Maximum amount of time before a request will timeout</param>
    /// <param name="grpcChannelOptions">Customizations to low-level gRPC channel configuration</param>
    /// <param name="minNumGrpcChannels">minimum number of gRPC channels to open</param>
    /// <param name="socketsHttpHandlerOptions">Customizations to the SocketsHttpHandler</param>
    public StaticGrpcConfiguration(
        TimeSpan deadline,
        GrpcChannelOptions? grpcChannelOptions = null,
        int minNumGrpcChannels = DEFAULT_MIN_CHANNELS, 
        int numStreamGrpcChannels = DEFAULT_STREAM_CHANNELS,
        int numUnaryGrpcChannels = DEFAULT_UNARY_CHANNELS,
        SocketsHttpHandlerOptions? socketsHttpHandlerOptions = null
    )
    {
        Utils.ArgumentStrictlyPositive(deadline, nameof(deadline));
        this.Deadline = deadline;
        this.MinNumGrpcChannels = minNumGrpcChannels;
        this.NumStreamGrpcChannels = numStreamGrpcChannels;
        this.NumUnaryGrpcChannels = numUnaryGrpcChannels;
        this.GrpcChannelOptions = grpcChannelOptions ?? DefaultGrpcChannelOptions();
        this.SocketsHttpHandlerOptions = socketsHttpHandlerOptions ?? new SocketsHttpHandlerOptions();
    }

    /// <summary>
    /// The grpc default value for max_send_message_length is 4mb. This function returns default grpc options that increase max message size to 5mb in order to support cases where users have requested a limit increase up to our maximum item size of 5mb.
    /// </summary>
    /// <returns>GrpcChannelOptions</returns>
    public static GrpcChannelOptions DefaultGrpcChannelOptions()
    {
        const int DEFAULT_MAX_MESSAGE_SIZE = 5_243_000;
        return new GrpcChannelOptions
        {
            MaxReceiveMessageSize = DEFAULT_MAX_MESSAGE_SIZE,
            MaxSendMessageSize = DEFAULT_MAX_MESSAGE_SIZE
        };
    }

    /// <inheritdoc/>
    public IGrpcConfiguration WithDeadline(TimeSpan deadline)
    {
        return new StaticGrpcConfiguration(deadline, GrpcChannelOptions, MinNumGrpcChannels, NumStreamGrpcChannels, MinNumGrpcChannels,  SocketsHttpHandlerOptions);
    }

    /// <inheritdoc/>
    public IGrpcConfiguration WithMinNumGrpcChannels(int minNumGrpcChannels)
    {
        return new StaticGrpcConfiguration(Deadline, GrpcChannelOptions, minNumGrpcChannels, NumStreamGrpcChannels, NumUnaryGrpcChannels, SocketsHttpHandlerOptions);
    }
    
    /// <inheritdoc/>
    public IGrpcConfiguration WithNumStreamGrpcChannels(int numStreamGrpcChannels)
    {
        return new StaticGrpcConfiguration(Deadline, GrpcChannelOptions, MinNumGrpcChannels, numStreamGrpcChannels, NumUnaryGrpcChannels, SocketsHttpHandlerOptions);
    }
    
    /// <inheritdoc/>
    public IGrpcConfiguration WithNumUnaryGrpcChannels(int numUnaryGrpcChannels)
    {
        return new StaticGrpcConfiguration(Deadline, GrpcChannelOptions, MinNumGrpcChannels, NumStreamGrpcChannels, numUnaryGrpcChannels, SocketsHttpHandlerOptions);
    }

    /// <inheritdoc/>
    public IGrpcConfiguration WithGrpcChannelOptions(GrpcChannelOptions grpcChannelOptions)
    {
        return new StaticGrpcConfiguration(Deadline, grpcChannelOptions, MinNumGrpcChannels);
    }

    /// <inheritdoc/>
    public IGrpcConfiguration WithSocketsHttpHandlerOptions(SocketsHttpHandlerOptions options)
    {
        return new StaticGrpcConfiguration(Deadline, GrpcChannelOptions, MinNumGrpcChannels, NumStreamGrpcChannels, NumUnaryGrpcChannels, options);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }

        var other = (StaticGrpcConfiguration)obj;

        return Deadline.Equals(other.Deadline) &&
                MinNumGrpcChannels == other.MinNumGrpcChannels &&
                SocketsHttpHandlerOptions.Equals(other.SocketsHttpHandlerOptions);
        // TODO: gRPC doesn't implement a to equals for this
        //GrpcChannelOptions.Equals(other.GrpcChannelOptions);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return base.GetHashCode();
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

    /// <inheritdoc />
    public ITransportStrategy WithSocketsHttpHandlerOptions(SocketsHttpHandlerOptions options)
    {
        return new StaticTransportStrategy(_loggerFactory, MaxConcurrentRequests, GrpcConfig.WithSocketsHttpHandlerOptions(options));
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
        return MaxConcurrentRequests == other.MaxConcurrentRequests &&
            ((EagerConnectionTimeout == null && other.EagerConnectionTimeout == null) ||
                EagerConnectionTimeout.Equals(other.EagerConnectionTimeout)) &&
            GrpcConfig.Equals(other.GrpcConfig);
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
