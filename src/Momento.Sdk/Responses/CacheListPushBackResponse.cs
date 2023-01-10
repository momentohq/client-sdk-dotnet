using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

public abstract class CacheListPushBackResponse
{
    public class Success : CacheListPushBackResponse
    {
        /// <summary>
        /// The length of the list post-push (and post-truncate, if that applies)
        /// </summary>
        public int ListLength { get; private set; }
        public Success(_ListPushBackResponse response)
        {
            ListLength = checked((int)response.ListLength);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: ListLength: {ListLength}";
        }
    }
    public class Error : CacheListPushBackResponse
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
