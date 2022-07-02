#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace MomentoSdk.Incubating.Responses
{
    public class CacheDictionarySetMultiResponse
    {
        public CacheDictionarySetMultiResponse()
        {
        }

        public string DictionaryName()
        {
            throw new NotImplementedException();
        }
        public Dictionary<byte[], byte>? DictionaryAsBytes()
        {
            throw new NotImplementedException();
        }
        public Dictionary<string, string>? Dictionary(Encoding? encoding = null)
        {
            throw new NotImplementedException();
        }
    }
}
