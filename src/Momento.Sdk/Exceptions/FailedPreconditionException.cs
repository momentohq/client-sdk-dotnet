namespace Momento.Sdk.Exceptions;

using System;

/// <summary>
/// The server did not meet the precondition to run a command.
/// 
/// For example, calling <c>Increment</c> on a key that doesn't store
/// a number.
/// </summary>
public class FailedPreconditionException : SdkException
{
    /// <include file="../docs.xml" path='docs/class[@name="SdkException"]/constructor/*' />
    public FailedPreconditionException(string message, MomentoErrorTransportDetails transportDetails, Exception? e = null) : base(MomentoErrorCode.FAILED_PRECONDITION_ERROR, message, transportDetails, e)
    {
        this.MessageWrapper = "System is not in a state required for the operation's execution";
    }
}
