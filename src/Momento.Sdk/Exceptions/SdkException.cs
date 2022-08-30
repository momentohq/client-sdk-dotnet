using System;

namespace Momento.Sdk.Exceptions;

public abstract class SdkException : Exception
{
    protected SdkException(string message) : base(message)
    {
    }
    protected SdkException(string message, Exception e) : base(message, e)
    {
    }
}
