namespace Momento.Sdk.Exceptions;

using System;

/// <summary>
/// Requested resource or the resource on which an operation was requested doesn't exist.
/// </summary>
public class NotFoundException : SdkException
{
    /// <include file="../docs.xml" path='docs/class[@name="SdkException"]/constructor/*' />
    public NotFoundException(string message, MomentoErrorTransportDetails transportDetails, Exception? e = null) : base(MomentoErrorCode.NOT_FOUND_ERROR, message, transportDetails, e)
    {
        this.MessageWrapper = "A cache with the specified name does not exist.  To resolve this error, make sure you have created the cache before attempting to use it";
    }
}
