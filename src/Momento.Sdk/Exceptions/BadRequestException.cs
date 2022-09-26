namespace Momento.Sdk.Exceptions;

using System;

/// <summary>
/// Invalid parameters sent to Momento Services.
/// </summary>
public class BadRequestException : SdkException
{
    public BadRequestException(string message, MomentoErrorTransportDetails? transportDetails=null, Exception? e=null) : base(MomentoErrorCode.BAD_REQUEST_ERROR, message, transportDetails, e)
    {
        this.MessageWrapper = "The request was invalid; please contact Momento";
    }
}
