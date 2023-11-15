using Momento.Sdk.Requests.Vector;

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
    /// The number of dimensions in the index.
    /// </summary>
    public int NumDimensions { get; }

    /// <summary>
    /// The similarity metric used by the index.
    /// </summary>
    public SimilarityMetric SimilarityMetric { get; }

    /// <summary>
    /// Constructs an IndexInfo.
    /// </summary>
    /// <param name="name">The name of the index.</param>
    /// <param name="numDimensions">The number of dimensions in the index.</param>
    /// <param name="similarityMetric">The similarity metric used by the index.</param>
    public IndexInfo(string name, int numDimensions, SimilarityMetric similarityMetric)
    {
        Name = name;
        NumDimensions = numDimensions;
        SimilarityMetric = similarityMetric;
    }


    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IndexInfo other && Name == other.Name && NumDimensions == other.NumDimensions && SimilarityMetric == other.SimilarityMetric;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = hash * 23 + Name.GetHashCode();
            hash = hash * 23 + NumDimensions.GetHashCode();
            hash = hash * 23 + SimilarityMetric.GetHashCode();
            return hash;
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"IndexInfo {{ Name = {Name} NumDimensions = {NumDimensions} SimilarityMetric = {SimilarityMetric} }}";
    }
}
