using System;

namespace Momento.Sdk.Responses;

public class CacheInfo : IEquatable<CacheInfo>
{
    public string Name { get; }

    public CacheInfo(string cacheName) => Name = cacheName;

    // override object.Equals
    public bool Equals(CacheInfo? other)
    {
        if (other == null)
        {
            return false;
        }

        return this.Name == other.Name;
    }

    public override bool Equals(Object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        return Equals(obj as CacheInfo);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}
