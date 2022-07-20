using System;
using System.Collections.Generic;
using System.Text;
using MomentoSdk.Responses;

namespace MomentoSdk.Incubating.Responses
{
    public class CacheDictionaryGetAllResponse
    {
        public CacheDictionaryGetAllResponse()
        {
        }

        public CacheGetStatus Status()
        {
            throw new NotImplementedException();
        }

        public Dictionary<byte[], byte[]>? DictionaryAsBytes()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string>? Dictionary(Encoding? encoding = null)
        {
            throw new NotImplementedException();
        }
    }
}
