using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

/// <summary>
/// Parent response type for a cache list length request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheListLengthResponse.Success</description></item>
/// <item><description>CacheListLengthResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheListLengthResponse.Success successResponse)
/// {
///     return successResponse.Length;
/// }
/// else if (response is CacheListLengthResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CacheListLengthResponse
{
    /// <include file="../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : CacheListLengthResponse
    {
        /// <summary>
        /// The length of the list. If the list is missing or empty, the result is zero.
        /// </summary>
        public int Length { get; private set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response">The cache response.</param>
        public Success(_ListLengthResponse response)
        {
            if (response.ListCase == _ListLengthResponse.ListOneofCase.Found)
            {
                Length = (int)response.Found.Length;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: Length: {Length}";
        }
    }

    /// <include file="../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheListLengthResponse, IError
    {
        private readonly SdkException _error;

        /// <include file="../docs.xml" path='docs/class[@name="Error"]/constructor/*' />
        public Error(SdkException error)
        {
            _error = error;
        }

        /// <inheritdoc />
        public SdkException InnerException
        {
            get => _error;
        }

        /// <inheritdoc />
        public MomentoErrorCode ErrorCode
        {
            get => _error.ErrorCode;
        }

        /// <inheritdoc />
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
