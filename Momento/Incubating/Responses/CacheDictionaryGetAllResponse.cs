using System;
using System.Collections.Generic;
using System.Text;
using CacheClient;
using Google.Protobuf;
using MomentoSdk.Exceptions;
using MomentoSdk.Responses;

namespace MomentoSdk.Incubating.Responses
{
    public class CacheDictionaryGetAllResponse
    {
        public CacheGetStatus Status { get; private set; }
        private readonly ByteString body;

		// TODO populate constructor with DictionaryResponse gRPC message
        public CacheDictionaryGetAllResponse()
        {
        }

        public Dictionary<string, CacheDictionaryValue>? Dictionary(Encoding encoding = null)
        {
            throw new NotImplementedException();
        }
    }
}
