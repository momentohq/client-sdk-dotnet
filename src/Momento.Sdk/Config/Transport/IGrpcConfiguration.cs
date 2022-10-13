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

    /// <summary>
    /// Copy constructor to override the Deadline
    /// </summary>
    /// <param name="deadline"></param>
    /// <returns>A new IGrpcConfiguration with the specified Deadline</returns>
    public IGrpcConfiguration WithDeadline(TimeSpan deadline);

    /// <summary>
    /// Copy constructor to override the channel options
    /// </summary>
    /// <param name="grpcChannelOptions"></param>
    /// <returns>A new IGrpcConfiguration with the specified channel options</returns>
    public IGrpcConfiguration WithGrpcChannelOptions(GrpcChannelOptions grpcChannelOptions);
}
