using Google.Protobuf;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
namespace Momento.Sdk.Responses;

public class CacheGetResponse
{
    public class Success : CacheGetResponse
    {
        public CacheGetStatus Status { get; }
        protected readonly ByteString? value;

        public Success(_GetResponse response)
        {
            if (response.Result is ECacheResult status) {
                Status = CacheGetStatusUtil.From(status);
                this.value = response.CacheBody;
            } else {
                Status = (CacheGetStatus)response.Result;
                this.value = (Status == CacheGetStatus.HIT) ? value : null;
            }
        }

        public byte[]? ByteArray
        {
            get => value != null ? value.ToByteArray() : null;
        }

        public string? String() => (value != null) ? value.ToStringUtf8() : null;

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
    }
}
