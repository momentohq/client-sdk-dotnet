using System.Collections.Generic;
using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

public abstract class CacheGetBatchResponse
{
    public class Success : CacheGetBatchResponse
    {
        public List<CacheGetResponse> Responses { get; }

        public Success(IEnumerable<CacheGetResponse> responses)
        {
            this.Responses = new(responses);
        }

        public IEnumerable<string?> Strings()
        {
            var ret = new List<string?>();
            foreach (CacheGetResponse response in Responses)
            {
                if (response is CacheGetResponse.Hit hitResponse)
                {
                    ret.Add(hitResponse.String());
                }
                else if (response is CacheGetResponse.Miss missResponse)
                {
                    ret.Add(null);
                }
            }
            return ret.ToArray();
        }

        public IEnumerable<byte[]?> ByteArrays
        {
            get
            {
                var ret = new List<byte[]?>();
                foreach (CacheGetResponse response in Responses)
                {
                    if (response is CacheGetResponse.Hit hitResponse)
                    {
                        ret.Add(hitResponse.ByteArray);
                    }
                    else if (response is CacheGetResponse.Miss missResponse)
                    {
                        ret.Add(null);
                    }
                }
                return ret.ToArray();
            }
        }
    }

    public class Error : CacheGetBatchResponse
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
