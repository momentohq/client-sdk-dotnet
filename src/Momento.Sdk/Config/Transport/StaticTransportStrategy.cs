using System;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Internal;

namespace Momento.Sdk.Config.Transport;


public class StaticGrpcConfiguration : IGrpcConfiguration
{
    public TimeSpan Deadline { get; }
    public GrpcChannelOptions GrpcChannelOptions { get; }

    public StaticGrpcConfiguration(TimeSpan deadline, GrpcChannelOptions? grpcChannelOptions = null)
    {
        Utils.ArgumentStrictlyPositive(deadline, nameof(deadline));
        this.Deadline = deadline;
        this.GrpcChannelOptions = grpcChannelOptions ?? new GrpcChannelOptions();
    }

    public IGrpcConfiguration WithDeadline(TimeSpan deadline)
    {
        return new StaticGrpcConfiguration(deadline, this.GrpcChannelOptions);
    }

    public IGrpcConfiguration WithGrpcChannelOptions(GrpcChannelOptions grpcChannelOptions)
    {
        return new StaticGrpcConfiguration(this.Deadline, grpcChannelOptions);
    }
}

public class StaticTransportStrategy : ITransportStrategy
{
    public ILoggerFactory? LoggerFactory { get; }
    public int MaxConcurrentRequests { get; }
    public IGrpcConfiguration GrpcConfig { get; }

    public StaticTransportStrategy(int maxConcurrentRequests, IGrpcConfiguration grpcConfig, ILoggerFactory? loggerFactory = null)
    {
        LoggerFactory = loggerFactory;
        MaxConcurrentRequests = maxConcurrentRequests;
        GrpcConfig = grpcConfig;
    }

    public StaticTransportStrategy WithLoggerFactory(ILoggerFactory loggerFactory)
    {
        return new(MaxConcurrentRequests, GrpcConfig, loggerFactory);
    }

    ITransportStrategy ITransportStrategy.WithLoggerFactory(ILoggerFactory loggerFactory)
    {
        return WithLoggerFactory(loggerFactory);
    }

    public ITransportStrategy WithMaxConcurrentRequests(int maxConcurrentRequests)
    {
        return new StaticTransportStrategy(maxConcurrentRequests, GrpcConfig, LoggerFactory);
    }

    public ITransportStrategy WithGrpcConfig(IGrpcConfiguration grpcConfig)
    {
        return new StaticTransportStrategy(MaxConcurrentRequests, grpcConfig, LoggerFactory);
    }

    public ITransportStrategy WithClientTimeout(TimeSpan clientTimeout)
    {
        return new StaticTransportStrategy(MaxConcurrentRequests, GrpcConfig.WithDeadline(clientTimeout), LoggerFactory);
    }
}
