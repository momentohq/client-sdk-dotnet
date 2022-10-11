using System;

namespace Momento.Sdk.Exceptions;

/// <summary>
/// SDK client side validation failed.
/// </summary>
public class InvalidArgumentException : SdkException
{
    /// <include file="../docs.xml" path='docs/class[@name="SdkException"]/constructor/*' />
    public InvalidArgumentException(string message, MomentoErrorTransportDetails? transportDetails = null, Exception? e = null) : base(MomentoErrorCode.INVALID_ARGUMENT_ERROR, message, transportDetails, e)
    {
        this.MessageWrapper = "Invalid argument passed to Momento client";
    }
}
