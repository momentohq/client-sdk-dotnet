namespace Momento.Sdk.Exceptions;

/// <summary>
/// Resource already exists
/// </summary>
public class AlreadyExistsException : SdkException
{
    public AlreadyExistsException(string message) : base(message, MomentoErrorCode.ALREADY_EXISTS_ERROR)
    {
    }
    public AlreadyExistsException(string message, MomentoErrorTransportDetails transportDetails) : base(message, MomentoErrorCode.ALREADY_EXISTS_ERROR, transportDetails)
    {
    }
}
