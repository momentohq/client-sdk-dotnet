﻿namespace Momento.Sdk.Exceptions;

/// <summary>
/// Authentication token is not provided or is invalid.
/// </summary>
public class AuthenticationException : MomentoServiceException
{
    public AuthenticationException(string message) : base(message)
    {
    }
}
