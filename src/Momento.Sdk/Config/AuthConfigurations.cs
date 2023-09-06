using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Config;

/// <summary>
/// Provide pre-built auth configurations.
/// </summary>
public class AuthConfigurations
{
    /// <summary>
    /// Default config provides defaults with permissive timeouts suitable for a medium-to-high-latency environment.
    /// </summary>
    public class Default : AuthConfiguration
    {
        private Default(ILoggerFactory loggerFactory, IAuthTransportStrategy transportStrategy) : base(loggerFactory,
            transportStrategy)
        {
        }

        /// <summary>
        /// Provides the latest recommended default configuration.
        /// </summary>
        /// <remark>
        /// This configuration may change in future releases to take advantage of
        /// improvements we identify for default configurations.
        /// </remark>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public static IAuthConfiguration latest(ILoggerFactory? loggerFactory = null)
        {
            var finalLoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            IAuthTransportStrategy transportStrategy = new StaticAuthTransportStrategy(
                loggerFactory: finalLoggerFactory,
                grpcConfig: new StaticGrpcConfiguration(deadline: TimeSpan.FromMilliseconds(15000))
            );
            return new Default(finalLoggerFactory, transportStrategy);
        }
    }
}