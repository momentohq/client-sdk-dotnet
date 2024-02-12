using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Config;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Messages.Vector;
using Momento.Sdk.Requests.Vector;
using Momento.Sdk.Responses.Vector;
using Vectorindex;

namespace Momento.Sdk.Internal;

internal sealed class VectorIndexDataClient : IDisposable
{
    private readonly VectorIndexDataGrpcManager grpcManager;
    private readonly TimeSpan deadline = TimeSpan.FromSeconds(60);

    private readonly ILogger _logger;
    private readonly CacheExceptionMapper _exceptionMapper;

    public VectorIndexDataClient(IVectorIndexConfiguration config, string authToken, string endpoint)
    {
        grpcManager = new VectorIndexDataGrpcManager(config, authToken, endpoint);
        _logger = config.LoggerFactory.CreateLogger<VectorIndexDataClient>();
        _exceptionMapper = new CacheExceptionMapper(config.LoggerFactory);
    }

    const string REQUEST_COUNT_ITEMS = "COUNT_ITEMS";
    public async Task<CountItemsResponse> CountItemsAsync(string indexName)
    {
        try
        {
            _logger.LogTraceVectorIndexRequest(REQUEST_COUNT_ITEMS, indexName);
            CheckValidIndexName(indexName);
            var request = new _CountItemsRequest() { IndexName = indexName, All = new _CountItemsRequest.Types.All() };

            var response =
                await grpcManager.Client.CountItemsAsync(request, new CallOptions(deadline: CalculateDeadline()));
            // To maintain CLS compliance we use a long here instead of a ulong.
            // The max value of a long is still over 9 quintillion so we should be good for a while.
            var itemCount = checked((long)response.ItemCount);
            return _logger.LogTraceVectorIndexRequestSuccess(REQUEST_COUNT_ITEMS, indexName,
                new CountItemsResponse.Success(itemCount));
        }
        catch (Exception e)
        {
            return _logger.LogTraceVectorIndexRequestError(REQUEST_COUNT_ITEMS, indexName,
                new CountItemsResponse.Error(_exceptionMapper.Convert(e)));
        }
    }

    const string REQUEST_UPSERT_ITEM_BATCH = "UPSERT_ITEM_BATCH";
    public async Task<UpsertItemBatchResponse> UpsertItemBatchAsync(string indexName,
        IEnumerable<Item> items)
    {
        try
        {
            _logger.LogTraceVectorIndexRequest(REQUEST_UPSERT_ITEM_BATCH, indexName);
            CheckValidIndexName(indexName);
            var request = new _UpsertItemBatchRequest() { IndexName = indexName, Items = { items.Select(Convert) } };

            await grpcManager.Client.UpsertItemBatchAsync(request, new CallOptions(deadline: CalculateDeadline()));
            return _logger.LogTraceVectorIndexRequestSuccess(REQUEST_UPSERT_ITEM_BATCH, indexName,
                new UpsertItemBatchResponse.Success());
        }
        catch (Exception e)
        {
            return _logger.LogTraceVectorIndexRequestError(REQUEST_UPSERT_ITEM_BATCH, indexName,
                new UpsertItemBatchResponse.Error(_exceptionMapper.Convert(e)));
        }
    }

    const string REQUEST_GET_ITEM_BATCH = "GET_ITEM_BATCH";
    public async Task<GetItemBatchResponse> GetItemBatchAsync(string indexName, IEnumerable<string> ids)
    {
        try
        {
            _logger.LogTraceVectorIndexRequest(REQUEST_GET_ITEM_BATCH, indexName);
            CheckValidIndexName(indexName);
            var request = new _GetItemBatchRequest()
            {
                IndexName = indexName,
                Filter = idsToFilterExpression(ids),
                MetadataFields = new _MetadataRequest { All = new _MetadataRequest.Types.All() }
            };

            var response =
                await grpcManager.Client.GetItemBatchAsync(request, new CallOptions(deadline: CalculateDeadline()));
            var items = response.ItemResponse.ToDictionary(
                item => item.Id, item => new Item(item.Id, item.Vector.Elements.ToList(), Convert(item.Metadata)));
            return _logger.LogTraceVectorIndexRequestSuccess(REQUEST_GET_ITEM_BATCH, indexName,
                new GetItemBatchResponse.Success(items));
        }
        catch (Exception e)
        {
            return _logger.LogTraceVectorIndexRequestError(REQUEST_GET_ITEM_BATCH, indexName,
                new GetItemBatchResponse.Error(_exceptionMapper.Convert(e)));
        }
    }

