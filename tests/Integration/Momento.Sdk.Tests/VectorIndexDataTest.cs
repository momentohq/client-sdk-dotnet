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
        var indexName = Utils.TestVectorIndexName("data-upsert-and-search-inner-product");
        using (Utils.WithVectorIndex(vectorIndexClient, indexName, 2, SimilarityMetric.InnerProduct))
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
    }

    [Theory]
    [MemberData(nameof(UpsertAndSearchTestData))]
    public async Task UpsertAndSearch_CosineSimilarity<T>(SearchDelegate<T> searchDelegate,
        AssertOnSearchResponse<T> assertOnSearchResponse)
    {
        var indexName = Utils.TestVectorIndexName("data-upsert-and-search-cosine-similarity");
        using (Utils.WithVectorIndex(vectorIndexClient, indexName, 2))
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
    }

    [Theory]
    [MemberData(nameof(UpsertAndSearchTestData))]
    public async Task UpsertAndSearch_EuclideanSimilarity<T>(SearchDelegate<T> searchDelegate,
        AssertOnSearchResponse<T> assertOnSearchResponse)
    {
        var indexName = Utils.TestVectorIndexName("data-upsert-and-search-euclidean-similarity");
        using (Utils.WithVectorIndex(vectorIndexClient, indexName, 2, SimilarityMetric.EuclideanSimilarity))
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
    }

    [Theory]
    [MemberData(nameof(UpsertAndSearchTestData))]
    public async Task UpsertAndSearch_TopKLimit<T>(SearchDelegate<T> searchDelegate,
        AssertOnSearchResponse<T> assertOnSearchResponse)
    {
        var indexName = Utils.TestVectorIndexName("data-upsert-and-search-top-k-limit");
        using (Utils.WithVectorIndex(vectorIndexClient, indexName, 2, SimilarityMetric.InnerProduct))
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
    }

    [Theory]
    [MemberData(nameof(UpsertAndSearchTestData))]
    public async Task UpsertAndSearch_WithMetadata<T>(SearchDelegate<T> searchDelegate,
        AssertOnSearchResponse<T> assertOnSearchResponse)
    {
        var indexName = Utils.TestVectorIndexName("data-upsert-and-search-with-metadata");
        using (Utils.WithVectorIndex(vectorIndexClient, indexName, 2, SimilarityMetric.InnerProduct))
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
    }

    [Theory]
    [MemberData(nameof(UpsertAndSearchTestData))]
    public async Task UpsertAndSearch_WithDiverseMetadata<T>(SearchDelegate<T> searchDelegate,
        AssertOnSearchResponse<T> assertOnSearchResponse)
    {
        var indexName = Utils.TestVectorIndexName("data-upsert-and-search-with-diverse-metadata");
        using (Utils.WithVectorIndex(vectorIndexClient, indexName, 2, SimilarityMetric.InnerProduct))
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
        var indexName = Utils.TestVectorIndexName("data-search-prune-based-on-threshold");
        using (Utils.WithVectorIndex(vectorIndexClient, indexName, 2, similarityMetric))
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
    }

    public delegate Task<T> GetItemDelegate<T>(IPreviewVectorIndexClient client, string indexName, IEnumerable<string> ids);
    public static GetItemDelegate<GetItemBatchResponse> GetItemBatchDelegate = new(
        (client, indexName, ids) => client.GetItemBatchAsync(indexName, ids));
    public static GetItemDelegate<GetItemMetadataBatchResponse> GetItemMetadataBatchDelegate = new(
        (client, indexName, ids) => client.GetItemMetadataBatchAsync(indexName, ids));
    public delegate void AssertOnGetItemResponse<T>(T response, Object expected);
    public static AssertOnGetItemResponse<GetItemBatchResponse> AssertOnGetItemBatchResponse = new(
        (response, expected) =>
        {
            Assert.True(response is GetItemBatchResponse.Success, $"Unexpected response: {response}");
            var successResponse = (GetItemBatchResponse.Success)response;
            Assert.Equal(expected, successResponse.Values);
        });
    public static AssertOnGetItemResponse<GetItemMetadataBatchResponse> AssertOnGetItemMetadataBatchResponse = new(
        (response, expected) =>
        {
            Assert.True(response is GetItemMetadataBatchResponse.Success, $"Unexpected response: {response}");
            var successResponse = (GetItemMetadataBatchResponse.Success)response;
            Assert.Equal(expected, successResponse.Values);
        });
    public static IEnumerable<object[]> GetItemAndGetItemMetadataTestData
    {
        get
        {
            return new List<object[]>
            {
                new object[]
                {
                    GetItemBatchDelegate,
                    AssertOnGetItemBatchResponse,
                    new List<string> {  },
                    new Dictionary<string, Item>()
                },
                new object[]
                {
                    GetItemMetadataBatchDelegate,
                    AssertOnGetItemMetadataBatchResponse,
                    new List<string> {  },
                    new Dictionary<string, Dictionary<string, MetadataValue>>()
                },
                new object[]
                {
                    GetItemBatchDelegate,
                    AssertOnGetItemBatchResponse,
                    new List<string> { "missing_id" },
                    new Dictionary<string, Item>()
                },
                new object[]
                {
                    GetItemMetadataBatchDelegate,
                    AssertOnGetItemMetadataBatchResponse,
                    new List<string> { "missing_id" },
                    new Dictionary<string, Dictionary<string, MetadataValue>>()
                },
                new object[]
                {
                    GetItemBatchDelegate,
                    AssertOnGetItemBatchResponse,
                    new List<string> { "test_item_1", "missing_id", "test_item_2" },
                    new Dictionary<string, Item>
                    {
                        { "test_item_1", new Item("test_item_1", new List<float> { 1.0f, 2.0f }, new Dictionary<string, MetadataValue> { { "key1", "value1" } }) },
                        { "test_item_2", new Item("test_item_2", new List<float> { 3.0f, 4.0f }) }
                    }
                },
                new object[]
                {
                    GetItemMetadataBatchDelegate,
                    AssertOnGetItemMetadataBatchResponse,
                    new List<string> { "test_item_1", "missing_id", "test_item_2" },
                    new Dictionary<string, Dictionary<string, MetadataValue>>
                    {
                        { "test_item_1", new Dictionary<string, MetadataValue> { { "key1", "value1" } } },
                        { "test_item_2", new Dictionary<string, MetadataValue>() }
                    }
                }
            };
        }
    }

    [Theory]
    [MemberData(nameof(GetItemAndGetItemMetadataTestData))]
    public async Task GetItemAndGetItemMetadata_HappyPath<T>(GetItemDelegate<T> getItemDelegate, AssertOnGetItemResponse<T> assertOnGetItemResponse, IEnumerable<string> ids, Object expected)
    {
        var indexName = Utils.TestVectorIndexName("data-get-item-and-get-item-metadata-happy-path");
        using (Utils.WithVectorIndex(vectorIndexClient, indexName, 2, SimilarityMetric.InnerProduct))
        {
            var items = new List<Item>
            {
                new("test_item_1", new List<float> { 1.0f, 2.0f }, new Dictionary<string, MetadataValue> { { "key1", "value1" } }),
                new("test_item_2", new List<float> { 3.0f, 4.0f }),
                new("test_item_3", new List<float> { 5.0f, 6.0f }),
            };

            var upsertResponse = await vectorIndexClient.UpsertItemBatchAsync(indexName, items);
            Assert.True(upsertResponse is UpsertItemBatchResponse.Success,
                $"Unexpected response: {upsertResponse}");

            await Task.Delay(2_000);

            var getResponse = await getItemDelegate.Invoke(vectorIndexClient, indexName, ids);
            assertOnGetItemResponse.Invoke(getResponse, expected);
        }
    }

    [Fact]
    public async Task CountItemsAsync_OnMissingIndex_ReturnsError()
    {
        var indexName = Utils.NewGuidString();
        var response = await vectorIndexClient.CountItemsAsync(indexName);
        Assert.True(response is CountItemsResponse.Error, $"Unexpected response: {response}");
        var error = (CountItemsResponse.Error)response;
        Assert.Equal(MomentoErrorCode.NOT_FOUND_ERROR, error.InnerException.ErrorCode);
    }

    [Fact]
    public async Task CountItemsAsync_OnEmptyIndex_ReturnsZero()
    {
        var indexName = Utils.TestVectorIndexName("data-count-items-on-empty-index");
        using (Utils.WithVectorIndex(vectorIndexClient, indexName, 2, SimilarityMetric.InnerProduct))
        {
            var response = await vectorIndexClient.CountItemsAsync(indexName);
            Assert.True(response is CountItemsResponse.Success, $"Unexpected response: {response}");
            var successResponse = (CountItemsResponse.Success)response;
            Assert.Equal(0, successResponse.ItemCount);
        }
    }

    [Fact]
    public async Task CountItemsAsync_HasItems_CountsCorrectly()
    {
        var indexName = Utils.TestVectorIndexName("data-count-items-has-items-counts-correctly");
        using (Utils.WithVectorIndex(vectorIndexClient, indexName, 2, SimilarityMetric.InnerProduct))
        {
            var items = new List<Item>
            {
                new("test_item_1", new List<float> { 1.0f, 2.0f }),
                new("test_item_2", new List<float> { 3.0f, 4.0f }),
                new("test_item_3", new List<float> { 5.0f, 6.0f }),
                new("test_item_4", new List<float> { 7.0f, 8.0f }),
                new("test_item_5", new List<float> { 9.0f, 10.0f }),
            };

            var upsertResponse = await vectorIndexClient.UpsertItemBatchAsync(indexName, items);
            Assert.True(upsertResponse is UpsertItemBatchResponse.Success,
                $"Unexpected response: {upsertResponse}");

            await Task.Delay(2_000);

            var response = await vectorIndexClient.CountItemsAsync(indexName);
            Assert.True(response is CountItemsResponse.Success, $"Unexpected response: {response}");
            var successResponse = (CountItemsResponse.Success)response;
            Assert.Equal(5, successResponse.ItemCount);

            // Delete two items
            var deleteResponse = await vectorIndexClient.DeleteItemBatchAsync(indexName,
                new List<string> { "test_item_1", "test_item_2" });
            Assert.True(deleteResponse is DeleteItemBatchResponse.Success, $"Unexpected response: {deleteResponse}");

            await Task.Delay(2_000);

            response = await vectorIndexClient.CountItemsAsync(indexName);
            Assert.True(response is CountItemsResponse.Success, $"Unexpected response: {response}");
            successResponse = (CountItemsResponse.Success)response;
            Assert.Equal(3, successResponse.ItemCount);
        }
    }
}
