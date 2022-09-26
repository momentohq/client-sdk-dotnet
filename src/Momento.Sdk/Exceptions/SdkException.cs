using System;
using Grpc.Core;

namespace Momento.Sdk.Exceptions;

public enum MomentoErrorCode
{
    /// <summary>
    /// Invalid argument passed to Momento client
    /// </summary>
    INVALID_ARGUMENT_ERROR,
    /// <summary>
    /// Service returned an unknown response
    /// </summary>
    UNKNOWN_SERVICE_ERROR,
    /// <summary>
    /// Cache with specified name already exists
    /// </summary>
    ALREADY_EXISTS_ERROR,
    /// <summary>
    /// Cache with specified name doesn't exist
    /// </summary>
    NOT_FOUND_ERROR,
    /// <summary>
    /// An unexpected error occurred while trying to fulfill the request
    /// </summary>
    INTERNAL_SERVER_ERROR,
    /// <summary>
    /// Insufficient permissions to perform operation
    /// </summary>
    PERMISSION_ERROR,
    /// <summary>
    /// Invalid authentication credentials to connect to cache service
    /// </summary>
    AUTHENTICATION_ERROR,
    /// <summary>
    /// Request was cancelled by the server
    /// </summary>
    CANCELLED_ERROR,
    /// <summary>
    /// Request rate exceeded the limits for the account
    /// </summary>
    LIMIT_EXCEEDED_ERROR,
    /// <summary>
    /// Request was invalid
    /// </summary>
    BAD_REQUEST_ERROR,
    /// <summary>
    /// Client's configured timeout was exceeded
    /// </summary>
    TIMEOUT_ERROR,
    /// <summary>
    /// Server was unable to handle the request
    /// </summary>
    SERVER_UNAVAILABLE,
    /// <summary>
    /// A client resource (most likely memory) was exhausted
    /// </summary>
    CLIENT_RESOURCE_EXHAUSTED,
    /// <summary>
    /// System is not in a state required for the operation's execution
    /// </summary>
    FAILED_PRECONDITION_ERROR,
    /// <summary>
    /// Unknown error has occurred
    /// </summary>
    UNKNOWN_ERROR
}

public class MomentoGrpcErrorDetails {
  public StatusCode Code { get; }
  public string Details { get; }
  public Metadata? Metadata { get; set; }

    public MomentoGrpcErrorDetails(StatusCode code, string details, Metadata? metadata = null)
    {
        this.Code = code;
        this.Details = details;
        this.Metadata = metadata;
    }
}

public class MomentoErrorTransportDetails
{
    public MomentoGrpcErrorDetails Grpc { get; }

    public MomentoErrorTransportDetails(MomentoGrpcErrorDetails grpc)
    {
        this.Grpc = grpc;
    }
}

public abstract class SdkException : Exception
{
    public MomentoErrorCode ErrorCode { get; }
    public MomentoErrorTransportDetails? TransportDetails { get; }
    public string MessageWrapper { get; set; }

    protected SdkException(MomentoErrorCode errorCode, string message, MomentoErrorTransportDetails? transportDetails = null, Exception? e = null) : base(message, e)
    {
      this.ErrorCode = errorCode;
      this.TransportDetails = transportDetails;
      this.MessageWrapper = "";
    }
}
