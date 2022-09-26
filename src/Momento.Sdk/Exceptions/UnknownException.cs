namespace Momento.Sdk.Exceptions;

using System;

/// <summary>
/// Unhandled exception from CacheExceptionMapper
/// </summary>
public class UnknownException : SdkException
{
    public UnknownException(string message, MomentoErrorTransportDetails? transportDetails=null, Exception? e=null) : base(MomentoErrorCode.UNKNOWN_ERROR, message, transportDetails, e)
    {
        this.MessageWrapper = "Unknown error has occurred";
    }
}
