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
            Console.WriteLine(emptyCreateResp);
            if (emptyCreateResp is CreateCacheResponse.Error error) {
                Console.WriteLine(error.Exception.errorCode);
                Console.WriteLine(error.Exception);
            }

            Console.WriteLine("Creating cache that already exists");
            CreateCacheResponse existsCreateResp = client.CreateCache("test1");
            if (existsCreateResp is CreateCacheResponse.Error errorResp) {
                if (errorResp.Exception.errorCode == MomentoErrorCode.ALREADY_EXISTS_ERROR) {
                    Console.WriteLine($"Cache with name {CACHE_NAME} already exists.");
                }
            }

            Console.WriteLine($"Creating cache {CACHE_NAME}");
            CreateCacheResponse createResp = client.CreateCache(CACHE_NAME);
            if (createResp is CreateCacheResponse.Error errorResp2) {
                if (errorResp2.Exception.errorCode == MomentoErrorCode.ALREADY_EXISTS_ERROR) {
                    Console.WriteLine($"Cache with name {CACHE_NAME} already exists.");
                } else if (errorResp2.Exception.errorCode == MomentoErrorCode.LIMIT_EXCEEDED_ERROR) {
                    Console.WriteLine($"Create error message: {errorResp2.Exception.Message}");
                    Console.WriteLine($"Limit exceeded: {errorResp2.Exception}");
                    Console.WriteLine($"gRPC code: {errorResp2.Exception.transportDetails.grpc.code}");
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

            Console.WriteLine("Deleting cache that doens't exist");
            DeleteCacheResponse deleteResp = client.DeleteCache("idon'texist");
            if (deleteResp is DeleteCacheResponse.Error delErrResp) {
                Console.WriteLine($"Delete error message: {delErrResp.Exception.Message}");
                Console.WriteLine($"gRPC code: {delErrResp.Exception.transportDetails.grpc.code}");
            }

            // Console.WriteLine($"\nSetting key: {KEY} with value: {VALUE}");
            // await client.SetAsync(CACHE_NAME, KEY, VALUE);
            // Console.WriteLine($"\nGet value for  key: {KEY}");
            // CacheGetResponse getResponse = await client.GetAsync(CACHE_NAME, KEY);
            // Console.WriteLine($"\nLookedup value: {getResponse.String()}, Stored value: {VALUE}");
        }
    }
}
