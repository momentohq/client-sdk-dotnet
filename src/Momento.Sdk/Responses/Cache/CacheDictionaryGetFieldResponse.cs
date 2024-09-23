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
/// else if (response is CacheDictionaryGetFieldResponse.Miss missResponse)
/// {
///     // handle miss as appropriate
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
#pragma warning disable 1591
    protected readonly ByteString field;
#pragma warning restore 1591

    /// <summary>
    /// The field queried as a <see cref="byte" /> array.
    /// </summary>
    public byte[] FieldByteArray
    {
        get => field.ToByteArray();
    }

    /// <summary>
    /// The field queried as a <see cref="string" />.
    /// </summary>
    public string FieldString { get => field.ToStringUtf8(); }

#pragma warning disable 1591
    protected CacheDictionaryGetFieldResponse(ByteString field)
    {
        this.field = field;
    }
#pragma warning restore 1591

    /// <include file="../../docs.xml" path='docs/class[@name="Hit"]/description/*' />
    public class Hit : CacheDictionaryGetFieldResponse
    {
#pragma warning disable 1591
        protected readonly ByteString value;
#pragma warning restore 1591

        /// <summary>
        /// </summary>
        /// <param name="field">The field queried.</param>
        /// <param name="response">The cache response.</param>
        public Hit(ByteString field, _DictionaryGetResponse response) : base(field)
        {
            this.value = response.Found.Items[0].CacheBody;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field">The field queried.</param>
        /// <param name="cacheBody">The cache response.</param>
        public Hit(ByteString field, ByteString cacheBody) : base(field)
        {
            this.value = cacheBody;
        }

        /// <summary>
        /// The value stored in the cache as a <see cref="byte" /> array.
        /// </summary>
        public byte[] ValueByteArray
        {
            get => value.ToByteArray();
        }

        /// <summary>
        /// The cached value as a <see cref="string" />.
        /// </summary>
        public string ValueString { get => value.ToStringUtf8(); }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: ValueString: \"{ValueString.Truncate()}\" ValueByteArray: \"{ValueByteArray.ToPrettyHexString().Truncate()}\"";
        }
    }

    /// <include file="../../docs.xml" path='docs/class[@name="Miss"]/description/*' />
    public class Miss : CacheDictionaryGetFieldResponse
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field">The field queried.</param>
        public Miss(ByteString field) : base(field)
        {
        }
    }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheDictionaryGetFieldResponse, IError
    {
        private readonly SdkException _error;

        /// <include file="../../docs.xml" path='docs/class[@name="Error"]/constructor/*' />
        public Error(SdkException error) : base(ByteString.Empty)
        {
            _error = error;
        }

        /// <include file="../../docs.xml" path='docs/class[@name="Error"]/constructor/*' />
        public Error(ByteString field, SdkException error) : base(field)
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
