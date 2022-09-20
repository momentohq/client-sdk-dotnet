namespace Momento.Sdk.Exceptions;

using System;

/// <summary>
/// Unhandled exception from CacheExceptionMapper
/// </summary>
public class UnknownException : SdkException
{
    public UnknownException(string message, MomentoErrorCode errorCode) : base(message, errorCode)
    {
    }
    public UnknownException(string message, Exception e) : base(message, MomentoErrorCode.UNKNOWN_ERROR, null, e)
    {
    }
}
