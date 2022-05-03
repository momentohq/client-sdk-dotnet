namespace MomentoSdkDotnet45.Exceptions
{
    public class CancelledException : MomentoServiceException
    {
        /// <summary>
        /// Operation was cancelled.
        /// </summary>
        public CancelledException(string message) : base(message)
        {
        }
    }
}
