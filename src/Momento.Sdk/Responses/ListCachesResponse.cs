using System.Collections.Generic;
using Momento.Protos.ControlClient;
using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

public abstract class ListCachesResponse
{
    public class Success : ListCachesResponse
    {
        public List<CacheInfo> Caches { get; }
        public string? NextPageToken { get; }

        public Success(_ListCachesResponse result)
        {
            NextPageToken = result.NextToken == "" ? null : result.NextToken;
            Caches = new List<CacheInfo>();
            foreach (_Cache c in result.Cache)
            {
                Caches.Add(new CacheInfo(c.CacheName));
            }
        }
    }

    public class Error : ListCachesResponse
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
