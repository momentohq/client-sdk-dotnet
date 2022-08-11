namespace Momento.Sdk.Exceptions;

/// <summary>
/// Insufficient permissions to execute an operation.
/// </summary>
public class PermissionDeniedException : MomentoServiceException
{
    public PermissionDeniedException(string message) : base(message)
    {
    }
}
