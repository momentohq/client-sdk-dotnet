﻿using System;
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

    public async Task<VectorUpsertItemBatchResponse> UpsertItemBatchAsync(string indexName,
        IEnumerable<VectorIndexItem> items)
    {
        try
        {
            _logger.LogTraceVectorIndexRequest("upsertItemBatch", indexName);
            CheckValidIndexName(indexName);
            var request = new _UpsertItemBatchRequest() { IndexName = indexName, Items = { items.Select(Convert) } };

            await grpcManager.Client.UpsertItemBatchAsync(request, new CallOptions(deadline: CalculateDeadline()));
            return _logger.LogTraceVectorIndexRequestSuccess("upsertItemBatch", indexName,
                new VectorUpsertItemBatchResponse.Success());
        }
        catch (Exception e)
        {
            return _logger.LogTraceVectorIndexRequestError("upsertItemBatch", indexName,
                new VectorUpsertItemBatchResponse.Error(_exceptionMapper.Convert(e)));
        }
    }

    public async Task<VectorDeleteItemBatchResponse> DeleteItemBatchAsync(string indexName, IEnumerable<string> ids)
    {
        try
        {
            _logger.LogTraceVectorIndexRequest("deleteItemBatch", indexName);
            CheckValidIndexName(indexName);
            var request = new _DeleteItemBatchRequest() { IndexName = indexName, Ids = { ids } };

            await grpcManager.Client.DeleteItemBatchAsync(request, new CallOptions(deadline: CalculateDeadline()));
            return _logger.LogTraceVectorIndexRequestSuccess("deleteItemBatch", indexName,
                new VectorDeleteItemBatchResponse.Success());
        }
        catch (Exception e)
        {
            return _logger.LogTraceVectorIndexRequestError("deleteItemBatch", indexName,
                new VectorDeleteItemBatchResponse.Error(_exceptionMapper.Convert(e)));
        }
    }

    public async Task<VectorSearchResponse> SearchAsync(string indexName, IEnumerable<float> queryVector, uint topK,
        MetadataFields? metadataFields)
    {
        try
        {
            _logger.LogTraceVectorIndexRequest("search", indexName);
            CheckValidIndexName(indexName);
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
                TopK = topK,
                MetadataFields = metadataRequest
            };

            var response =
                await grpcManager.Client.SearchAsync(request, new CallOptions(deadline: CalculateDeadline()));
            var searchHits = response.Hits.Select(Convert).ToList();
            return _logger.LogTraceVectorIndexRequestSuccess("search", indexName,
                new VectorSearchResponse.Success(searchHits));
        }
        catch (Exception e)
        {
            return _logger.LogTraceVectorIndexRequestError("search", indexName,
                new VectorSearchResponse.Error(_exceptionMapper.Convert(e)));
        }
    }

    private static _Item Convert(VectorIndexItem item)
    {
        return new _Item
        {
            Id = item.Id, Vector = new _Vector { Elements = { item.Vector } }, Metadata = { Convert(item.Metadata) }
        };
    }

    private static IEnumerable<_Metadata> Convert(Dictionary<string, MetadataValue> metadata)
    {
        var convertedMetadataList = new List<_Metadata>();
        foreach (var metadataPair in metadata)
        {
            _Metadata convertedMetadata;
            switch (metadataPair.Value)
            {
                case StringValue stringValue:
                    convertedMetadata = new _Metadata { Field = metadataPair.Key, StringValue = stringValue.Value };
                    break;
                case LongValue longValue:
                    convertedMetadata = new _Metadata { Field = metadataPair.Key, IntegerValue = longValue.Value };
                    break;
                case DoubleValue doubleValue:
                    convertedMetadata = new _Metadata { Field = metadataPair.Key, DoubleValue = doubleValue.Value };
                    break;
                case BoolValue boolValue:
                    convertedMetadata = new _Metadata { Field = metadataPair.Key, BooleanValue = boolValue.Value };
                    break;
                case StringListValue stringListValue:
                    var listOfStrings = new _Metadata.Types._ListOfStrings { Values = { stringListValue.Value } };
                    convertedMetadata = new _Metadata { Field = metadataPair.Key, ListOfStringsValue = listOfStrings };
                    break;
                default:
                    throw new InvalidArgumentException($"Unknown metadata type {metadataPair.Value.GetType()}");
            }

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
        return new SearchHit(hit.Id, hit.Distance, Convert(hit.Metadata));
    }

    private static void CheckValidIndexName(string indexName)
    {
        if (string.IsNullOrWhiteSpace(indexName))
        {
            throw new InvalidArgumentException("Index name must be nonempty");
        }
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