using System.Collections.Generic;

namespace Momento.Sdk.Config.Transport;


public class StaticGrpcConfiguration : IGrpcConfiguration
{
    public int NumChannels { get; }

    public int MaxSessionMemory { get; }
    public bool UseLocalSubChannelPool { get; }
    public IDictionary<string, string> GrpcChannelConfig { get; }

    public StaticGrpcConfiguration(int numChannels, int maxSessionMemory, bool useLocalSubChannelPool)
    {
        this.NumChannels = numChannels;
        this.MaxSessionMemory = maxSessionMemory;
        this.UseLocalSubChannelPool = useLocalSubChannelPool;
        this.GrpcChannelConfig = new Dictionary<string, string>();
    }
}

public class StaticTransportStrategy : ITransportStrategy
{
    public int MaxConcurrentRequests { get; }
    public IGrpcConfiguration GrpcConfig { get; }

    public StaticTransportStrategy(int maxConcurrentRequests, StaticGrpcConfiguration grpcConfig)
    {
        MaxConcurrentRequests = maxConcurrentRequests;
        GrpcConfig = grpcConfig;
    }
}
