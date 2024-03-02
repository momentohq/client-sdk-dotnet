using System;
using System.Net.Http;
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
    /// Configures the minimum number of gRPC channels that the client will open to the
    /// server.  By default, the client will only open one channel at startup, and will
    /// dynamically create new channels whenever the existing channels have exceeded
    /// the server's maximum number of concurrent requests (usually 100 requests per
    /// channel).  For applications where the number of concurrent requests generally
    /// stays below the server's threshold, this doesn't result in the ideal distribution
    /// of load to the servers.  Setting this value to a number greater than one
    /// ensures that there will be connections open to multiple servers, which can
    /// help improve the load distribution (and thus performance).
    /// </summary>
    public int MinNumGrpcChannels { get; }

    /// <summary>
    /// Override the default .NET GrpcChannelOptions.  (.NET only povides a strongly-typed
    /// interface for the channel options, which allows modifying specific values but does
    /// not allow the caller to use arbitrary strings to set the channel options.)
    /// </summary>
    public GrpcChannelOptions? GrpcChannelOptions { get; }

    /// <summary>
    /// Override the SocketsHttpHandler's options.
    /// This is irrelevant if the client is using the web client or the HttpClient (older .NET runtimes).
    /// </summary>
    /// <remarks>
    /// This is not part of the gRPC config because it is not part of <see cref="GrpcChannelOptions"/>.
    /// </remarks>
    public SocketsHttpHandlerOptions SocketsHttpHandlerOptions { get; }

    /// <summary>
    /// Copy constructor to override the Deadline
    /// </summary>
    /// <param name="deadline"></param>
    /// <returns>A new IGrpcConfiguration with the specified Deadline</returns>
    public IGrpcConfiguration WithDeadline(TimeSpan deadline);


    /// <summary>
    /// Copy constructor to override the minimum number of gRPC channels
    /// </summary>
    /// <param name="minNumGrpcChannels"></param>
    /// <returns>A new IGrpcConfiguration with the specified minimum number of gRPC channels</returns>
    public IGrpcConfiguration WithMinNumGrpcChannels(int minNumGrpcChannels);

    /// <summary>
    /// Copy constructor to override the channel options
    /// </summary>
    /// <param name="grpcChannelOptions"></param>
    /// <returns>A new IGrpcConfiguration with the specified channel options</returns>
    public IGrpcConfiguration WithGrpcChannelOptions(GrpcChannelOptions grpcChannelOptions);

    /// <summary>
    /// Copy constructor to override the SocketsHttpHandler's options.
    /// </summary>
    /// <param name="idleTimeout"></param>
    /// <returns></returns>
    public IGrpcConfiguration WithSocketsHttpHandlerOptions(SocketsHttpHandlerOptions idleTimeout);
}
