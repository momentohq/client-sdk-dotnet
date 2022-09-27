using Google.Protobuf;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
namespace Momento.Sdk.Responses;

public abstract class CacheGetResponse
{
    public class Hit : CacheGetResponse
    {
        protected readonly ByteString value;

        public Hit(_GetResponse response)
        {
            this.value = response.CacheBody;
        }

        public byte[] ByteArray
        {
            get => value.ToByteArray();
        }

        public string String() => value.ToStringUtf8();
    }

    public class Miss : CacheGetResponse
    {
        public Miss() { }
    }

    public class Error : CacheGetResponse
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

    }
}
