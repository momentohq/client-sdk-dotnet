using System;

namespace Momento.Sdk.Exceptions;

/// <summary>
/// Momento Service encountered an unexpected exception while trying to fulfill the request.
/// </summary>
public class InternalServerException : MomentoServiceException
{
    public InternalServerException(string message, Exception e) : base(message, e)
    {
    }
    public InternalServerException(string message) : base(message)
    {
    }
}
