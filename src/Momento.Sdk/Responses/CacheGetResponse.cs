using Google.Protobuf;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
namespace Momento.Sdk.Responses;

/// <summary>
/// Parent response type for a cache get request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheGetResponse.Hit</description></item>
/// <item><description>CacheGetResponse.Miss</description></item>
/// <item><description>CacheGetResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheGetResponse.Hit hitResponse)
/// {
///     return hitResponse.ValueString;
/// } else if (response is CacheGetResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// </code>
/// </summary>
public abstract class CacheGetResponse
{
    /// <summary>
    /// Class <c>Hit</c> contains the results of a cache hit.
    ///</summary>
    public class Hit : CacheGetResponse
    {
        /// <summary>
        /// The value returned from the cache for the specified key. Use the
        /// <c>ValueString</c> and <c>ValueByteArray</c> properites to access
        /// this value as a string or byte array respectively.
        /// </summary>
        protected readonly ByteString value;


        /// <summary>
        /// Class <c>Hit</c> contains the results of a cache hit.
        ///</summary>
        /// <param name="response">gRPC get request result</param>
        public Hit(_GetResponse response)
        {
            this.value = response.CacheBody;
        }

        /// <summary>
        /// Value from the cache as a byte array.
        /// </summary>
        public byte[] ValueByteArray
        {
            get => value.ToByteArray();
        }

        /// <summary>
        /// Value from the cache as a string.
        /// </summary>
        public string ValueString
        {
            get => value.ToStringUtf8();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: {this.ValueString}";
        }
    }

    /// <summary>
    /// Class <c>Miss</c> contains the results of a cache miss.
    ///</summary>
    public class Miss : CacheGetResponse
    {
        /// <summary>
        /// Class <c>Miss</c> contains the results of a cache miss.
        ///</summary>
        public Miss() { }
    }

    /// <include file="../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheGetResponse
    {
        private readonly SdkException _error;

        /// <include file="../docs.xml" path='docs/class[@name="Error"]/constructor/*' />
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

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: {this.Message}";
        }

    }
}
