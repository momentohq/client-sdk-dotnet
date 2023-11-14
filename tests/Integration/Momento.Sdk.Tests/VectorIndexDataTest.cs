using System.Collections.Generic;
using System.Linq;
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
        var response = await vectorIndexClient.UpsertItemBatchAsync(null!, new List<Item>());
        Assert.True(response is UpsertItemBatchResponse.Error, $"Unexpected response: {response}");
        var error = (UpsertItemBatchResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, error.InnerException.ErrorCode);

        response = await vectorIndexClient.UpsertItemBatchAsync("", new List<Item>());
        Assert.True(response is UpsertItemBatchResponse.Error, $"Unexpected response: {response}");
        error = (UpsertItemBatchResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, error.InnerException.ErrorCode);
    }

    [Fact]
    public async Task DeleteItemBatchAsync_InvalidIndexName()
    {
        var response = await vectorIndexClient.DeleteItemBatchAsync(null!, new List<string>());
        Assert.True(response is DeleteItemBatchResponse.Error, $"Unexpected response: {response}");
        var error = (DeleteItemBatchResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, error.InnerException.ErrorCode);

        response = await vectorIndexClient.DeleteItemBatchAsync("", new List<string>());
        Assert.True(response is DeleteItemBatchResponse.Error, $"Unexpected response: {response}");
        error = (DeleteItemBatchResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, error.InnerException.ErrorCode);
    }

    [Fact]
    public async Task SearchAsync_InvalidIndexName()
    {
        var response = await vectorIndexClient.SearchAsync(null!, new List<float> { 1.0f });
        Assert.True(response is SearchResponse.Error, $"Unexpected response: {response}");
        var error = (SearchResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, error.InnerException.ErrorCode);

        response = await vectorIndexClient.SearchAsync("", new List<float> { 1.0f });
        Assert.True(response is SearchResponse.Error, $"Unexpected response: {response}");
        error = (SearchResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, error.InnerException.ErrorCode);
    }

    public delegate Task<T> SearchDelegate<T>(IPreviewVectorIndexClient client, string indexName,
        IEnumerable<float> queryVector, int topK = 10,
        MetadataFields? metadataFields = null, float? scoreThreshold = null);

    public delegate void AssertOnSearchResponse<in T>(T response, List<SearchHit> expectedHits,
        List<List<float>> expectedVectors);

    public static IEnumerable<object[]> UpsertAndSearchTestData
    {
        get
        {
            return new List<object[]>
            {
                new object[]
                {
                    new SearchDelegate<SearchResponse>(
                        (client, indexName, queryVector, topK, metadata, scoreThreshold) =>
                            client.SearchAsync(indexName, queryVector, topK, metadata, scoreThreshold)),
                    new AssertOnSearchResponse<SearchResponse>((response, expectedHits, _) =>
                    {
                        Assert.True(response is SearchResponse.Success, $"Unexpected response: {response}");
                        var successResponse = (SearchResponse.Success)response;
                        Assert.Equal(expectedHits, successResponse.Hits);
                    })
                },
                new object[]
                {
                    new SearchDelegate<SearchAndFetchVectorsResponse>(
                        (client, indexName, queryVector, topK, metadata, scoreThreshold) =>
                            client.SearchAndFetchVectorsAsync(indexName, queryVector, topK, metadata,
                                scoreThreshold)),
                    new AssertOnSearchResponse<SearchAndFetchVectorsResponse>(
                        (response, expectedHits, expectedVectors) =>
                        {
                            Assert.True(response is SearchAndFetchVectorsResponse.Success,
                                $"Unexpected response: {response}");
                            var successResponse = (SearchAndFetchVectorsResponse.Success)response;
                            var expectedHitsAndVectors = expectedHits.Zip(expectedVectors,
                                (h, v) => new SearchAndFetchVectorsHit(h, v));
                            Assert.Equal(expectedHitsAndVectors, successResponse.Hits);
                        })
                }
            };
        }
    }

    [Theory]
    [MemberData(nameof(UpsertAndSearchTestData))]
    public async Task UpsertAndSearch_InnerProduct<T>(SearchDelegate<T> searchDelegate,
        AssertOnSearchResponse<T> assertOnSearchResponse)
    {
        var indexName = $"index-{Utils.NewGuidString()}";

        var createResponse = await vectorIndexClient.CreateIndexAsync(indexName, 2, SimilarityMetric.InnerProduct);
        Assert.True(createResponse is CreateIndexResponse.Success, $"Unexpected response: {createResponse}");

        try
        {
            var items = new List<Item>
            {
                new("test_item", new List<float> { 1.0f, 2.0f })
            };

            var upsertResponse = await vectorIndexClient.UpsertItemBatchAsync(indexName, items);
            Assert.True(upsertResponse is UpsertItemBatchResponse.Success,
                $"Unexpected response: {upsertResponse}");

            await Task.Delay(2_000);

            var searchResponse =
                await searchDelegate.Invoke(vectorIndexClient, indexName, new List<float> { 1.0f, 2.0f });
            assertOnSearchResponse.Invoke(searchResponse, new List<SearchHit>
            {
                new("test_item", 5.0f)
            }, items.Select(i => i.Vector).ToList());
        }
        finally
        {
            await vectorIndexClient.DeleteIndexAsync(indexName);
        }
    }

    [Theory]
    [MemberData(nameof(UpsertAndSearchTestData))]
    public async Task UpsertAndSearch_CosineSimilarity<T>(SearchDelegate<T> searchDelegate,
        AssertOnSearchResponse<T> assertOnSearchResponse)
    {
        var indexName = $"index-{Utils.NewGuidString()}";

        var createResponse = await vectorIndexClient.CreateIndexAsync(indexName, 2);
        Assert.True(createResponse is CreateIndexResponse.Success, $"Unexpected response: {createResponse}");

        try
        {
            var items = new List<Item>
            {
                new("test_item_1", new List<float> { 1.0f, 1.0f }),
                new("test_item_2", new List<float> { -1.0f, 1.0f }),
                new("test_item_3", new List<float> { -1.0f, -1.0f })
            };
            var upsertResponse = await vectorIndexClient.UpsertItemBatchAsync(indexName, items);
            Assert.True(upsertResponse is UpsertItemBatchResponse.Success,
                $"Unexpected response: {upsertResponse}");

            await Task.Delay(2_000);

            var searchResponse =
                await searchDelegate.Invoke(vectorIndexClient, indexName, new List<float> { 2.0f, 2.0f });
            assertOnSearchResponse.Invoke(searchResponse, new List<SearchHit>
            {
                new("test_item_1", 1.0f),
                new("test_item_2", 0.0f),
                new("test_item_3", -1.0f)
            }, items.Select(i => i.Vector).ToList());
        }
        finally
        {
            await vectorIndexClient.DeleteIndexAsync(indexName);
        }
    }

    [Theory]
    [MemberData(nameof(UpsertAndSearchTestData))]
    public async Task UpsertAndSearch_EuclideanSimilarity<T>(SearchDelegate<T> searchDelegate,
        AssertOnSearchResponse<T> assertOnSearchResponse)
    {
        var indexName = $"index-{Utils.NewGuidString()}";

        var createResponse =
            await vectorIndexClient.CreateIndexAsync(indexName, 2, SimilarityMetric.EuclideanSimilarity);
        Assert.True(createResponse is CreateIndexResponse.Success, $"Unexpected response: {createResponse}");

        try
        {
            var items = new List<Item>
            {
                new("test_item_1", new List<float> { 1.0f, 1.0f }),
                new("test_item_2", new List<float> { -1.0f, 1.0f }),
                new("test_item_3", new List<float> { -1.0f, -1.0f })
            };
            var upsertResponse = await vectorIndexClient.UpsertItemBatchAsync(indexName, items);
            Assert.True(upsertResponse is UpsertItemBatchResponse.Success,
                $"Unexpected response: {upsertResponse}");

            await Task.Delay(2_000);

            var searchResponse =
                await searchDelegate.Invoke(vectorIndexClient, indexName, new List<float> { 1.0f, 1.0f });
            assertOnSearchResponse.Invoke(searchResponse, new List<SearchHit>
            {
                new("test_item_1", 0.0f),
                new("test_item_2", 4.0f),
                new("test_item_3", 8.0f)
            }, items.Select(i => i.Vector).ToList());
        }
        finally
        {
            await vectorIndexClient.DeleteIndexAsync(indexName);
        }
    }

    [Theory]
    [MemberData(nameof(UpsertAndSearchTestData))]
    public async Task UpsertAndSearch_TopKLimit<T>(SearchDelegate<T> searchDelegate,
        AssertOnSearchResponse<T> assertOnSearchResponse)
    {
        var indexName = $"index-{Utils.NewGuidString()}";

        var createResponse = await vectorIndexClient.CreateIndexAsync(indexName, 2, SimilarityMetric.InnerProduct);
        Assert.True(createResponse is CreateIndexResponse.Success, $"Unexpected response: {createResponse}");

        try
        {
            var items = new List<Item>
            {
                new("test_item_1", new List<float> { 1.0f, 2.0f }),
                new("test_item_2", new List<float> { 3.0f, 4.0f }),
                new("test_item_3", new List<float> { 5.0f, 6.0f })
            };
            var upsertResponse = await vectorIndexClient.UpsertItemBatchAsync(indexName, items);
            Assert.True(upsertResponse is UpsertItemBatchResponse.Success,
                $"Unexpected response: {upsertResponse}");

            await Task.Delay(2_000);

            var searchResponse =
                await searchDelegate.Invoke(vectorIndexClient, indexName, new List<float> { 1.0f, 2.0f }, topK: 2);
            assertOnSearchResponse.Invoke(searchResponse, new List<SearchHit>
            {
                new("test_item_3", 17.0f),
                new("test_item_2", 11.0f)
            }, new List<List<float>>
            {
                new() { 5.0f, 6.0f },
                new() { 3.0f, 4.0f }
            });
        }
        finally
        {
            await vectorIndexClient.DeleteIndexAsync(indexName);
        }
    }

    [Theory]
    [MemberData(nameof(UpsertAndSearchTestData))]
    public async Task UpsertAndSearch_WithMetadata<T>(SearchDelegate<T> searchDelegate,
        AssertOnSearchResponse<T> assertOnSearchResponse)
    {
        var indexName = $"index-{Utils.NewGuidString()}";

        var createResponse = await vectorIndexClient.CreateIndexAsync(indexName, 2, SimilarityMetric.InnerProduct);
        Assert.True(createResponse is CreateIndexResponse.Success, $"Unexpected response: {createResponse}");

        try
        {
            var items = new List<Item>
            {
                new("test_item_1", new List<float> { 1.0f, 2.0f },
                    new Dictionary<string, MetadataValue> { { "key1", "value1" } }),
                new("test_item_2", new List<float> { 3.0f, 4.0f },
                    new Dictionary<string, MetadataValue> { { "key2", "value2" } }),
                new("test_item_3", new List<float> { 5.0f, 6.0f },
                    new Dictionary<string, MetadataValue>
                        { { "key1", "value3" }, { "key3", "value3" } })
            };
            var upsertResponse = await vectorIndexClient.UpsertItemBatchAsync(indexName, items);
            Assert.True(upsertResponse is UpsertItemBatchResponse.Success,
                $"Unexpected response: {upsertResponse}");

            await Task.Delay(2_000);

            var expectedVectors = new List<List<float>>
            {
                new() { 5.0f, 6.0f },
                new() { 3.0f, 4.0f },
                new() { 1.0f, 2.0f }
            };
            var searchResponse =
                await searchDelegate.Invoke(vectorIndexClient, indexName, new List<float> { 1.0f, 2.0f }, 3,
                    new List<string> { "key1" });
            assertOnSearchResponse.Invoke(searchResponse, new List<SearchHit>
            {
                new("test_item_3", 17.0f,
                    new Dictionary<string, MetadataValue> { { "key1", "value3" } }),
                new("test_item_2", 11.0f, new Dictionary<string, MetadataValue>()),
                new("test_item_1", 5.0f,
                    new Dictionary<string, MetadataValue> { { "key1", "value1" } })
            }, expectedVectors);

            searchResponse =
                await searchDelegate.Invoke(vectorIndexClient, indexName, new List<float> { 1.0f, 2.0f }, 3,
                    new List<string> { "key1", "key2", "key3", "key4" });
            assertOnSearchResponse.Invoke(searchResponse, new List<SearchHit>
            {
                new("test_item_3", 17.0f,
                    new Dictionary<string, MetadataValue>
                        { { "key1", "value3" }, { "key3", "value3" } }),
                new("test_item_2", 11.0f,
                    new Dictionary<string, MetadataValue> { { "key2", "value2" } }),
                new("test_item_1", 5.0f,
                    new Dictionary<string, MetadataValue> { { "key1", "value1" } })
            }, expectedVectors);

            searchResponse =
                await searchDelegate.Invoke(vectorIndexClient, indexName, new List<float> { 1.0f, 2.0f }, 3,
                    MetadataFields.All);
            assertOnSearchResponse.Invoke(searchResponse, new List<SearchHit>
            {
                new("test_item_3", 17.0f,
                    new Dictionary<string, MetadataValue>
                        { { "key1", "value3" }, { "key3", "value3" } }),
                new("test_item_2", 11.0f,
                    new Dictionary<string, MetadataValue> { { "key2", "value2" } }),
                new("test_item_1", 5.0f,
                    new Dictionary<string, MetadataValue> { { "key1", "value1" } })
            }, expectedVectors);
        }
        finally
        {
            await vectorIndexClient.DeleteIndexAsync(indexName);
        }
    }

    [Theory]
    [MemberData(nameof(UpsertAndSearchTestData))]
    public async Task UpsertAndSearch_WithDiverseMetadata<T>(SearchDelegate<T> searchDelegate,
        AssertOnSearchResponse<T> assertOnSearchResponse)
    {
        var indexName = $"index-{Utils.NewGuidString()}";

        var createResponse = await vectorIndexClient.CreateIndexAsync(indexName, 2, SimilarityMetric.InnerProduct);
        Assert.True(createResponse is CreateIndexResponse.Success, $"Unexpected response: {createResponse}");

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
            var items = new List<Item>
            {
                new("test_item_1", new List<float> { 1.0f, 2.0f }, metadata)
            };

            var upsertResponse = await vectorIndexClient.UpsertItemBatchAsync(indexName, items);
            Assert.True(upsertResponse is UpsertItemBatchResponse.Success,
                $"Unexpected response: {upsertResponse}");

            await Task.Delay(2_000);

            var searchResponse =
                await searchDelegate.Invoke(vectorIndexClient, indexName, new List<float> { 1.0f, 2.0f }, 1,
                    MetadataFields.All);
            assertOnSearchResponse.Invoke(searchResponse, new List<SearchHit>
            {
                new("test_item_1", 5.0f, metadata)
            }, items.Select(i => i.Vector).ToList());
        }
        finally
        {
            await vectorIndexClient.DeleteIndexAsync(indexName);
        }
    }

    public static IEnumerable<object[]> SearchThresholdTestCases =>
        new List<object[]>
        {
            // similarity metric, scores, thresholds
            new object[]
            {
                SimilarityMetric.CosineSimilarity,
                new List<float> { 1.0f, 0.0f, -1.0f },
                new List<float> { 0.5f, -1.01f, 1.0f }
            },
            new object[]
            {
                SimilarityMetric.InnerProduct,
                new List<float> { 4.0f, 0.0f, -4.0f },
                new List<float> { 0.0f, -4.01f, 4.0f }
            },
            new object[]
            {
                SimilarityMetric.EuclideanSimilarity,
                new List<float> { 2.0f, 10.0f, 18.0f },
                new List<float> { 3.0f, 20.0f, -0.01f }
            }
        };
    
    // Combine the search threshold parameters and the search/search with vectors parameters
    public static IEnumerable<object[]> UpsertAndSearchThresholdTestCases =>
        SearchThresholdTestCases.SelectMany(
            _ => UpsertAndSearchTestData,
            (firstArray, secondArray) => firstArray.Concat(secondArray).ToArray());

    [Theory]
    [MemberData(nameof(UpsertAndSearchThresholdTestCases))]
    public async Task Search_PruneBasedOnThreshold<T>(SimilarityMetric similarityMetric, List<float> scores,
        List<float> thresholds, SearchDelegate<T> searchDelegate, AssertOnSearchResponse<T> assertOnSearchResponse)
    {
        var indexName = $"index-{Utils.NewGuidString()}";

        var createResponse = await vectorIndexClient.CreateIndexAsync(indexName, 2, similarityMetric);
        Assert.True(createResponse is CreateIndexResponse.Success, $"Unexpected response: {createResponse}");

        try
        {
            var items = new List<Item>
            {
                new("test_item_1", new List<float> { 1.0f, 1.0f }),
                new("test_item_2", new List<float> { -1.0f, 1.0f }),
                new("test_item_3", new List<float> { -1.0f, -1.0f })
            };
            var upsertResponse = await vectorIndexClient.UpsertItemBatchAsync(indexName, items);
            Assert.True(upsertResponse is UpsertItemBatchResponse.Success,
                $"Unexpected response: {upsertResponse}");

            await Task.Delay(2_000);

            var queryVector = new List<float> { 2.0f, 2.0f };
            var searchHits = new List<SearchHit>
            {
                new("test_item_1", scores[0]),
                new("test_item_2", scores[1]),
                new("test_item_3", scores[2])
            };

            // Test threshold to get only the top result
            var searchResponse =
                await searchDelegate.Invoke(vectorIndexClient, indexName, queryVector, 3, scoreThreshold: thresholds[0]);
            assertOnSearchResponse.Invoke(searchResponse, new List<SearchHit>
            {
                searchHits[0]
            }, items.FindAll(i => i.Id == "test_item_1").Select(i => i.Vector).ToList());

            // Test threshold to get all results
            searchResponse =
                await searchDelegate.Invoke(vectorIndexClient, indexName, queryVector, 3, scoreThreshold: thresholds[1]);
            assertOnSearchResponse.Invoke(searchResponse, searchHits, items.Select(i => i.Vector).ToList());

            // Test threshold to get no results
            searchResponse =
                await searchDelegate.Invoke(vectorIndexClient, indexName, queryVector, 3, scoreThreshold: thresholds[2]);
            assertOnSearchResponse.Invoke(searchResponse, new List<SearchHit>(), new List<List<float>>());
        }
        finally
        {
            await vectorIndexClient.DeleteIndexAsync(indexName);
        }
    }
}