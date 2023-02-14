using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Retry;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Config;

/// <summary>
/// Provide pre-built configurations.
/// </summary>
public class Configurations
{
    /// <summary>
    /// Laptop config provides defaults suitable for a medium-to-high-latency dev environment.  Permissive timeouts, retries, and
    /// relaxed latency and throughput targets.
    /// </summary>
    public class Laptop : Configuration
    {
        private Laptop(ILoggerFactory loggerFactory, IRetryStrategy retryStrategy, ITransportStrategy transportStrategy)
            : base(loggerFactory, retryStrategy, transportStrategy)
        {

        }

        /// <summary>
        /// Provides the latest recommended configuration for a Laptop environment.
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public static IConfiguration Latest(ILoggerFactory? loggerFactory = null)
        {
            var finalLoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            IRetryStrategy retryStrategy = new FixedCountRetryStrategy(finalLoggerFactory, maxAttempts: 3);
            ITransportStrategy transportStrategy = new StaticTransportStrategy(
                loggerFactory: finalLoggerFactory,
                maxConcurrentRequests: 100,
                grpcConfig: new StaticGrpcConfiguration(deadline: TimeSpan.FromMilliseconds(15000))
            );
            return new Laptop(finalLoggerFactory, retryStrategy, transportStrategy);
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
        public class Default : Configuration
        {
            private Default(ILoggerFactory loggerFactory, IRetryStrategy retryStrategy, ITransportStrategy transportStrategy)
                : base(loggerFactory, retryStrategy, transportStrategy)
            {

            }

            /// <summary>
            /// Provides the latest recommended configuration for an InRegion environment.
            /// </summary>
            /// <param name="loggerFactory"></param>
            /// <returns></returns>
            public static IConfiguration Latest(ILoggerFactory? loggerFactory = null)
            {
                var finalLoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
                IRetryStrategy retryStrategy = new FixedCountRetryStrategy(finalLoggerFactory, maxAttempts: 3);
                ITransportStrategy transportStrategy = new StaticTransportStrategy(
                    loggerFactory: finalLoggerFactory,
                    maxConcurrentRequests: 200,
                    grpcConfig: new StaticGrpcConfiguration(deadline: TimeSpan.FromMilliseconds(1100)));
                return new Default(finalLoggerFactory, retryStrategy, transportStrategy);
            }
        }

        /// <summary>
        /// This config prioritizes keeping p99.9 latencies as low as possible, potentially sacrificing
        /// some throughput to achieve this.  Use this configuration if the most important factor is to ensure that cache
        /// unavailability doesn't force unacceptably high latencies for your own application.
        /// </summary>
        public class LowLatency : Configuration
        {
            private LowLatency(ILoggerFactory loggerFactory, IRetryStrategy retryStrategy, ITransportStrategy transportStrategy)
                : base(loggerFactory, retryStrategy, transportStrategy)
            {

            }

            /// <summary>
            /// Provides the latest recommended configuration for a low-latency in-region
            /// environment.
            /// </summary>
            /// <param name="loggerFactory"></param>
            /// <returns></returns>
            public static IConfiguration Latest(ILoggerFactory? loggerFactory = null)
            {
                var finalLoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
                IRetryStrategy retryStrategy = new FixedCountRetryStrategy(finalLoggerFactory, maxAttempts: 3);
                ITransportStrategy transportStrategy = new StaticTransportStrategy(
                    loggerFactory: finalLoggerFactory,
                    maxConcurrentRequests: 20,
                    grpcConfig: new StaticGrpcConfiguration(deadline: TimeSpan.FromMilliseconds(500))
                );
                return new LowLatency(finalLoggerFactory, retryStrategy, transportStrategy);
            }
        }
    }
}
