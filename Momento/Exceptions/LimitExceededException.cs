namespace MomentoSdkDotnet45.Exceptions
{
    /// <summary>
    /// Requested operation couldn't be completed because system limits were hit.
    /// </summary>
    public class LimitExceededException : MomentoServiceException
    {
        public LimitExceededException(string message) : base(message)
        {
        }
    }
}
