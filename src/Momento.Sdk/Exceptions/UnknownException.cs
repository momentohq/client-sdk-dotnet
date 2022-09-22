namespace Momento.Sdk.Exceptions;

using System;

/// <summary>
/// Unhandled exception from CacheExceptionMapper
/// </summary>
public class UnknownException : SdkException
{
    public UnknownException(string message) : base(MomentoErrorCode.UNKNOWN_ERROR, message)
    {
    }
    public UnknownException(string message, Exception e) : base(MomentoErrorCode.UNKNOWN_ERROR, message, null, e)
    {
    }
}
