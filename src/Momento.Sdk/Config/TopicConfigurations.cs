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
                grpcConfig: new StaticGrpcConfiguration(deadline: TimeSpan.FromMilliseconds(15000)),
                keepAlivePermitWithoutCalls: true,
                keepAlivePingDelay: TimeSpan.FromMilliseconds(5000),
                keepAlivePingTimeout: TimeSpan.FromMilliseconds(1000)
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
                grpcConfig: new StaticGrpcConfiguration(deadline: TimeSpan.FromMilliseconds(15000)),
                keepAlivePermitWithoutCalls: true,
                keepAlivePingDelay: TimeSpan.FromMilliseconds(5000),
                keepAlivePingTimeout: TimeSpan.FromMilliseconds(1000)
            );
            return new Mobile(finalLoggerFactory, transportStrategy);
        }
    }

    /// <summary>
    /// InRegion provides defaults suitable for an environment where your client is running in the same region as the Momento
    /// service.  It has more agressive timeouts and retry behavior than the Laptop config.
    /// </summary>
    public class InRegion
    {
        /// <summary>
        ///  This config prioritizes throughput and client resource utilization.
        /// </summary>
        public class Default : TopicConfiguration
        {
            private Default(ILoggerFactory loggerFactory, ITopicTransportStrategy transportStrategy)
                : base(loggerFactory, transportStrategy)
            {

            }

            /// <summary>
            /// Provides the latest recommended configuration for an InRegion environment.
            /// </summary>
            /// <remark>
            /// This configuration may change in future releases to take advantage of
            /// improvements we identify for default configurations.
            /// </remark>
            /// <param name="loggerFactory"></param>
            /// <returns></returns>
            public static ITopicConfiguration Latest(ILoggerFactory? loggerFactory = null)
            {
                var finalLoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
                ITopicTransportStrategy transportStrategy = new StaticTopicTransportStrategy(
                    loggerFactory: finalLoggerFactory,
                    grpcConfig: new StaticGrpcConfiguration(deadline: TimeSpan.FromMilliseconds(1100)),
                    keepAlivePermitWithoutCalls: true,
                    keepAlivePingDelay: TimeSpan.FromMilliseconds(5000),
                    keepAlivePingTimeout: TimeSpan.FromMilliseconds(1000)
                );
                return new Default(finalLoggerFactory, transportStrategy);
            }
        }

        /// <summary>
        /// This config prioritizes keeping p99.9 latencies as low as possible, potentially sacrificing
        /// some throughput to achieve this.  Use this configuration if the most important factor is to ensure that
        /// unavailability doesn't force unacceptably high latencies for your own application.
        /// </summary>
        public class LowLatency : TopicConfiguration
        {
            private LowLatency(ILoggerFactory loggerFactory, ITopicTransportStrategy transportStrategy)
                : base(loggerFactory, transportStrategy)
            {

            }

            /// <summary>
            /// Provides the latest recommended configuration for a low-latency in-region
            /// environment.
            /// </summary>
            /// <remark>
            /// This configuration may change in future releases to take advantage of
            /// improvements we identify for default configurations.
            /// </remark>
            /// <param name="loggerFactory"></param>
            /// <returns></returns>
            public static ITopicConfiguration Latest(ILoggerFactory? loggerFactory = null)
            {
                var finalLoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
                ITopicTransportStrategy transportStrategy = new StaticTopicTransportStrategy(
                    loggerFactory: finalLoggerFactory,
                    grpcConfig: new StaticGrpcConfiguration(deadline: TimeSpan.FromMilliseconds(500)),
                    keepAlivePermitWithoutCalls: true,
                    keepAlivePingDelay: TimeSpan.FromMilliseconds(5000),
                    keepAlivePingTimeout: TimeSpan.FromMilliseconds(1000)
                );
                return new LowLatency(finalLoggerFactory, transportStrategy);
            }
        }
    }
}
