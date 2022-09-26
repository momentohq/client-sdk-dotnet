namespace Momento.Sdk.Exceptions;

using System;

/// <summary>
/// Resource already exists
/// </summary>
public class AlreadyExistsException : SdkException
{
    public AlreadyExistsException(string message, MomentoErrorTransportDetails? transportDetails=null, Exception? e=null) : base(MomentoErrorCode.ALREADY_EXISTS_ERROR, message, transportDetails, e)
    {
        this.MessageWrapper = "A cache with the specified name already exists.  To resolve this error, either delete the existing cache and make a new one, or use a different name";
    }
}
