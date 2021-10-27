using System;
namespace MomentoSdk.Exceptions
{
    public class CacheServiceException : MomentoServiceException
    {
        public CacheServiceException(String message) : base(message)
        {
        }
    }
}
