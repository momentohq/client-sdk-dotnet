﻿using Momento.Sdk.Auth;
using Momento.Sdk.Tests;

namespace Momento.Sdk.Tests.Integration;

public class TestBase
{
    protected readonly ICredentialProvider authProvider;
    protected readonly string cacheName;
    protected TimeSpan defaultTtl;
    protected ICacheClient client;

    public TestBase(CacheClientFixture fixture)
    {
        this.client = fixture.Client;
        this.cacheName = fixture.CacheName;
        this.authProvider = fixture.AuthProvider;
        this.defaultTtl = fixture.DefaultTtl;
    }
}
