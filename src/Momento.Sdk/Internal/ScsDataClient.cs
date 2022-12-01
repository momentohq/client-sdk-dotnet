#pragma warning disable 1591
using System;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Protos.CacheClient;
using Momento.Sdk.Config;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal.ExtensionMethods;
using Momento.Sdk.Responses;

namespace Momento.Sdk.Internal;

public class ScsDataClientBase : IDisposable
{
    protected readonly DataGrpcManager grpcManager;
    private readonly TimeSpan defaultTtl;
    private readonly TimeSpan dataClientOperationTimeout;
    private readonly ILogger _logger;

    protected readonly CacheExceptionMapper _exceptionMapper;

    public ScsDataClientBase(IConfiguration config, string authToken, string endpoint, TimeSpan defaultTtl)
    {
        this.grpcManager = new(config, authToken, endpoint);
        this.defaultTtl = defaultTtl;
        this.dataClientOperationTimeout = config.TransportStrategy.GrpcConfig.Deadline;
        this._logger = config.LoggerFactory.CreateLogger<ScsDataClient>();
        this._exceptionMapper = new CacheExceptionMapper(config.LoggerFactory);
    }

    protected Metadata MetadataWithCache(string cacheName)
    {
        return new Metadata() { { "cache", cacheName } };
    }
    protected DateTime CalculateDeadline()
    {
        return DateTime.UtcNow.Add(dataClientOperationTimeout);
    }

    /// <summary>
    /// Converts TTL in seconds to milliseconds. Defaults to <see cref="ScsDataClientBase.defaultTtl" />.
    /// </summary>
    /// <remark>
    /// Conversion to <see langword="ulong"/> is safe here:
    /// (1) we already verified <paramref name="ttl"/> strictly positive, and
    /// (2) we know <paramref name="ttl.TotalMilliseconds"/> is less than <see cref="Int64.MaxValue"/>
    /// because <see cref="TimeSpan"/> counts number of ticks, 1ms = 10,000 ticks, and max number of
    /// ticks is <see cref="Int64.MaxValue"/>.
    /// </remark>
    /// <param name="ttl">The TTL to convert. Defaults to defaultTtl</param>
    /// <returns>Milliseconds representation of the TTL (if provided, else of <see cref="defaultTtl"/>)</returns>
    protected ulong TtlToMilliseconds(TimeSpan? ttl = null)
    {
        return Convert.ToUInt64((ttl ?? defaultTtl).TotalMilliseconds);
    }

    public void Dispose()
    {
        this.grpcManager.Dispose();
    }
}

internal sealed class ScsDataClient : ScsDataClientBase
{
    public ScsDataClient(IConfiguration config, string authToken, string endpoint, TimeSpan defaultTtl)
        : base(config, authToken, endpoint, defaultTtl)
    {

    }

    public async Task<CacheSetResponse> SetAsync(string cacheName, byte[] key, byte[] value, TimeSpan? ttl = null)
    {
        return await this.SendSetAsync(cacheName, value: value.ToByteString(), key: key.ToByteString(), ttl: ttl);
    }

    public async Task<CacheGetResponse> GetAsync(string cacheName, byte[] key)
    {
        return await this.SendGetAsync(cacheName, key.ToByteString());
    }

    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, byte[] key)
    {
        return await this.SendDeleteAsync(cacheName, key.ToByteString());
    }

    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, string value, TimeSpan? ttl = null)
    {
        return await this.SendSetAsync(cacheName, key: key.ToByteString(), value: value.ToByteString(), ttl: ttl);
    }

    public async Task<CacheGetResponse> GetAsync(string cacheName, string key)
    {
        return await this.SendGetAsync(cacheName, key.ToByteString());
    }

    public async Task<CacheDeleteResponse> DeleteAsync(string cacheName, string key)
    {
        return await this.SendDeleteAsync(cacheName, key.ToByteString());
    }

    public async Task<CacheSetResponse> SetAsync(string cacheName, string key, byte[] value, TimeSpan? ttl = null)
    {
        return await this.SendSetAsync(cacheName, value: value.ToByteString(), key: key.ToByteString(), ttl: ttl);
    }

    private async Task<CacheSetResponse> SendSetAsync(string cacheName, ByteString key, ByteString value, TimeSpan? ttl = null)
    {
        _SetRequest request = new _SetRequest() { CacheBody = value, CacheKey = key, TtlMilliseconds = TtlToMilliseconds(ttl) };
        var metadata = MetadataWithCache(cacheName);
        try
        {
            await this.grpcManager.Client.SetAsync(request, new CallOptions(headers: metadata, deadline: CalculateDeadline()));
        }
        catch (Exception e)
        {
            return new CacheSetResponse.Error(_exceptionMapper.Convert(e, metadata));
        }
        return new CacheSetResponse.Success();
    }

    private async Task<CacheGetResponse> SendGetAsync(string cacheName, ByteString key)
    {
        _GetRequest request = new _GetRequest() { CacheKey = key };
        _GetResponse response;
        var metadata = MetadataWithCache(cacheName);
        try
        {
            response = await this.grpcManager.Client.GetAsync(request, new CallOptions(headers: metadata, deadline: CalculateDeadline()));
        }
        catch (Exception e)
        {
            return new CacheGetResponse.Error(_exceptionMapper.Convert(e, metadata));
        }

        if (response.Result == ECacheResult.Miss)
        {
            return new CacheGetResponse.Miss();
        }
        return new CacheGetResponse.Hit(response);
    }

    private async Task<CacheDeleteResponse> SendDeleteAsync(string cacheName, ByteString key)
    {
        _DeleteRequest request = new _DeleteRequest() { CacheKey = key };
        var metadata = MetadataWithCache(cacheName);
        try
        {
            await this.grpcManager.Client.DeleteAsync(request, new CallOptions(headers: metadata, deadline: CalculateDeadline()));
        }
        catch (Exception e)
        {
            return new CacheDeleteResponse.Error(_exceptionMapper.Convert(e, metadata));
        }
        return new CacheDeleteResponse.Success();
    }
}
