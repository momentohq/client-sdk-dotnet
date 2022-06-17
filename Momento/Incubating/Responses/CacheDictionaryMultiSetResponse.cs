using System;
using System.Collections.Generic;
using System.Text;
using CacheClient;

namespace MomentoSdk.Incubating.Responses
{
    public class CacheDictionaryMultiSetResponse
    {
        public CacheDictionaryMultiSetResponse()
        {
        }

        public string DictionaryName()
        {
            throw new NotImplementedException();
        }
        public IDictionary<byte[], byte>? DictionaryAsBytes()
        {
            throw new NotImplementedException();
        }
        public IDictionary<string, string>? Dictionary(Encoding encoding = null)
        {
            throw new NotImplementedException();
        }
    }
}
