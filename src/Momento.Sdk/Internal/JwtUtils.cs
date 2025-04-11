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
    /// from the jwt
    /// </summary>
    /// <param name="jwt"></param>
    /// <returns></returns>
    public static Claims DecodeJwt(string jwt)
    {
        IJsonSerializer serializer = new JsonNetSerializer();

        IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
        JwtDecoder decoder = new JwtDecoder(serializer, urlEncoder);
        try
        {
            var decodedJwt = decoder.Decode(jwt);
            return JsonConvert.DeserializeObject<Claims>(decodedJwt);
        }
        catch (Exception)
        {
            throw new InvalidArgumentException("invalid jwt passed");
        }
    }
}

/// <summary>
/// Encapsulates claims embedded in a JWT token that specify host endpoints
/// for the control plane and the data plane.
/// </summary>
public class Claims
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
    public Claims(string cacheEndpoint, string controlEndpoint)
    {
        this.CacheEndpoint = cacheEndpoint;
        this.ControlEndpoint = controlEndpoint;
    }
}
