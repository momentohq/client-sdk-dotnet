#nullable enable

using System;
using System.Collections.Generic;
using System.Text;
using MomentoSdk.Responses;


namespace MomentoSdk.Incubating.Responses
{
    public class CacheDictionaryMultiGetResponse
    {
        public CacheDictionaryMultiGetResponse()
        {
        }

        public IList<CacheGetStatus> Status()
        {
            throw new NotImplementedException();
        }

        public IList<string?> Values(Encoding? encoding = null)
        {
            throw new NotImplementedException();
        }

        public IList<byte[]?> ValuesAsBytes()
        {
            throw new NotImplementedException();
        }

        public IList<CacheGetResponse> ToList()
        {

            throw new NotImplementedException();
        }
    }
}
