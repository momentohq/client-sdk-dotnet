using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal.ExtensionMethods;

namespace Momento.Sdk.Responses;

public abstract class CacheListFetchResponse
{
    public class Hit : CacheListFetchResponse
    {
        protected readonly RepeatedField<ByteString> values;
        protected readonly Lazy<IList<byte[]>> _byteArrayList;
        protected readonly Lazy<IList<string>> _stringList;

        public Hit(_ListFetchResponse response)
        {
            values = response.Found.Values;
            _byteArrayList = new(() =>
            {
                return new List<byte[]>(values.Select(v => v.ToByteArray()));
            });

            _stringList = new(() =>
            {
                return new List<string>(values.Select(v => v.ToStringUtf8()));
            });
        }

        public IList<byte[]> ValueListByteArray { get => _byteArrayList.Value; }

        public IList<string> ValueListString { get => _stringList.Value; }

        /// <inheritdoc />
        public override string ToString()
        {
            var stringRepresentation = String.Join(", ", ValueListString.Select(value => $"\"{value}\""));
            var byteArrayRepresentation = String.Join(", ", ValueListByteArray.Select(value => $"\"{value.ToPrettyHexString()}\""));
            return $"{base.ToString()}: ValueListString: [{stringRepresentation.Truncate()}] ValueListByteArray: [{byteArrayRepresentation.Truncate()}]";
        }
    }

    public class Miss : CacheListFetchResponse
    {

    }

    public class Error : CacheListFetchResponse
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
