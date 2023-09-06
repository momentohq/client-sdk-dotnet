using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Config;


/// <summary>
/// Contract for Topic SDK configurables.
/// </summary>
public interface ITopicConfiguration
{
    /// <inheritdoc cref="Microsoft.Extensions.Logging.ILoggerFactory" />
    public ILoggerFactory LoggerFactory { get; }
    /// <inheritdoc cref="Momento.Sdk.Config.Transport.ITransportStrategy" />
    public ITopicTransportStrategy TransportStrategy { get; }

    /// <summary>
    /// Creates a new instance of the Configuration object, updated to use the specified transport strategy.
    /// </summary>
    /// <param name="transportStrategy">This is responsible for configuring network tunables.</param>
    /// <returns>Configuration object with custom transport strategy provided</returns>
    public ITopicConfiguration WithTransportStrategy(ITopicTransportStrategy transportStrategy);
}
