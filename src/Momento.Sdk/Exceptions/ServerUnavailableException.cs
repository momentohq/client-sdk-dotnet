namespace Momento.Sdk.Exceptions;

using System;

/// <summary>
/// Server was unable to handle the request
/// </summary>
public class ServerUnavailableException : SdkException
{
    public ServerUnavailableException(string message, MomentoErrorTransportDetails transportDetails, Exception? e = null) : base(MomentoErrorCode.SERVER_UNAVAILABLE, message, transportDetails, e)
    {
        this.MessageWrapper = "The server was unable to handle the request; consider retrying.  If the error persists, please contact us at support@momentohq.com";
    }
}
