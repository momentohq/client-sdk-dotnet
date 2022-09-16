using System;
using Grpc.Core;

namespace Momento.Sdk.Exceptions;

class CacheExceptionMapper
{
    private const string INTERNAL_SERVER_ERROR_MESSAGE = "Unexpected exception occurred while trying to fulfill the request.";
    private const string SDK_ERROR_MESSAGE = "SDK Failed to process the request.";

    public static Exception Convert(Exception e)
    {
        if (e is SdkException exception)
        {
            return exception;
        }
        if (e is RpcException ex)
        {
            switch (ex.StatusCode)
            {
                case StatusCode.InvalidArgument:
                case StatusCode.OutOfRange:
                case StatusCode.Unimplemented:
                    return new BadRequestException(ex.Message);

                case StatusCode.FailedPrecondition:
                    return new FailedPreconditionException(ex.Message);

                case StatusCode.PermissionDenied:
                    return new PermissionDeniedException(ex.Message);

                case StatusCode.Unauthenticated:
                    return new AuthenticationException(ex.Message);

                case StatusCode.ResourceExhausted:
                    return new LimitExceededException(ex.Message);

                case StatusCode.NotFound:
                    return new NotFoundException(ex.Message);

                case StatusCode.AlreadyExists:
                    return new AlreadyExistsException(ex.Message);

                case StatusCode.DeadlineExceeded:
                    return new TimeoutException(ex.Message);

                case StatusCode.Cancelled:
                    return new CancelledException(ex.Message);

                case StatusCode.Unknown:
                case StatusCode.Unavailable:
                case StatusCode.Aborted:
                case StatusCode.DataLoss:
                default: return new InternalServerException(INTERNAL_SERVER_ERROR_MESSAGE, e);
            }
        }
        return new ClientSdkException(SDK_ERROR_MESSAGE, e);
    }
}
