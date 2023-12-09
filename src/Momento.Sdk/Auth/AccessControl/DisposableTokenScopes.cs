using System.Collections.Generic;

namespace Momento.Sdk.Auth.AccessControl;

/// <summary>
/// Represents the permissions of a disposable token.
/// </summary>
/// <param name="Permissions">The permissions.</param>
public record DisposableTokenScopes(List<DisposableTokenPermission> Permissions)
{
    /// <summary>
    /// Creates a new DisposableTokenScope that grants read-write access to a cache.
    /// </summary>
    /// <param name="cacheName">The name of the cache.</param>
    /// <returns>A new DisposableTokenScope that grants read-write access to a cache.</returns>
    public static DisposableTokenScope CacheReadWrite(string cacheName)
    {
        return CacheReadWrite(CacheSelector.ByName(cacheName));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants read-write access with a specific cache selector.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <returns>A new DisposableTokenScope that grants read-write access with a specific cache selector.</returns>
    public static DisposableTokenScope CacheReadWrite(CacheSelector cacheSelector)
    {
        return new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadWrite,
                cacheSelector,
                CacheItemSelector.AllCacheItems
            )
        });
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants read-only access to a cache.
    /// </summary>
    /// <param name="cacheName">The name of the cache.</param>
    /// <returns>A new DisposableTokenScope that grants read-only access to a cache.</returns>
    public static DisposableTokenScope CacheReadOnly(string cacheName)
    {
        return CacheReadOnly(CacheSelector.ByName(cacheName));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants read-only access with a specific cache selector.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <returns>A new DisposableTokenScope that grants read-only access with a specific cache selector.</returns>
    public static DisposableTokenScope CacheReadOnly(CacheSelector cacheSelector)
    {
        return new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadOnly,
                cacheSelector,
                CacheItemSelector.AllCacheItems
            )
        });
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants write-only access to a cache.
    /// </summary>
    /// <param name="cacheName">The name of the cache.</param>
    /// <returns>A new DisposableTokenScope that grants write-only access to a cache.</returns>
    public static DisposableTokenScope CacheWriteOnly(string cacheName)
    {
        return CacheWriteOnly(CacheSelector.ByName(cacheName));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants write-only access with a specific cache selector.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <returns>A new DisposableTokenScope that grants write-only access with a specific cache selector.</returns>
    public static DisposableTokenScope CacheWriteOnly(CacheSelector cacheSelector)
    {
        return new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.WriteOnly,
                cacheSelector,
                CacheItemSelector.AllCacheItems
            )
        });
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants read-write access to a cache item.
    /// </summary>
    /// <param name="cacheName">The name of the cache.</param>
    /// <param name="cacheKey">The key of the cache item.</param>
    /// <returns>A new DisposableTokenScope that grants read-write access to a cache item.</returns>
    public static DisposableTokenScope CacheKeyReadWrite(string cacheName, string cacheKey)
    {
        return CacheKeyReadWrite(CacheSelector.ByName(cacheName), CacheItemSelector.ByKey(cacheKey));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants read-write access to a cache item.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <param name="cacheKey">The key of the cache item.</param>
    /// <returns>A new DisposableTokenScope that grants read-write access to a cache item.</returns>
    public static DisposableTokenScope CacheKeyReadWrite(CacheSelector cacheSelector, string cacheKey)
    {
        return CacheKeyReadWrite(cacheSelector, CacheItemSelector.ByKey(cacheKey));
    }

    private static DisposableTokenScope CacheKeyReadWrite(CacheSelector cacheSelector, CacheItemSelector cacheItemSelector)
    {
        return new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadWrite,
                cacheSelector,
                cacheItemSelector
            )
        });
    }


    /// <summary>
    /// Creates a new DisposableTokenScope that grants read-only access to a cache item.
    /// </summary>
    /// <param name="cacheName">The name of the cache.</param>
    /// <param name="cacheKey">The key of the cache item.</param>
    /// <returns>A new DisposableTokenScope that grants read-only access to a cache item.</returns>
    public static DisposableTokenScope CacheKeyReadOnly(string cacheName, string cacheKey)
    {
        return CacheKeyReadOnly(CacheSelector.ByName(cacheName), CacheItemSelector.ByKey(cacheKey));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants read access to a cache item.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <param name="cacheKey">The key of the cache item.</param>
    /// <returns>A new DisposableTokenScope that grants read-only access to a cache item.</returns>
    public static DisposableTokenScope CacheKeyReadOnly(CacheSelector cacheSelector, string cacheKey)
    {
        return CacheKeyReadOnly(cacheSelector, CacheItemSelector.ByKey(cacheKey));
    }

    private static DisposableTokenScope CacheKeyReadOnly(CacheSelector cacheSelector, CacheItemSelector cacheItemSelector)
    {
        return new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadOnly,
                cacheSelector,
                cacheItemSelector
            )
        });
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants write-only access to a cache item.
    /// </summary>
    /// <param name="cacheName">The name of the cache.</param>
    /// <param name="cacheKey">The key of the cache item.</param>
    /// <returns>A new DisposableTokenScope that grants write access to a cache item.</returns>
    public static DisposableTokenScope CacheKeyWriteOnly(string cacheName, string cacheKey)
    {
        return CacheKeyWriteOnly(CacheSelector.ByName(cacheName), CacheItemSelector.ByKey(cacheKey));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants write-only access to a cache item.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <param name="cacheKey">The key of the cache item.</param>
    /// <returns>A new DisposableTokenScope that grants write-only access to a cache item.</returns>
    public static DisposableTokenScope CacheKeyWriteOnly(CacheSelector cacheSelector, string cacheKey)
    {
        return CacheKeyWriteOnly(cacheSelector, CacheItemSelector.ByKey(cacheKey));
    }

    private static DisposableTokenScope CacheKeyWriteOnly(CacheSelector cacheSelector, CacheItemSelector cacheItemSelector)
    {
        return new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.WriteOnly,
                cacheSelector,
                cacheItemSelector
            )
        });
    }


    /// <summary>
    /// Creates a new DisposableTokenScope that grants read-write access to a cache item prefix.
    /// </summary>
    /// <param name="cacheName">The name of the cache.</param>
    /// <param name="cacheKeyPrefix">The key prefix of the cache item.</param>
    /// <returns>A new DisposableTokenScope that grants read-write access to a cache item prefix.</returns>
    public static DisposableTokenScope CacheKeyPrefixReadWrite(string cacheName, string cacheKeyPrefix)
    {
        return CacheKeyPrefixReadWrite(CacheSelector.ByName(cacheName), CacheItemSelector.ByKeyPrefix(cacheKeyPrefix));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants read-write access to a cache item prefix.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <param name="cacheKeyPrefix">The key prefix of the cache item.</param>
    /// <returns>A new DisposableTokenScope that grants read-write access to a cache item prefix.</returns>
    public static DisposableTokenScope CacheKeyPrefixReadWrite(CacheSelector cacheSelector, string cacheKeyPrefix)
    {
        return CacheKeyPrefixReadWrite(cacheSelector, CacheItemSelector.ByKeyPrefix(cacheKeyPrefix));
    }

    private static DisposableTokenScope CacheKeyPrefixReadWrite(CacheSelector cacheSelector, CacheItemSelector cacheItemSelector)
    {
        return new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadWrite,
                cacheSelector,
                cacheItemSelector
            )
        });
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants read-only access to a cache item prefix.
    /// </summary>
    /// <param name="cacheName">The name of the cache.</param>
    /// <param name="cacheKeyPrefix">The key prefix of the cache item.</param>
    /// <returns>A new DisposableTokenScope that grants read-only access to a cache item prefix.</returns>
    public static DisposableTokenScope CacheKeyPrefixReadOnly(string cacheName, string cacheKeyPrefix)
    {
        return CacheKeyPrefixReadOnly(CacheSelector.ByName(cacheName), CacheItemSelector.ByKeyPrefix(cacheKeyPrefix));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants read-only access to a cache item prefix.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <param name="cacheKeyPrefix">The key prefix of the cache item.</param>
    /// <returns>A new DisposableTokenScope that grants read-only access to a cache item prefix.</returns>
    public static DisposableTokenScope CacheKeyPrefixReadOnly(CacheSelector cacheSelector, string cacheKeyPrefix)
    {
        return CacheKeyPrefixReadOnly(cacheSelector, CacheItemSelector.ByKeyPrefix(cacheKeyPrefix));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants read-only access to a cache item prefix.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <param name="cacheItemSelector">The cache item selector.</param>
    /// <returns>A new DisposableTokenScope that grants read-only access to a cache item prefix.</returns>
    private static DisposableTokenScope CacheKeyPrefixReadOnly(CacheSelector cacheSelector, CacheItemSelector cacheItemSelector)
    {
        return new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadOnly,
                cacheSelector,
                cacheItemSelector
            )
        });
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants write-only access to a cache item prefix.
    /// </summary>
    /// <param name="cacheName">The name of the cache.</param>
    /// <param name="cacheKeyPrefix">The key prefix of the cache item.</param>
    /// <returns>A new DisposableTokenScope that grants write-only access to a cache item prefix.</returns>
    public static DisposableTokenScope CacheKeyPrefixWriteOnly(string cacheName, string cacheKeyPrefix)
    {
        return CacheKeyPrefixWriteOnly(CacheSelector.ByName(cacheName), CacheItemSelector.ByKeyPrefix(cacheKeyPrefix));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants write-only access to a cache item prefix.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <param name="cacheKeyPrefix">The key prefix of the cache item.</param>
    /// <returns>A new DisposableTokenScope that grants write-only access to a cache item prefix.</returns>
    public static DisposableTokenScope CacheKeyPrefixWriteOnly(CacheSelector cacheSelector, string cacheKeyPrefix)
    {
        return CacheKeyPrefixWriteOnly(cacheSelector, CacheItemSelector.ByKeyPrefix(cacheKeyPrefix));
    }

    private static DisposableTokenScope CacheKeyPrefixWriteOnly(CacheSelector cacheSelector, CacheItemSelector cacheItemSelector)
    {
        return new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.WriteOnly,
                cacheSelector,
                cacheItemSelector
            )
        });
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants publish-subscribe access to a topic.
    /// </summary>
    /// <param name="cacheName">The name of the cache the topic is in.</param>
    /// <param name="topicName">The name of the topic.</param>
    /// <returns>A new DisposableTokenScope that grants publish-subscribe access to a topic.</returns>
    public static DisposableTokenScope TopicPublishSubscribe(string cacheName, string topicName)
    {
        return TopicPublishSubscribe(CacheSelector.ByName(cacheName), TopicSelector.ByName(topicName));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants publish-subscribe access to a topic.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <param name="topicName">The name of the topic.</param>
    /// <returns>A new DisposableTokenScope that grants publish-subscribe access to a topic.</returns>
    public static DisposableTokenScope TopicPublishSubscribe(CacheSelector cacheSelector, string topicName)
    {
        return TopicPublishSubscribe(cacheSelector, TopicSelector.ByName(topicName));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants publish-subscribe access to topics with a specific topic selector.
    /// </summary>
    /// <param name="cacheName">The name of the cache the topic is in.</param>
    /// <param name="topicSelector">The topic selector.</param>
    /// <returns>A new DisposableTokenScope that grants publish-subscribe access to topics with a specific topic selector.</returns>
    public static DisposableTokenScope TopicPublishSubscribe(string cacheName, TopicSelector topicSelector)
    {
        return TopicPublishSubscribe(CacheSelector.ByName(cacheName), topicSelector);
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants publish-subscribe access to topics with a specific topic selector.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <param name="topicSelector">The topic selector.</param>
    /// <returns>A new DisposableTokenScope that grants publish-subscribe access to topics with a specific topic selector.</returns>
    public static DisposableTokenScope TopicPublishSubscribe(CacheSelector cacheSelector, TopicSelector topicSelector)
    {
        return new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>
        {
            new DisposableToken.TopicPermission(
                TopicRole.PublishSubscribe,
                cacheSelector,
                topicSelector
            )
        });
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants subscribe-only access to a topic.
    /// </summary>
    /// <param name="cacheName">The name of the cache the topic is in.</param>
    /// <param name="topicName">The name of the topic.</param>
    /// <returns></returns>
    public static DisposableTokenScope TopicSubscribeOnly(string cacheName, string topicName)
    {
        return TopicSubscribeOnly(CacheSelector.ByName(cacheName), TopicSelector.ByName(topicName));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants subscribe-only access to a topic.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <param name="topicName">The name of the topic.</param>
    /// <returns>A new DisposableTokenScope that grants subscribe-only access to a topic.</returns>
    public static DisposableTokenScope TopicSubscribeOnly(CacheSelector cacheSelector, string topicName)
    {
        return TopicSubscribeOnly(cacheSelector, TopicSelector.ByName(topicName));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants subscribe-only access to topics with a specific topic selector.
    /// </summary>
    /// <param name="cacheName">The name of the cache the topic is in.</param>
    /// <param name="topicSelector">The topic selector.</param>
    /// <returns>A new DisposableTokenScope that grants subscribe-only access to topics with a specific topic selector.</returns>
    public static DisposableTokenScope TopicSubscribeOnly(string cacheName, TopicSelector topicSelector)
    {
        return TopicSubscribeOnly(CacheSelector.ByName(cacheName), topicSelector);
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants subscribe-only access to topics with a specific topic selector.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <param name="topicSelector">The topic selector.</param>
    /// <returns></returns>
    public static DisposableTokenScope TopicSubscribeOnly(CacheSelector cacheSelector, TopicSelector topicSelector)
    {
        return new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>
        {
            new DisposableToken.TopicPermission(
                TopicRole.SubscribeOnly,
                cacheSelector,
                topicSelector
            )
        });
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants publish-only access to a topic.
    /// </summary>
    /// <param name="cacheName">The name of the cache the topic is in.</param>
    /// <param name="topicName">The name of the topic.</param>
    /// <returns>A new DisposableTokenScope that grants publish-only access to a topic.</returns>
    public static DisposableTokenScope TopicPublishOnly(string cacheName, string topicName)
    {
        return TopicPublishOnly(CacheSelector.ByName(cacheName), TopicSelector.ByName(topicName));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants publish-only access to a topic.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <param name="topicName">The name of the topic.</param>
    /// <returns>A new DisposableTokenScope that grants publish-only access to a topic.</returns>
    public static DisposableTokenScope TopicPublishOnly(CacheSelector cacheSelector, string topicName)
    {
        return TopicPublishOnly(cacheSelector, TopicSelector.ByName(topicName));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants publish-only access to topics with a specific topic selector.
    /// </summary>
    /// <param name="cacheName">The name of the cache the topic is in.</param>
    /// <param name="topicSelector">The topic selector.</param>
    /// <returns>A new DisposableTokenScope that grants publish-only access to topics with a specific topic selector.</returns>
    public static DisposableTokenScope TopicPublishOnly(string cacheName, TopicSelector topicSelector)
    {
        return TopicPublishOnly(CacheSelector.ByName(cacheName), topicSelector);
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants publish-only access to topics with a specific topic selector.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <param name="topicSelector">The topic selector.</param>
    /// <returns>A new DisposableTokenScope that grants publish-only access to topics with a specific topic selector.</returns>
    public static DisposableTokenScope TopicPublishOnly(CacheSelector cacheSelector, TopicSelector topicSelector)
    {
        return new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>
        {
            new DisposableToken.TopicPermission(
                TopicRole.PublishOnly,
                cacheSelector,
                topicSelector
            )
        });
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants publish-subscribe access to topics with a specific topic name prefix.
    /// </summary>
    /// <param name="cacheName">The name of the cache the topic is in.</param>
    /// <param name="topicNamePrefix">The topic name prefix.</param>
    /// <returns>A new DisposableTokenScope that grants publish-subscribe access to topics with a specific topic name prefix.</returns>
    public static DisposableTokenScope TopicNamePrefixPublishSubscribe(string cacheName, string topicNamePrefix)
    {
        return TopicNamePrefixPublishSubscribe(CacheSelector.ByName(cacheName), TopicSelector.ByTopicNamePrefix(topicNamePrefix));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants publish-subscribe access to topics with a specific topic name prefix.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <param name="topicNamePrefix">The topic name prefix.</param>
    /// <returns>A new DisposableTokenScope that grants publish-subscribe access to topics with a specific topic name prefix.</returns>
    public static DisposableTokenScope TopicNamePrefixPublishSubscribe(CacheSelector cacheSelector, string topicNamePrefix)
    {
        return TopicNamePrefixPublishSubscribe(cacheSelector, TopicSelector.ByTopicNamePrefix(topicNamePrefix));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants publish-subscribe access to topics with a specific topic selector by name prefix.
    /// </summary>
    /// <param name="cacheName">The name of the cache the topic is in.</param>
    /// <param name="topicSelector">The topic selector.</param>
    /// <returns>A new DisposableTokenScope that grants publish-subscribe access to topics with a specific topic selector.</returns>
    public static DisposableTokenScope TopicNamePrefixPublishSubscribe(string cacheName, TopicSelector topicSelector)
    {
        return TopicNamePrefixPublishSubscribe(CacheSelector.ByName(cacheName), topicSelector);
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants publish-subscribe access to topics with a specific topic selector by name prefix.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <param name="topicSelector">The topic selector.</param>
    /// <returns>A new DisposableTokenScope that grants publish-subscribe access to topics with a specific topic selector.</returns>
    public static DisposableTokenScope TopicNamePrefixPublishSubscribe(CacheSelector cacheSelector, TopicSelector topicSelector)
    {
        return new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>
        {
            new DisposableToken.TopicPermission(
                TopicRole.PublishSubscribe,
                cacheSelector,
                topicSelector
            )
        });
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants subscribe-only access to topics with a specific topic name prefix.
    /// </summary>
    /// <param name="cacheName">The name of the cache the topic is in.</param>
    /// <param name="topicNamePrefix">The topic name prefix.</param>
    /// <returns>A new DisposableTokenScope that grants subscribe-only access to topics with a specific topic name prefix.</returns>
    public static DisposableTokenScope TopicNamePrefixSubscribeOnly(string cacheName, string topicNamePrefix)
    {
        return TopicNamePrefixSubscribeOnly(CacheSelector.ByName(cacheName), TopicSelector.ByTopicNamePrefix(topicNamePrefix));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants subscribe-only access to topics with a specific topic name prefix.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <param name="topicNamePrefix">The topic name prefix.</param>
    /// <returns>A new DisposableTokenScope that grants subscribe-only access to topics with a specific topic name prefix.</returns>
    public static DisposableTokenScope TopicNamePrefixSubscribeOnly(CacheSelector cacheSelector, string topicNamePrefix)
    {
        return TopicNamePrefixSubscribeOnly(cacheSelector, TopicSelector.ByTopicNamePrefix(topicNamePrefix));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants subscribe-only access to topics with a specific topic selector by name prefix.
    /// </summary>
    /// <param name="cacheName">The name of the cache the topic is in.</param>
    /// <param name="topicSelector">The topic selector.</param>
    /// <returns>A new DisposableTokenScope that grants subscribe-only access to topics with a specific topic selector.</returns>
    public static DisposableTokenScope TopicNamePrefixSubscribeOnly(string cacheName, TopicSelector topicSelector)
    {
        return TopicNamePrefixSubscribeOnly(CacheSelector.ByName(cacheName), topicSelector);
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants subscribe-only access to topics with a specific topic selector by name prefix.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <param name="topicSelector">The topic selector.</param>
    /// <returns>A new DisposableTokenScope that grants subscribe-only access to topics with a specific topic selector.</returns>
    public static DisposableTokenScope TopicNamePrefixSubscribeOnly(CacheSelector cacheSelector, TopicSelector topicSelector)
    {
        return new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>
        {
            new DisposableToken.TopicPermission(
                TopicRole.SubscribeOnly,
                cacheSelector,
                topicSelector
            )
        });
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants publish-only access to topics with a specific topic name prefix.
    /// </summary>
    /// <param name="cacheName">The name of the cache the topic is in.</param>
    /// <param name="topicNamePrefix">The topic name prefix.</param>
    /// <returns>A new DisposableTokenScope that grants publish-only access to topics with a specific topic name prefix.</returns>
    public static DisposableTokenScope TopicNamePrefixPublishOnly(string cacheName, string topicNamePrefix)
    {
        return TopicNamePrefixPublishOnly(CacheSelector.ByName(cacheName), TopicSelector.ByTopicNamePrefix(topicNamePrefix));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants publish-only access to topics with a specific topic name prefix.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <param name="topicNamePrefix">The topic name prefix.</param>
    /// <returns>A new DisposableTokenScope that grants publish-only access to topics with a specific topic name prefix.</returns>
    public static DisposableTokenScope TopicNamePrefixPublishOnly(CacheSelector cacheSelector, string topicNamePrefix)
    {
        return TopicNamePrefixPublishOnly(cacheSelector, TopicSelector.ByTopicNamePrefix(topicNamePrefix));
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants publish-only access to topics with a specific topic selector by name prefix.
    /// </summary>
    /// <param name="cacheName">The name of the cache the topic is in.</param>
    /// <param name="topicSelector">The topic selector.</param>
    /// <returns>A new DisposableTokenScope that grants publish-only access to topics with a specific topic selector.</returns>
    public static DisposableTokenScope TopicNamePrefixPublishOnly(string cacheName, TopicSelector topicSelector)
    {
        return TopicNamePrefixPublishOnly(CacheSelector.ByName(cacheName), topicSelector);
    }

    /// <summary>
    /// Creates a new DisposableTokenScope that grants publish-only access to topics with a specific topic selector by name prefix.
    /// </summary>
    /// <param name="cacheSelector">The cache selector.</param>
    /// <param name="topicSelector">The topic selector.</param>
    /// <returns>A new DisposableTokenScope that grants publish-only access to topics with a specific topic selector.</returns>
    public static DisposableTokenScope TopicNamePrefixPublishOnly(CacheSelector cacheSelector, TopicSelector topicSelector)
    {
        return new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>
        {
            new DisposableToken.TopicPermission(
                TopicRole.PublishOnly,
                cacheSelector,
                topicSelector
            )
        });
    }
}
