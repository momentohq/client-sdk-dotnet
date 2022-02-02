namespace MomentoSdk.Exceptions
{
    public class LimitExceededException : MomentoServiceException
    {
        /// <summary>
        /// Requested operation couldn't be completed because system limits were hit.
        /// </summary>
        public LimitExceededException(string message) : base(message)
        {
        }
    }
}
