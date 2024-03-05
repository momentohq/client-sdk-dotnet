using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Protos.ControlClient;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Requests.Vector;
using Momento.Sdk.Responses.Vector;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Transport;

namespace Momento.Sdk.Internal;

internal sealed class VectorIndexControlClient : IDisposable
{
    private readonly VectorIndexControlGrpcManager grpcManager;
    private readonly TimeSpan deadline = TimeSpan.FromSeconds(60);

    private readonly ILogger _logger;
    private readonly CacheExceptionMapper _exceptionMapper;

    public VectorIndexControlClient(IVectorIndexConfiguration config, string authToken, string endpoint)
    {
        // Override the sockets http handler options to disable keepalive
        var overrideKeepalive = SocketsHttpHandlerOptions.Of(
            pooledConnectionIdleTimeout: config.TransportStrategy.GrpcConfig.SocketsHttpHandlerOptions.PooledConnectionIdleTimeout,
            enableMultipleHttp2Connections: config.TransportStrategy.GrpcConfig.SocketsHttpHandlerOptions.EnableMultipleHttp2Connections,
            keepAlivePingTimeout: System.Threading.Timeout.InfiniteTimeSpan,
            keepAlivePingDelay: System.Threading.Timeout.InfiniteTimeSpan,
            keepAlivePermitWithoutCalls: false
        );
        var controlConfig = config.WithTransportStrategy(config.TransportStrategy.WithSocketsHttpHandlerOptions(overrideKeepalive));

        grpcManager = new VectorIndexControlGrpcManager(controlConfig, authToken, endpoint);
        _logger = config.LoggerFactory.CreateLogger<VectorIndexControlClient>();
        _exceptionMapper = new CacheExceptionMapper(config.LoggerFactory);
    }

    public async Task<CreateIndexResponse> CreateIndexAsync(string indexName, long numDimensions, SimilarityMetric similarityMetric)
    {
        try
        {
            _logger.LogTraceVectorIndexRequest("createVectorIndex", indexName);
            CheckValidIndexName(indexName);
            var validatedNumDimensions = ValidateNumDimensions(numDimensions);
            var request = new _CreateIndexRequest { IndexName = indexName, NumDimensions = validatedNumDimensions, SimilarityMetric = new _SimilarityMetric() };
            switch (similarityMetric)
            {
                case SimilarityMetric.CosineSimilarity:
                    request.SimilarityMetric.CosineSimilarity = new _SimilarityMetric.Types._CosineSimilarity();
                    break;
                case SimilarityMetric.InnerProduct:
                    request.SimilarityMetric.InnerProduct = new _SimilarityMetric.Types._InnerProduct();
                    break;
                case SimilarityMetric.EuclideanSimilarity:
                    request.SimilarityMetric.EuclideanSimilarity = new _SimilarityMetric.Types._EuclideanSimilarity();
                    break;
                default:
                    throw new InvalidArgumentException($"Unknown similarity metric {similarityMetric}");
            }

            await grpcManager.Client.CreateIndexAsync(request, new CallOptions(deadline: CalculateDeadline()));
            return _logger.LogTraceVectorIndexRequestSuccess("createVectorIndex", indexName, new CreateIndexResponse.Success());
        }
        catch (Exception e)
        {
            if (e is RpcException { StatusCode: StatusCode.AlreadyExists })
            {
                return _logger.LogTraceVectorIndexRequestSuccess("createVectorIndex", indexName, new CreateIndexResponse.AlreadyExists());
            }
            return _logger.LogTraceVectorIndexRequestError("createVectorIndex", indexName, new CreateIndexResponse.Error(_exceptionMapper.Convert(e)));
        }
    }

    public async Task<ListIndexesResponse> ListIndexesAsync()
    {
        try
        {
            _logger.LogTraceExecutingGenericRequest("listVectorIndexes");
            var request = new _ListIndexesRequest();
            var response = await grpcManager.Client.ListIndexesAsync(request, new CallOptions(deadline: CalculateDeadline()));
            return _logger.LogTraceGenericRequestSuccess("listVectorIndexes",
                new ListIndexesResponse.Success(
                    new List<IndexInfo>(response.Indexes.Select(n => new IndexInfo(n.IndexName, (int)n.NumDimensions, Convert(n.SimilarityMetric))))
                ));
        }
        catch (Exception e)
        {
            return _logger.LogTraceGenericRequestError("listVectorIndexes", new ListIndexesResponse.Error(_exceptionMapper.Convert(e)));
        }
    }

    private static SimilarityMetric Convert(_SimilarityMetric similarityMetric)
    {
        return similarityMetric.SimilarityMetricCase switch
        {
            _SimilarityMetric.SimilarityMetricOneofCase.InnerProduct => SimilarityMetric.InnerProduct,
            _SimilarityMetric.SimilarityMetricOneofCase.EuclideanSimilarity => SimilarityMetric.EuclideanSimilarity,
            _SimilarityMetric.SimilarityMetricOneofCase.CosineSimilarity => SimilarityMetric.CosineSimilarity,
            _ => throw new UnknownException($"Unknown similarity metric {similarityMetric}")
        };
    }

    public async Task<DeleteIndexResponse> DeleteIndexAsync(string indexName)
    {
        try
        {
            _logger.LogTraceVectorIndexRequest("deleteVectorIndex", indexName);
            CheckValidIndexName(indexName);
            var request = new _DeleteIndexRequest() { IndexName = indexName };
            await grpcManager.Client.DeleteIndexAsync(request, new CallOptions(deadline: CalculateDeadline()));
            return _logger.LogTraceVectorIndexRequestSuccess("createVectorIndex", indexName, new DeleteIndexResponse.Success());
        }
        catch (Exception e)
        {
            return _logger.LogTraceVectorIndexRequestError("createVectorIndex", indexName, new DeleteIndexResponse.Error(_exceptionMapper.Convert(e)));
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
