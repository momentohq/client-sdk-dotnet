using System;
using Grpc.Core;

namespace Momento.Sdk.Exceptions;

/// <summary>
/// A list of all available Momento error codes.  These can be used to check
/// for specific types of errors on a failure response.
/// </summary>
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

/// <summary>
/// Captures low-level information about an error, at the gRPC level.  Hopefully
/// this is only needed in rare cases, by Momento engineers, for debugging.
/// </summary>
public class MomentoGrpcErrorDetails
{
    /// <summary>
    /// The gRPC status code of the error repsonse
    /// </summary>
    public StatusCode Code { get; }
    /// <summary>
    /// Detailed information about the error
    /// </summary>
    public string Details { get; }
    /// <summary>
    /// Headers and other information about the error response
    /// </summary>
    public Metadata? Metadata { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="code"></param>
    /// <param name="details"></param>
    /// <param name="metadata"></param>
    public MomentoGrpcErrorDetails(StatusCode code, string details, Metadata? metadata = null)
    {
        this.Code = code;
        this.Details = details;
        this.Metadata = metadata;
    }
}

/// <summary>
/// Container for low-level error information, including details from the transport layer. 
/// </summary>
public class MomentoErrorTransportDetails
{
    /// <summary>
    /// 
    /// </summary>
    public MomentoGrpcErrorDetails Grpc { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="grpc"></param>
    public MomentoErrorTransportDetails(MomentoGrpcErrorDetails grpc)
    {
        this.Grpc = grpc;
    }
}

/// <summary>
/// Base class for all Momento client exceptions
/// </summary>
public abstract class SdkException : Exception
{
    /// <summary>
    /// Enumeration of all possible Momento error types.  Should be used in
    /// code to distinguish between different types of errors.
    /// </summary>
    public MomentoErrorCode ErrorCode { get; }
    /// <summary>
    /// Low-level error details, from the transport layer.  Hopefully only needed
    /// in rare cases, by Momento engineers, for debugging.
    /// </summary>
    public MomentoErrorTransportDetails? TransportDetails { get; }
    /// <summary>
    /// Prefix with basic information about the error class; this will be appended
    /// with specific information about the individual error instance at runtime.
    /// </summary>
    public string MessageWrapper { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="message"></param>
    /// <param name="transportDetails"></param>
    /// <param name="e"></param>
    protected SdkException(MomentoErrorCode errorCode, string message, MomentoErrorTransportDetails? transportDetails = null, Exception? e = null) : base(message, e)
    {
        this.ErrorCode = errorCode;
        this.TransportDetails = transportDetails;
        this.MessageWrapper = "";
    }
}
