namespace Momento.Sdk.Exceptions;

/// <summary>
/// Resource already exists
/// </summary>
public class AlreadyExistsException : SdkException
{
    public AlreadyExistsException(string message) : base(message)
    {
    }
}
