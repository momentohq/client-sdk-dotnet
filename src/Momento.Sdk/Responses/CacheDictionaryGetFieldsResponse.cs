using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;
using Momento.Sdk.Internal.ExtensionMethods;
using Momento.Sdk.Responses;
using static Momento.Protos.CacheClient._DictionaryGetResponse.Types;

namespace Momento.Sdk.Responses;

/// <summary>
/// Parent response type for a cache dictionary get fields request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheDictionaryGetFieldsResponse.Hit</description></item>
/// <item><description>CacheDictionaryGetFieldsResponse.Miss</description></item>
/// <item><description>CacheDictionaryGetFieldsResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheDictionaryGetFieldsResponse.Hit hitResponse)
/// {
///     return hitResponse.ValueDictionaryStringString;
/// }
/// else if (response is CacheDictionaryGetFieldsResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CacheDictionaryGetFieldsResponse
{
    /// <include file="../docs.xml" path='docs/class[@name="Hit"]/description/*' />
    public class Hit : CacheDictionaryGetFieldsResponse
    {
        /// <summary>
        /// Individual responses for each queried field.
        /// </summary>
        public List<CacheDictionaryGetFieldResponse> Responses { get; private set; }

#pragma warning disable 1591
        protected readonly Lazy<IDictionary<byte[], byte[]>> _dictionaryByteArrayByteArray;
        protected readonly Lazy<IDictionary<string, string>> _dictionaryStringString;
        protected readonly Lazy<IDictionary<string, byte[]>> _dictionaryStringByteArray;
#pragma warning restore 1591

        /// <summary>
        /// Instantiate a hit.
        /// </summary>
        /// <param name="fields">The fields queried.</param>
        /// <param name="responses">The responses from the cache.</param>
        public Hit(IEnumerable<ByteString> fields, _DictionaryGetResponse responses)
        {
            var responsesList = new List<CacheDictionaryGetFieldResponse>();
            List<ByteString> fieldsList = fields.ToList();
            var counter = 0;
            foreach (_DictionaryGetResponsePart response in responses.Found.Items)
            {
                if (response.Result == ECacheResult.Hit)
                {
                    responsesList.Add(new CacheDictionaryGetFieldResponse.Hit(fieldsList[counter], response.CacheBody));
                }
                else if (response.Result == ECacheResult.Miss)
                {
                    responsesList.Add(new CacheDictionaryGetFieldResponse.Miss(fieldsList[counter]));
                }
                else
                {
                    responsesList.Add(new CacheDictionaryGetFieldResponse.Error(fieldsList[counter], new UnknownException(response.Result.ToString())));
                }
                counter++;
            }
            this.Responses = responsesList;

            _dictionaryByteArrayByteArray = new(() =>
            {
                return new Dictionary<byte[], byte[]>(
                    fields.Zip(responses.Found.Items, (f, r) => new ValueTuple<ByteString, _DictionaryGetResponsePart>(f, r))
                        .Where(pair => pair.Item2.Result == ECacheResult.Hit)
                        .Select(pair => new KeyValuePair<byte[], byte[]>(pair.Item1.ToByteArray(), pair.Item2.CacheBody.ToByteArray())),
                    Utils.ByteArrayComparer);
            });

            _dictionaryStringString = new(() =>
            {
                return new Dictionary<string, string>(
                    fields.Zip(responses.Found.Items, (f, r) => new ValueTuple<ByteString, _DictionaryGetResponsePart>(f, r))
                        .Where(pair => pair.Item2.Result == ECacheResult.Hit)
                        .Select(pair => new KeyValuePair<string, string>(pair.Item1.ToStringUtf8(), pair.Item2.CacheBody.ToStringUtf8())));
            });
            _dictionaryStringByteArray = new(() =>
            {
                return new Dictionary<string, byte[]>(
                    fields.Zip(responses.Found.Items, (f, r) => new ValueTuple<ByteString, _DictionaryGetResponsePart>(f, r))
                        .Where(pair => pair.Item2.Result == ECacheResult.Hit)
                        .Select(pair => new KeyValuePair<string, byte[]>(pair.Item1.ToStringUtf8(), pair.Item2.CacheBody.ToByteArray())));
            });
        }

        /// <summary>
        /// The cached responses as a <see cref="byte"/> array to <see cref="byte"/> array mapping.
        /// </summary>
        public IDictionary<byte[], byte[]> ValueDictionaryByteArrayByteArray { get => _dictionaryByteArrayByteArray.Value; }

        /// <summary>
        /// The cached responses as a <see cref="string"/> to <see cref="string"/> mapping.
        /// </summary>
        public IDictionary<string, string> ValueDictionaryStringString { get => _dictionaryStringString.Value; }

        /// <summary>
        /// The cached responses as a <see cref="string"/> to <see cref="byte"/> array mapping.
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
    public class Miss : CacheDictionaryGetFieldsResponse
    {

    }

    /// <include file="../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheDictionaryGetFieldsResponse
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
