namespace MomentoSdkDotnet45.Exceptions
{
    /// <summary>
    /// Requested operation did not complete in allotted time.
    /// </summary>
    public class TimeoutException : MomentoServiceException
    {
        public TimeoutException(string message) : base(message)
        {
        }
    }
}
