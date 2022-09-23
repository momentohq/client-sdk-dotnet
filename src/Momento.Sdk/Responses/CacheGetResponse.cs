using Google.Protobuf;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
namespace Momento.Sdk.Responses;

public abstract class CacheGetResponse
{
    public class Hit : CacheGetResponse
    {
        public CacheGetStatus Status { get; }
        protected readonly ByteString? value;

        public Hit(_GetResponse response)
        {
            if (response.Result is ECacheResult status) {
                Status = CacheGetStatusUtil.From(status);
            } else {
                Status = (CacheGetStatus)response.Result;
            }
            this.value = (Status == CacheGetStatus.HIT) ? response.CacheBody : null;
        }

        public byte[]? ByteArray
        {
            get => value != null ? value.ToByteArray() : null;
        }

        public string? String() => (value != null) ? value.ToStringUtf8() : null;
    }

    public class Miss : CacheGetResponse {
        public CacheGetStatus Status { get; }

        public Miss() {
            Status = CacheGetStatus.MISS;
        }

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
