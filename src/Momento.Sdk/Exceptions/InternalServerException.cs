using System;

namespace Momento.Sdk.Exceptions;

/// <summary>
/// Momento Service encountered an unexpected exception while trying to fulfill the request.
/// </summary>
public class InternalServerException : SdkException
{
    /// <include file="../docs.xml" path='docs/class[@name="SdkException"]/constructor/*' />
    public InternalServerException(string message, MomentoErrorTransportDetails transportDetails, Exception? e = null) : base(MomentoErrorCode.INTERNAL_SERVER_ERROR, message, transportDetails, e)
    {
        this.MessageWrapper = "An unexpected error occurred while trying to fulfill the request; please contact us at support@momentohq.com";
    }
}
