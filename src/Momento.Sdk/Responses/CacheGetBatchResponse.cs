using System.Collections.Generic;
using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

public class CacheGetBatchResponse
{
    public class Success : CacheGetBatchResponse
    {
        public List<CacheGetResponse> Responses { get; }

        public Success(IEnumerable<CacheGetResponse> responses)
        {
            this.Responses = new(responses);
        }

        public IEnumerable<CacheGetStatus> Status
        {
            get
            {
                var ret = new List<CacheGetStatus>();
                foreach (CacheGetResponse.Success response in Responses)
                {
                    ret.Add(response.Status);
                }
                return ret;
            }
        }

        public IEnumerable<string?> Strings()
        {
            var ret = new List<string?>();
            foreach (CacheGetResponse.Success response in Responses)
            {
                ret.Add(response.String());
            }
            return ret.ToArray();
        }

        public IEnumerable<byte[]?> ByteArrays
        {
            get
            {
                var ret = new List<byte[]?>();
                foreach (CacheGetResponse.Success response in Responses)
                {
                    ret.Add(response.ByteArray);
                }
                return ret;
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
    }
}
