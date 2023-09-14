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
}
