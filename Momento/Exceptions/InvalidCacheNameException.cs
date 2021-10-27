using System;
namespace MomentoSdk.Exceptions
{
    /// <summary>
    /// This exception gets thrown when trying to get or create a cache with a name
    /// that does not conform to our naming standards. Cache names must be not be
    /// empty or null
    /// </summary>
    public class InvalidCacheNameException : ClientSdkException
    {
        public InvalidCacheNameException(String message) : base(message)
        {
        }
    }
}
