using System;
namespace MomentoSdk.Exceptions;

/// <summary>
/// SDK client side validation failed.
/// </summary>
public class InvalidArgumentException : ClientSdkException
{
    public InvalidArgumentException(string message) : base(message)
    {
    }

    public InvalidArgumentException(string message, Exception e) : base(message, e)
    {
    }
}
