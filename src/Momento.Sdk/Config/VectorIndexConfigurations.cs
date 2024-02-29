using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Config;

/// <summary>
/// Provide pre-built vector index client configurations.
/// </summary>
public class VectorIndexConfigurations
{
    /// <summary>
    /// Laptop config provides defaults suitable for a medium-to-high-latency dev environment. Permissive timeouts, retries, and
    /// relaxed latency and throughput targets.
    /// </summary>
    public class Laptop : VectorIndexConfiguration
    {
        private Laptop(ILoggerFactory loggerFactory, IVectorIndexTransportStrategy transportStrategy)
            : base(loggerFactory, transportStrategy)
        {

        }

        /// <summary>
        /// Provides the latest recommended configuration for a Laptop environment.
        /// </summary>
        /// <remark>
        /// This configuration may change in future releases to take advantage of
        /// improvements we identify for default configurations.
        /// </remark>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public static IVectorIndexConfiguration Latest(ILoggerFactory? loggerFactory = null)
        {
            return V1(loggerFactory);
        }

        /// <summary>
        /// Provides version 1 configuration for a Laptop environment.
        /// </summary>
        /// <remark>
        /// This configuration is guaranteed not to change in future
        /// releases of the Momento .NET SDK.
        /// </remark>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public static IVectorIndexConfiguration V1(ILoggerFactory? loggerFactory = null)
        {
            var finalLoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            IVectorIndexTransportStrategy transportStrategy = new StaticVectorIndexTransportStrategy(
                loggerFactory: finalLoggerFactory,
                grpcConfig: new StaticGrpcConfiguration(deadline: TimeSpan.FromMilliseconds(15000)),
                keepAlivePermitWithoutCalls: true,
                keepAlivePingDelay: TimeSpan.FromMilliseconds(5000),
                keepAlivePingTimeout: TimeSpan.FromMilliseconds(1000)
            );
            return new Laptop(finalLoggerFactory, transportStrategy);
        }
    }
}
