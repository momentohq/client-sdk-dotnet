using System;
using System.IdentityModel.Tokens.Jwt;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Requests;
using Xunit;

namespace Momento.Sdk.Tests;

public class MomentoSignerTest
{
    private const string RS256_JWK = "{\"p\":\"_oJd2v0VrexsvlfO0O0i6MFfgy3yyRh6cUjCOrFxGccFVFsIBfa6zbo78Wsx_kDx75Z4k9x_Mw8lueP7e4nGyzDDODPo2l5ERnVvtcKndN1N9WKp8MJHm7T9FTx6tXf2f6sfUmzDchTGZT3MuZ5K1lYpfAcWb9G7e_1kiUMfgTU\",\"kty\":\"RSA\",\"q\":\"vTSy0-0qS71GDt9e6Tj12XO_bhDVifgrkrSL92llmfRPxv7KQeUE3gDO90YR6K7PHBbcB60Z69lTpk374zniI4E38mvSmB4box0E2ATpzMvX6on-H4mCjUvePd2Qj0JAg2Uyqeze9lunh2C2KXul4VHzBiygCOIHu_r86Wa5iPs\",\"d\":\"Ou0zcMZ6gL3c79W82wqVqZTggJZSDOe_1l4klAvitTeg48nE31nQAzeT0cn8YyjY2nDA9iBc8jGrCnV9HaYhRlCZ_XE8036HVm9Sw1WjmJqDqWyhDsfV4o74jcxn8QNOKLX2NcWJxo11kOWdMh5AMYXQt2xh2yeV4QG996-EnC-8aoBYbRJO93ZTiJss1FPgXfe8gEJnv27ShjD1MjCl2ikGvTa5HOX3aUYvPJezN_HvKfovbp9h77DE5y9V2KewqGI5m25b97CzBn_pcQioiPlzoHck_x9C77GOsySOg3C3Le0fJdGXgMJmyBrDlmjF7YfeRbqwxUSX0CzdubVldQ\",\"e\":\"AQAB\",\"kid\":\"fooKeyId\",\"qi\":\"CnV6ziWMCxQvPe447kifPXAY9fqMv47RcHZmS3Co1EQNnP1CGO5H8tFgAso6IxPWyFexaRqPtX8JYgpjiR3fm-PwjD7RI50XU3cNOw2SRxpdA5KDGgZXecMOx81DRTBPAtDOUII-pmEfvJKXVQCiRnoiFbFqr66nXaO6mTe0Xm4\",\"dp\":\"MsSsli9f8La1pm57md-D1CwmslMrGAQjAJAD9pNIvVye6onSGuZxsvIQXQMGEPLBkApS-SPF19iQrPkWRDlih0ut1Xs9WrntIqTwaLBwmPZAQ8-vmJAYmq3Kwj6zN5m7eRIYiGebwRj1zmI6gVhbE1BSrCP5zMpofL46HMtf8HU\",\"alg\":\"RS256\",\"dq\":\"G5LliOskYdtYrWwyOcz6T1GGEXVUmYHYX83-I_VxQCmRws95DHdi6TO29eR5Ua7AMjjGojvA7lVC0pbE4c2avk_jpmm-TDr_Dht5jD3TEOyYL-8iYNg6dXscDWoP2kDug_eolYkWyVJ8LMeUZKFHgHnf8ANq40CFngiq-RzmZyc\",\"n\":\"vBqjjxPSxtoLutQ5C0ivkbQmjFoJNIMC4CjWDK_gMTU2wF8S5g3FRSsGjqIj0EIWUtQOn_wMU5n98bRRnOwhGVRqFsWNwNwxQ_v_9VTSPBDWOJOd9Zlsey6UjklA7SfikqCgwBK_hDIaWhrlt7vY0Zu2eMdGTVb9_lyfDPfEiv5ONPOnaFK98EP-WYvmfulaWvqTTUVHaRZps4sZsrftsVShupBKjzttADXz9KRePOjUgxIAfg42yAm1YnGIJk36tdUo_HczpDQat0UdI0x4gI3baNdXIYdDazzziJyxbZaD9c7ii1Vm4PAyhAIpxeIh8TDsrzUyTneGaVK6CdTW9w\"}";
    private const string ES256_JWK = "{\"kty\":\"EC\",\"d\":\"wLj6mq_IqGAqz40RyR1QiH1KElvhECQ8dQAcu7iRwWE\",\"crv\":\"P-256\",\"kid\":\"fooKeyId\",\"x\":\"pSU203ud3cNnVeCgaho2z-JBao21EHFm4or75sV8RkY\",\"y\":\"uSwevlzSV3kyArKAu7qv7I_ffaXAvAp98YM0zwUA5jA\",\"alg\":\"ES256\"}";
    private const string RS384_JWK = "{\"p\":\"_LVm8mWfx8Td7zg7Xupll_2pVGyhwmos18Mi6_vr0DglnDWOg3nsfjaS6j5a_mwgeEHUUjXaORf5zZMI3hVaD9-1WaaErmrUlZuNDvAtgb1MLLzgzP2WYmdAAzSn1-0fOqUiiZtAADEr-gmCrN-ofbDcKmppdwA1DbOaCMmVvc0\",\"kty\":\"RSA\",\"q\":\"yr9ZUQAotA7KllPB7klkeDwfDv2FIz-N7Yms20Nv20oALB3XLWlEkG85AOuYwAk-gXBtu45piM4D3jfHDsP5uycvXdxVGlna56XKUjgfSHW3UdfKxhd9FzOY4MvPG_Aj1rdkuNOPCNUGtRRbXA3HNjhBoJoSg5dbSCwnQzi10rM\",\"d\":\"DsnOr7w7Zo5qmNMr7sfhKvtQOJFaUwS_IhLyyNntmn57FuqlqMO1cv7_uzRer-LbI9dF7J4DVjtf6jEe7CcbYQHMCQqbj2RPrubprqYojjBKVtSlUt9hOIK8DswgyrXX8JnpMsLIw8Mdvyo5EbCw7tt4qKeTePFk-xA7GAqHi3FLisst3ijkpQT5OjbUA1GYMIijzJfcFOgjtbvLghDl1XLW29ZP3K1MYkNtX9P3bbbI5GRwWSo5SsGzzcNCbI4unfHU8MIf-cDUDopeAM6ixjB-OXzz6Fq4J-XeIS0-4vZMYD2l5OuD0gQfkCiYfOyrNPL0KqZxolioRvD1dRzTXQ\",\"e\":\"AQAB\",\"kid\":\"fooKeyId\",\"qi\":\"YWx6rzgadSyrpCc4YTmq_ly_JHXxPO5ddZoWmukVb_tF-3E-sBaOd7jBj4OJnrHqTz2EkwUw8krexoptZEvMPjvLRxdubwgqw6d8SCC-6DKOfUJB43diY56lzIxlIA32BemLx6B0SrBIbgWhY1IkAJlj2AiAh01ygDh1tH-FoKI\",\"dp\":\"x152MZZrUDfIwAolDOTv8dF13d02YSNS7YZN7s95Y3Rod6zpGmD-azSzA4reTwsPMtD8qT9DQvffZIgz3sIJo6xibrAozVILFVz7FGX4APtPNZxt3kvScR_0KJNKN9gjYykU7mtFOuGQSFtodOqfC0qU6AG74t6O_JhNVdF0CaE\",\"alg\":\"RS384\",\"dq\":\"C53ZFT4IFwD99I0KAIgt_IGdWfOGrFVY4XJQ-CMuBod_6QcwrAZrCkeFIZteHiqpbSsu7l8jhtYe_J1_h0YNSf7dxOf57E-XrkwegoV6rWEpRsQxdxYjca_gI4kp7bTdqNDLMZfVizEBeGCZN3YGowGoKPaK9wU2ErWM7loSeOc\",\"n\":\"yCQGvhlh6_RTK3gtJPgllHHUYT_hx9xgeDhLRQAadHd0FjCCBHXLisTABHIu568rn86chznbzN0yLDJ-C5Q8YM46xxJZgDfG3Sq4NlZJcrxjBeTIVXeqWj0W5HDK8PDIYgEZfXmJ4AKeB3PQT5o25zN92ja45bFHk8vHBo_TECYYxHvI7TzbMOs5Z4PvWQnfR34GR0TvD4iHSEq6r0TTrYhdtLRq-PsIOk--0subO7uTEVqLCU99wQmsty0VacmIeezuap2OgR0EG4nlAzjqs8EyZqJKin8b9dRfQm8UOpQ1GbU7pKxymQc7pb1K54YSXFF5lYalcQOWlhmM01bgVw\"}";

