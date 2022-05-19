using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MomentoSdk.Incubating.Responses;

namespace MomentoSdk.Incubating
{
    public class SimpleCacheClient : MomentoSdk.Responses.SimpleCacheClient
    {
        public SimpleCacheClient(string authToken, uint defaultTtlSeconds) : base(authToken, defaultTtlSeconds)
        {
        }

        public SimpleCacheClient(string authToken, uint defaultTtlSeconds, uint dataClientOperationTimeoutSeconds) : base(authToken, defaultTtlSeconds, dataClientOperationTimeoutSeconds)
        {
        }

        // TODO: Dictionary can be <string|byte[], string|byte[]>
        public CacheDictionarySetResponse DictionarySet(string cacheName, string dictionaryName, Dictionary<string, string> dictionary)
        {
            throw new NotImplementedException();
        }

        public async Task<CacheDictionarySetResponse> DictionarySetAsync(string cacheName, string dictionaryName, Dictionary<string, string> dictionary)
        {
            throw new NotImplementedException();
        }

        // TODO: key can be byte[]
        public CacheDictionaryGetResponse DictionaryGet(string cacheName, string dictionaryName, string key)
        {
            throw new NotImplementedException();
        }

        public async Task<CacheDictionaryGetResponse> DictionaryGetAsync(string cacheName, string dictionaryName, string key)
        {
            throw new NotImplementedException();
        }

        public CacheDictionaryGetAllResponse DictionaryGetAll(string cacheName, string dictionaryName)
        {
            throw new NotImplementedException();
        }

        public async Task<CacheDictionaryGetAllResponse> DictionaryGetAllAsync(string cacheName, string dictionaryName)
        {
            throw new NotImplementedException();
        }
    }
}

