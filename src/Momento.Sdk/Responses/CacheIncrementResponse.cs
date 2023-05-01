using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

/// <summary>
/// Parent response type for a cache increment request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheIncrementResponse.Success</description></item>
/// <item><description>CacheIncrementResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheIncrementResponse.Success successResponse)
/// {
///     return response.Value;
/// }
/// else if (response is CacheIncrementResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CacheIncrementResponse
{
    /// <include file="../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : CacheIncrementResponse
    {
        /// <summary>
        /// The value of the field post-increment.
        /// </summary>
        public long Value { get; private set; }

        /// <include file="../docs.xml" path='docs/class[@name="Success"]/description/*' />
        public Success(_IncrementResponse response)
        {
            Value = response.Value;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: Value: {Value}";
        }
    }

    /// <include file="../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheIncrementResponse, IError
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
