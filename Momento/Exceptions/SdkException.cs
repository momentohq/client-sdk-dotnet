using System;

namespace MomentoSdk.Exceptions;

public abstract class SdkException : Exception
{
    protected SdkException(string message) : base(message)
    {
    }
    protected SdkException(string message, Exception e) : base(message, e)
    {
    }
}
