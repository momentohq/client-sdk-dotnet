namespace Momento.Sdk.Exceptions;

using System;

/// <summary>
/// Requested operation did not complete in allotted time.
/// </summary>
public class TimeoutException : SdkException
{
    public TimeoutException(string message, MomentoErrorTransportDetails transportDetails, Exception? e=null) : base(MomentoErrorCode.TIMEOUT_ERROR, message, transportDetails, e)
    {
        this.MessageWrapper = "The client's configured timeout was exceeded; you may need to use a Configuration with more lenient timeouts";
    }
}
