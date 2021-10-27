using System;
namespace MomentoSdk.Exceptions
{
    /// <summary>
    /// InvalidJwtException gets thrown when jwt passed to the Momento service is
    /// not valid.
    /// </summary>
    public class InvalidJwtException : ClientSdkException
    {
        public InvalidJwtException(String message) : base(message)
        {
        }
    }
}
