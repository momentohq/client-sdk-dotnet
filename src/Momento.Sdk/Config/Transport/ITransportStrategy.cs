namespace Momento.Sdk.Config.Transport;

/// <summary>
/// This is responsible for configuring network tunables.
/// </summary>
public interface ITransportStrategy
{
    public int MaxConcurrentRequests { get; }
    public IGrpcConfiguration GrpcConfig { get; }

    public ITransportStrategy WithMaxConcurrentRequests(int maxConcurrentRequests);
    public ITransportStrategy WithGrpcConfig(IGrpcConfiguration grpcConfig);
}
