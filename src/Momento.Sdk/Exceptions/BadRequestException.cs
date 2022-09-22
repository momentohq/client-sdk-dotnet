namespace Momento.Sdk.Exceptions;

/// <summary>
/// Invalid parameters sent to Momento Services.
/// </summary>
public class BadRequestException : SdkException
{
    public BadRequestException(string message) : base(MomentoErrorCode.BAD_REQUEST_ERROR, message)
    {
    }
    public BadRequestException(string message, MomentoErrorTransportDetails transportDetails) : base(MomentoErrorCode.BAD_REQUEST_ERROR, message, transportDetails)
    {
    }
}
