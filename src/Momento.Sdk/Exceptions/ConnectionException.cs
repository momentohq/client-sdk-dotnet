namespace Momento.Sdk.Exceptions;

using System;

/// <summary>
/// Unable to connect to the server.
/// </summary>
public class ConnectionException : SdkException
{
    /// <include file="../docs.xml" path='docs/class[@name="SdkException"]/constructor/*' />
    public ConnectionException(string message, MomentoErrorTransportDetails transportDetails, Exception? e = null) : base(MomentoErrorCode.CONNECTION_ERROR, message, transportDetails, e)
    {
        this.MessageWrapper = "Unable to connect to the server; consider retrying.  If the error persists, please contact us at support@momentohq.com";
    }
}
