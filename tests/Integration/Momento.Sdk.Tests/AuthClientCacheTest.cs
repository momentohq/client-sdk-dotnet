using System.Collections.Generic;
using System.Threading.Tasks;
using Momento.Sdk.Auth;
using Momento.Sdk.Auth.AccessControl;
using Momento.Sdk.Config;

namespace Momento.Sdk.Tests;

[Collection("AuthClient")]
public class AuthClientCacheTest : IClassFixture<CacheClientFixture>, IClassFixture<AuthClientFixture>
{
    private readonly ICacheClient cacheClient;
    private readonly IAuthClient authClient;
    private readonly string cacheName;
    private readonly string key = "my-key";
    private readonly string keyPrefix;
    private readonly string value = "my-value";

    public AuthClientCacheTest(CacheClientFixture cacheFixture, AuthClientFixture authFixture)
    {
        authClient = authFixture.Client;
        cacheClient = cacheFixture.Client;
        cacheName = cacheFixture.CacheName;
        keyPrefix = key.Substring(0, 3);
    }

    private async Task<ICacheClient> GetClientForTokenScope(DisposableTokenScope scope)
    {
        var response = await authClient.GenerateDisposableTokenAsync(scope, ExpiresIn.Minutes(2));
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        string authToken = "";
        if (response is GenerateDisposableTokenResponse.Success token)
        {
            Assert.False(String.IsNullOrEmpty(token.AuthToken));
            authToken = token.AuthToken;
        }
        var authProvider = new StringMomentoTokenProvider(authToken);
        return new CacheClient(Configurations.Laptop.Latest(), authProvider, TimeSpan.FromSeconds(60));
    }

    private async Task SetCacheKey(string cacheName, string key, string value)
    {
        var setResponse = await cacheClient.SetAsync(cacheName, key, value);
        Assert.True(setResponse is CacheSetResponse.Success);
    }

    private async Task VerifyCacheKey(string cacheName, string key, string value)
    {
        var getResponse = await cacheClient.GetAsync(cacheName, key);
        Assert.True(getResponse is CacheGetResponse.Hit);
        if (getResponse is CacheGetResponse.Hit hit)
        {
            Assert.Equal(value, hit.ValueString);
        }
    }

    private void AssertPermissionError<TResp, TErrResp>(TResp response)
    {
        Assert.True(response is TErrResp);
        if (response is IError err)
        {
            Assert.Equal(err.ErrorCode, MomentoErrorCode.PERMISSION_ERROR);
        }
    }

