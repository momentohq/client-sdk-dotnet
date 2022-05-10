using System.Collections.Generic;

namespace MomentoSdk.Responses
{
    public class CacheMultiGetResponse
    {
        private readonly List<CacheGetResponse> successfulResponses;
        private readonly List<CacheMultiGetFailureResponse> failedResponses;

        private readonly CacheGetResponse successfulResponse = null;
        private readonly CacheMultiGetFailureResponse failedResponse = null;

        public CacheMultiGetResponse(List<CacheGetResponse> cacheGetResponses, List<CacheMultiGetFailureResponse> cacheMultiGetFailureResponses)
        {
            successfulResponses = cacheGetResponses;
            failedResponses = cacheMultiGetFailureResponses;
        }

        public CacheMultiGetResponse(CacheGetResponse cacheGetResponse)
        {
            successfulResponse = cacheGetResponse;
        }

        public CacheMultiGetResponse(CacheMultiGetFailureResponse cacheMultiGetFailureResponse)
        {
            failedResponse = cacheMultiGetFailureResponse;
        }

        public CacheGetResponse SuccessfulResponse()
        {
            return successfulResponse;
        }

        public CacheMultiGetFailureResponse FailedResponse()
        {
            return failedResponse;
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
