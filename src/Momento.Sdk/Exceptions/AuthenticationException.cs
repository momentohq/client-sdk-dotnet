namespace Momento.Sdk.Exceptions;

/// <summary>
/// Authentication token is not provided or is invalid.
/// </summary>
public class AuthenticationException : SdkException
{
    public AuthenticationException(string message) : base(MomentoErrorCode.AUTHENTICATION_ERROR, message)
    {
    }
    public AuthenticationException(string message, MomentoErrorTransportDetails transportDetails) : base(MomentoErrorCode.AUTHENTICATION_ERROR, message, transportDetails)
    {
    }
}
