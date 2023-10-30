using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Momento.Sdk.Requests.Vector;
using Momento.Sdk.Responses.Vector;

namespace Momento.Sdk;

/// <summary>
/// PREVIEW Vector Index Client
/// WARNING: the API for this client is not yet stable and may change without notice.
///
/// Includes control operations and data operations.
/// </summary>
public interface IPreviewVectorIndexClient : IDisposable
{
    /// <summary>
    /// Creates a vector index if it does not exist.
    /// </summary>
    /// <param name="indexName">The vector index to be created.</param>
    /// <param name="numDimensions">The number of dimensions per vector</param>
    /// <param name="similarityMetric">The metric used to quantify
    /// the distance between vectors. Can be cosine similarity,
    /// inner product, or euclidean similarity. Defaults to cosine similarity.</param>
    /// <returns>
    /// Task representing the result of the create vector index operation. This result
    /// is resolved to a type-safe object of one of
    /// the following subtypes:
    /// <list type="bullet">
    /// <item><description>CreateVectorIndexResponse.Success</description></item>
    /// <item><description>CreateVectorIndexResponse.AlreadyExists</description></item>
    /// <item><description>CreateVectorIndexResponse.Error</description></item>
    /// </list>
    /// Pattern matching can be used to operate on the appropriate subtype.
    /// For example:
    /// <code>
    /// if (response is CreateVectorIndexResponse.Error errorResponse)
    /// {
    ///     // handle error as appropriate
    /// }
    /// </code>
    /// <remarks>
    /// Remark on the choice of similarity metric:
    /// <list type="bullet">
    /// <item><description>
    /// Cosine similarity is appropriate for most embedding models as they tend to be optimized for this metric.
    /// </description></item>
    /// <item><description>
    /// If the vectors are unit normalized, cosine similarity is equivalent to inner product.
    /// If your vectors are already unit normalized, you can use inner product to improve performance.
    /// </description></item>
    /// <item><description>
    /// Euclidean similarity, the sum of squared differences, is appropriate for datasets where
    /// this metric is meaningful. For example, if the vectors represent images, and the embedding
    /// model is trained to optimize the euclidean distance between images, then euclidean
    /// similarity is appropriate.
    /// </description></item>
    /// </list>
    /// </remarks>
    /// </returns>
    public Task<CreateVectorIndexResponse> CreateIndexAsync(string indexName, long numDimensions, SimilarityMetric similarityMetric = SimilarityMetric.CosineSimilarity);
    
    /// <summary>
    /// Lists all vector indexes.
    /// </summary>
    /// <returns>
    /// Task representing the result of the list vector index operation. The
    /// response object is resolved to a type-safe object of one of
    /// the following subtypes:
    /// <list type="bullet">
    /// <item><description>ListVectorIndexesResponse.Success</description></item>
    /// <item><description>ListVectorIndexesResponse.Error</description></item>
    /// </list>
    /// Pattern matching can be used to operate on the appropriate subtype.
    /// For example:
    /// <code>
    /// if (response is ListVectorIndexesResponse.Error errorResponse)
    /// {
    ///     // handle error as appropriate
    /// }
    /// </code>
    /// </returns>
    public Task<ListVectorIndexesResponse> ListIndexesAsync();

    /// <summary>
    /// Deletes a vector index and all the vectors within it.
    /// </summary>
    /// <param name="indexName">The name of the vector index to delete.</param>
    /// <returns>
    /// Task representing the result of the delete vector index operation. The
    /// response object is resolved to a type-safe object of one of
    /// the following subtypes:
    /// <list type="bullet">
    /// <item><description>DeleteVectorIndexResponse.Success</description></item>
    /// <item><description>DeleteVectorIndexResponse.Error</description></item>
    /// </list>
    /// Pattern matching can be used to operate on the appropriate subtype.
    /// For example:
    /// <code>
    /// if (response is DeleteVectorIndexResponse.Error errorResponse)
    /// {
    ///     // handle error as appropriate
    /// }
    /// </code>
    ///</returns>
    public Task<DeleteVectorIndexResponse> DeleteIndexesAsync(string indexName);

    /// <summary>
    /// Upserts a batch of items into a vector index.
    /// If an item with the same ID already exists in the index, it will be replaced.
    /// Otherwise, it will be added to the index.
    /// </summary>
    /// <param name="indexName">The name of the vector index to delete.</param>
    /// <param name="items">The items to upsert into the index.</param>
    /// <returns>
    /// Task representing the result of the upsert operation. The
    /// response object is resolved to a type-safe object of one of
    /// the following subtypes:
    /// <list type="bullet">
    /// <item><description>VectorUpsertItemBatchResponse.Success</description></item>
    /// <item><description>VectorUpsertItemBatchResponse.Error</description></item>
    /// </list>
    /// Pattern matching can be used to operate on the appropriate subtype.
    /// For example:
    /// <code>
    /// if (response is VectorUpsertItemBatchResponse.Error errorResponse)
    /// {
    ///     // handle error as appropriate
    /// }
    /// </code>
    ///</returns>
    public Task<VectorUpsertItemBatchResponse> UpsertItemBatchAsync(string indexName,
        IEnumerable<VectorIndexItem> items);

    /// <summary>
    /// Deletes all items with the given IDs from the index.
    /// </summary>
    /// <param name="indexName">The name of the vector index to delete.</param>
    /// <param name="ids">The IDs of the items to delete from the index.</param>
    /// <returns>
    /// Task representing the result of the upsert operation. The
    /// response object is resolved to a type-safe object of one of
    /// the following subtypes:
    /// <list type="bullet">
    /// <item><description>VectorDeleteItemBatchResponse.Success</description></item>
    /// <item><description>VectorDeleteItemBatchResponse.Error</description></item>
    /// </list>
    /// Pattern matching can be used to operate on the appropriate subtype.
    /// For example:
    /// <code>
    /// if (response is VectorDeleteItemBatchResponse.Error errorResponse)
    /// {
    ///     // handle error as appropriate
    /// }
    /// </code>
    ///</returns>
    public Task<VectorDeleteItemBatchResponse> DeleteItemBatchAsync(string indexName, IEnumerable<string> ids);

    /// <summary>
    /// Searches for the most similar vectors to the query vector in the index.
    /// Ranks the vectors according to the similarity metric specified when the
    /// index was created.
    /// </summary>
    /// <param name="indexName">The name of the vector index to delete.</param>
    /// <param name="queryVector">The vector to search for.</param>
    /// <param name="topK">The number of results to return. Defaults to 10.</param>
    /// <param name="metadataFields">A list of metadata fields to return with each result.</param>
    /// <returns>
    /// Task representing the result of the upsert operation. The
    /// response object is resolved to a type-safe object of one of
    /// the following subtypes:
    /// <list type="bullet">
    /// <item><description>VectorDeleteItemBatchResponse.Success</description></item>
    /// <item><description>VectorDeleteItemBatchResponse.Error</description></item>
    /// </list>
    /// Pattern matching can be used to operate on the appropriate subtype.
    /// For example:
    /// <code>
    /// if (response is VectorDeleteItemBatchResponse.Error errorResponse)
    /// {
    ///     // handle error as appropriate
    /// }
    /// </code>
    ///</returns>
    public Task<VectorSearchResponse> SearchAsync(string indexName, IEnumerable<float> queryVector, int topK = 10,
        MetadataFields? metadataFields = null);
}