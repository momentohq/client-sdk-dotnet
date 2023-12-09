namespace Momento.Sdk.Auth.AccessControl;

/// <summary>
/// The permissions that can be granted to a cache.
/// </summary>
public enum CacheRole
{
    /// <summary>
    /// The cache can be read from and written to.
    /// </summary>
    ReadWrite,
    /// <summary>
    /// The cache can only be read from.
    /// </summary>
    ReadOnly,
    /// <summary>
    /// The cache can only be written to.
    /// </summary>
    WriteOnly
}

/// <summary>
/// How to select a cache to grant permissions to.
/// </summary>
public abstract record CacheSelector
{
    /// <summary>
    /// Select all caches.
    /// </summary>
    public record SelectAllCaches : CacheSelector;

    /// <summary>
    /// Helper object to select all caches.
    /// </summary>
    public static SelectAllCaches AllCaches = new SelectAllCaches();

    /// <summary>
    /// Select a cache by its name.
    /// </summary>
    /// <param name="CacheName">The name of the cache to select.</param>
    public record SelectByCacheName(string CacheName) : CacheSelector;

    /// <summary>
    /// Select a cache by its name.
    /// </summary>
    /// <param name="cacheName">The name of the cache to select.</param>
    /// <returns>A <see cref="SelectByCacheName"/> object.</returns>
    public static SelectByCacheName ByName(string cacheName)
    {
        return new SelectByCacheName(cacheName);
    }
}

/// <summary>
/// How to select a cache item to grant permissions to.
/// </summary>
public abstract record CacheItemSelector
{
    /// <summary>
    /// Select all cache items.
    /// </summary>
    public record SelectAllCacheItems : CacheItemSelector;

    /// <summary>
    /// Helper object to select all cache items.
    /// </summary>
    public static SelectAllCacheItems AllCacheItems = new SelectAllCacheItems();

    /// <summary>
    /// Select a cache item by its key.
    /// </summary>
    /// <param name="CacheKey">The key of the cache item to select.</param>
    public record SelectByKey(string CacheKey) : CacheItemSelector;

    /// <summary>
    /// Select a cache item by its key.
    /// </summary>
    /// <param name="cacheKey">The key of the cache item to select.</param>
    /// <returns>A <see cref="SelectByKey"/> object.</returns>
    public static SelectByKey ByKey(string cacheKey)
    {
        return new SelectByKey(cacheKey);
    }

    /// <summary>
    /// Select all cache items with a key that starts with a given prefix.
    /// </summary>
    /// <param name="CacheKeyPrefix">The prefix of the keys of the cache items to select.</param>
    public record SelectByKeyPrefix(string CacheKeyPrefix) : CacheItemSelector;

    /// <summary>
    /// Select all cache items with a key that starts with a given prefix.
    /// </summary>
    /// <param name="cacheKeyPrefix">The prefix of the keys of the cache items to select.</param>
    /// <returns>A <see cref="SelectByKeyPrefix"/> object.</returns>
    public static SelectByKeyPrefix ByKeyPrefix(string cacheKeyPrefix)
    {
        return new SelectByKeyPrefix(cacheKeyPrefix);
    }
}

/// <summary>
/// The permissions that can be granted to a topic.
/// </summary>
public enum TopicRole
{
    /// <summary>
    /// The topic can be published to and subscribed to.
    /// </summary>
    PublishSubscribe,
    /// <summary>
    /// The topic can only be published to.
    /// </summary>
    PublishOnly,
    /// <summary>
    /// The topic can only be subscribed to.
    /// </summary>
    SubscribeOnly
}

/// <summary>
/// How to select a topic to grant permissions to.
/// </summary>
public abstract record TopicSelector
{
    /// <summary>
    /// Select all topics.
    /// </summary>
    public record SelectAllTopics : TopicSelector;

    /// <summary>
    /// Helper object to select all topics.
    /// </summary>
    public static SelectAllTopics AllTopics = new SelectAllTopics();

    /// <summary>
    /// Select a topic by its name.
    /// </summary>
    /// <param name="TopicName">The name of the topic to select.</param>
    public record SelectByTopicName(string TopicName) : TopicSelector;

    /// <summary>
    /// Select a topic by its name.
    /// </summary>
    /// <param name="topicName">The name of the topic to select.</param>
    /// <returns>A <see cref="SelectByTopicName"/> object.</returns>
    public static SelectByTopicName ByName(string topicName)
    {
        return new SelectByTopicName(topicName);
    }

    /// <summary>
    /// Select all topics with a name that starts with a given prefix.
    /// </summary>
    /// <param name="TopicNamePrefix">The prefix of the names of the topics to select.</param>
    /// <remarks>
    /// This selector is currently only used to generate disposable tokens. There are plans to
    /// support topic name prefix in `generateApiToken` as well, but it is not yet implemented.
    /// Because we are only generating disposable tokens in the SDK now, this is fine. However,
    /// if topic name prefix is still unsupported server side when we add the code to generate
    /// API keys, we will need to add a second selector specific to API key generation that does
    /// not include `SelectByTopicNamePrefix` and refer to that selector when setting up API key
    /// permissions like the ones in `DisposableToken.cs`. That will ensure that usage of the
    /// correct type can be verified at compile time.
    /// </remarks>
    public record SelectByTopicNamePrefix(string TopicNamePrefix) : TopicSelector;

    /// <summary>
    /// Select all topics with a name that starts with a given prefix.
    /// </summary>
    /// <param name="topicNamePrefix">The prefix of the names of the topics to select.</param>
    /// <returns>A <see cref="SelectByTopicNamePrefix"/> object.</returns>
    public static SelectByTopicNamePrefix ByTopicNamePrefix(string topicNamePrefix)
    {
        return new SelectByTopicNamePrefix(topicNamePrefix);
    }
}
