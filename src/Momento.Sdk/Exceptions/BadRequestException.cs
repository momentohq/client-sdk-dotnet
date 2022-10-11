namespace Momento.Sdk.Exceptions;

using System;

/// <summary>
/// Invalid parameters sent to Momento Services.
/// </summary>
public class BadRequestException : SdkException
{
    /// <include file="../docs.xml" path='docs/class[@name="SdkException"]/constructor/*' />
    public BadRequestException(string message, MomentoErrorTransportDetails transportDetails, Exception? e = null) : base(MomentoErrorCode.BAD_REQUEST_ERROR, message, transportDetails, e)
    {
        this.MessageWrapper = "The request was invalid; please contact us at support@momentohq.com";
    }
}
