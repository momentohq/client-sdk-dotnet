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
    protected readonly ByteString field;

    public byte[] FieldByteArray
    {
        get => field.ToByteArray();
    }

    public string FieldString { get => field.ToStringUtf8(); }

    protected CacheDictionaryGetFieldResponse(ByteString field)
    {
        this.field = field;
    }

    public class Hit : CacheDictionaryGetFieldResponse
    {
        protected readonly ByteString value;

        public Hit(ByteString field, _DictionaryGetResponse response) : base(field)
        {
            this.value = response.Found.Items[0].CacheBody;
        }

        public Hit(ByteString field, ByteString cacheBody) : base(field)
        {
            this.value = cacheBody;
        }

        public byte[] ValueByteArray
        {
            get => value.ToByteArray();
        }

        public string ValueString { get => value.ToStringUtf8(); }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: ValueString: \"{ValueString.Truncate()}\" ValueByteArray: \"{ValueByteArray.ToPrettyHexString().Truncate()}\"";
        }
    }

    public class Miss : CacheDictionaryGetFieldResponse
    {
        public Miss(ByteString field) : base(field)
        {
        }
    }

    public class Error : CacheDictionaryGetFieldResponse
    {
        private readonly SdkException _error;
        
        public Error(SdkException error): base(ByteString.Empty)
        {
            _error = error;
        }

        public Error(ByteString field, SdkException error) : base(field)
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
