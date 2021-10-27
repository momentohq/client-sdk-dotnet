using System;
namespace MomentoSdk.Exceptions
{
    public abstract class SdkException : Exception
    {
        protected SdkException(String message) : base(message)
        {
        }
    }
}
