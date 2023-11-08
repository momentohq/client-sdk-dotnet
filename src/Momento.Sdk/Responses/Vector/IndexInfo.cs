namespace Momento.Sdk.Responses.Vector;

/// <summary>
/// Information about a vector index.
/// </summary>
public class IndexInfo
{
    /// <summary>
    /// The name of the index.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Constructs an IndexInfo.
    /// </summary>
    /// <param name="name">The name of the index.</param>
    public IndexInfo(string name)
    {
        Name = name;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IndexInfo other && Name == other.Name;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"IndexInfo {{ Name = {Name} }}";
    }
}