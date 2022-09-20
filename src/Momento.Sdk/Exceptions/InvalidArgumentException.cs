using System;

namespace Momento.Sdk.Exceptions;

/// <summary>
/// SDK client side validation failed.
/// </summary>
public class InvalidArgumentException : SdkException
{
    public InvalidArgumentException(string message) : base(message)
    {
    }

    public InvalidArgumentException(string message, Exception e) : base(message, e)
    {
    }
}
