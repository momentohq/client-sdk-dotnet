namespace Momento.Sdk.Exceptions;

public class CancelledException : SdkException
{
    /// <summary>
    /// Operation was cancelled.
    /// </summary>
    public CancelledException(string message) : base(message)
    {
    }
}
