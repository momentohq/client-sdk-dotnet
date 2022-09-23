namespace Momento.Sdk.Responses;

using Momento.Sdk.Exceptions;

public class CacheSetResponse
{

    public class Success : CacheSetResponse { }

    public class Error : CacheSetResponse
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
