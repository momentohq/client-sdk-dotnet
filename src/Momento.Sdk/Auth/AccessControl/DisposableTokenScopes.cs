#if !BUILD_FOR_UNITY
using System.Collections.Generic;

namespace Momento.Sdk.Auth.AccessControl;

public record DisposableTokenScopes(List<DisposableTokenPermission> Permissions)
{
    public static DisposableTokenScope CacheReadWrite(string cacheName)
    {
        return CacheReadWrite(CacheSelector.ByName(cacheName));
    }

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

    public static DisposableTokenScope CacheReadOnly(string cacheName)
    {
        return CacheReadOnly(CacheSelector.ByName(cacheName));
    }

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

    public static DisposableTokenScope CacheWriteOnly(string cacheName)
    {
        return CacheWriteOnly(CacheSelector.ByName(cacheName));
    }

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


    public static DisposableTokenScope CacheKeyReadWrite(string cacheName, string cacheKey)
    {
        return CacheKeyReadWrite(CacheSelector.ByName(cacheName), CacheItemSelector.ByKey(cacheKey));
    }

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


    public static DisposableTokenScope CacheKeyReadOnly(string cacheName, string cacheKey)
    {
        return CacheKeyReadOnly(CacheSelector.ByName(cacheName), CacheItemSelector.ByKey(cacheKey));
    }

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

    public static DisposableTokenScope CacheKeyWriteOnly(string cacheName, string cacheKey)
    {
        return CacheKeyWriteOnly(CacheSelector.ByName(cacheName), CacheItemSelector.ByKey(cacheKey));
    }

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


    public static DisposableTokenScope CacheKeyPrefixReadWrite(string cacheName, string cacheKeyPrefix)
    {
        return CacheKeyPrefixReadWrite(CacheSelector.ByName(cacheName), CacheItemSelector.ByKeyPrefix(cacheKeyPrefix));
    }

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


    public static DisposableTokenScope CacheKeyPrefixReadOnly(string cacheName, string cacheKeyPrefix)
    {
        return CacheKeyPrefixReadOnly(CacheSelector.ByName(cacheName), CacheItemSelector.ByKeyPrefix(cacheKeyPrefix));
    }

    public static DisposableTokenScope CacheKeyPrefixReadOnly(CacheSelector cacheSelector, string cacheKeyPrefix)
    {
        return CacheKeyPrefixReadOnly(cacheSelector, CacheItemSelector.ByKeyPrefix(cacheKeyPrefix));
    }

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

    public static DisposableTokenScope CacheKeyPrefixWriteOnly(string cacheName, string cacheKeyPrefix)
    {
        return CacheKeyPrefixWriteOnly(CacheSelector.ByName(cacheName), CacheItemSelector.ByKeyPrefix(cacheKeyPrefix));
    }

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

    public static DisposableTokenScope TopicPublishSubscribe(string cacheName, string topicName)
    {
        return TopicPublishSubscribe(CacheSelector.ByName(cacheName), TopicSelector.ByName(topicName));
    }

    public static DisposableTokenScope TopicPublishSubscribe(CacheSelector cacheSelector, string topicName)
    {
        return TopicPublishSubscribe(cacheSelector, TopicSelector.ByName(topicName));
    }

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


    public static DisposableTokenScope TopicSubscribeOnly(string cacheName, string topicName)
    {
        return TopicSubscribeOnly(CacheSelector.ByName(cacheName), TopicSelector.ByName(topicName));
    }

    public static DisposableTokenScope TopicSubscribeOnly(CacheSelector cacheSelector, string topicName)
    {
        return TopicSubscribeOnly(cacheSelector, TopicSelector.ByName(topicName));
    }

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

    public static DisposableTokenScope TopicPublishOnly(string cacheName, string topicName)
    {
        return TopicPublishOnly(CacheSelector.ByName(cacheName), TopicSelector.ByName(topicName));
    }

    public static DisposableTokenScope TopicPublishOnly(CacheSelector cacheSelector, string topicName)
    {
        return TopicPublishOnly(cacheSelector, TopicSelector.ByName(topicName));
    }

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
}
#endif
