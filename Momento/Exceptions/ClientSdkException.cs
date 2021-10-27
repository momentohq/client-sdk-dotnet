using System;
namespace MomentoSdk.Exceptions
{
    public abstract class ClientSdkException : SdkException
    {
        protected ClientSdkException(String message) : base(message)
        {
        }
    }
}
