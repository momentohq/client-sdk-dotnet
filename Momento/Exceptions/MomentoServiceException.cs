namespace MomentoSdk.Exceptions
{
    /// <summary>
    /// Base type for all the exceptions resulting from invalid interactions with Momento Services.
    /// </summary>
    public abstract class MomentoServiceException : SdkException
    {
        protected MomentoServiceException(string message) : base(message)
        {
        }
    }
}
