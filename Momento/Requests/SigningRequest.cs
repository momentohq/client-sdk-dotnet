namespace MomentoSdk.Requests;

/// <summary>
/// Type of operation to performed on the cache for the signed url.
/// </summary>
public enum CacheOperation
{
    /// <summary>
    /// Store an item in the cache.
    /// </summary>
    SET,
    /// <summary>
    /// Retrieve a value from the cache.
    /// </summary>
    GET
};

/// <summary>
/// Metadata to create a pre-signed URL.
/// </summary>
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

    /// <summary>
    /// Instantiate a request for a pre-signed URL.
    /// </summary>
    /// <param name="cacheName">The cache where the key to pre-sign is.</param>
    /// <param name="cacheKey">The key to pre-sign.</param>
    /// <param name="cacheOperation">Type of operation (eg `get`, `set`) to pre-sign.</param>
    /// <param name="expiryEpochSeconds">Duration the pre-signed URL is valid for.</param>
    public SigningRequest(string cacheName, string cacheKey, CacheOperation cacheOperation, uint expiryEpochSeconds)
    {
        CacheName = cacheName;
        CacheKey = cacheKey;
        CacheOperation = cacheOperation;
        ExpiryEpochSeconds = expiryEpochSeconds;
    }
}
