using System.Threading.Tasks;
    using Momento.Sdk.Responses.Vector;

namespace Momento.Sdk.Tests.Integration;

public class VectorIndexControlTest : IClassFixture<VectorIndexClientFixture>
{
    private readonly IPreviewVectorIndexClient vectorIndexClient;
    
    public VectorIndexControlTest(VectorIndexClientFixture vectorIndexFixture)
    {
        vectorIndexClient = vectorIndexFixture.Client;
    }

    [Fact]
    public async Task CreateListDelete_HappyPath()
    {
        var indexName = $"dotnet-integration-{Utils.NewGuidString()}";
        const int numDimensions = 3;

        try
        {
            var createResponse = await vectorIndexClient.CreateIndexAsync(indexName, numDimensions);
            Assert.True(createResponse is CreateVectorIndexResponse.Success, $"Unexpected response: {createResponse}");

            var listResponse = await vectorIndexClient.ListIndexesAsync();
            Assert.True(listResponse is ListVectorIndexesResponse.Success, $"Unexpected response: {listResponse}");
            var listOk = (ListVectorIndexesResponse.Success)listResponse;
            Assert.Contains(listOk.IndexNames, name => name == indexName);
        }
        finally
        {
            var deleteResponse = await vectorIndexClient.DeleteIndexesAsync(indexName);
            Assert.True(deleteResponse is DeleteVectorIndexResponse.Success, $"Unexpected response: {deleteResponse}");
        }
    }

    [Fact]
    public async Task CreateIndexAsync_AlreadyExistsError()
    {
        var indexName = $"index-{Utils.NewGuidString()}";
        const int numDimensions = 3;

        try
        {
            var createResponse = await vectorIndexClient.CreateIndexAsync(indexName, numDimensions);
            Assert.True(createResponse is CreateVectorIndexResponse.Success, $"Unexpected response: {createResponse}");

            var createAgainResponse = await vectorIndexClient.CreateIndexAsync(indexName, numDimensions);
            Assert.True(createAgainResponse is CreateVectorIndexResponse.AlreadyExists, $"Unexpected response: {createAgainResponse}");
        }
        finally
        {
            var deleteResponse = await vectorIndexClient.DeleteIndexesAsync(indexName);
            Assert.True(deleteResponse is DeleteVectorIndexResponse.Success, $"Unexpected response: {deleteResponse}");
        }
    }
    
    [Fact]
    public async Task CreateIndexAsync_InvalidIndexName()
    {
        var createResponse = await vectorIndexClient.CreateIndexAsync(null!, 3);
        Assert.True(createResponse is CreateVectorIndexResponse.Error, $"Unexpected response: {createResponse}");
        var createErr = (CreateVectorIndexResponse.Error)createResponse;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, createErr.InnerException.ErrorCode);
        
        createResponse = await vectorIndexClient.CreateIndexAsync("", 3);
        Assert.True(createResponse is CreateVectorIndexResponse.Error, $"Unexpected response: {createResponse}");
        createErr = (CreateVectorIndexResponse.Error)createResponse;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, createErr.InnerException.ErrorCode);
    }
    
    [Fact]
    public async Task CreateIndexAsync_InvalidNumDimensions()
    {
        var createResponse = await vectorIndexClient.CreateIndexAsync("index", 0);
        Assert.True(createResponse is CreateVectorIndexResponse.Error, $"Unexpected response: {createResponse}");
        var createErr = (CreateVectorIndexResponse.Error)createResponse;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, createErr.InnerException.ErrorCode);
    }

    [Fact]
    public async Task DeleteIndexAsync_DoesntExistError()
    {
        var indexName = $"index-{Utils.NewGuidString()}";
        var deleteResponse = await vectorIndexClient.DeleteIndexesAsync(indexName);
        Assert.True(deleteResponse is DeleteVectorIndexResponse.Error, $"Unexpected response: {deleteResponse}");
        var deleteErr = (DeleteVectorIndexResponse.Error)deleteResponse;
        Assert.Equal(MomentoErrorCode.NOT_FOUND_ERROR, deleteErr.InnerException.ErrorCode);
    }
}