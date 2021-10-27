using System;
namespace MomentoSdk.Exceptions
{
    /// <summary>
    /// This exception gets thrown when a bad parameter gets passed to the Momento
    /// backend. It will be accompanied by an error message with more details on
    /// which parameter is illegal.
    /// </summary>
    public class IllegalArgumentException : MomentoServiceException
    {
        public IllegalArgumentException(String message) : base(message)
        {
        }
    }
}
