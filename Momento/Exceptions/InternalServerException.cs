using System;
namespace MomentoSdk.Exceptions
{
    public class InternalServerException : MomentoServiceException
    {
        public InternalServerException(String message) : base(message)
        {
        }
    }
}
