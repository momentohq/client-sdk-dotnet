using System;

namespace Momento.Sdk.Exceptions;

/// <summary>
/// Momento Service encountered an unexpected exception while trying to fulfill the request.
/// </summary>
public class InternalServerException : SdkException
{
    public InternalServerException(string message) : base(message, MomentoErrorCode.INTERNAL_SERVER_ERROR)
    {
    }
    public InternalServerException(string message, Exception e) : base(message, MomentoErrorCode.INTERNAL_SERVER_ERROR, null, e)
    {
    }
    public InternalServerException(string message, MomentoErrorTransportDetails transportDetails, Exception e) : base(message, MomentoErrorCode.INTERNAL_SERVER_ERROR, transportDetails, e)
    {
    }
}
