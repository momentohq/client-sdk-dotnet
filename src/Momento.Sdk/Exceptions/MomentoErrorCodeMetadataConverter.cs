using System;

namespace Momento.Sdk.Exceptions;

/// <summary>
/// Converts Momento error codes to string values for use in metadata sent to momento-local.
/// </summary>
public static class MomentoErrorCodeMetadataConverter
{
    /// <summary>
    /// Converts the rpc method to a string value for use in metadata sent to momento-local.
    /// </summary>
    /// <param name="errorCode">to convert to a string</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">if given an unknown error code</exception>
    public static string ToStringValue(this MomentoErrorCode errorCode)
    {
      return errorCode switch
      {
        MomentoErrorCode.INVALID_ARGUMENT_ERROR => "invalid-argument",
        MomentoErrorCode.UNKNOWN_SERVICE_ERROR => "unknown",
        MomentoErrorCode.ALREADY_EXISTS_ERROR => "already-exists",
        MomentoErrorCode.NOT_FOUND_ERROR => "not-found",
        MomentoErrorCode.INTERNAL_SERVER_ERROR => "internal",
        MomentoErrorCode.PERMISSION_ERROR => "permission-denied",
        MomentoErrorCode.AUTHENTICATION_ERROR => "unauthenticated",
        MomentoErrorCode.CANCELLED_ERROR => "cancelled",
        MomentoErrorCode.CONNECTION_ERROR => "connection-error",
        MomentoErrorCode.LIMIT_EXCEEDED_ERROR => "resource-exhausted",
        MomentoErrorCode.BAD_REQUEST_ERROR => "invalid-argument",
        MomentoErrorCode.TIMEOUT_ERROR => "deadline-exceeded",
        MomentoErrorCode.SERVER_UNAVAILABLE => "unavailable",
        MomentoErrorCode.CLIENT_RESOURCE_EXHAUSTED => "resource-exhausted",
        MomentoErrorCode.UNKNOWN_ERROR => "unknown",
        _ => throw new ArgumentOutOfRangeException(nameof(errorCode), "Unknown error code to convert to string")
      };
    }
}