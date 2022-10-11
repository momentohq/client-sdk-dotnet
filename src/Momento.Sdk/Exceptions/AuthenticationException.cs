namespace Momento.Sdk.Exceptions;

using System;

/// <summary>
/// Authentication token is not provided or is invalid.
/// </summary>
public class AuthenticationException : SdkException
{
    /// <include file="../docs.xml" path='docs/class[@name="SdkException"]/constructor/*' />
    public AuthenticationException(string message, MomentoErrorTransportDetails transportDetails, Exception? e = null) : base(MomentoErrorCode.AUTHENTICATION_ERROR, message, transportDetails, e)
    {
        this.MessageWrapper = "Invalid authentication credentials to connect to cache service";
    }
}
