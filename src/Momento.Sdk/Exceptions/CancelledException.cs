namespace Momento.Sdk.Exceptions;

public class CancelledException : SdkException
{
    /// <summary>
    /// Operation was cancelled.
    /// </summary>
    public CancelledException(string message) : base(MomentoErrorCode.CANCELLED_ERROR, message)
    {
    }
    public CancelledException(string message, MomentoErrorTransportDetails transportDetails) : base(MomentoErrorCode.CANCELLED_ERROR, message, transportDetails)
    {
    }
}
