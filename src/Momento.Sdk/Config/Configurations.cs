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
        /// <remark>
        /// This configuration may change in future releases to take advantage of
        /// improvements we identify for default configurations.
        /// </remark>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public static IConfiguration Latest(ILoggerFactory? loggerFactory = null)
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
        public static IConfiguration V1(ILoggerFactory? loggerFactory = null)
        {
            var finalLoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            IRetryStrategy retryStrategy = new FixedCountRetryStrategy(finalLoggerFactory, maxAttempts: 3);
            ITransportStrategy transportStrategy = new StaticTransportStrategy(
                loggerFactory: finalLoggerFactory,
                maxConcurrentRequests: 200, // max of 2 connections https://github.com/momentohq/client-sdk-dotnet/issues/460
                grpcConfig: new StaticGrpcConfiguration(
                    deadline: TimeSpan.FromMilliseconds(15000),
                    keepAlivePermitWithoutCalls: true,
                    keepAlivePingDelay: TimeSpan.FromMilliseconds(5000),
                    keepAlivePingTimeout: TimeSpan.FromMilliseconds(1000)
                )
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
            /// <remark>
            /// This configuration may change in future releases to take advantage of
            /// improvements we identify for default configurations.
            /// </remark>
            /// <param name="loggerFactory"></param>
            /// <returns></returns>
            public static IConfiguration Latest(ILoggerFactory? loggerFactory = null)
            {
                return V1(loggerFactory);
            }

            /// <summary>
            /// Provides the version 1 configuration for an InRegion environment.
            /// </summary>
            /// <remark>
            /// This configuration is guaranteed not to change in future
            /// releases of the Momento .NET SDK.
            /// </remark>
            /// <param name="loggerFactory"></param>
            /// <returns></returns>
            public static IConfiguration V1(ILoggerFactory? loggerFactory = null)
            {
                var finalLoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
                IRetryStrategy retryStrategy = new FixedCountRetryStrategy(finalLoggerFactory, maxAttempts: 3);
                ITransportStrategy transportStrategy = new StaticTransportStrategy(
                    loggerFactory: finalLoggerFactory,
                    maxConcurrentRequests: 400, // max of 4 connections https://github.com/momentohq/client-sdk-dotnet/issues/460
                    grpcConfig: new StaticGrpcConfiguration(
                        deadline: TimeSpan.FromMilliseconds(1100),
                        keepAlivePermitWithoutCalls: true,
                        keepAlivePingDelay: TimeSpan.FromMilliseconds(5000),
                        keepAlivePingTimeout: TimeSpan.FromMilliseconds(1000)
                    )
                );
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
            /// <remark>
            /// This configuration may change in future releases to take advantage of
            /// improvements we identify for default configurations.
            /// </remark>
            /// <param name="loggerFactory"></param>
            /// <returns></returns>
            public static IConfiguration Latest(ILoggerFactory? loggerFactory = null)
            {
                return V1(loggerFactory);
            }

            /// <summary>
            /// Provides the latest recommended configuration for a low-latency in-region
            /// environment.
            /// </summary>
            /// <remark>
            /// This configuration is guaranteed not to change in future
            /// releases of the Momento .NET SDK.
            /// </remark>
            /// <param name="loggerFactory"></param>
            /// <returns></returns>
            public static IConfiguration V1(ILoggerFactory? loggerFactory = null)
            {
                var finalLoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
                IRetryStrategy retryStrategy = new FixedCountRetryStrategy(finalLoggerFactory, maxAttempts: 3);
                ITransportStrategy transportStrategy = new StaticTransportStrategy(
                    loggerFactory: finalLoggerFactory,
                    maxConcurrentRequests: 20,
                    grpcConfig: new StaticGrpcConfiguration(
                        deadline: TimeSpan.FromMilliseconds(500),
                        keepAlivePermitWithoutCalls: true,
                        keepAlivePingDelay: TimeSpan.FromMilliseconds(5000),
                        keepAlivePingTimeout: TimeSpan.FromMilliseconds(1000)
                    )
                );
                return new LowLatency(finalLoggerFactory, retryStrategy, transportStrategy);
            }
        }

        /// <summary>
        /// This config optimizes for lambda environments. In addition to the in region settings of
        /// <see cref="Default"/>, this configures the clients to eagerly connect to the Momento service
        /// to avoid the cold start penalty of establishing a connection on the first request.
        /// NOTE: keep-alives are very important for long-lived server environments where there may be periods of time
        /// when the connection is idle. However, they are very problematic for lambda environments where the lambda
        /// runtime is continuously frozen and unfrozen, because the lambda may be frozen before the "ACK" is received
        /// from the server. This can cause the keep-alive to timeout even though the connection is completely healthy.
        /// Therefore, keep-alives should be disabled in lambda and similar environments.
        /// </summary>
        public class Lambda : Configuration
        {
            private Lambda(ILoggerFactory loggerFactory, IRetryStrategy retryStrategy, ITransportStrategy transportStrategy)
                : base(loggerFactory, retryStrategy, transportStrategy)
            {

            }

            /// <summary>
            /// Provides the latest recommended configuration for a lambda environment.
            /// </summary>
            /// <param name="loggerFactory"></param>
            /// <returns></returns>
            public static IConfiguration V1(ILoggerFactory? loggerFactory = null)
            {
                var config = Default.V1(loggerFactory);
                var transportStrategy = config.TransportStrategy.WithSocketsHttpHandlerOptions(
                    SocketsHttpHandlerOptions.Of(pooledConnectionIdleTimeout: TimeSpan.FromMinutes(6)));
                return config.WithTransportStrategy(transportStrategy);
            }

            /// <summary>
            /// Provides the latest recommended configuration for a lambda environment.
            /// </summary>
            /// <param name="loggerFactory"></param>
            /// <returns></returns>
            public static IConfiguration Latest(ILoggerFactory? loggerFactory = null)
            {
                return V1(loggerFactory);
            }
        }
    }
}
