using System.Collections.Generic;
using System.Threading.Tasks;
using Momento.Sdk.Messages.Vector;
using Momento.Sdk.Requests.Vector;
using Momento.Sdk.Responses.Vector;

namespace Momento.Sdk.Tests.Integration;

public class VectorIndexDataTest : IClassFixture<VectorIndexClientFixture>
{
    private readonly IPreviewVectorIndexClient vectorIndexClient;

    public VectorIndexDataTest(VectorIndexClientFixture vectorIndexFixture)
    {
        vectorIndexClient = vectorIndexFixture.Client;
    }

    [Fact]
    public async Task UpsertItemBatchAsync_InvalidIndexName()
    {
        var response = await vectorIndexClient.UpsertItemBatchAsync(null!, new List<VectorIndexItem>());
        Assert.True(response is VectorUpsertItemBatchResponse.Error, $"Unexpected response: {response}");
        var error = (VectorUpsertItemBatchResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, error.InnerException.ErrorCode);

        response = await vectorIndexClient.UpsertItemBatchAsync("", new List<VectorIndexItem>());
        Assert.True(response is VectorUpsertItemBatchResponse.Error, $"Unexpected response: {response}");
        error = (VectorUpsertItemBatchResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, error.InnerException.ErrorCode);
    }

    [Fact]
    public async Task DeleteItemBatchAsync_InvalidIndexName()
    {
        var response = await vectorIndexClient.DeleteItemBatchAsync(null!, new List<string>());
        Assert.True(response is VectorDeleteItemBatchResponse.Error, $"Unexpected response: {response}");
        var error = (VectorDeleteItemBatchResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, error.InnerException.ErrorCode);

        response = await vectorIndexClient.DeleteItemBatchAsync("", new List<string>());
        Assert.True(response is VectorDeleteItemBatchResponse.Error, $"Unexpected response: {response}");
        error = (VectorDeleteItemBatchResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, error.InnerException.ErrorCode);
    }

    [Fact]
    public async Task SearchAsync_InvalidIndexName()
    {
        var response = await vectorIndexClient.SearchAsync(null!, new List<float> { 1.0f });
        Assert.True(response is VectorSearchResponse.Error, $"Unexpected response: {response}");
        var error = (VectorSearchResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, error.InnerException.ErrorCode);

        response = await vectorIndexClient.SearchAsync("", new List<float> { 1.0f });
        Assert.True(response is VectorSearchResponse.Error, $"Unexpected response: {response}");
        error = (VectorSearchResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, error.InnerException.ErrorCode);
    }

    [Fact]
    public async Task UpsertAndSearch_InnerProduct()
    {
        var indexName = $"index-{Utils.NewGuidString()}";

        var createResponse = await vectorIndexClient.CreateIndexAsync(indexName, 2, SimilarityMetric.InnerProduct);
        Assert.True(createResponse is CreateVectorIndexResponse.Success, $"Unexpected response: {createResponse}");

        try
        {
            var upsertResponse = await vectorIndexClient.UpsertItemBatchAsync(indexName, new List<VectorIndexItem>
            {
                new("test_item", new List<float> { 1.0f, 2.0f })
            });
            Assert.True(upsertResponse is VectorUpsertItemBatchResponse.Success,
                $"Unexpected response: {upsertResponse}");

            await Task.Delay(2_000);

            var searchResponse = await vectorIndexClient.SearchAsync(indexName, new List<float> { 1.0f, 2.0f });
            Assert.True(searchResponse is VectorSearchResponse.Success, $"Unexpected response: {searchResponse}");
            var successResponse = (VectorSearchResponse.Success)searchResponse;
            Assert.Equal(new List<SearchHit>
            {
                new("test_item", 5.0f)
            }, successResponse.Hits);
        }
        finally
        {
            await vectorIndexClient.DeleteIndexesAsync(indexName);
        }
    }

    [Fact]
    public async Task UpsertAndSearch_CosineSimilarity()
    {
        var indexName = $"index-{Utils.NewGuidString()}";

        var createResponse = await vectorIndexClient.CreateIndexAsync(indexName, 2);
        Assert.True(createResponse is CreateVectorIndexResponse.Success, $"Unexpected response: {createResponse}");

        try
        {
            var upsertResponse = await vectorIndexClient.UpsertItemBatchAsync(indexName, new List<VectorIndexItem>
            {
                new("test_item_1", new List<float> { 1.0f, 1.0f }),
                new("test_item_2", new List<float> { -1.0f, 1.0f }),
                new("test_item_3", new List<float> { -1.0f, -1.0f })
            });
            Assert.True(upsertResponse is VectorUpsertItemBatchResponse.Success,
                $"Unexpected response: {upsertResponse}");

            await Task.Delay(2_000);

            var searchResponse = await vectorIndexClient.SearchAsync(indexName, new List<float> { 2.0f, 2.0f });
            Assert.True(searchResponse is VectorSearchResponse.Success, $"Unexpected response: {searchResponse}");
            var successResponse = (VectorSearchResponse.Success)searchResponse;
            Assert.Equal(new List<SearchHit>
            {
                new("test_item_1", 1.0f),
                new("test_item_2", 0.0f),
                new("test_item_3", -1.0f)
            }, successResponse.Hits);
        }
        finally
        {
            await vectorIndexClient.DeleteIndexesAsync(indexName);
        }
    }

