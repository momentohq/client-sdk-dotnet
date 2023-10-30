using System.Collections.Generic;
using System.Threading.Tasks;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Internal;
using Momento.Sdk.Requests.Vector;
using Momento.Sdk.Responses.Vector;

namespace Momento.Sdk;

/// <summary>
/// PREVIEW Vector Index Client implementation
/// WARNING: the API for this client is not yet stable and may change without notice.
///
/// Includes control operations and data operations.
/// </summary>
public class PreviewVectorIndexClient: IPreviewVectorIndexClient
{
    private readonly VectorIndexControlClient controlClient;
    private readonly VectorIndexDataClient dataClient;


    /// <summary>
    /// Client to perform operations against Momento Serverless Cache.
    /// </summary>
    /// <param name="config">Configuration to use for the transport, retries, middlewares.
    /// See <see cref="Configurations"/> for out-of-the-box configuration choices,
    /// eg <see cref="Configurations.Laptop.Latest"/></param>
    /// <param name="authProvider">Momento auth provider.</param>
    public PreviewVectorIndexClient(IVectorIndexConfiguration config, ICredentialProvider authProvider)
    {
        var loggerFactory = config.LoggerFactory;
        controlClient =
            new VectorIndexControlClient(loggerFactory, authProvider.AuthToken, authProvider.ControlEndpoint);
        dataClient = new VectorIndexDataClient(config, authProvider.AuthToken, authProvider.CacheEndpoint);
    }

    /// <inheritdoc />
    public async Task<CreateVectorIndexResponse> CreateIndexAsync(string indexName, long numDimensions,
        SimilarityMetric similarityMetric = SimilarityMetric.CosineSimilarity)
    {
        return await controlClient.CreateIndexAsync(indexName, numDimensions, similarityMetric);
    }

    /// <inheritdoc />
    public async Task<ListVectorIndexesResponse> ListIndexesAsync()
    {
        return await controlClient.ListIndexesAsync();
    }

    /// <inheritdoc />
    public async Task<DeleteVectorIndexResponse> DeleteIndexesAsync(string indexName)
    {
        return await controlClient.DeleteIndexAsync(indexName);
    }

    /// <inheritdoc />
    public async Task<VectorUpsertItemBatchResponse> UpsertItemBatchAsync(string indexName,
        IEnumerable<VectorIndexItem> items)
    {
        return await dataClient.UpsertItemBatchAsync(indexName, items);
    }

    /// <inheritdoc />
    public async Task<VectorDeleteItemBatchResponse> DeleteItemBatchAsync(string indexName, IEnumerable<string> ids)
    {
        return await dataClient.DeleteItemBatchAsync(indexName, ids);
    }

    /// <inheritdoc />
    public async Task<VectorSearchResponse> SearchAsync(string indexName, IEnumerable<float> queryVector,
        int topK = 10, MetadataFields? metadataFields = null)
    {
        return await dataClient.SearchAsync(indexName, queryVector, topK, metadataFields);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        controlClient.Dispose();
    }
}