﻿using System;
using JWT;
using JWT.Serializers;
using Momento.Sdk.Exceptions;
using Newtonsoft.Json;

namespace Momento.Sdk.Internal;

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

public class Claims
{
    [JsonProperty(PropertyName = "cp", Required = Required.Always)]
    public string ControlEndpoint { get; private set; }
    [JsonProperty(PropertyName = "c", Required = Required.Always)]
    public string CacheEndpoint { get; private set; }

    public Claims(string cacheEndpoint, string controlEndpoint)
    {
        this.CacheEndpoint = cacheEndpoint;
        this.ControlEndpoint = controlEndpoint;
    }
}
