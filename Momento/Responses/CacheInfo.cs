using System;

namespace MomentoSdk.Responses;

public class CacheInfo : IEquatable<CacheInfo>
{
    private readonly string name;
    public CacheInfo(string cachename)
    {
        name = cachename;
    }

    public string Name()
    {
        return name;
    }

    // override object.Equals
    public bool Equals(CacheInfo? other)
    {
        if (other == null)
        {
            return false;
        }

        return this.name == other.name;
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
        return name.GetHashCode();
    }
}
