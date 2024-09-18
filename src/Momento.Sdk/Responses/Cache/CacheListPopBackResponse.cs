using Google.Protobuf;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal.ExtensionMethods;

namespace Momento.Sdk.Responses;

/// <summary>
/// Parent response type for a cache list pop back request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheListPopBackResponse.Hit</description></item>
/// <item><description>CacheListPopBackResponse.Miss</description></item>
/// <item><description>CacheListPopBackResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheListPopBackResponse.Hit hitResponse)
/// {
///     return hitResponse.ValueString;
/// }
/// else if (response is CacheListPopBackResponse.Miss missResponse)
/// {
///     // handle miss as appropriate
/// }
/// else if (response is CacheListPopBackResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CacheListPopBackResponse
{
    /// <include file="../../docs.xml" path='docs/class[@name="Hit"]/description/*' />
    public class Hit : CacheListPopBackResponse
    {
#pragma warning disable 1591
        protected readonly ByteString value;
#pragma warning restore 1591

        /// <summary>
        /// The length of the list post-pop (and post-truncate, if that applies).
        /// </summary>
        public int ListLength { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response">The cache response.</param>
        public Hit(_ListPopBackResponse response)
        {
            this.value = response.Found.Back;
            ListLength = checked((int)response.Found.ListLength);
        }

        /// <summary>
        /// The value popped from the list as a <see cref="byte"/> array.
        /// </summary>
        public byte[] ValueByteArray
        {
            get => value.ToByteArray();
        }

        /// <summary>
        /// The value popped from the list as a <see cref="string"/>.
        /// </summary>
        public string ValueString { get => value.ToStringUtf8(); }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: ValueString: \"{ValueString.Truncate()}\" ValueByteArray: \"{ValueByteArray.ToPrettyHexString().Truncate()}\" ListLength: {ListLength}";
        }
    }

    /// <include file="../../docs.xml" path='docs/class[@name="Miss"]/description/*' />
    public class Miss : CacheListPopBackResponse
    {

    }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheListPopBackResponse, IError
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
