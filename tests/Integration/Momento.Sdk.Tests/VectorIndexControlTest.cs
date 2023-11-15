using System.Linq;
using System.Threading.Tasks;
using Momento.Sdk.Responses.Vector;
using Momento.Sdk.Requests.Vector;
using System.Collections.Generic;

namespace Momento.Sdk.Tests.Integration;

public class VectorIndexControlTest : IClassFixture<VectorIndexClientFixture>
{
    private readonly IPreviewVectorIndexClient vectorIndexClient;

    public VectorIndexControlTest(VectorIndexClientFixture vectorIndexFixture)
    {
        vectorIndexClient = vectorIndexFixture.Client;
    }

    public static IEnumerable<object[]> CreateAndListIndexTestData
    {
        get
        {
            return new List<object[]>
            {
                new object[] { new IndexInfo(Utils.TestVectorIndexName(), 3, SimilarityMetric.CosineSimilarity) },
                new object[] { new IndexInfo(Utils.TestVectorIndexName(), 3, SimilarityMetric.InnerProduct) },
                new object[] { new IndexInfo(Utils.TestVectorIndexName(), 3, SimilarityMetric.EuclideanSimilarity) }
            };
        }
    }

    [Theory]
    [MemberData(nameof(CreateAndListIndexTestData))]
    public async Task CreateListDelete_HappyPath(IndexInfo indexInfo)
    {
        try
        {
            var createResponse = await vectorIndexClient.CreateIndexAsync(indexInfo.Name, indexInfo.NumDimensions, indexInfo.SimilarityMetric);
            Assert.True(createResponse is CreateIndexResponse.Success, $"Unexpected response: {createResponse}");

            var listResponse = await vectorIndexClient.ListIndexesAsync();
            Assert.True(listResponse is ListIndexesResponse.Success, $"Unexpected response: {listResponse}");
            var listOk = (ListIndexesResponse.Success)listResponse;
            Assert.Contains(indexInfo, listOk.Indexes);
        }
        finally
        {
            var deleteResponse = await vectorIndexClient.DeleteIndexAsync(indexInfo.Name);
            Assert.True(deleteResponse is DeleteIndexResponse.Success, $"Unexpected response: {deleteResponse}");
        }
    }

    [Fact]
    public async Task CreateIndexAsync_AlreadyExistsError()
    {
        var indexName = Utils.TestVectorIndexName();
        const int numDimensions = 3;

        var createResponse = await vectorIndexClient.CreateIndexAsync(indexName, numDimensions);
        Assert.True(createResponse is CreateIndexResponse.Success, $"Unexpected response: {createResponse}");

        var createAgainResponse = await vectorIndexClient.CreateIndexAsync(indexName, numDimensions);
        Assert.True(createAgainResponse is CreateIndexResponse.AlreadyExists, $"Unexpected response: {createAgainResponse}");
    }

    [Fact]
    public async Task CreateIndexAsync_InvalidIndexName()
    {
        var createResponse = await vectorIndexClient.CreateIndexAsync(null!, 3);
        Assert.True(createResponse is CreateIndexResponse.Error, $"Unexpected response: {createResponse}");
        var createErr = (CreateIndexResponse.Error)createResponse;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, createErr.InnerException.ErrorCode);

        createResponse = await vectorIndexClient.CreateIndexAsync("", 3);
        Assert.True(createResponse is CreateIndexResponse.Error, $"Unexpected response: {createResponse}");
        createErr = (CreateIndexResponse.Error)createResponse;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, createErr.InnerException.ErrorCode);
    }

    [Fact]
    public async Task CreateIndexAsync_InvalidNumDimensions()
    {
        var createResponse = await vectorIndexClient.CreateIndexAsync("index", 0);
        Assert.True(createResponse is CreateIndexResponse.Error, $"Unexpected response: {createResponse}");
        var createErr = (CreateIndexResponse.Error)createResponse;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, createErr.InnerException.ErrorCode);
    }

    [Fact]
    public async Task DeleteIndexAsync_DoesntExistError()
    {
        var indexName = Utils.TestVectorIndexName();
        var deleteResponse = await vectorIndexClient.DeleteIndexAsync(indexName);
        Assert.True(deleteResponse is DeleteIndexResponse.Error, $"Unexpected response: {deleteResponse}");
        var deleteErr = (DeleteIndexResponse.Error)deleteResponse;
        Assert.Equal(MomentoErrorCode.NOT_FOUND_ERROR, deleteErr.InnerException.ErrorCode);
    }
}
