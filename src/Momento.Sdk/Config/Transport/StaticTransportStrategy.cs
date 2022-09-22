using System;
using System.Collections.Generic;

namespace Momento.Sdk.Config.Transport;


public class StaticGrpcConfiguration : IGrpcConfiguration
{
    public int NumChannels { get; set; }

    public int MaxSessionMemory { get; set; }
    public bool UseLocalSubChannelPool { get; set; }
    private uint _deadlineMilliseconds;
    public uint DeadlineMilliseconds
    {
        get => _deadlineMilliseconds;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException($"Deadline must be strictly positive. Value was: {value}", "DeadlineMilliseconds");
            }
            _deadlineMilliseconds = value;
        }
    }
    public IDictionary<string, string> GrpcChannelConfig { get; set; }

    public StaticGrpcConfiguration(int numChannels, int maxSessionMemory, bool useLocalSubChannelPool, uint deadlineMilliseconds = 5000)
    {
        this.NumChannels = numChannels;
        this.MaxSessionMemory = maxSessionMemory;
        this.UseLocalSubChannelPool = useLocalSubChannelPool;
        this.DeadlineMilliseconds = deadlineMilliseconds;
        this.GrpcChannelConfig = new Dictionary<string, string>();
    }
}

public class StaticTransportStrategy : ITransportStrategy
{
    public int MaxConcurrentRequests { get; set; }
    public IGrpcConfiguration GrpcConfig { get; set; }

    public StaticTransportStrategy(int maxConcurrentRequests, StaticGrpcConfiguration grpcConfig)
    {
        MaxConcurrentRequests = maxConcurrentRequests;
        GrpcConfig = grpcConfig;
    }
}
