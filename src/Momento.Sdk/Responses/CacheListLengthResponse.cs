using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

public abstract class CacheListLengthResponse
{
    public class Success : CacheListLengthResponse
    {
        public int Length { get; private set; } = 0;
        public Success(_ListLengthResponse response)
        {
            if (response.ListCase == _ListLengthResponse.ListOneofCase.Found)
            {
                Length = checked((int)response.Found.Length);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: Length: {Length}";
        }
    }
    public class Error : CacheListLengthResponse
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
