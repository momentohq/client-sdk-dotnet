namespace Momento.Sdk.Auth.AccessControl;

public enum CacheRole
{
    ReadWrite,
    ReadOnly,
    WriteOnly
}

public abstract record CacheSelector
{
    public record SelectAllCaches : CacheSelector;

    public static SelectAllCaches AllCaches = new SelectAllCaches();

    public record SelectByCacheName(string CacheName) : CacheSelector;

    public static SelectByCacheName ByName(string cacheName)
    {
        return new SelectByCacheName(cacheName);
    }
}


public abstract record CacheItemSelector
{
    public record SelectAllCacheItems : CacheItemSelector;

    public static SelectAllCacheItems AllCacheItems = new SelectAllCacheItems();

    public record SelectByKey(string CacheKey) : CacheItemSelector;

    public static SelectByKey ByKey(string cacheKey)
    {
        return new SelectByKey(cacheKey);
    }

    public record SelectByKeyPrefix(string CacheKeyPrefix) : CacheItemSelector;

    public static SelectByKeyPrefix ByKeyPrefix(string cacheKeyPrefix)
    {
        return new SelectByKeyPrefix(cacheKeyPrefix);
    }
}


public enum TopicRole
{
    PublishSubscribe,
    PublishOnly,
    SubscribeOnly
}


public abstract record TopicSelector
{
    public record SelectAllTopics : TopicSelector;

    public static SelectAllTopics AllTopics = new SelectAllTopics();

    public record SelectByTopicName(string TopicName) : TopicSelector;

    public static SelectByTopicName ByName(string topicName)
    {
        return new SelectByTopicName(topicName);
    }

    // This selector is currently only used to generate disposable tokens. There are plans to
    // support topic name prefix in `generateApiToken` as well, but it is not yet implemented.
    // Because we are only generating disposable tokens in the SDK now, this is fine. However,
    // if topic name prefix is still unsupported server side when we add the code to generate
    // API keys, we will need to add a second selector specific to API key generation that does
    // not include `SelectByTopicNamePrefix` and refer to that selector when setting up API key
    // permissions like the ones in `DisposableToken.cs`. That will ensure that usage of the
    // correct type can be verified at compile time.
    public record SelectByTopicNamePrefix(string TopicNamePrefix) : TopicSelector;

    public static SelectByTopicNamePrefix ByTopicNamePrefix(string topicNamePrefix)
    {
        return new SelectByTopicNamePrefix(topicNamePrefix);
    }
}
