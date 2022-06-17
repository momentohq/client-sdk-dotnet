using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MomentoSdk.Incubating.Responses;

namespace MomentoSdk.Incubating
{
    public class SimpleCacheClient : MomentoSdk.SimpleCacheClient
    {
        public SimpleCacheClient(string authToken, uint defaultTtlSeconds) : base(authToken, defaultTtlSeconds)
        {
        }

        public CacheDictionaryMultiSetResponse DictionaryMultiSet(string cacheName, string dictionaryName, Dictionary<byte[], byte[]> dictionary)
        {
            throw new NotImplementedException();
        }

        public CacheDictionaryMultiSetResponse DictionaryMultiSet(string cacheName, string dictionaryName, Dictionary<string, string> dictionary)
        {
            throw new NotImplementedException();
        }

        public async Task<CacheDictionaryMultiSetResponse> DictionaryMultiSetAsync(string cacheName, string dictionaryName, Dictionary<string, string> dictionary)
        {
            return await Task.FromException<CacheDictionaryMultiSetResponse>(new NotImplementedException());
        }

        public async Task<CacheDictionaryMultiSetResponse> DictionaryMultiSetAsync(string cacheName, string dictionaryName, Dictionary<byte[], byte[]> dictionary)
        {
            return await Task.FromException<CacheDictionaryMultiSetResponse>(new NotImplementedException());
        }

        public CacheDictionaryGetResponse DictionaryGet(string cacheName, string dictionaryName, byte[] key)
        {
            throw new NotImplementedException();
        }

        public CacheDictionaryGetResponse DictionaryGet(string cacheName, string dictionaryName, string key)
        {
            throw new NotImplementedException();
        }

        public async Task<CacheDictionaryGetResponse> DictionaryGetAsync(string cacheName, string dictionaryName, byte[] key)
        {
            return await Task.FromException<CacheDictionaryGetResponse>(new NotImplementedException());
        }

        public async Task<CacheDictionaryGetResponse> DictionaryGetAsync(string cacheName, string dictionaryName, string key)
        {
            return await Task.FromException<CacheDictionaryGetResponse>(new NotImplementedException());
        }

        public CacheDictionaryGetAllResponse DictionaryGetAll(string cacheName, string dictionaryName)
        {
            throw new NotImplementedException();
        }

        public async Task<CacheDictionaryGetAllResponse> DictionaryGetAllAsync(string cacheName, string dictionaryName)
        {
            return await Task.FromException<CacheDictionaryGetAllResponse>(new NotImplementedException());
        }
    }
}

