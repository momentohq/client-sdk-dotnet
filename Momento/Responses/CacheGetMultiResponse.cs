using System.Collections.Generic;
using System.Linq;

namespace MomentoSdk.Responses
{
    public class CacheGetMultiResponse
    {
        private readonly List<CacheGetResponse> responses;

        public CacheGetMultiResponse(IEnumerable<CacheGetResponse> responses)
        {
            this.responses = new(responses);
        }

        public List<CacheGetResponse> ToList()
        {
            return responses;
        }

        public List<CacheGetStatus> Status()
        {
            return responses.Select(response => response.Status).ToList();
        }

        public List<string?> Strings()
        {
            return responses.Select(response => response.String()).ToList();
        }

        public List<byte[]?> Bytes()
        {
            return responses.Select(response => response.Bytes()).ToList();
        }
    }
}
