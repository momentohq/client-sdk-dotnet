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
  public StatusCode code;
  public string details;
  public Metadata? metadata = null;

  public MomentoGrpcErrorDetails(StatusCode code, string details, Metadata? metadata)
  {
    this.code = code;
    this.details = details;
    this.metadata = metadata;
  }

}

public class MomentoErrorTransportDetails {
  public MomentoGrpcErrorDetails grpc;

  public MomentoErrorTransportDetails(MomentoGrpcErrorDetails grpc) {
    this.grpc = grpc;
  }
}

public abstract class SdkException : Exception
{
    public MomentoErrorCode errorCode;
    public MomentoErrorTransportDetails? transportDetails = null;

    protected SdkException(MomentoErrorCode errorCode, string message, MomentoErrorTransportDetails? transportDetails=null, Exception? e=null) : base(message, e)
    {
      this.errorCode = errorCode;
      this.transportDetails = transportDetails;
    }
}
