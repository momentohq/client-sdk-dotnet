using System;
using System.Threading.Tasks;
using Momento.Sdk;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Responses;

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
            using SimpleCacheClient client = new SimpleCacheClient(MOMENTO_AUTH_TOKEN, DEFAULT_TTL_SECONDS);

            Console.WriteLine("Creating cache with an illegal empty name");
            CreateCacheResponse emptyCreateResp = client.CreateCache("");
            if (emptyCreateResp is CreateCacheResponse.Error error) {
                Console.WriteLine($"Exception error code: {error.Exception.ErrorCode}");
                Console.WriteLine($"Exception msg: {error.Exception.Message}");
            }

            Console.WriteLine($"Creating cache {CACHE_NAME}");
            CreateCacheResponse createResp = client.CreateCache(CACHE_NAME);
            if (createResp is CreateCacheResponse.Error errorResp2) {
                if (errorResp2.Exception.ErrorCode == MomentoErrorCode.ALREADY_EXISTS_ERROR) {
                    Console.WriteLine($"Cache with name {CACHE_NAME} already exists.");
                } else {
                    Console.WriteLine($"Create error message: {errorResp2.Exception.Message}");
                }
            }

            Console.WriteLine("Listing caches");
            String token = null;
            do {
                ListCachesResponse listResp = client.ListCaches(token);
                if (listResp is ListCachesResponse.Success listSuccessResp) {
                    foreach (CacheInfo cacheInfo in listSuccessResp.Caches)
                    {
                        Console.WriteLine($" -- {cacheInfo.Name}");
                    }
                }
            } while (!String.IsNullOrEmpty(token));

            Console.WriteLine("Deleting cache using null name");
            DeleteCacheResponse nullDeleteResp = client.DeleteCache(null);
            if (nullDeleteResp is DeleteCacheResponse.Error nullDelErrResp) {
                Console.WriteLine($"Delete error message: {nullDelErrResp.Exception.Message}");
            }


            Console.WriteLine("Deleting cache that doesn't exist");
            DeleteCacheResponse deleteResp = client.DeleteCache("idon'texist");
            if (deleteResp is DeleteCacheResponse.Error delErrResp) {
                Console.WriteLine($"Delete error message: {delErrResp.Exception.Message}");
                Console.WriteLine($"gRPC code: {delErrResp.Exception.TransportDetails.Grpc.Code}");
            }

            Console.WriteLine($"\nSetting key: {KEY} with value: {VALUE}");
            await client.SetAsync(CACHE_NAME, KEY, VALUE);
            Console.WriteLine($"\nGet value for  key: {KEY}");
            CacheGetResponse getResponse = await client.GetAsync(CACHE_NAME, KEY);
            Console.WriteLine($"\nLookedup value: {getResponse.String()}, Stored value: {VALUE}");
        }
    }
}
