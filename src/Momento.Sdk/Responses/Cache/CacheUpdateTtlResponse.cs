namespace Momento.Sdk.Responses;

using Momento.Sdk.Exceptions;


/// <summary>
/// Parent response type for a cache update ttl request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheUpdateTtlResponse.Set</description></item>
/// <item><description>CacheUpdateTtlResponse.Miss</description></item>
/// <item><description>CacheUpdateTtlResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheUpdateTtlResponse.Set setResponse)
/// {
///    // handle ttl updated as appropriate
/// }
/// else if (response is CacheUpdateTtlResponse.Miss missResponse)
/// {
///    // handle ttl not updated because key was not found as appropriate
/// }
/// else if (response is CacheUpdateTtlResponse.Error errorResponse)
/// {
///   // handle error as appropriate
///   return errorResponse.Message;
/// }
/// else
/// {
///   // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CacheUpdateTtlResponse
{
    /// <summary>
    /// Indicates the key was found in the cache and the ttl was updated.
    /// </summary>
    public class Set : CacheUpdateTtlResponse { }

    /// <summary>
    /// Indicates the key was not found in the cache, hence the ttl was not updated.
    /// </summary>
    public class Miss : CacheUpdateTtlResponse { }

    /// <include file = "../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheUpdateTtlResponse, IError
    {
        private readonly SdkException _error;

        /// <include file = "../../docs.xml" path='docs/class[@name="Error"]/constructor/*' />
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
            get => _error.Message;
        }
    }
}
