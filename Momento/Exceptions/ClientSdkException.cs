using System;

namespace MomentoSdk.Exceptions
{
    public class ClientSdkException : SdkException
    {
        public ClientSdkException(string message) : base(message)
        {
        }

        public ClientSdkException(string message, Exception e) : base(message, e)
        {
        }
    }
}
