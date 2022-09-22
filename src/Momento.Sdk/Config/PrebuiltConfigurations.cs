using System.Collections.Generic;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Retry;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Config;

public class PrebuiltConfigurations
{
    /// <summary>
    /// Laptop config provides defaults suitable for a medium-to-high-latency dev environment.  Permissive timeouts, retries, potentially
    /// a higher number of connections, etc.
    /// </summary>
    public class Laptop : BaseConfiguration
    {
        private Laptop(IRetryStrategy retryStrategy, ITransportStrategy transportStrategy)
            : base(retryStrategy, transportStrategy)
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
                return new Laptop(retryStrategy, transportStrategy);
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
        public class Default : BaseConfiguration
        {
            private Default(IRetryStrategy retryStrategy, ITransportStrategy transportStrategy)
                : base(retryStrategy, transportStrategy)
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
                    return new Default(retryStrategy, transportStrategy);
                }
            }
        }

        /// <summary>
        /// This config prioritizes keeping p99.9 latencies as low as possible, potentially sacrificing
        /// some throughput to achieve this.  Use this configuration if the most important factor is to ensure that cache
        /// unavailability doesn't force unacceptably high latencies for your own application.
        /// </summary>
        public class LowLatency : BaseConfiguration
        {
            private LowLatency(IRetryStrategy retryStrategy, ITransportStrategy transportStrategy)
                : base(retryStrategy, transportStrategy)
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
                    return new LowLatency(retryStrategy, transportStrategy);
                }
            }
        }
    }

    /// <inheritDoc cref="Laptop" />
    public static readonly IConfiguration DevConfig = Laptop.Latest;
    /// <inheritDoc cref="InRegion.Default" />
    public static readonly IConfiguration ProdConfig = InRegion.Default.Latest;
    /// <inheritDoc cref="InRegion.LowLatency" />    
    public static readonly IConfiguration ProdLowLatencyConfig = InRegion.LowLatency.Latest;
}
