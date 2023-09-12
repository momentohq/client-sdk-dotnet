using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Auth.AccessControl;
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
        IAuthClient authClient = new AuthClient(
            AuthConfigurations.Default.Latest(), 
            new EnvMomentoTokenProvider("MOMENTO_AUTH_TOKEN")
        );
        ITopicClient topicClient = new TopicClient(
            TopicConfigurations.Laptop.latest(), 
            new EnvMomentoTokenProvider("MOMENTO_AUTH_TOKEN")
        );

        await Example_API_CreateCache(client);
        await Example_API_FlushCache(client);
        await Example_API_DeleteCache(client);
        await Example_API_ListCaches(client);

        await Example_API_CreateCache(client);
        await Example_API_Set(client);
        await Example_API_Get(client);
        await Example_API_Delete(client);

        await Example_API_GenerateDisposableToken(authClient);

        await Example_API_InstantiateTopicClient();
        await Example_API_TopicSubscribe(topicClient);
        await Example_API_TopicPublish(topicClient);
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
        else if (result is CreateCacheResponse.Error error)
        {
            throw new Exception($"An error occurred while attempting to create cache 'test-cache': {error.ErrorCode}: {error}");
        }
    }

    public static async Task Example_API_DeleteCache(CacheClient cacheClient)
    {
        var result = await cacheClient.DeleteCacheAsync("test-cache");
        if (result is DeleteCacheResponse.Success)
        {
            Console.WriteLine("Cache 'test-cache' deleted");
        }
        else if (result is DeleteCacheResponse.Error error)
        {
            throw new Exception($"An error occurred while attempting to delete cache 'test-cache': {error.ErrorCode}: {error}");
        }
    }

    public static async Task Example_API_ListCaches(CacheClient cacheClient)
    {
        var result = await cacheClient.ListCachesAsync();
        if (result is ListCachesResponse.Success success)
        {
            Console.WriteLine($"Caches:\n{string.Join("\n", success.Caches.Select(c => c.Name))}\n\n");
        }
        else if (result is ListCachesResponse.Error error)
        {
            throw new Exception($"An error occurred while attempting to list caches: {error.ErrorCode}: {error}");
        }
    }

    public static async Task Example_API_FlushCache(CacheClient cacheClient)
    {
        var result = await cacheClient.FlushCacheAsync("test-cache");
        if (result is FlushCacheResponse.Success)
        {
            Console.WriteLine("Cache 'test-cache' flushed");
        }
        else if (result is FlushCacheResponse.Error error)
        {
            throw new Exception($"An error occurred while attempting to flush cache 'test-cache': {error.ErrorCode}: {error}");
        }
    }

    public static async Task Example_API_Set(CacheClient cacheClient)
    {
        var result = await cacheClient.SetAsync("test-cache", "test-key", "test-value");
        if (result is CacheSetResponse.Success)
        {
            Console.WriteLine("Key 'test-key' stored successfully");
        }
        else if (result is CacheSetResponse.Error error)
        {
            throw new Exception($"An error occurred while attempting to store key 'test-key' in cache 'test-cache': {error.ErrorCode}: {error}");
        }
    }

    public static async Task Example_API_Get(CacheClient cacheClient)
    {
        var result = await cacheClient.GetAsync("test-cache", "test-key");
        if (result is CacheGetResponse.Hit hit)
        {
            Console.WriteLine($"Retrieved value for key 'test-key': {hit.ValueString}");
        }
        else if (result is CacheGetResponse.Miss)
        {
            Console.WriteLine("Key 'test-key' was not found in cache 'test-cache'");
        }
        else if (result is CacheGetResponse.Error error)
        {
            throw new Exception($"An error occurred while attempting to get key 'test-key' from cache 'test-cache': {error.ErrorCode}: {error}");
        }
    }

    public static async Task Example_API_Delete(CacheClient cacheClient)
    {
        var result = await cacheClient.DeleteAsync("test-cache", "test-key");
        if (result is CacheDeleteResponse.Success)
        {
            Console.WriteLine("Key 'test-key' deleted successfully");
        }
        else if (result is CacheDeleteResponse.Error error)
        {
            throw new Exception($"An error occurred while attempting to delete key 'test-key' from cache 'test-cache': {error.ErrorCode}: {error}");
        }
    }

    public static async Task Example_API_GenerateDisposableToken(IAuthClient authClient)
    {
        var scope = new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadWrite,
                CacheSelector.ByName("cache"),
                CacheItemSelector.AllCacheItems
            ),
            new DisposableToken.CachePermission(
                CacheRole.ReadOnly,
                CacheSelector.ByName("topsecret")
            ),
            new DisposableToken.TopicPermission(
                TopicRole.PublishSubscribe,
                CacheSelector.ByName("cache"),
                TopicSelector.ByName("example-topic")
            )
        });
        var tokenResponse = await authClient.GenerateDisposableTokenAsync(
            scope,
            ExpiresIn.Minutes(5)
        );

        if (tokenResponse is GenerateDisposableTokenResponse.Success token)
        {
            Console.WriteLine("The generated disposable token is: " + token.AuthToken);
            Console.WriteLine("The token endpoint is: " + token.Endpoint);
            Console.WriteLine("The token expires at (epoch timestamp): " + token.ExpiresAt.Epoch());
        }
        else if (tokenResponse is GenerateDisposableTokenResponse.Error err)
        {
            Console.WriteLine("Error generating disposable token: " + err.Message);
        }
    }

    public static async Task Example_API_InstantiateTopicClient()
    {
        new TopicClient(
            TopicConfigurations.Laptop.latest(), 
            new EnvMomentoTokenProvider("MOMENTO_AUTH_TOKEN")
        );
    }
    public static async Task Example_API_TopicPublish(ITopicClient topicClient)
    {
       var publishResponse =
            await topicClient.PublishAsync("test-cache", "test-topic", "test-topic-value");
        switch (publishResponse)
        {
            case TopicPublishResponse.Success:
                Console.WriteLine("Successfully published message to 'test-topic'");
                break;
            case TopicPublishResponse.Error error:
                throw new Exception($"An error occurred while publishing topic message: {error.ErrorCode}: {error}");
                break;
        }
    }
    public static async Task Example_API_TopicSubscribe(ITopicClient topicClient)
    {
        var produceCancellation = new CancellationTokenSource();
        produceCancellation.CancelAfter(2000);

        var subscribeResponse = await topicClient.SubscribeAsync("test-cache", "test-topic");
        switch (subscribeResponse)
        {
            case TopicSubscribeResponse.Subscription subscription:
                var cancellableSubscription = subscription.WithCancellation(produceCancellation.Token);

                await Task.Delay(1_000);
                await topicClient.PublishAsync("test-cache", "test-topic", "test-topic-value");
                await Task.Delay(1_000);

                await foreach (var message in cancellableSubscription)
                {
                    switch (message)
                    {
                        case TopicMessage.Binary:
                            Console.WriteLine("Received unexpected binary message from topic.");
                            break;
                        case TopicMessage.Text text:
                            Console.WriteLine($"Received string message from topic: {text.Value}");
                            break;
                        case TopicMessage.Error error:
                            throw new Exception($"An error occurred while receiving topic message: {error.ErrorCode}: {error}");
                            break;
                        default:
                            throw new Exception("Bad message received");
                    }
                }
                subscription.Dispose();
                break;
            case TopicSubscribeResponse.Error error:
                throw new Exception($"An error occurred subscribing to a topic: {error.ErrorCode}: {error}");
                break;
        }
    }
}
