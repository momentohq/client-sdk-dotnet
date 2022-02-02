namespace MomentoSdk.Exceptions
{
    public class TimeoutException : MomentoServiceException
    {
        /// <summary>
        /// Requested operation did not complete in allotted time.
        /// </summary>
        public TimeoutException(string message) : base(message)
        {
        }
    }
}
