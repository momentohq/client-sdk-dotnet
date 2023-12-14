using System;
using Momento.Sdk.Requests.Vector;
using Momento.Sdk.Responses.Vector;
namespace Momento.Sdk.Tests.Integration;

/// <summary>
/// Ad-hoc utility methods.
/// </summary>
public static class Utils
{

    public static string TestCacheName() => "dotnet-integration-" + NewGuidString();

    /// <summary>
    /// Returns a test vector index name that is unique to this test run.
    /// </summary>
    /// <remarks>
    /// This is useful for debugging leaking test vector indexes.
    /// </remarks>
    /// <param name="meaningfulIdentifier">A string that uniquely identifies the test, e.g. "test1".</param>
    /// <returns>A test vector index name that is unique to this test run.</returns>
    public static string TestVectorIndexName(string meaningfulIdentifier)
    {
        return $"dotnet-integration-{NewGuidString()}-{meaningfulIdentifier}";
    }

    public static string NewGuidString() => Guid.NewGuid().ToString();

    public static byte[] NewGuidByteArray() => Guid.NewGuid().ToByteArray();

    public static int InitialRefreshTtl { get; } = 4;

    public static int UpdatedRefreshTtl { get; } = 10;

    public static int WaitForItemToBeSet { get; } = 100;

    public static int WaitForInitialItemToExpire { get; } = 4900;

    public static void CreateCacheForTest(ICacheClient cacheClient, string cacheName)
    {
        var result = cacheClient.CreateCacheAsync(cacheName).Result;
        if (result is not (CreateCacheResponse.Success or CreateCacheResponse.CacheAlreadyExists))
        {
            throw new Exception($"Error when creating cache: {result}");
        }
    }

    public static _WithVectorIndex WithVectorIndex(IPreviewVectorIndexClient vectorIndexClient, IndexInfo indexInfo)
    {
        return new _WithVectorIndex(vectorIndexClient, indexInfo);
    }

    public static _WithVectorIndex WithVectorIndex(IPreviewVectorIndexClient vectorIndexClient, string indexName, int numDimensions, SimilarityMetric similarityMetric = SimilarityMetric.CosineSimilarity)
    {
        return WithVectorIndex(vectorIndexClient, new IndexInfo(indexName, numDimensions, similarityMetric));
    }

    public class _WithVectorIndex : IDisposable
    {
        public IPreviewVectorIndexClient VectorIndexClient { get; }

        public IndexInfo IndexInfo { get; }

        public _WithVectorIndex(IPreviewVectorIndexClient vectorIndexClient, IndexInfo indexInfo)
        {
            VectorIndexClient = vectorIndexClient;
            IndexInfo = indexInfo;

            // This usually isn't kosher in a constructor; because we want this class to encapsulate
            // index creation and deletion in a using block, the class has to do RAII.
            var createResponse = vectorIndexClient.CreateIndexAsync(indexInfo.Name, indexInfo.NumDimensions, indexInfo.SimilarityMetric).Result;
            if (createResponse is not (CreateIndexResponse.Success or CreateIndexResponse.AlreadyExists))
            {
                throw new Exception($"Error when creating index: {createResponse}");
            }
        }

        public _WithVectorIndex(IPreviewVectorIndexClient vectorIndexClient, string indexName, int numDimensions, SimilarityMetric similarityMetric = SimilarityMetric.CosineSimilarity)
            : this(vectorIndexClient, new IndexInfo(indexName, numDimensions, similarityMetric))
        {
        }

        public void Dispose()
        {
            var deleteResponse = VectorIndexClient.DeleteIndexAsync(IndexInfo.Name).Result;
            if (deleteResponse is not DeleteIndexResponse.Success)
            {
                throw new Exception($"Error when deleting index: {deleteResponse}");
            }
        }
    }
}
