using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;
using Momento.Sdk.Internal.ExtensionMethods;

namespace Momento.Sdk.Responses;

public abstract class CacheSetFetchResponse
{
    public class Hit : CacheSetFetchResponse
    {
        protected readonly RepeatedField<ByteString> elements;
        protected readonly Lazy<ISet<byte[]>> _byteArraySet;
        protected readonly Lazy<ISet<string>> _stringSet;

        public Hit(_SetFetchResponse response)
        {
            elements = response.Found.Elements;
            _byteArraySet = new(() =>
            {

                return new HashSet<byte[]>(
                    elements.Select(element => element.ToByteArray()),
                    Utils.ByteArrayComparer);
            });

            _stringSet = new(() =>
            {

                return new HashSet<string>(elements.Select(element => element.ToStringUtf8()));
            });
        }

        public ISet<byte[]> ValueSetByteArray { get => _byteArraySet.Value; }

        public ISet<string> ValueSetString { get => _stringSet.Value; }

        /// <inheritdoc />
        public override string ToString()
        {
            var stringRepresentation = String.Join(", ", ValueSetString.Select(value => $"\"{value}\""));
            var byteArrayRepresentation = String.Join(", ", ValueSetByteArray.Select(value => $"\"{value.ToPrettyHexString()}\""));
            return $"{base.ToString()}: ValueSetString: {{{stringRepresentation.Truncate()}}} ValueSetByteArray: {{{byteArrayRepresentation.Truncate()}}}";
        }
    }

    public class Miss : CacheSetFetchResponse
    {

    }

    public class Error : CacheSetFetchResponse
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
