namespace Momento.Sdk.Exceptions;

/// <summary>
/// Requested operation did not complete in allotted time.
/// </summary>
public class TimeoutException : SdkException
{
    public TimeoutException(string message) : base(message)
    {
    }
}
