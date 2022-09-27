using System;
using Grpc.Net.Client;

namespace Momento.Sdk.Config.Transport;


public class StaticGrpcConfiguration : IGrpcConfiguration
{
    public uint DeadlineMilliseconds { get; }
    public GrpcChannelOptions GrpcChannelOptions { get; }

    public StaticGrpcConfiguration(uint deadlineMilliseconds, GrpcChannelOptions? grpcChannelOptions = null)
    {
        if (deadlineMilliseconds <= 0)
        {
            throw new ArgumentException($"Deadline must be strictly positive. Value was: {deadlineMilliseconds}", "DeadlineMilliseconds");
        }
        this.DeadlineMilliseconds = deadlineMilliseconds;
        this.GrpcChannelOptions = grpcChannelOptions ?? new GrpcChannelOptions();
    }

    public IGrpcConfiguration WithDeadlineMilliseconds(uint deadlineMilliseconds)
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
    public int MaxConcurrentRequests { get; }
    public IGrpcConfiguration GrpcConfig { get; }

    public StaticTransportStrategy(int maxConcurrentRequests, IGrpcConfiguration grpcConfig)
    {
        MaxConcurrentRequests = maxConcurrentRequests;
        GrpcConfig = grpcConfig;
    }

    public ITransportStrategy WithMaxConcurrentRequests(int maxConcurrentRequests)
    {
        return new StaticTransportStrategy(maxConcurrentRequests, GrpcConfig);
    }

    public ITransportStrategy WithGrpcConfig(IGrpcConfiguration grpcConfig)
    {
        return new StaticTransportStrategy(MaxConcurrentRequests, grpcConfig);
    }
}
