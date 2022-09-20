namespace Momento.Sdk.Exceptions;

public class CancelledException : SdkException
{
    /// <summary>
    /// Operation was cancelled.
    /// </summary>
    public CancelledException(string message) : base(message, MomentoErrorCode.CANCELLED_ERROR)
    {
    }
    public CancelledException(string message, MomentoErrorTransportDetails transportDetails) : base(message, MomentoErrorCode.CANCELLED_ERROR, transportDetails)
    {
    }
}
