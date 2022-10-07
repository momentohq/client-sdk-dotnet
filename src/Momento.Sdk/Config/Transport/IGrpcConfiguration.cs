using System;
using Grpc.Net.Client;

namespace Momento.Sdk.Config.Transport;

/// <summary>
/// Abstracts away the gRPC configuration tunables.
/// </summary>
public interface IGrpcConfiguration
{
    /// <summary>
    /// How long the client is willing to wait for an RPC to complete before it is terminated with <see cref="Grpc.Core.StatusCode.DeadlineExceeded"/>
    /// </summary>
    public TimeSpan Deadline { get; }

    /// <summary>
    /// Override the default .NET GrpcChannelOptions.  (.NET only povides a strongly-typed
    /// interface for the channel options, which allows modifying specific values but does
    /// not allow the caller to use arbitrary strings to set the channel options.)
    /// </summary>
    public GrpcChannelOptions GrpcChannelOptions { get; }

    public IGrpcConfiguration WithDeadline(TimeSpan deadline);
    public IGrpcConfiguration WithGrpcChannelOptions(GrpcChannelOptions grpcChannelOptions);
}
