using System.Collections.Generic;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Retry;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Config;

public class PreBuiltConfigurations
{
    /// <summary>
    /// Laptop config provides defaults suitable for a medium-to-high-latency dev environment.  Permissive timeouts, retries, potentially
    /// a higher number of connections, etc.
    /// </summary>
    public class Laptop : Configuration
    {
        private Laptop(IRetryStrategy retryStrategy, IList<IMiddleware> middlewares, ITransportStrategy transportStrategy)
            : base(retryStrategy, middlewares, transportStrategy)
        {

        }

        public static Laptop Latest
        {
            get
            {
                /*retryableStatusCodes = DEFAULT_RETRYABLE_STATUS_CODES,*/
                IRetryStrategy retryStrategy = new FixedCountRetryStrategy(maxAttempts: 3);
                ITransportStrategy transportStrategy = new StaticTransportStrategy(
                    maxConcurrentRequests: 1,
                    grpcConfig: new StaticGrpcConfiguration(numChannels: 6, maxSessionMemory: 128, useLocalSubChannelPool: true));
                return new Laptop(retryStrategy, new List<IMiddleware>(), transportStrategy);
            }
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
            private Default(IRetryStrategy retryStrategy, IList<IMiddleware> middlewares, ITransportStrategy transportStrategy)
                : base(retryStrategy, middlewares, transportStrategy)
            {

            }

            public static Default Latest
            {
                get
                {
                    /*retryableStatusCodes = DEFAULT_RETRYABLE_STATUS_CODES,*/
                    IRetryStrategy retryStrategy = new FixedCountRetryStrategy(maxAttempts: 3);
                    ITransportStrategy transportStrategy = new StaticTransportStrategy(
                        maxConcurrentRequests: 1,
                        grpcConfig: new StaticGrpcConfiguration(numChannels: 6, maxSessionMemory: 128, useLocalSubChannelPool: true));
                    return new Default(retryStrategy, new List<IMiddleware>(), transportStrategy);
                }
            }
        }

        /// <summary>
        /// This config prioritizes keeping p99.9 latencies as low as possible, potentially sacrificing
        /// some throughput to achieve this.  Use this configuration if the most important factor is to ensure that cache
        /// unavailability doesn't force unacceptably high latencies for your own application.
        /// </summary>
        public class LowLatency : Configuration
        {
            private LowLatency(IRetryStrategy retryStrategy, IList<IMiddleware> middlewares, ITransportStrategy transportStrategy)
                : base(retryStrategy, middlewares, transportStrategy)
            {

            }

            public static LowLatency Latest
            {
                get
                {
                    /*retryableStatusCodes = DEFAULT_RETRYABLE_STATUS_CODES,*/
                    IRetryStrategy retryStrategy = new FixedCountRetryStrategy(maxAttempts: 3);
                    ITransportStrategy transportStrategy = new StaticTransportStrategy(
                        maxConcurrentRequests: 1,
                        grpcConfig: new StaticGrpcConfiguration(numChannels: 6, maxSessionMemory: 128, useLocalSubChannelPool: true));
                    return new LowLatency(retryStrategy, new List<IMiddleware>(), transportStrategy);
                }
            }
        }
    }

    /// <inheritDoc cref="Laptop" />
    public static readonly Configuration DevConfig = Laptop.Latest;
    /// <inheritDoc cref="InRegion.Default" />
    public static readonly Configuration ProdConfig = InRegion.Default.Latest;
    /// <inheritDoc cref="InRegion.LowLatency" />    
    public static readonly Configuration ProdLowLatencyConfig = InRegion.LowLatency.Latest;
}