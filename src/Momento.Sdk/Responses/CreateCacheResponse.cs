namespace Momento.Sdk.Responses;

using Momento.Sdk.Exceptions;

public class CreateCacheResponse {

    public class Success : CreateCacheResponse { }

    public class Error: CreateCacheResponse
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
    }

}
