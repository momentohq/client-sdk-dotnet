using System;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;
using Newtonsoft.Json;

namespace Momento.Sdk.Auth;

internal class TokenAndEndpoints
{
    public string AuthToken { get; }
    public string ControlEndpoint { get; }
    public string CacheEndpoint { get; }
    public string TokenEndpoint { get; }

    public TokenAndEndpoints(string authToken, string controlEndpoint, string cacheEndpoint, string tokenEndpoint)
    {
        AuthToken = authToken;
        ControlEndpoint = controlEndpoint;
        CacheEndpoint = cacheEndpoint;
        TokenEndpoint = tokenEndpoint;
    }
}

internal class Base64DecodedV1Token
{
    public string? api_key = null;
    public string? endpoint = null;
}

internal static class AuthUtils
{
    public static bool IsBase64String(string base64)
    {
        try
        {
            Convert.FromBase64String(base64);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static TokenAndEndpoints TryDecodeAuthToken(string authToken)
    {
        try
        {
            if (AuthUtils.IsBase64String(authToken))
            {
                var base64Bytes = System.Convert.FromBase64String(authToken);
                var theString = System.Text.Encoding.UTF8.GetString(base64Bytes);
                var decodedToken = JsonConvert.DeserializeObject<Base64DecodedV1Token>(theString);
                if (String.IsNullOrEmpty(decodedToken.api_key) || String.IsNullOrEmpty(decodedToken.endpoint))
                {
                    throw new InvalidArgumentException("");
                }
                return new TokenAndEndpoints(
                    decodedToken.api_key!,
                    "control." + decodedToken.endpoint,
                    "cache." + decodedToken.endpoint,
                    "token." + decodedToken.endpoint
                );
            }
            else
            {
                var claims = JwtUtils.DecodeJwt(authToken);
                return new TokenAndEndpoints(
                    authToken,
                    claims.ControlEndpoint,
                    claims.CacheEndpoint,
                    // TODO: assuming legacy tokens will never have a token endpoint. AFAIK, they can't be used
                    //  to generate disposable tokens anyway, but verify and see if this needs to be fixed.
                    claims.CacheEndpoint
                );
            }
        }
        catch (Exception e) when (e is InvalidArgumentException || e is JsonReaderException)
        {
            throw new InvalidArgumentException("The supplied Momento authToken is not valid.");
        }
    }
}
