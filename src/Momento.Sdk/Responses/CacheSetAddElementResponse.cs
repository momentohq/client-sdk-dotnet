using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

public abstract class CacheSetAddElementResponse
{
    public class Success : CacheSetAddElementResponse
    {
    }
    public class Error : CacheSetAddElementResponse
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
