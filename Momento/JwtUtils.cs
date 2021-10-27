using System;
using JWT;
using JWT.Serializers;
using MomentoSdk.Exceptions;
using Newtonsoft.Json;

namespace MomentoSdk
{
    public class JwtUtils
    {
        /// <summary>
        /// extracts the controlEndpoint and cacheEndpoint
        /// from the jwt
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public static Claims decodeJwt(string jwt)
        {
            IJsonSerializer serializer = new JsonNetSerializer();

            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            JwtDecoder decoder = new JWT.JwtDecoder(serializer, urlEncoder);
            try
            {
                var decodedJwt = decoder.Decode(jwt);
                return JsonConvert.DeserializeObject<Claims>(decodedJwt);
            } catch(Exception)
            {
                throw new InvalidJwtException("invalid jwt passed");
            }
        }
    }

    public class Claims
    {
        [JsonProperty(PropertyName = "cp", Required = Required.Always)]
        public string controlEndpoint { get; set; }
        [JsonProperty(PropertyName = "c", Required = Required.Always)]
        public string cacheEndpoint { get; set; }

        public Claims(string cacheEndpoint, string controlEndpoint)
        {
            this.cacheEndpoint = cacheEndpoint;
            this.controlEndpoint = controlEndpoint;
        }
    }
}