    const string REQUEST_GET_ITEM_METADATA_BATCH = "GET_ITEM_METADATA_BATCH";
    public async Task<GetItemMetadataBatchResponse> GetItemMetadataBatchAsync(string indexName, IEnumerable<string> ids)
    {
        try
        {
            _logger.LogTraceVectorIndexRequest(REQUEST_GET_ITEM_METADATA_BATCH, indexName);
            CheckValidIndexName(indexName);
            var request = new _GetItemMetadataBatchRequest()
            {
                IndexName = indexName,
                Filter = idsToFilterExpression(ids),
                MetadataFields = new _MetadataRequest { All = new _MetadataRequest.Types.All() }
            };

            var response =
                await grpcManager.Client.GetItemMetadataBatchAsync(request,
                    new CallOptions(deadline: CalculateDeadline()));
            var items = response.ItemMetadataResponse.ToDictionary(
                item => item.Id, item => Convert(item.Metadata));
            return _logger.LogTraceVectorIndexRequestSuccess(REQUEST_GET_ITEM_METADATA_BATCH, indexName,
                new GetItemMetadataBatchResponse.Success(items));
        }
        catch (Exception e)
        {
            return _logger.LogTraceVectorIndexRequestError(REQUEST_GET_ITEM_METADATA_BATCH, indexName,
                new GetItemMetadataBatchResponse.Error(_exceptionMapper.Convert(e)));
        }
    }

    /// <summary>
    /// Convert a list of ids to an id-in-set filter expression.
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    private static _FilterExpression idsToFilterExpression(IEnumerable<string> ids)
    {
        return new _FilterExpression
        {
            IdInSetExpression = new _IdInSetExpression()
            {
                Ids = { ids }
            }
        };
    }

    const string REQUEST_DELETE_ITEM_BATCH = "DELETE_ITEM_BATCH";
    public async Task<DeleteItemBatchResponse> DeleteItemBatchAsync(string indexName, IEnumerable<string> ids)
    {
        try
        {
            _logger.LogTraceVectorIndexRequest(REQUEST_DELETE_ITEM_BATCH, indexName);
            CheckValidIndexName(indexName);
            var request = new _DeleteItemBatchRequest() { IndexName = indexName, Filter = idsToFilterExpression(ids) };

            await grpcManager.Client.DeleteItemBatchAsync(request, new CallOptions(deadline: CalculateDeadline()));
            return _logger.LogTraceVectorIndexRequestSuccess(REQUEST_DELETE_ITEM_BATCH, indexName,
                new DeleteItemBatchResponse.Success());
        }
        catch (Exception e)
        {
            return _logger.LogTraceVectorIndexRequestError(REQUEST_DELETE_ITEM_BATCH, indexName,
                new DeleteItemBatchResponse.Error(_exceptionMapper.Convert(e)));
        }
    }

    const string REQUEST_SEARCH = "SEARCH";
    public async Task<SearchResponse> SearchAsync(string indexName, IEnumerable<float> queryVector, int topK,
        MetadataFields? metadataFields, float? scoreThreshold)
    {
        try
        {
            _logger.LogTraceVectorIndexRequest(REQUEST_SEARCH, indexName);
            CheckValidIndexName(indexName);
            var validatedTopK = ValidateTopK(topK);
            metadataFields ??= new List<string>();
            var metadataRequest = metadataFields switch
            {
                MetadataFields.AllFields => new _MetadataRequest { All = new _MetadataRequest.Types.All() },
                MetadataFields.List list => new _MetadataRequest
                {
                    Some = new _MetadataRequest.Types.Some { Fields = { list.Fields } }
                },
                _ => throw new InvalidArgumentException($"Unknown metadata fields type {metadataFields.GetType()}")
            };

            var request = new _SearchRequest
            {
                IndexName = indexName,
                QueryVector = new _Vector { Elements = { queryVector } },
                TopK = validatedTopK,
                MetadataFields = metadataRequest,
            };

            if (scoreThreshold != null)
            {
                request.ScoreThreshold = scoreThreshold.Value;
            }
            else
            {
                request.NoScoreThreshold = new _NoScoreThreshold();
            }

            var response =
                await grpcManager.Client.SearchAsync(request, new CallOptions(deadline: CalculateDeadline()));
            var searchHits = response.Hits.Select(Convert).ToList();
            return _logger.LogTraceVectorIndexRequestSuccess(REQUEST_SEARCH, indexName,
                new SearchResponse.Success(searchHits));
        }
        catch (Exception e)
        {
            return _logger.LogTraceVectorIndexRequestError(REQUEST_SEARCH, indexName,
                new SearchResponse.Error(_exceptionMapper.Convert(e)));
        }
    }