    [Fact]
    public async Task UpsertAndSearch_EuclideanSimilarity()
    {
        var indexName = $"index-{Utils.NewGuidString()}";

        var createResponse =
            await vectorIndexClient.CreateIndexAsync(indexName, 2, SimilarityMetric.EuclideanSimilarity);
        Assert.True(createResponse is CreateVectorIndexResponse.Success, $"Unexpected response: {createResponse}");

        try
        {
            var upsertResponse = await vectorIndexClient.UpsertItemBatchAsync(indexName, new List<VectorIndexItem>
            {
                new("test_item_1", new List<float> { 1.0f, 1.0f }),
                new("test_item_2", new List<float> { -1.0f, 1.0f }),
                new("test_item_3", new List<float> { -1.0f, -1.0f })
            });
            Assert.True(upsertResponse is VectorUpsertItemBatchResponse.Success,
                $"Unexpected response: {upsertResponse}");

            await Task.Delay(2_000);

            var searchResponse = await vectorIndexClient.SearchAsync(indexName, new List<float> { 1.0f, 1.0f });
            Assert.True(searchResponse is VectorSearchResponse.Success, $"Unexpected response: {searchResponse}");
            var successResponse = (VectorSearchResponse.Success)searchResponse;
            Assert.Equal(new List<SearchHit>
            {
                new("test_item_1", 0.0f),
                new("test_item_2", 4.0f),
                new("test_item_3", 8.0f)
            }, successResponse.Hits);
        }
        finally
        {
            await vectorIndexClient.DeleteIndexesAsync(indexName);
        }
    }

    [Fact]
    public async Task UpsertAndSearch_TopKLimit()
    {
        var indexName = $"index-{Utils.NewGuidString()}";

        var createResponse = await vectorIndexClient.CreateIndexAsync(indexName, 2, SimilarityMetric.InnerProduct);
        Assert.True(createResponse is CreateVectorIndexResponse.Success, $"Unexpected response: {createResponse}");

        try
        {
            var upsertResponse = await vectorIndexClient.UpsertItemBatchAsync(indexName, new List<VectorIndexItem>
            {
                new("test_item_1", new List<float> { 1.0f, 2.0f }),
                new("test_item_2", new List<float> { 3.0f, 4.0f }),
                new("test_item_3", new List<float> { 5.0f, 6.0f })
            });
            Assert.True(upsertResponse is VectorUpsertItemBatchResponse.Success,
                $"Unexpected response: {upsertResponse}");

            await Task.Delay(2_000);

            var searchResponse = await vectorIndexClient.SearchAsync(indexName, new List<float> { 1.0f, 2.0f }, 2);
            Assert.True(searchResponse is VectorSearchResponse.Success, $"Unexpected response: {searchResponse}");
            var successResponse = (VectorSearchResponse.Success)searchResponse;
            Assert.Equal(new List<SearchHit>
            {
                new("test_item_3", 17.0f),
                new("test_item_2", 11.0f)
            }, successResponse.Hits);
        }
        finally
        {
            await vectorIndexClient.DeleteIndexesAsync(indexName);
        }
    }

