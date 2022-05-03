using System;
namespace MomentoSdkDotnet45
{
    public enum CacheOperation
    {
        SET,
        GET
    };

    public class SigningRequest
    {
        /// <summary>
        /// The name of the cache.
        /// </summary>
        public string CacheName
        {
            get;
        }
        /// <summary>
        /// The key of the object.
        /// </summary>
        public string CacheKey
        {
            get;
        }
        /// <summary>
        /// The operation performed on the item in the cache.
        /// </summary>
        public CacheOperation CacheOperation
        {
            get;
        }
        /// <summary>
        /// The timestamp that the pre-signed URL is valid until.
        /// </summary>
        public uint ExpiryEpochSeconds
        {
            get;
        }
        /// <summary>
        /// Time to Live for the item in Cache.
        /// This is an optional property that will only be used for CacheOperation.SET
        /// </summary>
        public uint TtlSeconds
        {
            get;
            set;
        }

        public SigningRequest(string cacheName, string cacheKey, CacheOperation cacheOperation, uint expiryEpochSeconds)
        {
            CacheName = cacheName;
            CacheKey = cacheKey;
            CacheOperation = cacheOperation;
            ExpiryEpochSeconds = expiryEpochSeconds;
        }

    }
}