    const string REQUEST_SEARCH_AND_FETCH_VECTORS = "SEARCH_AND_FETCH_VECTORS";
    public async Task<SearchAndFetchVectorsResponse> SearchAndFetchVectorsAsync(string indexName,
        IEnumerable<float> queryVector, int topK, MetadataFields? metadataFields, float? scoreThreshold)
    {
        try
        {
            _logger.LogTraceVectorIndexRequest(REQUEST_SEARCH_AND_FETCH_VECTORS, indexName);
            CheckValidIndexName(indexName);
            var validatedTopK = ValidateTopK(topK);
            metadataFields ??= new List<string>();
            var metadataRequest = metadataFields switch
            {
                MetadataFields.AllFields => new _MetadataRequest { All = new _MetadataRequest.Types.All() },
                MetadataFields.List list => new _MetadataRequest
                {
                    Some = new _MetadataRequest.Types.Some { Fields = { list.Fields } }
                },
                _ => throw new InvalidArgumentException($"Unknown metadata fields type {metadataFields.GetType()}")
            };

            var request = new _SearchAndFetchVectorsRequest()
            {
                IndexName = indexName,
                QueryVector = new _Vector { Elements = { queryVector } },
                TopK = validatedTopK,
                MetadataFields = metadataRequest,
            };

            if (scoreThreshold != null)
            {
                request.ScoreThreshold = scoreThreshold.Value;
            }
            else
            {
                request.NoScoreThreshold = new _NoScoreThreshold();
            }

            var response =
                await grpcManager.Client.SearchAndFetchVectorsAsync(request,
                    new CallOptions(deadline: CalculateDeadline()));
            var searchHits = response.Hits.Select(h =>
                new SearchAndFetchVectorsHit(h.Id, h.Score, h.Vector.Elements.ToList(), Convert(h.Metadata))).ToList();
            return _logger.LogTraceVectorIndexRequestSuccess(REQUEST_SEARCH_AND_FETCH_VECTORS, indexName,
                new SearchAndFetchVectorsResponse.Success(searchHits));
        }
        catch (Exception e)
        {
            return _logger.LogTraceVectorIndexRequestError(REQUEST_SEARCH_AND_FETCH_VECTORS, indexName,
                new SearchAndFetchVectorsResponse.Error(_exceptionMapper.Convert(e)));
        }
    }

    private static _Item Convert(Item item)
    {
        return new _Item
        {
            Id = item.Id,
            Vector = new _Vector { Elements = { item.Vector } },
            Metadata = { Convert(item.Metadata) }
        };
    }

    private static IEnumerable<_Metadata> Convert(Dictionary<string, MetadataValue> metadata)
    {
        var convertedMetadataList = new List<_Metadata>();
        foreach (var metadataPair in metadata)
        {
            var convertedMetadata = metadataPair.Value switch
            {
                StringValue stringValue => new _Metadata { Field = metadataPair.Key, StringValue = stringValue.Value },
                LongValue longValue => new _Metadata { Field = metadataPair.Key, IntegerValue = longValue.Value },
                DoubleValue doubleValue => new _Metadata { Field = metadataPair.Key, DoubleValue = doubleValue.Value },
                BoolValue boolValue => new _Metadata { Field = metadataPair.Key, BooleanValue = boolValue.Value },
                StringListValue stringListValue => new _Metadata
                {
                    Field = metadataPair.Key,
                    ListOfStringsValue = new _Metadata.Types._ListOfStrings { Values = { stringListValue.Value } }
                },
                _ => throw new InvalidArgumentException($"Unknown metadata type {metadataPair.Value.GetType()}")
            };

            convertedMetadataList.Add(convertedMetadata);
        }

        return convertedMetadataList;
    }

    private static Dictionary<string, MetadataValue> Convert(IEnumerable<_Metadata> metadata)
    {
        return metadata.ToDictionary(m => m.Field, Convert);
    }

    private static MetadataValue Convert(_Metadata metadata)
    {
        switch (metadata.ValueCase)
        {
            case _Metadata.ValueOneofCase.StringValue:
                return new StringValue(metadata.StringValue);
            case _Metadata.ValueOneofCase.IntegerValue:
                return new LongValue(metadata.IntegerValue);
            case _Metadata.ValueOneofCase.DoubleValue:
                return new DoubleValue(metadata.DoubleValue);
            case _Metadata.ValueOneofCase.BooleanValue:
                return new BoolValue(metadata.BooleanValue);
            case _Metadata.ValueOneofCase.ListOfStringsValue:
                return new StringListValue(metadata.ListOfStringsValue.Values.ToList());
            case _Metadata.ValueOneofCase.None:
            default:
                throw new UnknownException($"Unknown metadata type {metadata.ValueCase}");
        }
    }

    private static SearchHit Convert(_SearchHit hit)
    {
        return new SearchHit(hit.Id, hit.Score, Convert(hit.Metadata));
    }

    private static SearchAndFetchVectorsHit Convert(_SearchAndFetchVectorsHit hit)
    {
        return new SearchAndFetchVectorsHit(hit.Id, hit.Score, hit.Vector.Elements.ToList(), Convert(hit.Metadata));
    }

    private static void CheckValidIndexName(string indexName)
    {
        if (string.IsNullOrWhiteSpace(indexName))
        {
            throw new InvalidArgumentException("Index name must be nonempty");
        }
    }

    private static uint ValidateTopK(long topK)
    {
        if (topK <= 0)
        {
            throw new InvalidArgumentException("topK must be greater than 0");
        }

        return (uint)topK;
    }

    private DateTime CalculateDeadline()
    {
        return DateTime.UtcNow.Add(deadline);
    }

    public void Dispose()
    {
        grpcManager.Dispose();
    }
}
