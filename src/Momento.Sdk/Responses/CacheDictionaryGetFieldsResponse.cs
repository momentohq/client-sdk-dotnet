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

public abstract class CacheDictionaryGetFieldsResponse
{
    public class Hit : CacheDictionaryGetFieldsResponse
    {
        public List<CacheDictionaryGetFieldResponse> Responses { get; private set; }
        protected readonly Lazy<Dictionary<byte[], byte[]>> _dictionaryByteArrayByteArray;
        protected readonly Lazy<Dictionary<string, string>> _dictionaryStringString;
        protected readonly Lazy<Dictionary<string, byte[]>> _dictionaryStringByteArray;

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

        public Dictionary<byte[], byte[]> ValueDictionaryByteArrayByteArray { get => _dictionaryByteArrayByteArray.Value; }

        public Dictionary<string, string> ValueDictionaryStringString { get => _dictionaryStringString.Value; }

        public Dictionary<string, byte[]> ValueDictionaryStringByteArray { get => _dictionaryStringByteArray.Value; }

        /// <inheritdoc />
        public override string ToString()
        {
            var stringRepresentation = String.Join(", ", ValueDictionaryStringString.Select(kv => $"\"{kv.Key}\": \"{kv.Value}\""));
            var byteArrayRepresentation = String.Join(", ", ValueDictionaryByteArrayByteArray.Select(kv => $"\"{kv.Key.ToPrettyHexString()}\": \"{kv.Value.ToPrettyHexString()}\""));
            return $"{base.ToString()}: ValueDictionaryStringString: {{{stringRepresentation.Truncate()}}} ValueDictionaryByteArrayByteArray: {{{byteArrayRepresentation.Truncate()}}}";
        }
    }

    public class Miss : CacheDictionaryGetFieldsResponse
    {

    }

    public class Error : CacheDictionaryGetFieldsResponse
    {
        private readonly SdkException _error;
        public Error(SdkException error)
        {
            _error = error;
        }

        public SdkException Exception
        {
            get => _error;
        }

        public MomentoErrorCode ErrorCode
        {
            get => _error.ErrorCode;
        }

        public string Message
        {
            get => $"{_error.MessageWrapper}: {_error.Message}";
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: {Message}";
        }
    }
}
