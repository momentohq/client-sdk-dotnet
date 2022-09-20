using System;

namespace Momento.Sdk.Exceptions;

/// <summary>
/// SDK client side validation failed.
/// </summary>
public class InvalidArgumentException : SdkException
{
    public InvalidArgumentException(string message) : base(message, MomentoErrorCode.INVALID_ARGUMENT_ERROR)
    {
    }

    public InvalidArgumentException(string message, Exception e) : base(message, MomentoErrorCode.INVALID_ARGUMENT_ERROR, null, e)
    {
    }
}
