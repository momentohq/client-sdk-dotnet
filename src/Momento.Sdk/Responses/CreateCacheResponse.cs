namespace Momento.Sdk.Responses;

using Momento.Sdk.Exceptions;

public abstract class CreateCacheResponse
{

    public class Success : CreateCacheResponse { }

    public class CacheAlreadyExists : CreateCacheResponse { }

    public class Error : CreateCacheResponse
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
            return $"{base.ToString()}: {this.Message}";
        }

    }

}
