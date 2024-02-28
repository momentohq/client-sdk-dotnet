#pragma warning disable 1591
using System;
using Momento.Sdk.Internal;
namespace Momento.Sdk.Config.Transport;

public class SocketsHttpHandlerOptions
{
    public static TimeSpan DefaultPooledConnectionIdleTimeout { get; } = TimeSpan.FromMinutes(1);
    public TimeSpan PooledConnectionIdleTimeout { get; } = DefaultPooledConnectionIdleTimeout;
    public bool EnableMultipleHttp2Connections { get; } = true;

    public SocketsHttpHandlerOptions() { }
    public SocketsHttpHandlerOptions(TimeSpan pooledConnectionIdleTimeout) : this(pooledConnectionIdleTimeout, true) { }
    public SocketsHttpHandlerOptions(bool enableMultipleHttp2Connections) : this(DefaultPooledConnectionIdleTimeout, enableMultipleHttp2Connections) { }

    public SocketsHttpHandlerOptions(TimeSpan pooledConnectionIdleTimeout, bool enableMultipleHttp2Connections)
    {
        Utils.ArgumentStrictlyPositive(pooledConnectionIdleTimeout, nameof(pooledConnectionIdleTimeout));
        PooledConnectionIdleTimeout = pooledConnectionIdleTimeout;
        EnableMultipleHttp2Connections = enableMultipleHttp2Connections;
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
