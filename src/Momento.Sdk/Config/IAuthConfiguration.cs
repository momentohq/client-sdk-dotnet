using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Config;

/// <summary>
/// Contract for Auth SDK configurables.
/// </summary>
public interface IAuthConfiguration
{
    /// <inheritdoc cref="Microsoft.Extensions.Logging.ILoggerFactory" />
    public ILoggerFactory LoggerFactory { get; }
    /// <inheritdoc cref="Momento.Sdk.Config.Transport.IAuthTransportStrategy" />
    public IAuthTransportStrategy TransportStrategy { get; }

    /// <summary>
    /// Creates a new instance of the Configuration object, updated to use the specified transport strategy.
    /// </summary>
    /// <param name="transportStrategy">This is responsible for configuring network tunables.</param>
    /// <returns>Configuration object with custom transport strategy provided</returns>
    public IAuthConfiguration WithTransportStrategy(IAuthTransportStrategy transportStrategy);
}
