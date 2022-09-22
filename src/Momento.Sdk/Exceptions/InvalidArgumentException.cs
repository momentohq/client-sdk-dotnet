using System;

namespace Momento.Sdk.Exceptions;

/// <summary>
/// SDK client side validation failed.
/// </summary>
public class InvalidArgumentException : SdkException
{
    public InvalidArgumentException(string message) : base(MomentoErrorCode.INVALID_ARGUMENT_ERROR, message)
    {
    }

    public InvalidArgumentException(string message, Exception e) : base(MomentoErrorCode.INVALID_ARGUMENT_ERROR, message, null, e)
    {
    }
}
