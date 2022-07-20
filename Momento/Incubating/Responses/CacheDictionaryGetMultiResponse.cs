using System;
using System.Collections.Generic;
using System.Text;
using MomentoSdk.Responses;


namespace MomentoSdk.Incubating.Responses
{
    public class CacheDictionaryGetMultiResponse
    {
        public CacheDictionaryGetMultiResponse()
        {
        }

        public List<CacheGetStatus> Status()
        {
            throw new NotImplementedException();
        }

        public List<string?> Values(Encoding? encoding = null)
        {
            throw new NotImplementedException();
        }

        public List<byte[]?> ValuesAsBytes()
        {
            throw new NotImplementedException();
        }

        public List<CacheGetResponse> ToList()
        {
            throw new NotImplementedException();
        }
    }
}
