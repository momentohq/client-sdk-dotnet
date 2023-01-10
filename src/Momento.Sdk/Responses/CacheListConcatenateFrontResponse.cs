using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

/// <summary>
/// The result of a <c>ListConcatenateFront</c> command
/// </summary>
///
public abstract class CacheListConcatenateFrontResponse
{
    public class Success : CacheListConcatenateFrontResponse
    {
        public int ListLength { get; private set; }
        public Success(_ListConcatenateFrontResponse response)
        {
            ListLength = checked((int)response.ListLength);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: ListLength: {ListLength}";
        }
    }
    public class Error : CacheListConcatenateFrontResponse
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
