using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Responses;


public class Program
{
    public static async Task Main(string[] args)
    {
        var config = Configurations.Laptop.V1();
        var client = new CacheClient(config,
            new EnvMomentoTokenProvider("MOMENTO_AUTH_TOKEN"),
            TimeSpan.FromSeconds(10));

        await Example_API_CreateCache(client);
    }

    public static async Task Example_API_CreateCache(CacheClient cacheClient)
    {
        var result = await cacheClient.CreateCacheAsync("test-cache");
        if (result is CreateCacheResponse.Success)
        {
            Console.WriteLine("Cache 'test-cache' created");
        }
        else if (result is CreateCacheResponse.CacheAlreadyExists)
        {
            Console.WriteLine("Cache 'test-cache' already exists");
        }
        else
        {
            throw new Exception($"An error occurred while attempting to create cache 'test-cache': {result}");
        }
    }
    /*
    async function example_API_CreateCache(cacheClient: CacheClient) {
      const result = await cacheClient.createCache('test-cache');
      if (result instanceof CreateCache.Success) {
        console.log("Cache 'test-cache' created");
      } else if (result instanceof CreateCache.AlreadyExists) {
        console.log("Cache 'test-cache' already exists");
      } else {
        throw new Error(`An error occurred while attempting to create cache 'test-cache': ${result.toString()}`);
      }
    }

    async function example_API_DeleteCache(cacheClient: CacheClient) {
      const result = await cacheClient.deleteCache('test-cache');
      if (result instanceof DeleteCache.Success) {
        console.log("Cache 'test-cache' deleted");
      } else if (result instanceof DeleteCache.Error) {
        throw new Error(
          `An error occurred while attempting to delete cache 'test-cache': ${result.errorCode()}: ${result.toString()}`
        );
      }
    }

    async function example_API_ListCaches(cacheClient: CacheClient) {
      const result = await cacheClient.listCaches();
      if (result instanceof ListCaches.Success) {
        console.log(
          `Caches:\n${result
            .getCaches()
            .map(c => c.getName())
            .join('\n')}\n\n`
        );
      } else if (result instanceof ListCaches.Error) {
        throw new Error(`An error occurred while attempting to list caches: ${result.errorCode()}: ${result.toString()}`);
      }
    }

    async function example_API_FlushCache(cacheClient: CacheClient) {
      const result = await cacheClient.flushCache('test-cache');
      if (result instanceof CacheFlush.Success) {
        console.log("Cache 'test-cache' flushed");
      } else if (result instanceof CacheFlush.Error) {
        throw new Error(
          `An error occurred while attempting to flush cache 'test-cache': ${result.errorCode()}: ${result.toString()}`
        );
      }
    }
    */
}
