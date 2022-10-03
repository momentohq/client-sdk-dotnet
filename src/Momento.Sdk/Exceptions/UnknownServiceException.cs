namespace Momento.Sdk.Exceptions;

using System;

/// <summary>
/// Service returned an unknown response.
/// </summary>
public class UnknownServiceException : SdkException
{
    public UnknownServiceException(string message, MomentoErrorTransportDetails transportDetails, Exception? e = null) : base(MomentoErrorCode.BAD_REQUEST_ERROR, message, transportDetails, e)
    {
        this.MessageWrapper = "Service returned an unknown response; please contact us at support@momentohq.com";
    }
}
