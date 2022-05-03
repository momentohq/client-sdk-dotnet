namespace MomentoSdkDotnet45.Exceptions
{
    /// <summary>
    /// Requested resource or the resource on which an operation was requested doesn't exist.
    /// </summary>
    public class NotFoundException : MomentoServiceException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
