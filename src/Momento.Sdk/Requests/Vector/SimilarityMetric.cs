namespace Momento.Sdk.Requests.Vector;

/// <summary>
/// The similarity metric to use when comparing vectors in the index.
/// </summary>
public enum SimilarityMetric
{
    /// <summary>
    /// The cosine similarity between two vectors, ie the cosine of the angle between them.
    /// Bigger is better. Ranges from -1 to 1.
    /// </summary>
    CosineSimilarity,
    
    
    /// <summary>
    /// The inner product between two vectors, ie the sum of the element-wise products.
    /// Bigger is better. Ranges from 0 to infinity.
    /// </summary>
    InnerProduct,
    
    
    /// <summary>
    /// The Euclidean distance squared between two vectors, ie the sum of squared differences between each element.
    /// Smaller is better. Ranges from 0 to infinity.
    /// </summary>
    EuclideanSimilarity
}