#if !BUILD_FOR_UNITY
using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Config;

/// <inheritdoc cref="IAuthConfiguration" />
public class AuthConfiguration : IAuthConfiguration
{
    /// <inheritdoc />
    public ILoggerFactory LoggerFactory { get; }
    /// <inheritdoc />
    public IAuthTransportStrategy TransportStrategy { get; }

    
    /// <summary>
    /// Create a new instance of an Auth Configuration object with provided arguments: <see cref="Momento.Sdk.Config.IAuthConfiguration.TransportStrategy"/>, and <see cref="Momento.Sdk.Config.IAuthConfiguration.LoggerFactory"/>
    /// </summary>
    /// <param name="transportStrategy">This is responsible for configuring network tunables.</param>
    /// <param name="loggerFactory">This is responsible for configuring logging.</param>
    public AuthConfiguration(ILoggerFactory loggerFactory, IAuthTransportStrategy transportStrategy)
    {
        LoggerFactory = loggerFactory;
        TransportStrategy = transportStrategy;
    }

    /// <inheritdoc />
    public IAuthConfiguration WithTransportStrategy(IAuthTransportStrategy transportStrategy)
    {
        return new AuthConfiguration(LoggerFactory, transportStrategy);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }

        var other = (Configuration)obj;
        return TransportStrategy.Equals(other.TransportStrategy) &&
            LoggerFactory.Equals(other.LoggerFactory);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}
#endif
