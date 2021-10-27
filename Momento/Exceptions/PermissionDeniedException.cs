using System;
namespace MomentoSdk.Exceptions
{
    /// <summary>
    /// This exception is thrown when the user tries to perform any actions that
    /// they do not have permissions for.
    /// </summary>
    public class PermissionDeniedException : MomentoServiceException
    {
        public PermissionDeniedException(String message) : base(message)
        {
        }
    }
}
