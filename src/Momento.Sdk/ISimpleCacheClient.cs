using System;
namespace Momento.Sdk
{
    /// <summary>
    /// Minimum viable functionality of a cache client.
    ///
    /// Includes control operations and data operations.
    /// </summary>
    [Obsolete("This interface has been renamed to ICacheClient, and is now deprecated")]
    public interface ISimpleCacheClient : ICacheClient
	{
	}
}

