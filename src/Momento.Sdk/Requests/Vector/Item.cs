using System.Collections.Generic;
using Momento.Sdk.Messages.Vector;

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
}