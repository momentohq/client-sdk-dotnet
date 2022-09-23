namespace Momento.Sdk.Responses;

using Momento.Sdk.Exceptions;

public abstract class CacheSetBatchResponse
{

    public class Success : CacheSetBatchResponse { }

    public class Error: CacheSetBatchResponse
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
