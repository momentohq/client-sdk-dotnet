using Momento.Sdk.Auth;
using Momento.Sdk.Exceptions;
using System;
using Xunit;

namespace Momento.Sdk.Tests.Unit;


public class CredentialProviderTests
{
    const string TEST_INVALID_LEGACY_AUTH_TOKEN = "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJpbnRlZ3JhdGlvbiJ9.ZOgkTs";
    const string TEST_LEGACY_AUTH_TOKEN_ENDPOINT = "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJzcXVpcnJlbCIsImNwIjoiY29udHJvbCBwbGFuZSBlbmRwb2ludCIsImMiOiJkYXRhIHBsYW5lIGVuZHBvaW50In0.zsTsEXFawetTCZI";
    const string TEST_V1_API_TOKEN = "eyJhcGlfa2V5IjogImV5SjBlWEFpT2lKS1YxUWlMQ0poYkdjaU9pSklVekkxTmlKOS5leUpwYzNNaU9pSlBibXhwYm1VZ1NsZFVJRUoxYVd4a1pYSWlMQ0pwWVhRaU9qRTJOemd6TURVNE1USXNJbVY0Y0NJNk5EZzJOVFV4TlRReE1pd2lZWFZrSWpvaUlpd2ljM1ZpSWpvaWFuSnZZMnRsZEVCbGVHRnRjR3hsTG1OdmJTSjkuOEl5OHE4NExzci1EM1lDb19IUDRkLXhqSGRUOFVDSXV2QVljeGhGTXl6OCIsICJlbmRwb2ludCI6ICJ0ZXN0Lm1vbWVudG9ocS5jb20ifQ==";
    const string TEST_V1_API_KEY = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2NzgzMDU4MTIsImV4cCI6NDg2NTUxNTQxMiwiYXVkIjoiIiwic3ViIjoianJvY2tldEBleGFtcGxlLmNvbSJ9.8Iy8q84Lsr-D3YCo_HP4d-xjHdT8UCIuvAYcxhFMyz8";
    const string MALFORMED_TOKEN = "asdf";
    const string TEST_V1_MISSING_ENDPOINT = "eyJhcGlfa2V5IjogImV5SmxibVJ3YjJsdWRDSTZJbU5sYkd3dE5DMTFjeTEzWlhOMExUSXRNUzV3Y205a0xtRXViVzl0Wlc1MGIyaHhMbU52YlNJc0ltRndhVjlyWlhraU9pSmxlVXBvWWtkamFVOXBTa2xWZWtreFRtbEtPUzVsZVVwNlpGZEphVTlwU25kYVdGSnNURzFrYUdSWVVuQmFXRXBCV2pJeGFHRlhkM1ZaTWpsMFNXbDNhV1J0Vm5sSmFtOTRabEV1VW5OMk9GazVkRE5KVEMwd1RHRjZiQzE0ZDNaSVZESmZZalJRZEhGTlVVMDVRV3hhVlVsVGFrbENieUo5In0=";
    const string TEST_V1_MISSING_API_KEY = "eyJlbmRwb2ludCI6ICJhLmIuY29tIn0=";

    [Fact]
    public void StringMomentoTokenProvider_EmptyOrNull_ThrowsException()
    {
        Assert.Throws<InvalidArgumentException>(
            () => new StringMomentoTokenProvider(TEST_INVALID_LEGACY_AUTH_TOKEN)
        );
        Assert.Throws<InvalidArgumentException>(() => new StringMomentoTokenProvider(null!));
    }

    [Fact]
    public void StringMomentoTokenProvider_BadToken_ThrowsException()
    {
        Assert.Throws<InvalidArgumentException>(
            () => new StringMomentoTokenProvider(MALFORMED_TOKEN)
        );
    }

    [Fact]
    public void StringMomentoTokenProvider_LegacyAuthToken_HappyPath()
    {
        var cp = new StringMomentoTokenProvider(TEST_LEGACY_AUTH_TOKEN_ENDPOINT);
        Assert.Equal(TEST_LEGACY_AUTH_TOKEN_ENDPOINT, cp.AuthToken);
        Assert.Equal("control plane endpoint", cp.ControlEndpoint);
        Assert.Equal("data plane endpoint", cp.CacheEndpoint);
    }

    [Fact]
    public void StringMomentoTokenProvider_LegacyAuthToken_OverrideCacheEndpoint_HappyPath()
    {
        var cp = new StringMomentoTokenProvider(TEST_LEGACY_AUTH_TOKEN_ENDPOINT).WithCacheEndpoint("my.cache.endpoint");
        Assert.Equal(TEST_LEGACY_AUTH_TOKEN_ENDPOINT, cp.AuthToken);
        Assert.Equal("control plane endpoint", cp.ControlEndpoint);
        Assert.Equal("my.cache.endpoint", cp.CacheEndpoint);
    }

    [Fact]
    public void StringMomentoTokenProvider_V1AuthToken_HappyPath()
    {
        var cp = new StringMomentoTokenProvider(TEST_V1_API_TOKEN);
        Assert.Equal(TEST_V1_API_KEY, cp.AuthToken);
        Assert.Equal("control.test.momentohq.com", cp.ControlEndpoint);
        Assert.Equal("cache.test.momentohq.com", cp.CacheEndpoint);
    }

