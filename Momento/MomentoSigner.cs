using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using MomentoSdk.Exceptions;

namespace MomentoSdk
{
    public class MomentoSigner
    {

        /// <summary>
        /// Create a pre-signed HTTPS URL to retrieve an object.
        /// </summary>
        /// <param name="hostname">Hostname of the SimpleCacheService. Use the value returned from CreateSigningKey's response.</param>
        /// <param name="jwkJsonString">JWK in JSON string. Use the value returned from CreateSigningKey's response.</param>
        /// <param name="cacheName">The name of the Cache.</param>
        /// <param name="cacheKey">The key of the Object.</param>
        /// <param name="expiryEpochSeconds">The timestamp that the pre-signed URL is valid until.</param>
        /// <returns></returns>
        public string CreatePresignedUrlForGet(string hostname, string jwkJsonString, string cacheName, string cacheKey, uint expiryEpochSeconds)
        {
            var payload = CommonJwtBody(cacheName, cacheKey, expiryEpochSeconds);
            payload.Add("method", new string[] { "get" });

            var encodedJwt = CreateJwtToken(jwkJsonString, payload);

            return $"https://{hostname}/cache/get/{cacheName}/{cacheKey}?token={encodedJwt}";
        }

        /// <summary>
        /// Create a pre-signed HTTPS URL to upload an object.
        /// </summary>
        /// <param name="hostname">Hostname of the SimpleCacheService. Use the value returned from CreateSigningKey's response.</param>
        /// <param name="jwkJsonString">JWK in JSON string. Use the value returned from CreateSigningKey's response.</param>
        /// <param name="cacheName">The name of the Cache.</param>
        /// <param name="cacheKey">The key of the Object.</param>
        /// <param name="expiryEpochSeconds">The timestamp that the pre-signed URL is valid until.</param>
        /// <param name="ttlSeconds">The timestamp that the object is valid until.</param>
        /// <returns></returns>
        public string CreatePresignedUrlForSet(string hostname, string jwkJsonString, string cacheName, string cacheKey, uint expiryEpochSeconds, uint ttlSeconds)
        {
            var payload = CommonJwtBody(cacheName, cacheKey, expiryEpochSeconds);
            payload.Add("method", new string[] { "set" });
            payload.Add("ttl", ttlSeconds);

            var encodedJwt = CreateJwtToken(jwkJsonString, payload);

            return $"https://{hostname}/cache/set/{cacheName}/{cacheKey}?token={encodedJwt}";
        }

        internal string CreateJwtToken(string jwkJsonString, JwtPayload jwtPayload)
        {
            try
            {
                var securityKey = new JsonWebKey(jwkJsonString);
                var credentials = new SigningCredentials(securityKey, securityKey.Alg);
                var JWTHeader = new JwtHeader(credentials);

                var jwtToken = new JwtSecurityToken(JWTHeader, jwtPayload);
                return new JwtSecurityTokenHandler().WriteToken(jwtToken);
            }
            catch (Exception e)
            {
                throw new InvalidArgumentException($"Invalid JWK: {jwkJsonString}", e);
            }
        }


        private JwtPayload CommonJwtBody(string cacheName, string cacheKey, uint expiryEpochSeconds)
        {
            return new JwtPayload()
            {
                { "exp", expiryEpochSeconds },
                { "cache", cacheName },
                { "key", cacheKey }
            };
        }
    }
}
