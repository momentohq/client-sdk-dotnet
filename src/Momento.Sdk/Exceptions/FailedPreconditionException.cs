namespace Momento.Sdk.Exceptions;

/// <summary>
/// The server did not meet the precondition to run a command.
/// 
/// For example, calling <c>Increment</c> on a key that doesn't store
/// a number.
/// </summary>
public class FailedPreconditionException : SdkException
{
    public FailedPreconditionException(string message) : base(message, MomentoErrorCode.FAILED_PRECONDITION_ERROR)
    {
    }
    public FailedPreconditionException(string message, MomentoErrorTransportDetails transportDetails) : base(message, MomentoErrorCode.FAILED_PRECONDITION_ERROR, transportDetails)
    {
    }
}