    [Fact]
    public void StringMomentoTokenProvider_V1AuthToken_OverrideCacheEndpoint_HappyPath()
    {
        var cp = new StringMomentoTokenProvider(TEST_V1_API_TOKEN).WithCacheEndpoint("my.cache.endpoint");
        Assert.Equal(TEST_V1_API_KEY, cp.AuthToken);
        Assert.Equal("control.test.momentohq.com", cp.ControlEndpoint);
        Assert.Equal("my.cache.endpoint", cp.CacheEndpoint);
    }

    [Fact]
    public void StringMomentoTokenProvider_V1AuthToken_MissingEndpoints()
    {
        Assert.Throws<InvalidArgumentException>(() => new StringMomentoTokenProvider(TEST_V1_MISSING_ENDPOINT));
    }

    [Fact]
    public void StringMomentoTokenProvider_V1AuthToken_MissingApiKey()
    {
        Assert.Throws<InvalidArgumentException>(() => new StringMomentoTokenProvider(TEST_V1_MISSING_API_KEY));
    }

    [Fact]
    public void EnvMomentoTokenProvider_EmptyOrNull_ThrowsException()
    {
        var envVar = "TEST_INVALID_LEGACY";
        Environment.SetEnvironmentVariable(envVar, TEST_INVALID_LEGACY_AUTH_TOKEN);
        Assert.Throws<InvalidArgumentException>(
            () => new EnvMomentoTokenProvider(envVar)
        );
        Assert.Throws<InvalidArgumentException>(() => new EnvMomentoTokenProvider(null!));
        Environment.SetEnvironmentVariable(envVar, "");
    }

    [Fact]
    public void EnvMomentoTokenProvider_BadToken_ThrowsException()
    {
        var envVar = "TEST_MALFORMED";
        Environment.SetEnvironmentVariable(envVar, MALFORMED_TOKEN);
        Assert.Throws<InvalidArgumentException>(
            () => new EnvMomentoTokenProvider(envVar)
        );
        Environment.SetEnvironmentVariable(envVar, "");
    }

    [Fact]
    public void EnvMomentoTokenProvider_LegacyAuthToken_HappyPath()
    {
        var envVar = "TEST_LEGACY";
        Environment.SetEnvironmentVariable(envVar, TEST_LEGACY_AUTH_TOKEN_ENDPOINT);
        var cp = new EnvMomentoTokenProvider(envVar);
        Assert.Equal(TEST_LEGACY_AUTH_TOKEN_ENDPOINT, cp.AuthToken);
        Assert.Equal("control plane endpoint", cp.ControlEndpoint);
        Assert.Equal("data plane endpoint", cp.CacheEndpoint);
        Environment.SetEnvironmentVariable(envVar, "");
    }

    [Fact]
    public void EnvMomentoTokenProvider_LegacyAuthToken_OverrideCacheEndpoint_HappyPath()
    {
        var envVar = "TEST_LEGACY";
        Environment.SetEnvironmentVariable(envVar, TEST_LEGACY_AUTH_TOKEN_ENDPOINT);
        var cp = new EnvMomentoTokenProvider(envVar).WithCacheEndpoint("my.cache.endpoint");
        Assert.Equal(TEST_LEGACY_AUTH_TOKEN_ENDPOINT, cp.AuthToken);
        Assert.Equal("control plane endpoint", cp.ControlEndpoint);
        Assert.Equal("my.cache.endpoint", cp.CacheEndpoint);
        Environment.SetEnvironmentVariable(envVar, "");
    }

    [Fact]
    public void EnvMomentoTokenProvider_V1AuthToken_HappyPath()
    {
        var envVar = "TEST_V1";
        Environment.SetEnvironmentVariable(envVar, TEST_V1_API_TOKEN);
        var cp = new EnvMomentoTokenProvider(envVar);
        Assert.Equal(TEST_V1_API_KEY, cp.AuthToken);
        Assert.Equal("control.test.momentohq.com", cp.ControlEndpoint);
        Assert.Equal("cache.test.momentohq.com", cp.CacheEndpoint);
        Environment.SetEnvironmentVariable(envVar, "");
    }

    [Fact]
    public void EnvMomentoTokenProvider_V1AuthToken_OverrideCacheEndpoint_HappyPath()
    {
        var envVar = "TEST_V1";
        Environment.SetEnvironmentVariable(envVar, TEST_V1_API_TOKEN);
        var cp = new EnvMomentoTokenProvider(envVar).WithCacheEndpoint("my.cache.endpoint");
        Assert.Equal(TEST_V1_API_KEY, cp.AuthToken);
        Assert.Equal("control.test.momentohq.com", cp.ControlEndpoint);
        Assert.Equal("my.cache.endpoint", cp.CacheEndpoint);
        Environment.SetEnvironmentVariable(envVar, "");
    }

    [Fact]
    public void EnvMomentoTokenProvider_V1AuthToken_MissingEndpoints()
    {
        var envVar = "TEST_V1";
        Environment.SetEnvironmentVariable(envVar, TEST_V1_MISSING_ENDPOINT);
        Assert.Throws<InvalidArgumentException>(() => new EnvMomentoTokenProvider(envVar));
        Environment.SetEnvironmentVariable(envVar, "");
    }

    [Fact]
    public void EnvMomentoTokenProvider_V1AuthToken_MissingApiKey()
    {
        var envVar = "TEST_V1";
        Environment.SetEnvironmentVariable(envVar, TEST_V1_MISSING_API_KEY);
        Assert.Throws<InvalidArgumentException>(() => new EnvMomentoTokenProvider(envVar));
        Environment.SetEnvironmentVariable(envVar, "");
    }
}
