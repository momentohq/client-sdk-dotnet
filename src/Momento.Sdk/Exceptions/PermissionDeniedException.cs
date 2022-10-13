namespace Momento.Sdk.Exceptions;

using System;

/// <summary>
/// Insufficient permissions to execute an operation.
/// </summary>
public class PermissionDeniedException : SdkException
{
    /// <include file="../docs.xml" path='docs/class[@name="SdkException"]/constructor/*' />
    public PermissionDeniedException(string message, MomentoErrorTransportDetails transportDetails, Exception? e = null) : base(MomentoErrorCode.PERMISSION_ERROR, message, transportDetails, e)
    {
    }
}
