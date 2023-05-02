using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

/// <summary>
/// Represents an error response
/// </summary>
public interface IError
{
    /// <summary>
    /// The <see cref="SdkException"/> object used to construct the response.
    /// </summary>
    SdkException InnerException { get; }

    /// <summary>
    /// The <see cref="MomentoErrorCode"/> value for the
    /// <see cref="IError"/> object. Example:<br/>
    /// <code>
    /// if (errorResponse.ErrorCode == MomentoErrorCode.TIMEOUT_ERROR)
    /// {
    ///    // handle timeout error
    /// }
    /// </code>
    /// </summary>
    MomentoErrorCode ErrorCode { get; }

    /// <summary>
    /// An explanation of conditions that caused and potential
    /// ways to resolve the error.
    /// </summary>
    string Message { get; }
}

