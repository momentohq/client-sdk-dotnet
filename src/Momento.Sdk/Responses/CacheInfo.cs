using System;

namespace Momento.Sdk.Responses;

/// <summary>
/// Contains a Momento cache's name.
/// </summary>
public class CacheInfo : IEquatable<CacheInfo>
{
    /// <summary>
    /// Holds the name of the cache.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Contains a Momento cache's name.
    /// </summary>
    /// <param name="cacheName">The name of the cache.</param>
    public CacheInfo(string cacheName) => Name = cacheName;

    /// <inheritdoc />
    public bool Equals(CacheInfo? other)
    {
        if (other == null)
        {
            return false;
        }

        return this.Name == other.Name;
    }

    /// <inheritdoc />
    public override bool Equals(Object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        return Equals(obj as CacheInfo);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}
