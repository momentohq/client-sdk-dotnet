using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Config;

/// <inheritdoc cref="IVectorIndexConfiguration" />
public class VectorIndexConfiguration : IVectorIndexConfiguration
{
    /// <inheritdoc />
    public ILoggerFactory LoggerFactory { get; }
    /// <inheritdoc />
    public IVectorIndexTransportStrategy TransportStrategy { get; }

    
    /// <summary>
    /// Create a new instance of a Vector Index Configuration object with provided arguments:
    /// <see cref="Momento.Sdk.Config.IVectorIndexConfiguration.TransportStrategy"/>,
    /// and <see cref="Momento.Sdk.Config.IVectorIndexConfiguration.LoggerFactory"/>
    /// </summary>
    /// <param name="transportStrategy">This is responsible for configuring network tunables.</param>
    /// <param name="loggerFactory">This is responsible for configuring logging.</param>
    public VectorIndexConfiguration(ILoggerFactory loggerFactory, IVectorIndexTransportStrategy transportStrategy)
    {
        LoggerFactory = loggerFactory;
        TransportStrategy = transportStrategy;
    }

    /// <inheritdoc />
    public IVectorIndexConfiguration WithTransportStrategy(IVectorIndexTransportStrategy transportStrategy)
    {
        return new VectorIndexConfiguration(LoggerFactory, transportStrategy);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }

        var other = (VectorIndexConfiguration)obj;
        return TransportStrategy.Equals(other.TransportStrategy) &&
            LoggerFactory.Equals(other.LoggerFactory);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}
