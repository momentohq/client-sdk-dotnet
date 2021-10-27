using System;
namespace MomentoSdk.Exceptions
{
    public class InvalidCacheNameException : ClientSdkException
    {
        public InvalidCacheNameException(String message) : base(message)
        {
        }
    }
}
