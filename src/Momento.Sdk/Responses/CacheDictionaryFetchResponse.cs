using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;
using Momento.Sdk.Internal.ExtensionMethods;

namespace Momento.Sdk.Responses;

/// <summary>
/// Parent response type for a cache dictionary fetch request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheDictionaryFetchResponse.Hit</description></item>
/// <item><description>CacheDictionaryFetchResponse.Miss</description></item>
/// <item><description>CacheDictionaryFetchResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheDictionaryFetchResponse.Hit hitResponse)
/// {
///     return hitResponse.ValueDictionaryStringString;
/// }
/// else if (response is CacheDictionaryFetchResponse.Miss missResponse)
/// {
///     // handle miss as appropriate
/// }
/// else if (response is CacheDictionaryFetchResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CacheDictionaryFetchResponse
{
    /// <include file="../docs.xml" path='docs/class[@name="Hit"]/description/*' />
    public class Hit : CacheDictionaryFetchResponse
    {
#pragma warning disable 1591
        protected readonly RepeatedField<_DictionaryFieldValuePair>? items;
        protected readonly Lazy<IDictionary<byte[], byte[]>> _dictionaryByteArrayByteArray;
        protected readonly Lazy<IDictionary<string, string>> _dictionaryStringString;
        protected readonly Lazy<IDictionary<string, byte[]>> _dictionaryStringByteArray;
#pragma warning restore 1591

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response">The cache response.</param>
        public Hit(_DictionaryFetchResponse response)
        {
            items = response.Found.Items;
            _dictionaryByteArrayByteArray = new(() =>
            {
                var dictionary = new Dictionary<byte[], byte[]>(Utils.ByteArrayComparer);
                dictionary.AddRange(items.Select(kv => new KeyValuePair<byte[], byte[]>(kv.Field.ToByteArray(), kv.Value.ToByteArray())));
                return dictionary;

            });

            _dictionaryStringString = new(() =>
            {
                var dictionary = new Dictionary<string, string>();
                dictionary.AddRange(items.Select(kv => new KeyValuePair<string, string>(kv.Field.ToStringUtf8(), kv.Value.ToStringUtf8())));
                return dictionary;
            });
            _dictionaryStringByteArray = new(() =>
            {
                var dictionary = new Dictionary<string, byte[]>();
                dictionary.AddRange(items.Select(kv => new KeyValuePair<string, byte[]>(kv.Field.ToStringUtf8(), kv.Value.ToByteArray())));
                return dictionary;
            });
        }

        /// <summary>
        /// The cached dictionary as a <see cref="byte"/> array to <see cref="byte"/> array mapping.
        /// </summary>
        public IDictionary<byte[], byte[]> ValueDictionaryByteArrayByteArray { get => _dictionaryByteArrayByteArray.Value; }

        /// <summary>
        /// The cached dictionary as a <see cref="string"/> to <see cref="string"/> mapping.
        /// </summary>
        public IDictionary<string, string> ValueDictionaryStringString { get => _dictionaryStringString.Value; }

        /// <summary>
        /// The cached dictionary as a <see cref="string"/> to <see cref="byte"/> array mapping.
        /// </summary>
        public IDictionary<string, byte[]> ValueDictionaryStringByteArray { get => _dictionaryStringByteArray.Value; }

        /// <inheritdoc />
        public override string ToString()
        {
            var stringRepresentation = String.Join(", ", ValueDictionaryStringString.Select(kv => $"\"{kv.Key}\": \"{kv.Value}\""));
            var byteArrayRepresentation = String.Join(", ", ValueDictionaryByteArrayByteArray.Select(kv => $"\"{kv.Key.ToPrettyHexString()}\": \"{kv.Value.ToPrettyHexString()}\""));
            return $"{base.ToString()}: ValueDictionaryStringString: {{{stringRepresentation.Truncate()}}} ValueDictionaryByteArrayByteArray: {{{byteArrayRepresentation.Truncate()}}}";
        }
    }

    /// <include file="../docs.xml" path='docs/class[@name="Miss"]/description/*' />
    public class Miss : CacheDictionaryFetchResponse
    {

    }

    /// <include file="../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheDictionaryFetchResponse
    {
        private readonly SdkException _error;

        /// <include file="../docs.xml" path='docs/class[@name="Error"]/constructor/*' />
        public Error(SdkException error)
        {
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
