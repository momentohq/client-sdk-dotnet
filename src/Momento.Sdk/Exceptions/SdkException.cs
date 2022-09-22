using System;
using Grpc.Core;

namespace Momento.Sdk.Exceptions;

public enum MomentoErrorCode {
  INVALID_ARGUMENT_ERROR, // Invalid argument passed to Momento client
  UNKNOWN_SERVICE_ERROR,  // Service returned an unknown response
  ALREADY_EXISTS_ERROR,  // Cache with specified name already exists
  NOT_FOUND_ERROR,  // Cache with specified name doesn't exists
  INTERNAL_SERVER_ERROR,  // An unexpected error occurred while trying to fulfill the request
  PERMISSION_ERROR,  // Insufficient permissions to perform operation
  AUTHENTICATION_ERROR,  // Invalid authentication credentials to connect to cache service
  CANCELLED_ERROR,  // Request was cancelled by the server
  LIMIT_EXCEEDED_ERROR,  // Request rate exceeded the limits for the account
  BAD_REQUEST_ERROR,  // Request was invalid
  TIMEOUT_ERROR,  // Client's configured timeout was exceeded
  SERVER_UNAVAILABLE,  // Serer was unable to handle the request
  CLIENT_RESOURCE_EXHAUSTED,  // A client resource (most likely memory) was exhausted
  FAILED_PRECONDITION_ERROR,  // System is not in a state required for the operation's execution
  UNKNOWN_ERROR  // Unknown error has occurred
}

public class MomentoGrpcErrorDetails {
  public StatusCode Code { get; }
  public string Details { get; }
  public Metadata? Metadata { get; }

  public MomentoGrpcErrorDetails(StatusCode code, string details, Metadata? metadata=null)
  {
    this.Code = code;
    this.Details = details;
    this.Metadata = metadata;
  }

}

public class MomentoErrorTransportDetails {
  public MomentoGrpcErrorDetails Grpc { get; }

  public MomentoErrorTransportDetails(MomentoGrpcErrorDetails grpc) {
    this.Grpc = grpc;
  }
}

public abstract class SdkException : Exception
{
    public MomentoErrorCode ErrorCode { get; }
    public MomentoErrorTransportDetails? TransportDetails { get; }

    protected SdkException(MomentoErrorCode errorCode, string message, MomentoErrorTransportDetails? transportDetails=null, Exception? e=null) : base(message, e)
    {
      this.ErrorCode = errorCode;
      this.TransportDetails = transportDetails;
    }
}
