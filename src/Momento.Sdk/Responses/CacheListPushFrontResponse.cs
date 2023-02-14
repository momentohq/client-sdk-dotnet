using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

/// <summary>
/// The result of a <c>ListPushFront</c> command
/// </summary>
///
public abstract class CacheListPushFrontResponse
{
    public class Success : CacheListPushFrontResponse
    {
        public int ListLength { get; private set; }
        public Success(_ListPushFrontResponse response)
        {
            ListLength = checked((int)response.ListLength);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: ListLength: {ListLength}";
        }
    }
    public class Error : CacheListPushFrontResponse
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
