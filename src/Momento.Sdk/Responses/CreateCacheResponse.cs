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
/// if (response is CreateCacheResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// </code>
/// </summary>
public abstract class CreateCacheResponse
{

    public class Success : CreateCacheResponse { }

    public class CacheAlreadyExists : CreateCacheResponse { }

    /// <include file="../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CreateCacheResponse
    {
        private readonly SdkException _error;
        public Error(SdkException error)
        {
            _error = error;
        }

        /// <include file="../docs.xml" path='docs/class[@name="Error"]/prop[@name="Exception"]/*' />
        public SdkException Exception
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

        public override string ToString()
        {
            return $"{base.ToString()}: {this.Message}";
        }

    }

}
