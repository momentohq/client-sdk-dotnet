using System.Collections.Generic;
using Google.Protobuf;
using System.Linq;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal.ExtensionMethods;
using Google.Protobuf.WellKnownTypes;

namespace Momento.Sdk.Responses;

public abstract class CacheDictionaryGetFieldResponse
{
    public class Hit : CacheDictionaryGetFieldResponse
    {
        protected readonly ByteString value;
        protected readonly ByteString field;

        public Hit(IEnumerable<ByteString> fields, _DictionaryGetResponse response)
        {
            this.value = response.Found.Items[0].CacheBody;
            this.field = fields.ToList()[0];
        }

        public Hit(ByteString field, ByteString cacheBody)
        {
            this.value = cacheBody;
            this.field = field;
        }

        public byte[] ValueByteArray
        {
            get => value.ToByteArray();
        }

        public byte[] FieldByteArray
        {
            get => field.ToByteArray();
        }

        public string ValueString { get => value.ToStringUtf8(); }

        public string FieldString { get => field.ToStringUtf8(); }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: ValueString: \"{ValueString.Truncate()}\" ValueByteArray: \"{ValueByteArray.ToPrettyHexString().Truncate()}\"";
        }
    }

    public class Miss : CacheDictionaryGetFieldResponse
    {
        protected readonly ByteString field;

        public Miss(IEnumerable<ByteString> fields)
        {
            this.field = fields.ToList()[0];
        }

        public Miss(ByteString field)
        {
            this.field = field;
        }

        public byte[] FieldByteArray
        {
            get => field.ToByteArray();
        }

        public string FieldString { get => field.ToStringUtf8(); }
    }

    public class Error : CacheDictionaryGetFieldResponse
    {
        private readonly SdkException _error;
        protected readonly ByteString field;

        public Error(SdkException error)
        {
            _error = error;
        }

        public Error(IEnumerable<ByteString> fields, SdkException error)
        {
            this.field = fields.ToList()[0];
            _error = error;
        }

        public Error(ByteString field, SdkException error)
        {
            this.field = field;
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

        public byte[] FieldByteArray
        {
            get => field.ToByteArray();
        }

        public string FieldString { get => field.ToStringUtf8(); }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: {Message}";
        }
    }
}