    [Theory]
    [InlineData(RS256_JWK)]
    [InlineData(ES256_JWK)]
    [InlineData(RS384_JWK)]
    public void TestJwkRoundTrip(string jwk)
    {
        MomentoSigner signer = new MomentoSigner(jwk);
        uint expiryEpochSeconds = uint.MaxValue;
        var url = signer.CreatePresignedUrl("foobar.com", new SigningRequest("testCacheName", "testCacheKey", CacheOperation.GET, expiryEpochSeconds)); ;

        string? jwt = HttpUtility.ParseQueryString(new Uri(url).Query).Get("token");

        var securityKey = new JsonWebKey(jwk);
        TokenValidationParameters validationParameters = new TokenValidationParameters()
        {
            RequireExpirationTime = false,
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = securityKey
        };
        SecurityToken validatedToken;
        new JwtSecurityTokenHandler().ValidateToken(jwt, validationParameters, out validatedToken);
        Assert.Equal(new DateTime(2106, 02, 07, 06, 28, 15), validatedToken.ValidTo);
    }

    [Fact]
    public void TestPresignedUrlForGet()
    {
        MomentoSigner signer = new MomentoSigner(RS256_JWK);
        uint expiryEpochSeconds = uint.MaxValue;
        var url = signer.CreatePresignedUrl("foobar.com", new SigningRequest("testCacheName", "testCacheKey", CacheOperation.GET, expiryEpochSeconds));

        Uri? uriResult;
        bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult);
        Assert.True(result);
        Assert.Equal(Uri.UriSchemeHttps, uriResult?.Scheme);
        Assert.StartsWith("https://rest.foobar.com/cache/get/testCacheName/testCacheKey?token=", url);
    }

    [Fact]
    public void TestPresignedUrlForSet()
    {
        MomentoSigner signer = new MomentoSigner(RS256_JWK);
        uint expiryEpochSeconds = uint.MaxValue;
        var req = new SigningRequest("testCacheName", "testCacheKey", CacheOperation.SET, expiryEpochSeconds)
        {
            TtlSeconds = uint.MaxValue
        };
        var url = signer.CreatePresignedUrl("foobar.com", req);

        Uri? uriResult;
        bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult);
        Assert.True(result);
        Assert.Equal(Uri.UriSchemeHttps, uriResult?.Scheme);
        Assert.StartsWith("https://rest.foobar.com/cache/set/testCacheName/testCacheKey?ttl_milliseconds=4294967295000&token=", url);
    }

    [Fact]
    public void TestUrlEncoding()
    {
        MomentoSigner signer = new MomentoSigner(RS256_JWK);
        uint expiryEpochSeconds = uint.MaxValue;
        var testCacheKey = "#$&\\'+,/:;=?@[]";
        var url = signer.CreatePresignedUrl("foobar.com", new SigningRequest("testCacheName", testCacheKey, CacheOperation.GET, expiryEpochSeconds));

        Uri? uriResult;
        bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult);
        Assert.True(result);
        Assert.Equal(Uri.UriSchemeHttps, uriResult?.Scheme);
        Assert.StartsWith("https://rest.foobar.com/cache/get/testCacheName/%23%24%26%5c%27%2b%2c%2f%3a%3b%3d%3f%40%5b%5d?token=", url);
    }

    [Fact]
    public void TestJwkError()
    {
        uint expiryEpochSeconds = uint.MaxValue;
        var invalidJwk = "{\"alg\":\"foo\"}";
        MomentoSigner signer = new MomentoSigner(invalidJwk);

        Assert.Throws<InvalidArgumentException>(() => signer.SignAccessToken(new SigningRequest("testCacheName", "testCacheKey", CacheOperation.GET, expiryEpochSeconds)));
    }
}
