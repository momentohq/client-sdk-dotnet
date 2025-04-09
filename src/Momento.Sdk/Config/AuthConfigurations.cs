using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Momento.Sdk.Config.Transport;
using System;

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
        /// Provides version 1 of the recommended default configuration.
        /// </summary>
        /// <remark>
        /// This configuration is guaranteed not to change in future
        /// releases of the Momento .NET SDK.
        /// </remark>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public static IAuthConfiguration V1(ILoggerFactory? loggerFactory = null)
        {
            var finalLoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            IAuthTransportStrategy transportStrategy = new StaticAuthTransportStrategy(
                loggerFactory: finalLoggerFactory,
                grpcConfig: new StaticGrpcConfiguration(deadline: TimeSpan.FromMilliseconds(15000))
            );
            return new Default(finalLoggerFactory, transportStrategy);
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
        public static IAuthConfiguration Latest(ILoggerFactory? loggerFactory = null)
        {
            return V1(loggerFactory);
        }
    }
}
