namespace Momento.Sdk.Exceptions;

/// <summary>
/// Resource already exists
/// </summary>
public class AlreadyExistsException : SdkException
{
    public AlreadyExistsException(string message) : base(MomentoErrorCode.ALREADY_EXISTS_ERROR, message)
    {
    }
    public AlreadyExistsException(string message, MomentoErrorTransportDetails transportDetails) : base(MomentoErrorCode.ALREADY_EXISTS_ERROR, message, transportDetails)
    {
    }
}
