using System;
namespace MomentoSdk.Exceptions
{
    /// <summary>
    /// This is the base class for all exceptions thrown by the Cache Service. This exception
    /// will be thrown if there is an unknown exception thrown by the Cache Service.
    /// </summary>
    abstract class CacheServiceException : MomentoServiceException
    {
        public CacheServiceException(String message) : base(message)
        {
        }
    }
}
