namespace Momento.Sdk.Responses;

using Momento.Sdk.Exceptions;

/// <summary>
/// Parent response type for a create cache request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CreateCacheResponse.Success</description></item>
/// <item><description>CreateCacheResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CreateCacheResponse.Success successResponse)
/// {
///     // handle success if needed
/// }
/// else if (response is CreateCacheResponse.CacheAlreadyExists alreadyExistsResponse)
/// {
///     // handle already exists as appropriate
/// }
/// else if (response is CreateCacheResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// </code>
/// </summary>
public abstract class CreateCacheResponse
{

    /// <include file="../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : CreateCacheResponse { }

    /// <summary>
    /// Class <c>CacheAlreadyExists</c> indicates that a cache with the requested name
    /// has already been created in the requesting account.
    /// </summary>
    public class CacheAlreadyExists : CreateCacheResponse { }

    /// <include file="../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CreateCacheResponse, IError
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
