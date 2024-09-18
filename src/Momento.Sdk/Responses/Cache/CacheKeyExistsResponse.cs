namespace Momento.Sdk.Responses;

using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;

/// <summary>
/// Parent response type for a cache KeyExists request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheKeyExistsResponse.Success</description></item>
/// <item><description>CacheKeyExistsResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheKeyExistsResponse.Success successResponse)
/// {
///     // handle success as appropriate
/// }
/// else if (response is CacheKeyExistsResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CacheKeyExistsResponse
{

    /// <include file="../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : CacheKeyExistsResponse
    {
        /// <summary>
        /// True if the specified key exists in the cache, false otherwise.
        /// </summary>
        public bool Exists { get; private set; }

        /// <include file="../docs.xml" path='docs/class[@name="Success"]/description/*' />
        public Success(bool exists)
        {
            Exists = exists;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: Exists: {Exists}";
        }
    }

    /// <include file="../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheKeyExistsResponse, IError
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
