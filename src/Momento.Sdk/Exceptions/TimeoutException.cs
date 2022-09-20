namespace Momento.Sdk.Exceptions;

/// <summary>
/// Requested operation did not complete in allotted time.
/// </summary>
public class TimeoutException : SdkException
{
    public TimeoutException(string message) : base(message, MomentoErrorCode.TIMEOUT_ERROR)
    {
    }
    public TimeoutException(string message, MomentoErrorTransportDetails transportDetails) : base(message, MomentoErrorCode.TIMEOUT_ERROR, transportDetails)
    {
    }
}
