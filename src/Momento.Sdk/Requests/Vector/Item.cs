using System.Collections.Generic;
using System.Linq;
using Momento.Sdk.Messages.Vector;
using Momento.Sdk.Internal.ExtensionMethods;

namespace Momento.Sdk.Requests.Vector;

/// <summary>
/// An item in a vector index. Contains an ID, the vector, and any associated metadata.
/// </summary>
public class Item
{
    /// <summary>
    /// Constructs a Item with no metadata.
    /// </summary>
    /// <param name="id">the ID of the vector.</param>
    /// <param name="vector">the vector.</param>
    public Item(string id, List<float> vector)
    {
        Id = id;
        Vector = vector;
        Metadata = new Dictionary<string, MetadataValue>();
    }

    /// <summary>
    /// Constructs a Item.
    /// </summary>
    /// <param name="id">the ID of the vector.</param>
    /// <param name="vector">the vector.</param>
    /// <param name="metadata">Metadata associated with the vector.</param>
    public Item(string id, List<float> vector, Dictionary<string, MetadataValue> metadata)
    {
        Id = id;
        Vector = vector;
        Metadata = metadata;
    }

    /// <summary>
    /// The ID of the vector.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// The vector.
    /// </summary>
    public List<float> Vector { get; }

    /// <summary>
    /// Metadata associated with the vector.
    /// </summary>
    public Dictionary<string, MetadataValue> Metadata { get; }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (Item)obj;
        return Id == other.Id && Vector.SequenceEqual(other.Vector) && Metadata.MetadataEquals(other.Metadata);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 23 + Id.GetHashCode();
        hash = hash * 23 + Vector.GetHashCode();
        hash = hash * 23 + Metadata.MetadataHashCode();
        return hash;
    }
}
