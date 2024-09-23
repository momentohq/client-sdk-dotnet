using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

/// <summary>
/// Parent response type for a cache dictionary length request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheDictionaryLengthResponse.Hit</description></item>
/// <item><description>CacheDictionaryLengthResponse.Miss</description></item>
/// <item><description>CacheDictionaryLengthResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheDictionaryLengthResponse.Hit hitResponse)
/// {
///     return hitResponse.Length;
/// }
/// else if (response is CacheDictionaryLengthResponse.Miss missResponse)
/// {
///     // handle missResponse as appropriate
/// }
/// else if (response is CacheDictionaryLengthResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CacheDictionaryLengthResponse
{
    /// <include file="../../docs.xml" path='docs/class[@name="Hit"]/description/*' />
    public class Hit : CacheDictionaryLengthResponse
    {
        /// <summary>
        /// The length of the dictionary. If the dictionary is missing or empty, the result is a miss.
        /// </summary>
        public int Length { get; private set; } = 0;

        /// <summary>
        ///
        /// </summary>
        /// <param name="response">The cache response.</param>
        public Hit(_DictionaryLengthResponse response)
        {
            if(response.DictionaryCase == _DictionaryLengthResponse.DictionaryOneofCase.Found) {
                Length = checked((int)response.Found.Length);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: Length: {Length}";
        }
    }

    /// <include file="../../docs.xml" path='docs/class[@name="Miss"]/description/*' />
    public class Miss : CacheDictionaryLengthResponse
    {

    }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheDictionaryLengthResponse, IError
    {
        private readonly SdkException _error;

        /// <include file="../../docs.xml" path='docs/class[@name="Error"]/constructor/*' />
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
