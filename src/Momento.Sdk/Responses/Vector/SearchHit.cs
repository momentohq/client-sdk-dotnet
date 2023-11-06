using Momento.Sdk.Messages.Vector;

namespace Momento.Sdk.Responses.Vector;

using System.Collections.Generic;

/// <summary>
/// A hit from a vector search. Contains the ID of the vector, the distance from the query vector,
/// and any requested metadata.
/// </summary>
public class SearchHit
{
    /// <summary>
    /// The ID of the hit.
    /// </summary>
    public string Id { get; }
    
    /// <summary>
    /// The distance from the query vector.
    /// </summary>
    public double Distance { get; }
    
    /// <summary>
    /// Requested metadata associated with the hit.
    /// </summary>
    public Dictionary<string, MetadataValue> Metadata { get; }
    
    /// <summary>
    /// Constructs a SearchHit with no metadata.
    /// </summary>
    /// <param name="id">The ID of the hit.</param>
    /// <param name="distance">The distance from the query vector.</param>
    public SearchHit(string id, double distance)
    {
        Id = id;
        Distance = distance;
        Metadata = new Dictionary<string, MetadataValue>();
    }
    
    /// <summary>
    /// Constructs a SearchHit.
    /// </summary>
    /// <param name="id">The ID of the hit.</param>
    /// <param name="distance">The distance from the query vector.</param>
    /// <param name="metadata">Requested metadata associated with the hit</param>
    public SearchHit(string id, double distance, Dictionary<string, MetadataValue> metadata)
    {
        Id = id;
        Distance = distance;
        Metadata = metadata;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is null || GetType() != obj.GetType()) return false;

        var other = (SearchHit)obj;

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (Id != other.Id || Distance != other.Distance) return false;
        
        // Compare Metadata dictionaries
        if (Metadata.Count != other.Metadata.Count) return false;

        foreach (var pair in Metadata)
        {
            if (!other.Metadata.TryGetValue(pair.Key, out var value)) return false;
            if (!value.Equals(pair.Value)) return false;
        }

        return true;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            var hash = 17;

            hash = hash * 23 + Id.GetHashCode();
            hash = hash * 23 + Distance.GetHashCode();

            foreach (var pair in Metadata)
            {
                hash = hash * 23 + pair.Key.GetHashCode();
                hash = hash * 23 + (pair.Value?.GetHashCode() ?? 0);
            }

            return hash;
        }
    }
}

