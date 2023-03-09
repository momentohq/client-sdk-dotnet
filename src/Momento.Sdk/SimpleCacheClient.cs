using System;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;

namespace Momento.Sdk
{

    /// <summary>
    /// Client to perform control and data operations against the Momento Serverless Cache Service.
    /// 
    /// See <see href="https://github.com/momentohq/client-sdk-dotnet/tree/main/examples">the examples directory</see> for complete workflows.
    /// </summary>
    [Obsolete("This class has been renamed to CacheClient, and is now deprecated")]
    public class SimpleCacheClient : CacheClient
	{
        /// <summary>
        /// Client to perform operations against the Momento Serverless Cache Service
        /// </summary>
        /// <param name="config">Configuration to use for the transport, retries, middlewares. See <see cref="Configurations"/> for out-of-the-box configuration choices, eg <see cref="Configurations.Laptop.Latest"/></param>
        /// <param name="authProvider">Momento auth provider.</param>
        /// <param name="defaultTtl">Default time to live for the item in cache.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="defaultTtl"/> is zero or negative.</exception>
        [Obsolete("This class has been renamed to CacheClient, and is now deprecated")]
        public SimpleCacheClient(IConfiguration config, ICredentialProvider authProvider, TimeSpan defaultTtl) : base(config, authProvider, defaultTtl)
		{
		}
	}
}

