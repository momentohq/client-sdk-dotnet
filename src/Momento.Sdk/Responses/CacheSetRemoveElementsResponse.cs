using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

public abstract class CacheSetRemoveElementsResponse
{
    public class Success : CacheSetRemoveElementsResponse
    {
    }
    public class Error : CacheSetRemoveElementsResponse
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
