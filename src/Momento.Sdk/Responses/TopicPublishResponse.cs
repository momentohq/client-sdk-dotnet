#if NETSTANDARD2_0_OR_GREATER

using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

/// <summary>
/// Parent response type for a topic publish request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>TopicPublishResponse.Success</description></item>
/// <item><description>TopicPublishResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is TopicPublishResponse.Success successResponse)
/// {
///     // handle success as appropriate
/// }
/// else if (response is TopicPublishResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class TopicPublishResponse
{
    /// <include file="../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : TopicPublishResponse {}

    /// <include file="../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : TopicPublishResponse, IError
    {
        private readonly SdkException _error;

        /// <include file="../docs.xml" path='docs/class[@name="Error"]/constructor/*' />
        public Error(SdkException error)
        {
            _error = error;
        }

        /// <inheritdoc />
        public SdkException InnerException => _error;

        /// <inheritdoc />
        public MomentoErrorCode ErrorCode => _error.ErrorCode;

        /// <inheritdoc />
        public string Message => $"{_error.MessageWrapper}: {_error.Message}";

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: {this.Message}";
        }

    }
}
#endif