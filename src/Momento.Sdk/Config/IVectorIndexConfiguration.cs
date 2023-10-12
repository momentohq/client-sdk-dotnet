using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Config;

/// <summary>
/// Contract for the Vector Index SDK configurables.
/// </summary>
public interface IVectorIndexConfiguration
{
    /// <inheritdoc cref="Microsoft.Extensions.Logging.ILoggerFactory" />
    public ILoggerFactory LoggerFactory { get; }
    /// <inheritdoc cref="Momento.Sdk.Config.Transport.IVectorIndexTransportStrategy" />
    public IVectorIndexTransportStrategy TransportStrategy { get; }

    /// <summary>
    /// Creates a new instance of the VectorIndexConfiguration object, updated to use the specified transport strategy.
    /// </summary>
    /// <param name="transportStrategy">This is responsible for configuring network tunables.</param>
    /// <returns>A VectorIndexConfiguration object using the provided transport strategy.</returns>
    public IVectorIndexConfiguration WithTransportStrategy(IVectorIndexTransportStrategy transportStrategy);
}
