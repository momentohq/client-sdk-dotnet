namespace Momento.Sdk.Exceptions;

/// <summary>
/// Invalid parameters sent to Momento Services.
/// </summary>
public class BadRequestException : SdkException
{
    public BadRequestException(string message) : base(message)
    {
    }
}
