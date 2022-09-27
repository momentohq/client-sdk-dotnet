using System;
using System.Threading.Tasks;
using Momento.Sdk;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Responses;
using Momento.Sdk.Config;
using Microsoft.Extensions.Logging;

namespace MomentoApplication
{
    class Program
    {
        static readonly String MOMENTO_AUTH_TOKEN = Environment.GetEnvironmentVariable("MOMENTO_AUTH_TOKEN");
        static readonly String CACHE_NAME = "cache";
        static readonly String KEY = "MyKey";
        static readonly String VALUE = "MyData";
        static readonly uint DEFAULT_TTL_SECONDS = 60;

        async static Task Main(string[] args)
        {
            using SimpleCacheClient client = new SimpleCacheClient(Configurations.Laptop.Latest, MOMENTO_AUTH_TOKEN, DEFAULT_TTL_SECONDS);
            var createCacheResult = client.CreateCache(CACHE_NAME);
            
            Console.WriteLine("Listing caches:");
            String token = null;
            do
            {
                ListCachesResponse resp = client.ListCaches(token);
                if (resp is ListCachesResponse.Success successResult)
                {
                    foreach (CacheInfo cacheInfo in successResult.Caches)
                    {
                        Console.WriteLine(cacheInfo.Name);
                    }
                    token = successResult.NextPageToken;
                }
            } while (!String.IsNullOrEmpty(token));
            Console.WriteLine($"\nSetting key: {KEY} with value: {VALUE}");
            await client.SetAsync(CACHE_NAME, KEY, VALUE);
            Console.WriteLine($"\nGet value for  key: {KEY}");
            CacheGetResponse getResponse = await client.GetAsync(CACHE_NAME, KEY);
            if (getResponse is CacheGetResponse.Hit hitResponse)
            {
                Console.WriteLine($"\nLookedup value: {hitResponse.String()}, Stored value: {VALUE}");
            }
        }
    }
}
