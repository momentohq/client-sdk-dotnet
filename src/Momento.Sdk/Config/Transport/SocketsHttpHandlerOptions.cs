#pragma warning disable 1591
using System;
using System.Net.Http;
using Momento.Sdk.Internal;
namespace Momento.Sdk.Config.Transport;

public class SocketsHttpHandlerOptions
{
    public static TimeSpan DefaultPooledConnectionIdleTimeout { get; } = TimeSpan.FromMinutes(1);
    public TimeSpan PooledConnectionIdleTimeout { get; } = DefaultPooledConnectionIdleTimeout;
    public bool EnableMultipleHttp2Connections { get; } = true;

    /// <summary>
    /// Override the time to wait for a response from a keepalive or ping.
    /// NOTE: keep-alives are very important for long-lived server environments where there may be periods of time
    /// when the connection is idle. However, they are very problematic for lambda environments where the lambda
    /// runtime is continuously frozen and unfrozen, because the lambda may be frozen before the "ACK" is received
    /// from the server. This can cause the keep-alive to timeout even though the connection is completely healthy.
    /// Therefore, keep-alives should be disabled in lambda and similar environments.
    /// </summary>
    public TimeSpan KeepAlivePingTimeout { get; } = TimeSpan.FromMilliseconds(1000);

    /// <summary>
    /// After a duration of this time the client/server pings its peer to see if the transport is still alive.
    /// NOTE: keep-alives are very important for long-lived server environments where there may be periods of time
    /// when the connection is idle. However, they are very problematic for lambda environments where the lambda
    /// runtime is continuously frozen and unfrozen, because the lambda may be frozen before the "ACK" is received
    /// from the server. This can cause the keep-alive to timeout even though the connection is completely healthy.
    /// Therefore, keep-alives should be disabled in lambda and similar environments.
    /// </summary>
    public TimeSpan KeepAlivePingDelay { get; } = TimeSpan.FromMilliseconds(5000);

    /// <summary>
    /// Indicates if it permissible to send keepalive pings from the client without any outstanding streams.
    /// NOTE: keep-alives are very important for long-lived server environments where there may be periods of time
    /// when the connection is idle. However, they are very problematic for lambda environments where the lambda
    /// runtime is continuously frozen and unfrozen, because the lambda may be frozen before the "ACK" is received
    /// from the server. This can cause the keep-alive to timeout even though the connection is completely healthy.
    /// Therefore, keep-alives should be disabled in lambda and similar environments.
    /// </summary>
    public bool KeepAlivePermitWithoutCalls { get; } = true;

    public SocketsHttpHandlerOptions() { }
    public SocketsHttpHandlerOptions(TimeSpan pooledConnectionIdleTimeout) : this(pooledConnectionIdleTimeout, true) { }
    public SocketsHttpHandlerOptions(bool enableMultipleHttp2Connections) : this(DefaultPooledConnectionIdleTimeout, enableMultipleHttp2Connections) { }

    public SocketsHttpHandlerOptions(TimeSpan pooledConnectionIdleTimeout, bool enableMultipleHttp2Connections)
    {
        Utils.ArgumentStrictlyPositive(pooledConnectionIdleTimeout, nameof(pooledConnectionIdleTimeout));
        PooledConnectionIdleTimeout = pooledConnectionIdleTimeout;
        EnableMultipleHttp2Connections = enableMultipleHttp2Connections;
    }
    public SocketsHttpHandlerOptions(
        TimeSpan pooledConnectionIdleTimeout, 
        bool enableMultipleHttp2Connections,
        TimeSpan keepAlivePingTimeout,
        TimeSpan keepAlivePingDelay,
        bool keepAlivePermitWithoutCalls
    )
    {
        Utils.ArgumentStrictlyPositive(pooledConnectionIdleTimeout, nameof(pooledConnectionIdleTimeout));
        PooledConnectionIdleTimeout = pooledConnectionIdleTimeout;
        EnableMultipleHttp2Connections = enableMultipleHttp2Connections;
        KeepAlivePingTimeout = keepAlivePingTimeout;
        KeepAlivePingDelay = keepAlivePingDelay;
        KeepAlivePermitWithoutCalls = keepAlivePermitWithoutCalls;
    }

    public SocketsHttpHandlerOptions WithPooledConnectionIdleTimeout(TimeSpan pooledConnectionIdleTimeout)
    {
        return new SocketsHttpHandlerOptions(pooledConnectionIdleTimeout, EnableMultipleHttp2Connections);
    }

    public SocketsHttpHandlerOptions WithEnableMultipleHttp2Connections(bool enableMultipleHttp2Connections)
    {
        return new SocketsHttpHandlerOptions(PooledConnectionIdleTimeout, enableMultipleHttp2Connections);
    }

    public static SocketsHttpHandlerOptions Of(TimeSpan pooledConnectionIdleTimeout)
    {
        return new SocketsHttpHandlerOptions(pooledConnectionIdleTimeout);
    }

    public static SocketsHttpHandlerOptions Of(bool enableMultipleHttp2Connections)
    {
        return new SocketsHttpHandlerOptions(enableMultipleHttp2Connections);
    }

    public static SocketsHttpHandlerOptions Of(TimeSpan pooledConnectionIdleTimeout, bool enableMultipleHttp2Connections)
    {
        return new SocketsHttpHandlerOptions(pooledConnectionIdleTimeout, enableMultipleHttp2Connections);
    }

    public static SocketsHttpHandlerOptions Of(
        TimeSpan pooledConnectionIdleTimeout, 
        bool enableMultipleHttp2Connections,
        TimeSpan keepAlivePingTimeout,
        TimeSpan keepAlivePingDelay,
        bool keepAlivePermitWithoutCalls
    )
    {
        return new SocketsHttpHandlerOptions(
            pooledConnectionIdleTimeout, 
            enableMultipleHttp2Connections,
            keepAlivePingTimeout,
            keepAlivePingDelay,
            keepAlivePermitWithoutCalls
        );
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (SocketsHttpHandlerOptions)obj;
        return PooledConnectionIdleTimeout.Equals(other.PooledConnectionIdleTimeout) &&
            EnableMultipleHttp2Connections.Equals(other.EnableMultipleHttp2Connections);
    }

    public override int GetHashCode()
    {
        return PooledConnectionIdleTimeout.GetHashCode() * 17 + EnableMultipleHttp2Connections.GetHashCode();
    }


}
