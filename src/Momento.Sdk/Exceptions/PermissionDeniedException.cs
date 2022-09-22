namespace Momento.Sdk.Exceptions;

/// <summary>
/// Insufficient permissions to execute an operation.
/// </summary>
public class PermissionDeniedException : SdkException
{
    public PermissionDeniedException(string message) : base(MomentoErrorCode.PERMISSION_ERROR, message)
    {
    }
    public PermissionDeniedException(string message, MomentoErrorTransportDetails transportDetails) : base(MomentoErrorCode.PERMISSION_ERROR, message, transportDetails)
    {
    }
}
