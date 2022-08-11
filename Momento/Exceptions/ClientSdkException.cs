using System;

namespace Momento.Sdk.Exceptions;

public class ClientSdkException : SdkException
{
    public ClientSdkException(string message) : base(message)
    {
    }
    public ClientSdkException(string message, Exception e) : base(message, e)
    {
    }
}
