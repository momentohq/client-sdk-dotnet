namespace Momento.Sdk.Exceptions;

/// <summary>
/// Requested operation couldn't be completed because system limits were hit.
/// </summary>
public class LimitExceededException : SdkException
{
    public LimitExceededException(string message) : base(message)
    {
    }
}
