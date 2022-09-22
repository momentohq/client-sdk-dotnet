using System;

namespace Momento.Sdk.Exceptions;

/// <summary>
/// Momento Service encountered an unexpected exception while trying to fulfill the request.
/// </summary>
public class InternalServerException : SdkException
{
    public InternalServerException(string message) : base(MomentoErrorCode.INTERNAL_SERVER_ERROR, message)
    {
    }
    public InternalServerException(string message, Exception e) : base(MomentoErrorCode.INTERNAL_SERVER_ERROR, message, null, e)
    {
    }
    public InternalServerException(string message, MomentoErrorTransportDetails transportDetails, Exception e) : base(MomentoErrorCode.INTERNAL_SERVER_ERROR, message, transportDetails, e)
    {
    }
}
