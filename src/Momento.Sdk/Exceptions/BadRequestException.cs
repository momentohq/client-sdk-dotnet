namespace Momento.Sdk.Exceptions;

/// <summary>
/// Invalid parameters sent to Momento Services.
/// </summary>
public class BadRequestException : MomentoServiceException
{
    public BadRequestException(string message) : base(message)
    {
    }
}
