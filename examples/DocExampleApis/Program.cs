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
            new EnvMomentoV2TokenProvider(),
            TimeSpan.FromSeconds(10));
        IAuthClient authClient = new AuthClient(
            AuthConfigurations.Default.Latest(),
            new EnvMomentoTokenProvider("V1_API_KEY")
        );
        ITopicClient topicClient = new TopicClient(
            TopicConfigurations.Laptop.latest(),
            new EnvMomentoV2TokenProvider()
        );

        await Example_API_CreateCache(client);
        await Example_API_FlushCache(client);
        await Example_API_DeleteCache(client);
        await Example_API_ListCaches(client);

        await Example_API_CreateCache(client);
        await Example_API_Set(client);
        await Example_API_Get(client);
        await Example_API_Delete(client);

        await Example_API_SetSample(client);

        await Example_API_GenerateDisposableToken(authClient);

        await Example_API_InstantiateTopicClient();
        await Example_API_TopicSubscribe(topicClient);
        await Example_API_TopicPublish(topicClient);

        await Example_API_CredentialProviderFromEnvVarV2Default();
        await Example_API_CredentialProviderFromEnvVarV2();
        await Example_API_CredentialProviderFromApiKeyV2();
        await Example_API_CredentialProviderFromDisposableToken();
    }

    private static string RetrieveApiKeyFromYourSecretsManager()
    {
        // this is not a valid API key but conforms to the syntax requirements.
        return "eyJhcGlfa2V5IjogImV5SjBlWEFpT2lKS1YxUWlMQ0poYkdjaU9pSklVekkxTmlKOS5leUpwYzNNaU9pSlBibXhwYm1VZ1NsZFVJRUoxYVd4a1pYSWlMQ0pwWVhRaU9qRTJOemd6TURVNE1USXNJbVY0Y0NJNk5EZzJOVFV4TlRReE1pd2lZWFZrSWpvaUlpd2ljM1ZpSWpvaWFuSnZZMnRsZEVCbGVHRnRjR3hsTG1OdmJTSjkuOEl5OHE4NExzci1EM1lDb19IUDRkLXhqSGRUOFVDSXV2QVljeGhGTXl6OCIsICJlbmRwb2ludCI6ICJ0ZXN0Lm1vbWVudG9ocS5jb20ifQo=";
    }

    private static string RetrieveApiKeyV2FromYourSecretsManager()
    {
        // this is not a valid API key but conforms to the syntax requirements.
        return "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ0IjoiZyIsImp0aSI6InNvbWUtaWQifQ.GMr9nA6HE0ttB6llXct_2Sg5-fOKGFbJCdACZFgNbN1fhT6OPg_hVc8ThGzBrWC_RlsBpLA1nzqK3SOJDXYxAw";
    }

    public static async Task Example_API_CredentialProviderFromEnvVarV2()
    {
        new EnvMomentoV2TokenProvider("MOMENTO_API_KEY", "MOMENTO_ENDPOINT");
    }

    public static async Task Example_API_CredentialProviderFromEnvVarV2Default()
    {
        new EnvMomentoV2TokenProvider();
    }

    public static async Task Example_API_CredentialProviderFromApiKeyV2()
    {
        var endpoint = "cell-4-us-west-2-1.prod.a.momentohq.com";
        var apiKey = RetrieveApiKeyV2FromYourSecretsManager();
        new ApiKeyV2TokenProvider(apiKey, endpoint);
    }

    public static async Task Example_API_CredentialProviderFromDisposableToken()
    {
        var authToken = RetrieveApiKeyFromYourSecretsManager();
        new DisposableTokenProvider(authToken);
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

    public static async Task Example_API_SetSample(CacheClient cacheClient)
    {
        var setAddResult = await cacheClient.SetAddElementsAsync("test-cache", "test-set", new string[] { "foo", "bar", "baz" });
        if (setAddResult is CacheSetAddElementsResponse.Success)
        {
            Console.WriteLine("Added elements to 'test-set' successfully");
        }
        else if (setAddResult is CacheSetAddElementsResponse.Error error)
        {
            throw new Exception($"An error occurred while attempting to delete key 'test-key' from cache 'test-cache': {error.ErrorCode}: {error}");
        }

        var setSampleResult = await cacheClient.SetSampleAsync("test-cache", "test-set", 2);
        if (setSampleResult is CacheSetSampleResponse.Hit setSampleHit)
        {
            Console.WriteLine($"Sampled random elements from 'test-set': {String.Join(", ", setSampleHit.ValueSetString)}");
        }
        else if (setSampleResult is CacheSetSampleResponse.Error error)
        {
            throw new Exception($"An error occurred while attempting to sample from 'test-set' from cache 'test-cache': {error.ErrorCode}: {error}");
        }
    }

    public static async Task Example_API_GenerateDisposableToken(IAuthClient authClient)
    {
        // Generate a disposable token with read-write access to a specific key in one cache
        var oneKeyOneCacheToken = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyReadWrite("squirrels", "mo"),
            ExpiresIn.Minutes(30)
        );

        if (oneKeyOneCacheToken is GenerateDisposableTokenResponse.Success token1)
        {
            // logging only a substring of the tokens, because logging security credentials is not advisable :)
            Console.WriteLine("The generated disposable token starts with: " + token1.AuthToken.Substring(0, 10));
            Console.WriteLine("The token expires at (epoch timestamp): " + token1.ExpiresAt.Epoch());
        }
        else if (oneKeyOneCacheToken is GenerateDisposableTokenResponse.Error err)
        {
            Console.WriteLine("Error generating disposable token: " + err.Message);
        }

        // Generate a disposable token with read-write access to a specific key prefix in all caches
        var keyPrefixAllCachesToken = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyPrefixReadWrite(CacheSelector.AllCaches, "squirrel"),
            ExpiresIn.Minutes(30)
        );

        if (keyPrefixAllCachesToken is GenerateDisposableTokenResponse.Success token2)
        {
            // logging only a substring of the tokens, because logging security credentials is not advisable :)
            Console.WriteLine("The generated disposable token starts with: " + token2.AuthToken.Substring(0, 10));
            Console.WriteLine("The token expires at (epoch timestamp): " + token2.ExpiresAt.Epoch());
        }
        else if (keyPrefixAllCachesToken is GenerateDisposableTokenResponse.Error err)
        {
            Console.WriteLine("Error generating disposable token: " + err.Message);
        }

        // Generate a disposable token with read-only access to all topics in one cache
        var allTopicsOneCacheToken = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicSubscribeOnly("squirrel", TopicSelector.AllTopics),
            ExpiresIn.Minutes(30)
        );

        if (allTopicsOneCacheToken is GenerateDisposableTokenResponse.Success token3)
        {
            // logging only a substring of the tokens, because logging security credentials is not advisable :)
            Console.WriteLine("The generated disposable token starts with: " + token3.AuthToken.Substring(0, 10));
            Console.WriteLine("The token expires at (epoch timestamp): " + token3.ExpiresAt.Epoch());
        }
        else if (allTopicsOneCacheToken is GenerateDisposableTokenResponse.Error err)
        {
            Console.WriteLine("Error generating disposable token: " + err.Message);
        }

        // Generate a disposable token with write-only access to a single topic in all caches
        var oneTopicAllCachesToken = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicPublishOnly(CacheSelector.AllCaches, "acorn"),
            ExpiresIn.Minutes(30)
        );

        if (oneTopicAllCachesToken is GenerateDisposableTokenResponse.Success token4)
        {
            // logging only a substring of the tokens, because logging security credentials is not advisable :)
            Console.WriteLine("The generated disposable token starts with: " + token4.AuthToken.Substring(0, 10));
            Console.WriteLine("The token expires at (epoch timestamp): " + token4.ExpiresAt.Epoch());
        }
        else if (oneTopicAllCachesToken is GenerateDisposableTokenResponse.Error err)
        {
            Console.WriteLine("Error generating disposable token: " + err.Message);
        }

    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public static async Task Example_API_InstantiateTopicClient()
    {
        new TopicClient(
            TopicConfigurations.Laptop.latest(),
            new EnvMomentoV2TokenProvider()
        );
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

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
        }
    }
    public static async Task Example_API_TopicSubscribe(ITopicClient topicClient)
    {
        var produceCancellation = new CancellationTokenSource();
        produceCancellation.CancelAfter(5_000);

        var subscribeResponse = await topicClient.SubscribeAsync("test-cache", "test-topic");
        switch (subscribeResponse)
        {
            case TopicSubscribeResponse.Subscription subscription:
                // Note: use `WithCancellation` to filter only the `TopicMessage` types
                var cancellableSubscription = subscription.WithCancellationForAllEvents(produceCancellation.Token);

                await Task.Delay(1_000);
                await topicClient.PublishAsync("test-cache", "test-topic", "test-topic-value");
                await Task.Delay(1_000);

                await foreach (var topicEvent in cancellableSubscription)
                {
                    switch (topicEvent)
                    {
                        case TopicMessage.Binary:
                            Console.WriteLine("Received unexpected binary message from topic.");
                            break;
                        case TopicMessage.Text text:
                            Console.WriteLine($"Received string message from topic: {text.Value}");
                            break;
                        case TopicSystemEvent.Heartbeat:
                            Console.WriteLine("Received heartbeat from topic.");
                            break;
                        case TopicSystemEvent.Discontinuity discontinuity:
                            Console.WriteLine($"Received discontinuity from topic: {discontinuity}");
                            break;
                        case TopicMessage.Error error:
                            throw new Exception($"An error occurred while receiving topic message: {error.ErrorCode}: {error}");
                        default:
                            throw new Exception("Bad message received");
                    }
                }
                subscription.Dispose();
                break;
            case TopicSubscribeResponse.Error error:
                throw new Exception($"An error occurred subscribing to a topic: {error.ErrorCode}: {error}");
        }
    }
}
