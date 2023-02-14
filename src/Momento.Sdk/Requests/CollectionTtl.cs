using System;
namespace Momento.Sdk.Requests
{
    /// <summary>
    /// Represents the desired behavior for managing the TTL on collection
    /// objects (dictionaries, lists, sets) in your cache.
    ///
    /// For cache operations that modify a collection, there are a few things
    /// to consider.  The first time the collection is created, we need to
    /// set a TTL on it.  For subsequent operations that modify the collection
    /// you may choose to update the TTL in order to prolong the life of the
    /// cached collection object, or you may choose to leave the TTL unmodified
    /// in order to ensure that the collection expires at the original TTL.
    ///
    /// The default behavior is to refresh the TTL (to prolong the life of the
    /// collection) each time it is written.  This behavior can be modified
    /// by calling the <see cref="WithNoRefreshTtlOnUpdates"/>
    /// </summary>
    /// 
    /// <param name="Ttl">The <see cref="TimeSpan"/> after which the cached collection
    /// should be expired from the cache.  If <code>null</code>, we use the default
    /// TTL <see cref="TimeSpan"/> that was passed to the <see cref="SimpleCacheClient"/>constructor</param>.
    /// <param name="RefreshTtl">If <see langword="true"/>, the collection's TTL will be refreshed (to
    /// prolong the life of the collection) on every update.  If <see langword="false"/>, the collection's
    /// TTL will only be set when the collection is initially created.</param>
    public record struct CollectionTtl(TimeSpan? Ttl = null, bool RefreshTtl = true)
    {
        /// <summary>
        /// The default way to handle TTLs for collections.  The default TTL
        /// <see cref="TimeSpan"/> that was specified when constructing the <see cref="SimpleCacheClient"/>
        /// will be used, and the TTL for the collection will be refreshed any
        /// time the collection is modified.
        /// </summary>
        /// <returns></returns>
        public static CollectionTtl FromCacheTtl()
        {
            return new CollectionTtl(Ttl: null, RefreshTtl: true);
        }

        /// <summary>
        /// Constructs a CollectionTtl with the specified <see cref="TimeSpan"/>.  The TTL
        /// for the collection will be refreshed any time the collection is
        /// modified.
        /// </summary>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public static CollectionTtl Of(TimeSpan ttl)
        {
            return new CollectionTtl(Ttl: ttl);
        }

        /// <summary>
        /// Constructs a <see cref="CollectionTtl"/> with the specified <see cref="TimeSpan"/>.
        /// Will only refresh if the TTL is provided (ie not <see langword="null" />).
        /// </summary>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public static CollectionTtl RefreshTtlIfProvided(TimeSpan? ttl = null)
        {
            return new CollectionTtl(Ttl: ttl, RefreshTtl: ttl.HasValue);
        }

        /// <summary>
        /// Specifies that the TTL for the collection should be refreshed when
        /// the collection is modified.  (This is the default behavior.)
        /// </summary>
        /// <returns>A copy of this CollectionTtl with the refresh TTL behavior enabled.</returns>
        public CollectionTtl WithRefreshTtlOnUpdates()
        {
            return new CollectionTtl(Ttl: this.Ttl, RefreshTtl: true);
        }

        /// <summary>
        /// Specifies that the TTL for the collection should not be refreshed
        /// when the collection is modified.  Use this if you want to ensure
        /// that your collection expires at the originally specified time, even
        /// if you make modifications to the value of the collection.
        /// </summary>
        /// <returns>A copy of this CollectionTtl with the refresh TTL behavior disabled.</returns>
        public CollectionTtl WithNoRefreshTtlOnUpdates()
        {
            return new CollectionTtl(Ttl: this.Ttl, RefreshTtl: false);
        }
    }
}

