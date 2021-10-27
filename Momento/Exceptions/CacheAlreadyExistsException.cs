using System;
namespace MomentoSdk.Exceptions
{
    public class CacheAlreadyExistsException : CacheServiceException
    {
        public CacheAlreadyExistsException(String message) : base(message)
        {
        }
    }
}
