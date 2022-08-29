using Google.Protobuf;
using Momento.Protos.CacheClient;

namespace Momento.Sdk.Responses;

/// <summary>
/// Base class encapsulating a state of a key in the cache and its value.
/// Derived classes unpack the values from gRPC response objects in particular ways.
/// </summary>
public class CacheGetResponseBase
{
    public CacheGetStatus Status { get; }
    protected readonly ByteString? value;

    public CacheGetResponseBase(CacheGetStatus status, ByteString? value)
    {
        Status = status;
        this.value = (Status == CacheGetStatus.HIT) ? value : null;
    }

    public CacheGetResponseBase(ECacheResult status, ByteString value) : this(CacheGetStatusUtil.From(status), value)
    {
    }

    public byte[]? ByteArray
    {
        get => value != null ? value.ToByteArray() : null;
    }

    public string? String() => (value != null) ? value.ToStringUtf8() : null;
}
