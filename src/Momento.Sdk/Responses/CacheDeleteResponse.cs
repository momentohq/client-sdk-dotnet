namespace Momento.Sdk.Responses;

using Momento.Sdk.Exceptions;

public class CacheDeleteResponse
{
    public class Success : CacheDeleteResponse { }

    public class Error : CacheDeleteResponse
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
