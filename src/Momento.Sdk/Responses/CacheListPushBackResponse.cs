using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;


/// <summary>
/// Parent response type for a cache list push back request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheListPushBackResponse.Success</description></item>
/// <item><description>CacheListPushBackResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheListPushBackResponse.Success successResponse)
/// {
///     return successResponse.ListLength;
/// }
/// else if (response is CacheListPushBackResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CacheListPushBackResponse
{
    /// <include file="../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : CacheListPushBackResponse
    {
        /// <summary>
        /// The length of the list post-push (and post-truncate, if that applies).
        /// </summary>
        public int ListLength { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response">The cache response</param>
        public Success(_ListPushBackResponse response)
        {
            ListLength = checked((int)response.ListLength);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: ListLength: {ListLength}";
        }
    }

    /// <include file="../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheListPushBackResponse
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
