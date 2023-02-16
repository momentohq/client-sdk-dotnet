using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal.ExtensionMethods;

namespace Momento.Sdk.Responses;

/// <summary>
/// Parent response type for a cache dictionary get field request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheDictionaryGetFieldResponse.Hit</description></item>
/// <item><description>CacheDictionaryGetFieldResponse.Miss</description></item>
/// <item><description>CacheDictionaryGetFieldResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheDictionaryGetFieldResponse.Hit hitResponse)
/// {
///     return hitResponse.ValueString;
/// }
/// else if (response is CacheDictionaryGetFieldResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CacheDictionaryGetFieldResponse
{
    /// <include file="../docs.xml" path='docs/class[@name="Hit"]/description/*' />
    public class Hit : CacheDictionaryGetFieldResponse
    {
#pragma warning disable 1591
        protected readonly ByteString value;
        protected readonly ByteString field;
#pragma warning restore 1591

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field">The field queried.</param>
        /// <param name="response">The cache response.</param>
        public Hit(ByteString field, _DictionaryGetResponse response)
        {
            this.value = response.Found.Items[0].CacheBody;
            this.field = field;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field">The field queried.</param>
        /// <param name="cacheBody">The cache response.</param>
        public Hit(ByteString field, ByteString cacheBody)
        {
            this.value = cacheBody;
            this.field = field;
        }

        /// <summary>
        /// The value stored in the cache as a <see cref="byte" /> array.
        /// </summary>
        public byte[] ValueByteArray
        {
            get => value.ToByteArray();
        }

        /// <summary>
        /// The field queried in the cache operation as a <see cref="byte" /> array.
        /// </summary>
        public byte[] FieldByteArray
        {
            get => field.ToByteArray();
        }

        /// <summary>
        /// The cached value as a <see cref="string" />.
        /// </summary>
        public string ValueString { get => value.ToStringUtf8(); }

        /// <summary>
        /// The field queried in the cache operation as a <see cref="string" />.
        /// </summary>
        public string FieldString { get => field.ToStringUtf8(); }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: ValueString: \"{ValueString.Truncate()}\" ValueByteArray: \"{ValueByteArray.ToPrettyHexString().Truncate()}\"";
        }
    }

    /// <include file="../docs.xml" path='docs/class[@name="Miss"]/description/*' />
    public class Miss : CacheDictionaryGetFieldResponse
    {
        /// <summary>
        /// The field queried.
        /// </summary>
        protected readonly ByteString field;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields">The fields queried.</param>
        public Miss(IEnumerable<ByteString> fields)
        {
            this.field = fields.ToList()[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field">The field queried.</param>
        public Miss(ByteString field)
        {
            this.field = field;
        }

        /// <summary>
        /// The field queried as a <see cref="byte" /> array.
        /// </summary>
        public byte[] FieldByteArray
        {
            get => field.ToByteArray();
        }

        /// <summary>
        /// The field queried as a <see langword="string" />
        /// </summary>
        public string FieldString { get => field.ToStringUtf8(); }
    }

    /// <include file="../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheDictionaryGetFieldResponse
    {
        private readonly SdkException _error;
        /// <summary>
        /// The field queried in the cache operation.
        /// </summary>
        protected readonly ByteString? field;

        /// <include file="../docs.xml" path='docs/class[@name="Error"]/constructor/*' />
        public Error(SdkException error)
        {
            _error = error;
        }

        /// <include file="../docs.xml" path='docs/class[@name="Error"]/constructor/*' />
        public Error(ByteString? field, SdkException error)
        {
            this.field = field;
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

        /// <summary>
        /// The field queried in the cache operation.
        /// </summary>
        public byte[]? FieldByteArray
        {
            get => field?.ToByteArray();
        }

        /// <summary>
        /// The field queried in the cache operation.
        /// </summary>
        public string? FieldString { get => field?.ToStringUtf8(); }

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
