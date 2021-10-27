using System;
namespace MomentoSdk.Exceptions
{
    /// <summary>
    /// Exception thrown when trying to delete a cache that doesn't exist.
    /// </summary>
    public class CacheNotFoundException : CacheServiceException
    {
        public CacheNotFoundException(String message) : base(message)
        {
        }
    }
}
