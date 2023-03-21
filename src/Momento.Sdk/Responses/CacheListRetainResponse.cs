using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

/// <summary>
/// Parent response type for a cache list retain request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheListRetainResponse.Success</description></item>
/// <item><description>CacheListRetainResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheListRetainResponse.Success successResponse)
/// {
///     // handle success as appropriate
/// }
/// else if (response is CacheListRetainResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CacheListRetainResponse
{
    /// <include file="../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : CacheListRetainResponse
    {
    }

    /// <include file="../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheListRetainResponse
    {
        private readonly SdkException _error;

        /// <include file="../docs.xml" path='docs/class[@name="Error"]/constructor/*' />
        public Error(SdkException error)
        {
            _error = error;
        }

        /// <include file="../docs.xml" path='docs/class[@name="Error"]/prop[@name="InnerException"]/*' />
        public SdkException InnerException
        {
            get => _error;
        }

        /// <include file="../docs.xml" path='docs/class[@name="Error"]/prop[@name="ErrorCode"]/*' />
        public MomentoErrorCode ErrorCode
        {
            get => _error.ErrorCode;
        }

        /// <include file="../docs.xml" path='docs/class[@name="Error"]/prop[@name="Message"]/*' />
        public string Message
        {
            get => $"{_error.MessageWrapper}: {_error.Message}";
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: {this.Message}";
        }
    }
}
