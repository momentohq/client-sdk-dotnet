using FluentAssertions;
using Momento.Sdk.Auth.AccessControl;
using System.Collections.Generic;
using Xunit;

[assembly: TestFramework("Momento.Sdk.Tests.Unit.MomentoXunitFramework", "Momento.Sdk.Tests.Unit")]

namespace Momento.Sdk.Tests.Unit.Auth.AccessControl;

public class DisposableTokenScopeTest
{
    [Fact]
    public void DisposableTokenScopes_CacheReadWrite()
    {
        var scope = DisposableTokenScopes.CacheReadWrite("mycache");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadWrite,
                CacheSelector.ByName("mycache"),
                CacheItemSelector.AllCacheItems
            )
        }));
        scope = DisposableTokenScopes.CacheReadWrite(CacheSelector.AllCaches);
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadWrite,
                CacheSelector.AllCaches,
                CacheItemSelector.AllCacheItems
            )
        }));

        scope = DisposableTokenScopes.CacheReadWrite(CacheSelector.ByName("mycache"));
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadWrite,
                CacheSelector.ByName("mycache"),
                CacheItemSelector.AllCacheItems
            )
        }));
    }

    [Fact]
    public void DisposableTokenScopes_CacheReadOnly()
    {
        var scope = DisposableTokenScopes.CacheReadOnly("mycache");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadOnly,
                CacheSelector.ByName("mycache"),
                CacheItemSelector.AllCacheItems
            )
        }));
        scope = DisposableTokenScopes.CacheReadOnly(CacheSelector.AllCaches);
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadOnly,
                CacheSelector.AllCaches,
                CacheItemSelector.AllCacheItems
            )
        }));

        scope = DisposableTokenScopes.CacheReadOnly(CacheSelector.ByName("mycache"));
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadOnly,
                CacheSelector.ByName("mycache"),
                CacheItemSelector.AllCacheItems
            )
        }));
    }

    [Fact]
    public void DisposableTokenScopes_CacheWriteOnly()
    {
        var scope = DisposableTokenScopes.CacheWriteOnly("mycache");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.WriteOnly,
                CacheSelector.ByName("mycache"),
                CacheItemSelector.AllCacheItems
            )
        }));
        scope = DisposableTokenScopes.CacheWriteOnly(CacheSelector.AllCaches);
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.WriteOnly,
                CacheSelector.AllCaches,
                CacheItemSelector.AllCacheItems
            )
        }));

        scope = DisposableTokenScopes.CacheWriteOnly(CacheSelector.ByName("mycache"));
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.WriteOnly,
                CacheSelector.ByName("mycache"),
                CacheItemSelector.AllCacheItems
            )
        }));
    }

    [Fact]
    public void DisposableTokenScopes_CacheKeyReadWrite()
    {
        var scope = DisposableTokenScopes.CacheKeyReadWrite("mycache", "mykey");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadWrite,
                CacheSelector.ByName("mycache"),
                CacheItemSelector.ByKey("mykey")
            )
        }));
        scope = DisposableTokenScopes.CacheKeyReadWrite(CacheSelector.AllCaches, "mykey");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadWrite,
                CacheSelector.AllCaches,
                CacheItemSelector.ByKey("mykey")
            )
        }));

        scope = DisposableTokenScopes.CacheKeyReadWrite(CacheSelector.ByName("mycache"), "mykey");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadWrite,
                CacheSelector.ByName("mycache"),
                CacheItemSelector.ByKey("mykey")
            )
        }));
    }

    [Fact]
    public void DisposableTokenScopes_CacheKeyReadOnly()
    {
        var scope = DisposableTokenScopes.CacheKeyReadOnly("mycache", "mykey");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadOnly,
                CacheSelector.ByName("mycache"),
                CacheItemSelector.ByKey("mykey")
            )
        }));
        scope = DisposableTokenScopes.CacheKeyReadOnly(CacheSelector.AllCaches, "mykey");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadOnly,
                CacheSelector.AllCaches,
                CacheItemSelector.ByKey("mykey")
            )
        }));

        scope = DisposableTokenScopes.CacheKeyReadOnly(CacheSelector.ByName("mycache"), "mykey");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadOnly,
                CacheSelector.ByName("mycache"),
                CacheItemSelector.ByKey("mykey")
            )
        }));
    }

    [Fact]
    public void DisposableTokenScopes_CacheKeyWriteOnly()
    {
        var scope = DisposableTokenScopes.CacheKeyWriteOnly("mycache", "mykey");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.WriteOnly,
                CacheSelector.ByName("mycache"),
                CacheItemSelector.ByKey("mykey")
            )
        }));
        scope = DisposableTokenScopes.CacheKeyWriteOnly(CacheSelector.AllCaches, "mykey");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.WriteOnly,
                CacheSelector.AllCaches,
                CacheItemSelector.ByKey("mykey")
            )
        }));

        scope = DisposableTokenScopes.CacheKeyWriteOnly(CacheSelector.ByName("mycache"), "mykey");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.WriteOnly,
                CacheSelector.ByName("mycache"),
                CacheItemSelector.ByKey("mykey")
            )
        }));
    }

    [Fact]
    public void DisposableTokenScopes_CacheKeyPrefixReadWrite()
    {
        var scope = DisposableTokenScopes.CacheKeyPrefixReadWrite("mycache", "mykeyprefix");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadWrite,
                CacheSelector.ByName("mycache"),
                CacheItemSelector.ByKeyPrefix("mykeyprefix")
            )
        }));
        scope = DisposableTokenScopes.CacheKeyPrefixReadWrite(CacheSelector.AllCaches, "mykeyprefix");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadWrite,
                CacheSelector.AllCaches,
                CacheItemSelector.ByKeyPrefix("mykeyprefix")
            )
        }));

        scope = DisposableTokenScopes.CacheKeyPrefixReadWrite(CacheSelector.ByName("mycache"), "mykeyprefix");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadWrite,
                CacheSelector.ByName("mycache"),
                CacheItemSelector.ByKeyPrefix("mykeyprefix")
            )
        }));
    }

    [Fact]
    public void DisposableTokenScopes_CacheKeyPrefixReadOnly()
    {
        var scope = DisposableTokenScopes.CacheKeyPrefixReadOnly("mycache", "mykeyprefix");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadOnly,
                CacheSelector.ByName("mycache"),
                CacheItemSelector.ByKeyPrefix("mykeyprefix")
            )
        }));
        scope = DisposableTokenScopes.CacheKeyPrefixReadOnly(CacheSelector.AllCaches, "mykeyprefix");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadOnly,
                CacheSelector.AllCaches,
                CacheItemSelector.ByKeyPrefix("mykeyprefix")
            )
        }));

        scope = DisposableTokenScopes.CacheKeyPrefixReadOnly(CacheSelector.ByName("mycache"), "mykeyprefix");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadOnly,
                CacheSelector.ByName("mycache"),
                CacheItemSelector.ByKeyPrefix("mykeyprefix")
            )
        }));
    }

    [Fact]
    public void DisposableTokenScopes_CacheKeyPrefixWriteOnly()
    {
        var scope = DisposableTokenScopes.CacheKeyPrefixWriteOnly("mycache", "mykeyprefix");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.WriteOnly,
                CacheSelector.ByName("mycache"),
                CacheItemSelector.ByKeyPrefix("mykeyprefix")
            )
        }));
        scope = DisposableTokenScopes.CacheKeyPrefixWriteOnly(CacheSelector.AllCaches, "mykeyprefix");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.WriteOnly,
                CacheSelector.AllCaches,
                CacheItemSelector.ByKeyPrefix("mykeyprefix")
            )
        }));

        scope = DisposableTokenScopes.CacheKeyPrefixWriteOnly(CacheSelector.ByName("mycache"), "mykeyprefix");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.WriteOnly,
                CacheSelector.ByName("mycache"),
                CacheItemSelector.ByKeyPrefix("mykeyprefix")
            )
        }));
    }


    [Fact]
    public void DisposableTokenScopes_TopicPublishSubscribe()
    {
        var scope = DisposableTokenScopes.TopicPublishSubscribe("mycache", "mytopic");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.TopicPermission(
                TopicRole.PublishSubscribe,
                CacheSelector.ByName("mycache"),
                TopicSelector.ByName("mytopic")
            )
        }));
        scope = DisposableTokenScopes.TopicPublishSubscribe(CacheSelector.AllCaches, "mytopic");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.TopicPermission(
                TopicRole.PublishSubscribe,
                CacheSelector.AllCaches,
                TopicSelector.ByName("mytopic")
            )
        }));

        scope = DisposableTokenScopes.TopicPublishSubscribe(CacheSelector.ByName("mycache"), "mytopic");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.TopicPermission(
                TopicRole.PublishSubscribe,
                CacheSelector.ByName("mycache"),
                TopicSelector.ByName("mytopic")
            )
        }));
    }


    [Fact]
    public void DisposableTokenScopes_TopicSubscribeOnly()
    {
        var scope = DisposableTokenScopes.TopicSubscribeOnly("mycache", "mytopic");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.TopicPermission(
                TopicRole.SubscribeOnly,
                CacheSelector.ByName("mycache"),
                TopicSelector.ByName("mytopic")
            )
        }));
        scope = DisposableTokenScopes.TopicSubscribeOnly(CacheSelector.AllCaches, "mytopic");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.TopicPermission(
                TopicRole.SubscribeOnly,
                CacheSelector.AllCaches,
                TopicSelector.ByName("mytopic")
            )
        }));

        scope = DisposableTokenScopes.TopicSubscribeOnly(CacheSelector.ByName("mycache"), "mytopic");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.TopicPermission(
                TopicRole.SubscribeOnly,
                CacheSelector.ByName("mycache"),
                TopicSelector.ByName("mytopic")
            )
        }));
    }

    [Fact]
    public void DisposableTokenScopes_TopicPublishOnly()
    {
        var scope = DisposableTokenScopes.TopicPublishOnly("mycache", "mytopic");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.TopicPermission(
                TopicRole.PublishOnly,
                CacheSelector.ByName("mycache"),
                TopicSelector.ByName("mytopic")
            )
        }));
        scope = DisposableTokenScopes.TopicPublishOnly(CacheSelector.AllCaches, "mytopic");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.TopicPermission(
                TopicRole.PublishOnly,
                CacheSelector.AllCaches,
                TopicSelector.ByName("mytopic")
            )
        }));

        scope = DisposableTokenScopes.TopicPublishOnly(CacheSelector.ByName("mycache"), "mytopic");
        scope.Should().BeEquivalentTo(new DisposableTokenScope(new List<DisposableTokenPermission>
        {
            new DisposableToken.TopicPermission(
                TopicRole.PublishOnly,
                CacheSelector.ByName("mycache"),
                TopicSelector.ByName("mytopic")
            )
        }));
    }
}
