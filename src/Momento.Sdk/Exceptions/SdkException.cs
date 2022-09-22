using System;
using Grpc.Core;

namespace Momento.Sdk.Exceptions;

public enum MomentoErrorCode {
  INVALID_ARGUMENT_ERROR,
  UNKNOWN_SERVICE_ERROR,
  ALREADY_EXISTS_ERROR,
  NOT_FOUND_ERROR,
  INTERNAL_SERVER_ERROR,
  PERMISSION_ERROR,
  AUTHENTICATION_ERROR,
  CANCELLED_ERROR,
  LIMIT_EXCEEDED_ERROR,
  BAD_REQUEST_ERROR,
  TIMEOUT_ERROR,
  SERVER_UNAVAILABLE,
  CLIENT_RESOURCE_EXHAUSTED,
  FAILED_PRECONDITION_ERROR,
  UNKNOWN_ERROR
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
