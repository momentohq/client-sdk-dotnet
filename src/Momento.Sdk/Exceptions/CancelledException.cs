namespace Momento.Sdk.Exceptions;

using System;

public class CancelledException : SdkException
{
    /// <summary>
    /// Operation was cancelled.
    /// </summary>
    public CancelledException(string message, MomentoErrorTransportDetails? transportDetails=null, Exception? e=null) : base(MomentoErrorCode.CANCELLED_ERROR, message, transportDetails)
    {
        this.MessageWrapper = "The request was cancelled by the server; please contact us at support@momentohq.com";
    }
}
