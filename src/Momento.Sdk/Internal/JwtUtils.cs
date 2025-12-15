using JWT;
using JWT.Serializers;
using Momento.Sdk.Exceptions;
using Newtonsoft.Json;
using System;

namespace Momento.Sdk.Internal;

/// <summary>
/// Provides methods for dealing with JWT tokens.
/// </summary>
public class JwtUtils
{
    /// <summary>
    /// extracts the controlEndpoint and cacheEndpoint
    /// from the legacy jwt
    /// </summary>
    /// <param name="jwt"></param>
    /// <returns></returns>
    public static LegacyClaims DecodeLegacyJwt(string jwt)
    {
        IJsonSerializer serializer = new JsonNetSerializer();

        IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
        JwtDecoder decoder = new JwtDecoder(serializer, urlEncoder);
        try
        {
            var decodedJwt = decoder.Decode(jwt);
            return JsonConvert.DeserializeObject<LegacyClaims>(decodedJwt);
        }
        catch (Exception)
        {
            throw new InvalidArgumentException("invalid jwt passed");
        }
    }

    /// <summary>
    /// extracts the type, id, and optional expiration
    /// from the v2 jwt
    /// </summary>
    /// <param name="jwt"></param>
    /// <returns></returns>
    public static V2Claims DecodeV2Jwt(string jwt)
    {
        IJsonSerializer serializer = new JsonNetSerializer();

        IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
        JwtDecoder decoder = new JwtDecoder(serializer, urlEncoder);
        try
        {
            var decodedJwt = decoder.Decode(jwt);
            return JsonConvert.DeserializeObject<V2Claims>(decodedJwt);
        }
        catch (Exception)
        {
            throw new InvalidArgumentException("invalid jwt passed");
        }
    }
}

/// <summary>
/// Encapsulates claims embedded in a legacy JWT token that specify host endpoints
/// for the control plane and the data plane.
/// </summary>
public class LegacyClaims
{

    /// <summary>
    /// Endpoint for issuing control plane requests.
    /// </summary>
    [JsonProperty(PropertyName = "cp", Required = Required.Always)]
    public string ControlEndpoint { get; private set; }

    /// <summary>
    /// Endpoint for issuing data plane requests.
    /// </summary>
    [JsonProperty(PropertyName = "c", Required = Required.Always)]
    public string CacheEndpoint { get; private set; }

    /// <summary>
    /// Encapsulates claims embedded in a JWT token that specify host endpoints
    /// for the control plane and the data plane.
    /// </summary>
    /// <param name="cacheEndpoint">Data plane endpoint</param>
    /// <param name="controlEndpoint">Control plane endpoint</param>
    public LegacyClaims(string cacheEndpoint, string controlEndpoint)
    {
        this.CacheEndpoint = cacheEndpoint;
        this.ControlEndpoint = controlEndpoint;
    }
}

/// <summary>
/// Encapsulates claims embedded in a v2 JWT token.
/// </summary>
public class V2Claims
{

    /// <summary>
    /// Type of api key. "g" for global (v2) api keys.
    /// </summary>
    [JsonProperty(PropertyName = "t", Required = Required.Always)]
    public string Type { get; private set; }

    /// <summary>
    /// Encapsulates claims embedded in a v2 JWT token that specify type of key,
    /// key ID, and optional expiration.
    /// </summary>
    /// <param name="type">Type of api key</param>
    public V2Claims(string type)
    {
        this.Type = type;
    }
}
