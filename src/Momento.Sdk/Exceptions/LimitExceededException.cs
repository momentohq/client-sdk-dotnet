namespace Momento.Sdk.Exceptions;

using System;

/// <summary>
/// Requested operation couldn't be completed because system limits were hit.
/// </summary>
public class LimitExceededException : SdkException
{
    /// <include file="../docs.xml" path='docs/class[@name="SdkException"]/constructor/*' />
    public LimitExceededException(string message, MomentoErrorTransportDetails transportDetails, Exception? e = null) : base(MomentoErrorCode.LIMIT_EXCEEDED_ERROR, message, transportDetails, e)
    {
        this.MessageWrapper = "Request rate exceeded the limits for this account.  To resolve this error, reduce your request rate, or contact us at support@momentohq.com to request a limit increase";
    }
}
