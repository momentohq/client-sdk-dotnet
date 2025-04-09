using Momento.Sdk.Exceptions;
using Momento.Sdk.Responses;
using System.Collections.Generic;

namespace Momento.Sdk.Responses;

// NB: we exclude this from the build; once we have server-side support we will re-enable and change appropriately
#if USE_UNARY_BATCH
public abstract class CacheGetBatchResponse
{
    public class Success : CacheGetBatchResponse
    {
        public List<CacheGetResponse> Responses { get; }

        public Success(IEnumerable<CacheGetResponse> responses)
        {
            this.Responses = new(responses);
        }

        public IEnumerable<string?> ValueStrings
        {
            get
            {
                var ret = new List<string?>();
                foreach (CacheGetResponse response in Responses)
                {
                    if (response is CacheGetResponse.Hit hitResponse)
                    {
                        ret.Add(hitResponse.ValueString);
                    }
                    else if (response is CacheGetResponse.Miss missResponse)
                    {
                        ret.Add(null);
                    }
                }
                return ret.ToArray();
            }
        }

        public IEnumerable<byte[]?> ValueByteArrays
        {
            get
            {
                var ret = new List<byte[]?>();
                foreach (CacheGetResponse response in Responses)
                {
                    if (response is CacheGetResponse.Hit hitResponse)
                    {
                        ret.Add(hitResponse.ValueByteArray);
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

    public class Error : CacheGetBatchResponse, IError
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