    [Fact]
    public async Task UpsertAndSearch_WithMetadata()
    {
        var indexName = $"index-{Utils.NewGuidString()}";

        var createResponse = await vectorIndexClient.CreateIndexAsync(indexName, 2, SimilarityMetric.InnerProduct);
        Assert.True(createResponse is CreateVectorIndexResponse.Success, $"Unexpected response: {createResponse}");

        try
        {
            var upsertResponse = await vectorIndexClient.UpsertItemBatchAsync(indexName, new List<VectorIndexItem>
            {
                new("test_item_1", new List<float> { 1.0f, 2.0f },
                    new Dictionary<string, MetadataValue> { { "key1", "value1" } }),
                new("test_item_2", new List<float> { 3.0f, 4.0f },
                    new Dictionary<string, MetadataValue> { { "key2", "value2" } }),
                new("test_item_3", new List<float> { 5.0f, 6.0f },
                    new Dictionary<string, MetadataValue>
                        { { "key1", "value3" }, { "key3", "value3" } })
            });
            Assert.True(upsertResponse is VectorUpsertItemBatchResponse.Success,
                $"Unexpected response: {upsertResponse}");

            await Task.Delay(2_000);

            var searchResponse = await vectorIndexClient.SearchAsync(indexName, new List<float> { 1.0f, 2.0f }, 3,
                new List<string> { "key1" });
            Assert.True(searchResponse is VectorSearchResponse.Success, $"Unexpected response: {searchResponse}");
            var successResponse = (VectorSearchResponse.Success)searchResponse;
            Assert.Equal(new List<SearchHit>
            {
                new("test_item_3", 17.0f,
                    new Dictionary<string, MetadataValue> { { "key1", "value3" } }),
                new("test_item_2", 11.0f, new Dictionary<string, MetadataValue>()),
                new("test_item_1", 5.0f,
                    new Dictionary<string, MetadataValue> { { "key1", "value1" } })
            }, successResponse.Hits);

            searchResponse = await vectorIndexClient.SearchAsync(indexName, new List<float> { 1.0f, 2.0f }, 3,
                new List<string> { "key1", "key2", "key3", "key4" });
            Assert.True(searchResponse is VectorSearchResponse.Success, $"Unexpected response: {searchResponse}");
            successResponse = (VectorSearchResponse.Success)searchResponse;
            Assert.Equal(new List<SearchHit>
            {
                new("test_item_3", 17.0f,
                    new Dictionary<string, MetadataValue>
                        { { "key1", "value3" }, { "key3", "value3" } }),
                new("test_item_2", 11.0f,
                    new Dictionary<string, MetadataValue> { { "key2", "value2" } }),
                new("test_item_1", 5.0f,
                    new Dictionary<string, MetadataValue> { { "key1", "value1" } })
            }, successResponse.Hits);

            searchResponse =
                await vectorIndexClient.SearchAsync(indexName, new List<float> { 1.0f, 2.0f }, 3, MetadataFields.All);
            Assert.True(searchResponse is VectorSearchResponse.Success, $"Unexpected response: {searchResponse}");
            successResponse = (VectorSearchResponse.Success)searchResponse;
            Assert.Equal(new List<SearchHit>
            {
                new("test_item_3", 17.0f,
                    new Dictionary<string, MetadataValue>
                        { { "key1", "value3" }, { "key3", "value3" } }),
                new("test_item_2", 11.0f,
                    new Dictionary<string, MetadataValue> { { "key2", "value2" } }),
                new("test_item_1", 5.0f,
                    new Dictionary<string, MetadataValue> { { "key1", "value1" } })
            }, successResponse.Hits);
        }
        finally
        {
            await vectorIndexClient.DeleteIndexesAsync(indexName);
        }
    }

    [Fact]
    public async Task UpsertAndSearch_WithDiverseMetadata()
    {
        var indexName = $"index-{Utils.NewGuidString()}";

        var createResponse = await vectorIndexClient.CreateIndexAsync(indexName, 2, SimilarityMetric.InnerProduct);
        Assert.True(createResponse is CreateVectorIndexResponse.Success, $"Unexpected response: {createResponse}");

        try
        {
            var metadata = new Dictionary<string, MetadataValue>
            {
                { "string_key", "string_value" },
                { "long_key", 123 },
                { "double_key", 3.14 },
                { "bool_key", true },
                { "list_key", new List<string> { "a", "b", "c" } },
                { "empty_list_key", new List<string>() }
            };
            var upsertResponse = await vectorIndexClient.UpsertItemBatchAsync(indexName, new List<VectorIndexItem>
            {
                new("test_item_1", new List<float> { 1.0f, 2.0f }, metadata)
            });
            Assert.True(upsertResponse is VectorUpsertItemBatchResponse.Success,
                $"Unexpected response: {upsertResponse}");

            await Task.Delay(2_000);

            var searchResponse =
                await vectorIndexClient.SearchAsync(indexName, new List<float> { 1.0f, 2.0f }, 1, MetadataFields.All);
            Assert.True(searchResponse is VectorSearchResponse.Success, $"Unexpected response: {searchResponse}");
            var successResponse = (VectorSearchResponse.Success)searchResponse;
            Assert.Equal(new List<SearchHit>
            {
                new("test_item_1", 5.0f, metadata)
            }, successResponse.Hits);
        }
        finally
        {
            await vectorIndexClient.DeleteIndexesAsync(indexName);
        }
    }

    [Fact]
    public async Task TempDeleteAllIndexes()
    {
        var listResponse = await vectorIndexClient.ListIndexesAsync();
        Assert.True(listResponse is ListVectorIndexesResponse.Success, $"Unexpected response: {listResponse}");
        var listOk = (ListVectorIndexesResponse.Success)listResponse;
        foreach (var indexName in listOk.IndexNames)
        {
            var deleteResponse = await vectorIndexClient.DeleteIndexesAsync(indexName);
            Assert.True(deleteResponse is DeleteVectorIndexResponse.Success, $"Unexpected response: {deleteResponse}");
        }
    }
}