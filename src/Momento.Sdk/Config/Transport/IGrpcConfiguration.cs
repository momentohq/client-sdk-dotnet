using System.Collections.Generic;

namespace Momento.Sdk.Config.Transport;

/// <summary>
/// Abstracts away the gRPC configuration tunables.
/// 
/// <see cref="IGrpcConfiguration.MaxSessionMemory" /> and <see cref="IGrpcConfiguration.UseLocalSubChannelPool" /> expose individual settings for gRPC channels.  They are just here to ensure that
/// strategy implementations provide values for settings that we know to be important.  These may vary by language
/// since the gRPC implementations in each language have subtly different behaviors.
/// </summary>
public interface IGrpcConfiguration
{
    public int NumChannels { get; }

    public int MaxSessionMemory { get; }
    public bool UseLocalSubChannelPool { get; }

    /// <summary>
    /// This is a dictionary that encapsulates the settings above, and may also include other channel-specific settings.
    /// This allows strategy implementations to provide gRPC config key/value pairs for any available setting, even
    /// if it's not one we've explicitly tried / recommended.  The strategy implementation should implement this by
    /// calling the functions above, along with allowing a mechanism for specifying additional key/value pairs.
    /// </summary>
    public IDictionary<string, string> GrpcChannelConfig { get; }
}