    [Fact]
    public async Task GenerateDisposableCacheAuthToken_HappyPath()
    {
        var response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheReadWrite("cache"), ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheReadWrite(CacheSelector.ByName("cache")), ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);

        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheReadOnly("cache"), ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheReadOnly(CacheSelector.ByName("cache")), ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);

        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheWriteOnly("cache"), ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheWriteOnly(CacheSelector.ByName("cache")), ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
    }

    [Theory]
    [InlineData(null)]
    public async Task GenerateDisposableCacheAuthToken_ErrorsOnNull(string cacheName)
    {
        var response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheReadWrite(cacheName),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, 
            ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheReadOnly(cacheName),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, 
            ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheWriteOnly(cacheName),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, 
            ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task GenerateDisposableCacheAuthToken_ErrorsOnEmpty()
    {
        var response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheReadWrite(""),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, 
            ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheReadOnly(""),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, 
            ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheWriteOnly(""),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, 
            ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
    }


    [Fact]
    public async Task GenerateDisposableCacheAuthToken_ReadWrite_HappyPath()
    {
        var readwriteCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheReadWrite(cacheName)
        );
        var setResponse = await readwriteCacheClient.SetAsync(cacheName, key, value);
        Assert.True(setResponse is CacheSetResponse.Success);
        var getResponse = await readwriteCacheClient.GetAsync(cacheName, key);
        Assert.True(getResponse is CacheGetResponse.Hit);
        if (getResponse is CacheGetResponse.Hit hit)
        {
            Assert.Equal(value, hit.ValueString);
        }
        readwriteCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheReadWrite("someothercache")
        );
        setResponse = await readwriteCacheClient.SetAsync(cacheName, key, value);
        AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
        getResponse = await readwriteCacheClient.GetAsync(cacheName, key);
        AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);
    }

    [Fact]
    public async Task GenerateDisposableCacheAuthToken_ReadOnly_HappyPath()
    {
        var readonlyCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheReadOnly(cacheName)
        );
        var setResponse = await readonlyCacheClient.SetAsync(cacheName, key, value);
        AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
        await SetCacheKey(cacheName, key, value);
        var getResponse = await readonlyCacheClient.GetAsync(cacheName, key);
        Assert.True(getResponse is CacheGetResponse.Hit);
        if (getResponse is CacheGetResponse.Hit hit)
        {
            Assert.Equal(value, hit.ValueString);
        }
    }

    [Fact]
    public async Task GenerateDisposableCacheAuthToken_WriteOnly_HappyPath()
    {
        var writeonlyCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheWriteOnly(cacheName)
        );
        var setResponse = await writeonlyCacheClient.SetAsync(cacheName, key, value);
        Assert.True(setResponse is CacheSetResponse.Success);
        var getResponse = await writeonlyCacheClient.GetAsync(cacheName, key);
        AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);
        await VerifyCacheKey(cacheName, key, value);
        writeonlyCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheWriteOnly("someothercache")
        );
        setResponse = await writeonlyCacheClient.SetAsync(cacheName, key, value);
        AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
    }

    [Fact]
    public async Task GenerateDisposableCacheKeyAuthToken_HappyPath()
    {
        var response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyReadWrite("cache", "cacheKey"), ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyReadWrite(CacheSelector.ByName("cache"), "cacheKey"), ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);

        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyReadOnly("cache", "cacheKey"), ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyReadOnly(CacheSelector.ByName("cache"), "cacheKey"), ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);

        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyWriteOnly("cache", "cacheKey"), ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyWriteOnly(CacheSelector.ByName("cache"), "cacheKey"), ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
    }

    [Theory]
    [InlineData(null, "cacheKey")]
    [InlineData("cache", null)]
    public async Task GenerateDisposableCacheKeyAuthToken_ErrorsOnNull(string cacheName, string cacheKey)
    {
        var response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyReadWrite(cacheName, cacheKey),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, 
            ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyReadOnly(cacheName, cacheKey),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, 
            ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyWriteOnly(cacheName, cacheKey),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, 
            ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
    }

    [Theory]
    [InlineData("", "cacheKey")]
    [InlineData("cache", "")]
    public async Task GenerateDisposableCacheKeyAuthToken_ErrorsOnEmpty(string cacheName, string cacheKey)
    {
        var response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyReadWrite(cacheName, cacheKey),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, 
            ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyReadOnly(cacheName, cacheKey),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, 
            ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyWriteOnly(cacheName, cacheKey),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, 
            ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task GenerateDisposableCacheKeyAuthToken_ReadWrite_HappyPath()
    {
        var readwriteCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheKeyReadWrite(cacheName, key)
        );
        var setResponse = await readwriteCacheClient.SetAsync(cacheName, key, value);
        Assert.True(setResponse is CacheSetResponse.Success);
        var getResponse = await readwriteCacheClient.GetAsync(cacheName, key);
        Assert.True(getResponse is CacheGetResponse.Hit);
        if (getResponse is CacheGetResponse.Hit hit)
        {
            Assert.Equal(value, hit.ValueString);
        }
        readwriteCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheKeyReadWrite("othercache", key)
        );
        setResponse = await readwriteCacheClient.SetAsync(cacheName, key, value);
        AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
        getResponse = await readwriteCacheClient.GetAsync(cacheName, key);
        AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);
        readwriteCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheKeyReadWrite(cacheName, "otherkey")
        );
        setResponse = await readwriteCacheClient.SetAsync(cacheName, key, value);
        AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
        getResponse = await readwriteCacheClient.GetAsync(cacheName, key);
        AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);
    }

    [Fact]
    public async Task GenerateDisposableCacheKeyAuthToken_ReadOnly_HappyPath()
    {
        var readonlyCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheKeyReadOnly(cacheName, key)
        );
        var setResponse = await readonlyCacheClient.SetAsync(cacheName, key, value);
        AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
        await SetCacheKey(cacheName, key, value);
        var getResponse = await readonlyCacheClient.GetAsync(cacheName, key);
        Assert.True(getResponse is CacheGetResponse.Hit);
        if (getResponse is CacheGetResponse.Hit hit)
        {
            Assert.Equal(value, hit.ValueString);
        }
        readonlyCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheKeyReadOnly("othercache", key)
        );
        getResponse = await readonlyCacheClient.GetAsync(cacheName, key);
        AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);
        readonlyCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheKeyReadOnly(cacheName, "otherkey")
        );
        getResponse = await readonlyCacheClient.GetAsync(cacheName, key);
        AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);
    }

    [Fact]
    public async Task GenerateDisposableCacheKeyAuthToken_WriteOnly_HappyPath()
    {
        var writeonlyCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheKeyWriteOnly(cacheName, key)
        );
        var setResponse = await writeonlyCacheClient.SetAsync(cacheName, key, value);
        Assert.True(setResponse is CacheSetResponse.Success);
        var getResponse = await writeonlyCacheClient.GetAsync(cacheName, key);
        AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);
        await VerifyCacheKey(cacheName, key, value);
        writeonlyCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheKeyWriteOnly("otherCache", key)
        );
        setResponse = await writeonlyCacheClient.SetAsync(cacheName, key, value);
        AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
        writeonlyCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheKeyWriteOnly(cacheName, "otherkey")
        );
        setResponse = await writeonlyCacheClient.SetAsync(cacheName, key, value);
        AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
    }

    [Fact]
    public async Task GenerateDisposableCacheKeyPrefixAuthToken_HappyPath()
    {
        var response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyPrefixReadWrite("cache", "cacheKey"), ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyPrefixReadWrite(CacheSelector.ByName("cache"), "cacheKey"), 
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);

        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyPrefixReadOnly("cache", "cacheKey"), ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyPrefixReadOnly(CacheSelector.ByName("cache"), "cacheKey"), 
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);

        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyPrefixWriteOnly("cache", "cacheKey"), ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyPrefixWriteOnly(CacheSelector.ByName("cache"), "cacheKey"), 
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
    }

    [Theory]
    [InlineData(null, "cacheKeyPrefix")]
    [InlineData("cache", null)]
    public async Task GenerateDisposableCacheKeyPrefixAuthToken_ErrorsOnNull(string cacheName, string cacheKeyPrefix)
    {
        var response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyPrefixReadWrite(cacheName, cacheKeyPrefix),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, 
            ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyPrefixReadOnly(cacheName, cacheKeyPrefix),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, 
            ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyPrefixWriteOnly(cacheName, cacheKeyPrefix),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, 
            ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
    }

    [Theory]
    [InlineData("", "cacheKeyPrefix")]
    [InlineData("cache", "")]
    public async Task GenerateDisposableCacheKeyPrefixAuthToken_ErrorsOnEmpty(string cacheName, string cacheKeyPrefix)
    {
        var response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyPrefixReadWrite(cacheName, cacheKeyPrefix),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, 
            ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyPrefixReadOnly(cacheName, cacheKeyPrefix),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, 
            ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.CacheKeyPrefixWriteOnly(cacheName, cacheKeyPrefix),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, 
            ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task GenerateDisposableCacheKeyPrefixAuthToken_ReadWrite_HappyPath()
    {
        var readwriteCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheKeyPrefixReadWrite(cacheName, keyPrefix)
        );
        var setResponse = await readwriteCacheClient.SetAsync(cacheName, key, value);
        Assert.True(setResponse is CacheSetResponse.Success);
        var getResponse = await readwriteCacheClient.GetAsync(cacheName, key);
        Assert.True(getResponse is CacheGetResponse.Hit);
        if (getResponse is CacheGetResponse.Hit hit)
        {
            Assert.Equal(value, hit.ValueString);
        }
        readwriteCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheKeyPrefixReadWrite(cacheName, "otherPrefix")
        );
        setResponse = await readwriteCacheClient.SetAsync(cacheName, key, value);
        AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
        getResponse = await readwriteCacheClient.GetAsync(cacheName, key);
        AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);
        readwriteCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheKeyPrefixReadWrite("otherCache", keyPrefix)
        );
        setResponse = await readwriteCacheClient.SetAsync(cacheName, key, value);
        AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
        getResponse = await readwriteCacheClient.GetAsync(cacheName, key);
        AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);
    }

    [Fact]
    public async Task GenerateDisposableCacheKeyPrefixAuthToken_ReadOnly_HappyPath()
    {
        var readonlyCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheKeyPrefixReadOnly(cacheName, keyPrefix)
        );
        var setResponse = await readonlyCacheClient.SetAsync(cacheName, key, value);
        AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
        await SetCacheKey(cacheName, key, value);
        var getResponse = await readonlyCacheClient.GetAsync(cacheName, key);
        Assert.True(getResponse is CacheGetResponse.Hit);
        if (getResponse is CacheGetResponse.Hit hit)
        {
            Assert.Equal(value, hit.ValueString);
        }
        readonlyCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheKeyPrefixReadOnly("otherCache", keyPrefix)
        );
        setResponse = await readonlyCacheClient.SetAsync(cacheName, key, value);
        AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
        getResponse = await readonlyCacheClient.GetAsync(cacheName, key);
        AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);
        readonlyCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheKeyPrefixReadOnly(cacheName, "otherPrefix")
        );
        setResponse = await readonlyCacheClient.SetAsync(cacheName, key, value);
        AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
        getResponse = await readonlyCacheClient.GetAsync(cacheName, key);
        AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);
    }

    [Fact]
    public async Task GenerateDisposableCacheKeyPrefixAuthToken_WriteOnly_HappyPath()
    {
        var writeonlyCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheKeyPrefixWriteOnly(cacheName, keyPrefix)
        );
        var setResponse = await writeonlyCacheClient.SetAsync(cacheName, key, value);
        Assert.True(setResponse is CacheSetResponse.Success);
        var getResponse = await writeonlyCacheClient.GetAsync(cacheName, key);
        AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);
        await VerifyCacheKey(cacheName, key, value);
        writeonlyCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheKeyPrefixWriteOnly("otherCache", keyPrefix)
        );
        setResponse = await writeonlyCacheClient.SetAsync(cacheName, key, value);
        AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
        getResponse = await writeonlyCacheClient.GetAsync(cacheName, key);
        AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);
        writeonlyCacheClient = await GetClientForTokenScope(
            DisposableTokenScopes.CacheKeyPrefixWriteOnly(cacheName, "otherPrefix")
        );
        setResponse = await writeonlyCacheClient.SetAsync(cacheName, key, value);
        AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
        getResponse = await writeonlyCacheClient.GetAsync(cacheName, key);
        AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);
    }

    // Tests using DisposableTokenScopes composed from multiple permissions

    [Fact]
    public async Task GenerateDisposableMultiPermissionScope_ReadWriteWithSelectors()
    {
        var scope = new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>{
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadWrite,
                CacheSelector.ByName(cacheName),
                CacheItemSelector.ByKey("cow")
            ),
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadWrite,
                CacheSelector.ByName(cacheName),
                CacheItemSelector.ByKeyPrefix("pet")
            )
        });
        var client = await GetClientForTokenScope(scope);

        // we can read/write our key and key prefix in the test cache
        var setResponse = await client.SetAsync(cacheName, "cow", "moo");
        Assert.True(setResponse is CacheSetResponse.Success);
        var getResponse = await client.GetAsync(cacheName, "cow");
        Assert.True(getResponse is CacheGetResponse.Hit);
        if (getResponse is CacheGetResponse.Hit hit)
        {
            Assert.Equal(hit.ValueString, "moo");
        }

        setResponse = await client.SetAsync(cacheName, "pet-cat", "meow");
        Assert.True(setResponse is CacheSetResponse.Success);
        getResponse = await client.GetAsync(cacheName, "pet-cat");
        Assert.True(getResponse is CacheGetResponse.Hit);
        if (getResponse is CacheGetResponse.Hit hit2)
        {
            Assert.Equal(hit2.ValueString, "meow");
        }

        setResponse = await client.SetAsync(cacheName, "giraffe", "noidea");
        AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
        getResponse = await client.GetAsync(cacheName, "giraffe");
        AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);

        // we cannot read/write a specified key or keyPrefix to a different cache
        scope = new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>{
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadWrite,
                CacheSelector.ByName("a-totally-different-cache"),
                CacheItemSelector.ByKey("cow")
            ),
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadWrite,
                CacheSelector.ByName("a-totally-different-cache"),
                CacheItemSelector.ByKeyPrefix("pet")
            )
        });
        client = await GetClientForTokenScope(scope);

        setResponse = await client.SetAsync(cacheName, "cow", "moo");
        AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
        getResponse = await client.GetAsync(cacheName, "cow");
        AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);

        setResponse = await client.SetAsync(cacheName, "pet-cat", "meow");
        AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
        getResponse = await client.GetAsync(cacheName, "pet-cat");
        AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);
    }

    [Fact]
    public async Task GenerateDisposableMultiPermission_ReadOnlyWithSelectorsAllCaches()
    {
        var cache2Name = cacheName + "-2";
        try
        {
            // create a second cache
            Assert.True(await cacheClient.CreateCacheAsync(cache2Name) is CreateCacheResponse.Success);

            var scope = new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>{
                new DisposableToken.CacheItemPermission(
                    CacheRole.ReadOnly,
                    CacheSelector.AllCaches,
                    CacheItemSelector.ByKey("cow")
                ),
                new DisposableToken.CacheItemPermission(
                    CacheRole.ReadOnly,
                    CacheSelector.AllCaches,
                    CacheItemSelector.ByKeyPrefix("pet")
                )
            });
            var client = await GetClientForTokenScope(scope);

            // sets should fail for both caches
            var setResponse = await client.SetAsync(cacheName, "cow", "moo");
            AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
            setResponse = await client.SetAsync(cache2Name, "pet-koala", "awwww");
            AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);

            // gets should succeed for specified key and key prefix but fail for other keys
            await SetCacheKey(cacheName, "cow", "moo");
            await SetCacheKey(cacheName, "pet-koala", "awww");
            await SetCacheKey(cacheName, "dog", "woof");
            await SetCacheKey(cache2Name, "cow", "moo");
            await SetCacheKey(cache2Name, "pet-koala", "awww");
            await SetCacheKey(cache2Name, "dog", "woof");

            var getResponse = await client.GetAsync(cacheName, "cow");
            Assert.True(getResponse is CacheGetResponse.Hit);
            getResponse = await client.GetAsync(cacheName, "pet-koala");
            Assert.True(getResponse is CacheGetResponse.Hit);
            getResponse = await client.GetAsync(cache2Name, "cow");
            Assert.True(getResponse is CacheGetResponse.Hit);
            getResponse = await client.GetAsync(cache2Name, "pet-koala");
            Assert.True(getResponse is CacheGetResponse.Hit);
            getResponse = await client.GetAsync(cacheName, "dog");
            AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);
            getResponse = await client.GetAsync(cache2Name, "dog");
            AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);
        }
        finally
        {
            // delete the second cache
            Assert.True(await cacheClient.DeleteCacheAsync(cache2Name) is DeleteCacheResponse.Success);
        }
    }

    [Fact]
    public async Task GenerateDisposableMultiPermission_ReadOnlyWriteOnly()
    {
        var cache2Name = cacheName + "-2";
        try
        {
            // create a second cache
            Assert.True(await cacheClient.CreateCacheAsync(cache2Name) is CreateCacheResponse.Success);

            var scope = new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>{
                new DisposableToken.CacheItemPermission(
                    CacheRole.WriteOnly,
                    CacheSelector.ByName(cacheName),
                    CacheItemSelector.ByKey("cow")
                ),
                new DisposableToken.CacheItemPermission(
                    CacheRole.ReadOnly,
                    CacheSelector.ByName(cache2Name),
                    CacheItemSelector.ByKeyPrefix("pet")
                )
            });
            var client = await GetClientForTokenScope(scope);

            // we can write to only one key and not read in test cache
            var setResponse = await client.SetAsync(cacheName, "cow", "moo");
            Assert.True(setResponse is CacheSetResponse.Success);
            var getResponse = await client.GetAsync(cacheName, "cow");
            AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);
            setResponse = await client.SetAsync(cacheName, "parrot", "somethingaboutcrackers");
            AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
            await VerifyCacheKey(cacheName, "cow", "moo");

            // we can read prefixed keys but no others and cannot write in the second cache
            await SetCacheKey(cache2Name, "pet-armadillo", "thunk");
            await SetCacheKey(cache2Name, "snake", "hiss");
            getResponse = await client.GetAsync(cache2Name, "pet-armadillo");
            Assert.True(getResponse is CacheGetResponse.Hit);
            if (getResponse is CacheGetResponse.Hit hit)
            {
                Assert.Equal(hit.ValueString, "thunk");
            }
            setResponse = await client.SetAsync(cache2Name, "pet-armadillo", "meow");
            AssertPermissionError<CacheSetResponse, CacheSetResponse.Error>(setResponse);
            getResponse = await client.GetAsync(cache2Name, "snake");
            AssertPermissionError<CacheGetResponse, CacheGetResponse.Error>(getResponse);
        }
        finally
        {
            // delete the second cache
            Assert.True(await cacheClient.DeleteCacheAsync(cache2Name) is DeleteCacheResponse.Success);
        }
    }
}
