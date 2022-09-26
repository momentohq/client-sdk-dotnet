using System;
using System.Collections.Generic;
using Momento.Sdk.Internal.ExtensionMethods;

namespace Momento.Sdk.Config.Transport;


public class StaticGrpcConfiguration : IGrpcConfiguration
{
    public int NumChannels { get; }

    public int MaxSessionMemory { get; }
    public bool UseLocalSubChannelPool { get; }
    public uint DeadlineMilliseconds { get; }

    public IDictionary<string, string> GrpcChannelConfig { get; }

    public StaticGrpcConfiguration(int numChannels, int maxSessionMemory, bool useLocalSubChannelPool, uint deadlineMilliseconds, IDictionary<string, string> grpcChannelConfig)
    {
        this.NumChannels = numChannels;
        this.MaxSessionMemory = maxSessionMemory;
        this.UseLocalSubChannelPool = useLocalSubChannelPool;

        if (deadlineMilliseconds <= 0)
        {
            throw new ArgumentException($"Deadline must be strictly positive. Value was: {deadlineMilliseconds}", "DeadlineMilliseconds");
        }
        this.DeadlineMilliseconds = deadlineMilliseconds;
        this.GrpcChannelConfig = grpcChannelConfig;
    }

    public StaticGrpcConfiguration(int numChannels, int maxSessionMemory, bool useLocalSubChannelPool, uint deadlineMilliseconds)
        : this(numChannels, maxSessionMemory, useLocalSubChannelPool, deadlineMilliseconds, new Dictionary<string, string>())
    {

    }

    public IGrpcConfiguration WithNumChannels(int numChannels)
    {
        return new StaticGrpcConfiguration(numChannels, MaxSessionMemory, UseLocalSubChannelPool, DeadlineMilliseconds, GrpcChannelConfig.Clone());
    }

    public IGrpcConfiguration WithMaxSessionMemory(int maxSessionMemory)
    {
        return new StaticGrpcConfiguration(NumChannels, maxSessionMemory, UseLocalSubChannelPool, DeadlineMilliseconds, GrpcChannelConfig.Clone());
    }
    public IGrpcConfiguration WithUseLocalSubChannelPool(bool useLocalSubChannelPool)
    {
        return new StaticGrpcConfiguration(NumChannels, MaxSessionMemory, useLocalSubChannelPool, DeadlineMilliseconds, GrpcChannelConfig.Clone());
    }
    public IGrpcConfiguration WithDeadlineMilliseconds(uint deadlineMilliseconds)
    {
        return new StaticGrpcConfiguration(NumChannels, MaxSessionMemory, UseLocalSubChannelPool, deadlineMilliseconds, GrpcChannelConfig.Clone());
    }
    public IGrpcConfiguration WithGrpcChannelConfig(IDictionary<string, string> grpcChannelConfig)
    {
        return new StaticGrpcConfiguration(NumChannels, MaxSessionMemory, UseLocalSubChannelPool, DeadlineMilliseconds, grpcChannelConfig);
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
