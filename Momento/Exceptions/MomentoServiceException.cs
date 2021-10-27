using System;
namespace MomentoSdk.Exceptions
{
    public abstract class MomentoServiceException : SdkException
    {
        protected MomentoServiceException(String message) : base(message)
        {
        }
    }
}
