using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;
using Momento.Sdk.Internal.ExtensionMethods;

namespace Momento.Sdk.Responses;

public abstract class CacheDictionaryFetchResponse
{
    public class Hit : CacheDictionaryFetchResponse
    {
        protected readonly RepeatedField<_DictionaryFieldValuePair>? items;
        protected readonly Lazy<IDictionary<byte[], byte[]>> _dictionaryByteArrayByteArray;
        protected readonly Lazy<IDictionary<string, string>> _dictionaryStringString;
        protected readonly Lazy<IDictionary<string, byte[]>> _dictionaryStringByteArray;

        public Hit(_DictionaryFetchResponse response)
        {
            items = response.Found.Items;
            _dictionaryByteArrayByteArray = new(() =>
            {
                return new Dictionary<byte[], byte[]>(
                    items.Select(kv => new KeyValuePair<byte[], byte[]>(kv.Field.ToByteArray(), kv.Value.ToByteArray())),
                    Utils.ByteArrayComparer);
            });

            _dictionaryStringString = new(() =>
            {
                return new Dictionary<string, string>(
                    items.Select(kv => new KeyValuePair<string, string>(kv.Field.ToStringUtf8(), kv.Value.ToStringUtf8())));
            });
            _dictionaryStringByteArray = new(() =>
            {
                return new Dictionary<string, byte[]>(
                    items.Select(kv => new KeyValuePair<string, byte[]>(kv.Field.ToStringUtf8(), kv.Value.ToByteArray())));
            });
        }

        public IDictionary<byte[], byte[]> ValueDictionaryByteArrayByteArray { get => _dictionaryByteArrayByteArray.Value; }

        public IDictionary<string, string> ValueDictionaryStringString { get => _dictionaryStringString.Value; }

        public IDictionary<string, byte[]> ValueDictionaryStringByteArray { get => _dictionaryStringByteArray.Value; }

        /// <inheritdoc />
        public override string ToString()
        {
            var stringRepresentation = String.Join(", ", ValueDictionaryStringString.Select(kv => $"\"{kv.Key}\": \"{kv.Value}\""));
            var byteArrayRepresentation = String.Join(", ", ValueDictionaryByteArrayByteArray.Select(kv => $"\"{kv.Key.ToPrettyHexString()}\": \"{kv.Value.ToPrettyHexString()}\""));
            return $"{base.ToString()}: ValueDictionaryStringString: {{{stringRepresentation.Truncate()}}} ValueDictionaryByteArrayByteArray: {{{byteArrayRepresentation.Truncate()}}}";
        }
    }

    public class Miss : CacheDictionaryFetchResponse
    {

    }

    public class Error : CacheDictionaryFetchResponse
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
