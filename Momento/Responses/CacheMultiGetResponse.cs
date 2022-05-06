using System.Collections.Generic;

namespace MomentoSdk.Responses
{
    public class CacheMultiGetResponse
    {
        private readonly List<CacheGetResponse> successfulResponses;
        private readonly List<CacheMultiGetFailureResponse> failedResponses;

        public CacheMultiGetResponse(List<CacheGetResponse> cacheGetResponses, List<CacheMultiGetFailureResponse> cacheMultiGetFailureResponse)
        {
            successfulResponses = cacheGetResponses;
            failedResponses = cacheMultiGetFailureResponse;
        }

        public List<CacheGetResponse> SuccessfulResponses()
        {
            return successfulResponses;
        }

        public List<CacheMultiGetFailureResponse> FailedResponses()
        {
            return failedResponses;
        }

        public List<string> Strings()
        {
            List<string> values = new();
            foreach (CacheGetResponse response in successfulResponses)
            {
                values.Add(response.String());
            }
            return values;
        }

        public List<byte[]> Bytes()
        {
            List<byte[]> values = new();
            foreach (CacheGetResponse response in successfulResponses)
            {
                values.Add(response.Bytes());
            }
            return values;
        }
    }
}
