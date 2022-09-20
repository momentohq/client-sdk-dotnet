namespace Momento.Sdk.Exceptions;

/// <summary>
/// Insufficient permissions to execute an operation.
/// </summary>
public class PermissionDeniedException : SdkException
{
    public PermissionDeniedException(string message) : base(message, MomentoErrorCode.PERMISSION_ERROR)
    {
    }
    public PermissionDeniedException(string message, MomentoErrorTransportDetails transportDetails) : base(message, MomentoErrorCode.PERMISSION_ERROR, transportDetails)
    {
    }
}
