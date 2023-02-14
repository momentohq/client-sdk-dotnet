namespace Momento.Sdk.Responses;

using Momento.Sdk.Exceptions;

// NB: we exclude this from the build; once we have server-side support we will re-enable and change appropriately
#if USE_UNARY_BATCH
public abstract class CacheSetBatchResponse
{

    public class Success : CacheSetBatchResponse { }

    public class Error : CacheSetBatchResponse
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

        public override string ToString()
        {
            return base.ToString() + ": " + Message;
        }
    }

}
#endif
