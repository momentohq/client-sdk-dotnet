using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Protos.ControlClient;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Requests.Vector;
using Momento.Sdk.Responses.Vector;

namespace Momento.Sdk.Internal;

internal sealed class VectorIndexControlClient : IDisposable
{
    private readonly VectorIndexControlGrpcManager grpcManager;
    private readonly TimeSpan deadline = TimeSpan.FromSeconds(60);

    private readonly ILogger _logger;
    private readonly CacheExceptionMapper _exceptionMapper;

    public VectorIndexControlClient(ILoggerFactory loggerFactory, string authToken, string endpoint)
    {
        grpcManager = new VectorIndexControlGrpcManager(loggerFactory, authToken, endpoint);
        _logger = loggerFactory.CreateLogger<VectorIndexControlClient>();
        _exceptionMapper = new CacheExceptionMapper(loggerFactory);
    }

    public async Task<CreateVectorIndexResponse> CreateIndexAsync(string indexName, long numDimensions, SimilarityMetric similarityMetric)
    {
        try
        {
            _logger.LogTraceVectorIndexRequest("createVectorIndex", indexName);
            CheckValidIndexName(indexName);
            var validatedNumDimensions = ValidateNumDimensions(numDimensions);
            var request = new _CreateIndexRequest { IndexName = indexName, NumDimensions = validatedNumDimensions };
            switch (similarityMetric)
            {
                case SimilarityMetric.CosineSimilarity:
                    request.CosineSimilarity = new _CreateIndexRequest.Types._CosineSimilarity();
                    break;
                case SimilarityMetric.InnerProduct:
                    request.InnerProduct = new _CreateIndexRequest.Types._InnerProduct();
                    break;
                case SimilarityMetric.EuclideanSimilarity:
                    request.EuclideanSimilarity = new _CreateIndexRequest.Types._EuclideanSimilarity();
                    break;
                default:
                    throw new InvalidArgumentException($"Unknown similarity metric {similarityMetric}");
            }
            
            await grpcManager.Client.CreateIndexAsync(request, new CallOptions(deadline: CalculateDeadline()));
            return _logger.LogTraceVectorIndexRequestSuccess("createVectorIndex", indexName, new CreateVectorIndexResponse.Success());
        }
        catch (Exception e)
        {
            if (e is RpcException { StatusCode: StatusCode.AlreadyExists })
            {
                return _logger.LogTraceVectorIndexRequestSuccess("createVectorIndex", indexName, new CreateVectorIndexResponse.IndexAlreadyExists());
            }
            return _logger.LogTraceVectorIndexRequestError("createVectorIndex", indexName, new CreateVectorIndexResponse.Error(_exceptionMapper.Convert(e)));
        }
    }

    public async Task<ListVectorIndexesResponse> ListIndexesAsync()
    {
        try
        {
            _logger.LogTraceExecutingGenericRequest("listVectorIndexes");
            var request = new _ListIndexesRequest();
            var response = await grpcManager.Client.ListIndexesAsync(request, new CallOptions(deadline: CalculateDeadline()));
            return _logger.LogTraceGenericRequestSuccess("listVectorIndexes",
                new ListVectorIndexesResponse.Success(new List<string>(response.IndexNames)));
        }
        catch (Exception e)
        {
            return _logger.LogTraceGenericRequestError("listVectorIndexes", new ListVectorIndexesResponse.Error(_exceptionMapper.Convert(e)));
        }
    }

    public async Task<DeleteVectorIndexResponse> DeleteIndexAsync(string indexName)
    {
        try
        {
            _logger.LogTraceVectorIndexRequest("deleteVectorIndex", indexName);
            CheckValidIndexName(indexName);
            var request = new _DeleteIndexRequest() { IndexName = indexName };
            await grpcManager.Client.DeleteIndexAsync(request, new CallOptions(deadline: CalculateDeadline()));
            return _logger.LogTraceVectorIndexRequestSuccess("createVectorIndex", indexName, new DeleteVectorIndexResponse.Success());
        }
        catch (Exception e)
        {
            return _logger.LogTraceVectorIndexRequestError("createVectorIndex", indexName, new DeleteVectorIndexResponse.Error(_exceptionMapper.Convert(e)));
        }
    }

    private static void CheckValidIndexName(string indexName)
    {
        if (string.IsNullOrWhiteSpace(indexName))
        {
            throw new InvalidArgumentException("Index name must be nonempty");
        }
    }
    
    private static ulong ValidateNumDimensions(long numDimensions)
    {
        if (numDimensions <= 0)
        {
            throw new InvalidArgumentException("numDimensions must be greater than 0");
        }

        return (ulong)numDimensions;
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
