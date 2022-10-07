using System;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace Momento.Sdk.Config.Transport;


public class StaticGrpcConfiguration : IGrpcConfiguration
{
    public int DeadlineMilliseconds { get; }
    public GrpcChannelOptions GrpcChannelOptions { get; }

    public StaticGrpcConfiguration(int deadlineMilliseconds, GrpcChannelOptions? grpcChannelOptions = null)
    {
        if (deadlineMilliseconds <= 0)
        {
            throw new ArgumentException($"Deadline must be strictly positive. Value was: {deadlineMilliseconds}", "DeadlineMilliseconds");
        }
        this.DeadlineMilliseconds = deadlineMilliseconds;
        this.GrpcChannelOptions = grpcChannelOptions ?? new GrpcChannelOptions();
    }

    public IGrpcConfiguration WithDeadlineMilliseconds(int deadlineMilliseconds)
    {
        return new StaticGrpcConfiguration(deadlineMilliseconds, this.GrpcChannelOptions);
    }

    public IGrpcConfiguration WithGrpcChannelOptions(GrpcChannelOptions grpcChannelOptions)
    {
        return new StaticGrpcConfiguration(this.DeadlineMilliseconds, grpcChannelOptions);
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

    public ITransportStrategy WithClientTimeoutMillis(int clientTimeoutMillis)
    {
        return new StaticTransportStrategy(MaxConcurrentRequests, GrpcConfig.WithDeadlineMilliseconds(clientTimeoutMillis), LoggerFactory);
    }

}
