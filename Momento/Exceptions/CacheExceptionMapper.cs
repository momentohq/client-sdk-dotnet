using System;
using Grpc.Core;
namespace MomentoSdk.Exceptions
{
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
                return ex.StatusCode switch
                {
                    StatusCode.InvalidArgument or StatusCode.OutOfRange or StatusCode.FailedPrecondition or StatusCode.Unimplemented => new BadRequestException(ex.Message),
                    StatusCode.PermissionDenied => new PermissionDeniedException(ex.Message),
                    StatusCode.Unauthenticated => new AuthenticationException(ex.Message),
                    StatusCode.ResourceExhausted => new LimitExceededException(ex.Message),
                    StatusCode.NotFound => new NotFoundException(ex.Message),
                    StatusCode.AlreadyExists => new AlreadyExistsException(ex.Message),
                    StatusCode.DeadlineExceeded => new TimeoutException(ex.Message),
                    StatusCode.Cancelled => new CancelledException(ex.Message),
                    _ => new InternalServerException(INTERNAL_SERVER_ERROR_MESSAGE),
                };
            }
            return new ClientSdkException(SDK_ERROR_MESSAGE, e);
        }
    }
}
