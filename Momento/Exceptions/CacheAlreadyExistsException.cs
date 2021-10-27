using System;
namespace MomentoSdk.Exceptions
{
    /// <summary>
    /// Exception thrown when trying to create a cache that already exists. Caches
    /// must have a unique name.
    /// </summary>
    public class CacheAlreadyExistsException : CacheServiceException
    {
        public CacheAlreadyExistsException(String message) : base(message)
        {
        }
    }
}
