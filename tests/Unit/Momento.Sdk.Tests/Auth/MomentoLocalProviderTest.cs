using Momento.Sdk.Auth;
using Momento.Sdk.Exceptions;
using System;
using Xunit;

namespace Momento.Sdk.Tests.Unit;


public class MomentoLocalProviderTests
{
    [Fact]
    public void MomentoLocalProvider_DefaultConstructor() {
        var provider = new MomentoLocalProvider();
        Assert.Equal("", provider.AuthToken);
        Assert.Equal(8080, provider.Port);
        Assert.Equal("127.0.0.1:8080", provider.ControlEndpoint);
        Assert.Equal("127.0.0.1:8080", provider.CacheEndpoint);
        Assert.Equal("127.0.0.1:8080", provider.TokenEndpoint);
    }

    [Fact]
    public void MomentoLocalProvider_ConstructorWithHostnameAndPort() {
        var provider = new MomentoLocalProvider("localhost", 9090);
        Assert.Equal("", provider.AuthToken);
        Assert.Equal(9090, provider.Port);
        Assert.Equal("localhost:9090", provider.ControlEndpoint);
        Assert.Equal("localhost:9090", provider.CacheEndpoint);
        Assert.Equal("localhost:9090", provider.TokenEndpoint);
    }

    [Fact]
    public void MomentoLocalProvider_ConstructorWithHostname() {
        var provider = new MomentoLocalProvider("localhost");
        Assert.Equal("", provider.AuthToken);
        Assert.Equal(8080, provider.Port);
        Assert.Equal("localhost:8080", provider.ControlEndpoint);
        Assert.Equal("localhost:8080", provider.CacheEndpoint);
        Assert.Equal("localhost:8080", provider.TokenEndpoint);
    }

    [Fact]
    public void MomentoLocalProvider_ConstructorWithPort() {
        var provider = new MomentoLocalProvider(9090);
        Assert.Equal("", provider.AuthToken);
        Assert.Equal(9090, provider.Port);
        Assert.Equal("127.0.0.1:9090", provider.ControlEndpoint);
        Assert.Equal("127.0.0.1:9090", provider.CacheEndpoint);
        Assert.Equal("127.0.0.1:9090", provider.TokenEndpoint);
    }
}