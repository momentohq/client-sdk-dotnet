using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Config;

/// <summary>
/// Provide pre-built topic configurations.
/// </summary>
public class TopicConfigurations
{
    /// <summary>
    /// Laptop config provides defaults suitable for a medium-to-high-latency environment. Permissive timeouts, retries, and
    /// relaxed latency and throughput targets.
    /// </summary>
    public class Laptop : TopicConfiguration
    {
        private Laptop(ILoggerFactory loggerFactory, ITopicTransportStrategy transportStrategy) : base(loggerFactory,
            transportStrategy)
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
        public static ITopicConfiguration latest(ILoggerFactory? loggerFactory = null)
        {
            var finalLoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            ITopicTransportStrategy transportStrategy = new StaticTopicTransportStrategy(
                loggerFactory: finalLoggerFactory,
                grpcConfig: new StaticGrpcConfiguration(deadline: TimeSpan.FromMilliseconds(15000))
            );
            return new Laptop(finalLoggerFactory, transportStrategy);
        }
    }

    /// <summary>
    /// Mobile config provides defaults suitable for a medium-to-high-latency mobile environment.
    /// </summary>
    public class Mobile : TopicConfiguration
    {
        private Mobile(ILoggerFactory loggerFactory, ITopicTransportStrategy transportStrategy) : base(loggerFactory,
            transportStrategy)
        {
        }

        /// <summary>
        /// Provides the latest recommended configuration for a Mobile environment.
        /// </summary>
        /// <remark>
        /// This configuration may change in future releases to take advantage of
        /// improvements we identify for default configurations.
        /// </remark>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public static ITopicConfiguration latest(ILoggerFactory? loggerFactory = null)
        {
            var finalLoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            ITopicTransportStrategy transportStrategy = new StaticTopicTransportStrategy(
                loggerFactory: finalLoggerFactory,
                grpcConfig: new StaticGrpcConfiguration(deadline: TimeSpan.FromMilliseconds(15000))
            );
            return new Mobile(finalLoggerFactory, transportStrategy);
        }
    }
}