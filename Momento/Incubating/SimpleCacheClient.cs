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

        public CacheDictionarySetResponse DictionarySet(string cacheName, string dictionaryName, byte[] key, byte[] value)
        {
            throw new NotImplementedException();
        }

        public CacheDictionarySetResponse DictionarySet(string cacheName, string dictionaryName, string key, string value)
        {
            throw new NotImplementedException();
        }

        public async Task<CacheDictionarySetResponse> DictionarySetAsync(string cacheName, string dictionaryName, byte[] key, byte[] value)
        {
            return await Task.FromException<CacheDictionarySetResponse>(new NotImplementedException());
        }

        public async Task<CacheDictionarySetResponse> DictionarySetAsync(string cacheName, string dictionaryName, string key, string value)
        {
            return await Task.FromException<CacheDictionarySetResponse>(new NotImplementedException());
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

        public CacheDictionaryMultiSetResponse DictionaryMultiSet(string cacheName, string dictionaryName, IDictionary<byte[], byte[]> dictionary)
        {
            throw new NotImplementedException();
        }

        public CacheDictionaryMultiSetResponse DictionaryMultiSet(string cacheName, string dictionaryName, IDictionary<string, string> dictionary)
        {
            throw new NotImplementedException();
        }

        public async Task<CacheDictionaryMultiSetResponse> DictionaryMultiSetAsync(string cacheName, string dictionaryName, IDictionary<string, string> dictionary)
        {
            return await Task.FromException<CacheDictionaryMultiSetResponse>(new NotImplementedException());
        }

        public async Task<CacheDictionaryMultiSetResponse> DictionaryMultiSetAsync(string cacheName, string dictionaryName, IDictionary<byte[], byte[]> dictionary)
        {
            return await Task.FromException<CacheDictionaryMultiSetResponse>(new NotImplementedException());
        }

        public CacheDictionaryMultiGetResponse DictionaryMultiGet(string cacheName, string dictionaryName, params byte[][] keys)
        {
            throw new NotImplementedException();
        }

        public CacheDictionaryMultiGetResponse DictionaryMultiGet(string cacheName, string dictionaryName, params string[] key)
        {
            throw new NotImplementedException();
        }

        public async Task<CacheDictionaryMultiGetResponse> DictionaryMultiGetAsync(string cacheName, string dictionaryName, params byte[][] key)
        {
            return await Task.FromException<CacheDictionaryMultiGetResponse>(new NotImplementedException());
        }

        public async Task<CacheDictionaryMultiGetResponse> DictionaryMultiGetAsync(string cacheName, string dictionaryName, params string[] key)
        {
            return await Task.FromException<CacheDictionaryMultiGetResponse>(new NotImplementedException());
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

