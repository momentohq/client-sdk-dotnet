namespace MomentoSdkDotnet45.Exceptions
{
    /// <summary>
    /// Resource already exists
    /// </summary>
    public class AlreadyExistsException : MomentoServiceException
    {
        public AlreadyExistsException(string message) : base(message)
        {
        }
    }
}
