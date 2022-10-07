using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Momento.Sdk.Exceptions;

public class CacheExceptionMapper
{
    private const string INTERNAL_SERVER_ERROR_MESSAGE = "Unexpected exception occurred while trying to fulfill the request.";
    private const string SDK_ERROR_MESSAGE = "SDK Failed to process the request.";

    private readonly ILogger _logger;

    public CacheExceptionMapper(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CacheExceptionMapper>();
    }

    public SdkException Convert(Exception e)
    {
        _logger.LogDebug("Mapping exception to SdkException: {}", e);

        var unwrappedException = e;
        // If any Middleware uses a `ContinueWith` to modify the response, then
        // any Exception that gets thrown will be wrapped in an AggregateException.
        // Therefore we need to check for that case and unwrap here, otherwise
        // we will not surface the correct error code.
        if (e is AggregateException aggregateException)
        {
            unwrappedException = aggregateException.InnerException;
        }

        if (unwrappedException is SdkException exception)
        {
            return exception;
        }
        if (unwrappedException is RpcException ex)
        {
            MomentoErrorTransportDetails transportDetails = new MomentoErrorTransportDetails(
                new MomentoGrpcErrorDetails(ex.StatusCode, ex.Message, null)
            );

            switch (ex.StatusCode)
            {
                case StatusCode.InvalidArgument:
                    return new InvalidArgumentException(ex.Message, transportDetails);

                case StatusCode.OutOfRange:
                case StatusCode.Unimplemented:
                    return new BadRequestException(ex.Message, transportDetails);

                case StatusCode.FailedPrecondition:
                    return new FailedPreconditionException(ex.Message, transportDetails);

                case StatusCode.PermissionDenied:
                    return new PermissionDeniedException(ex.Message, transportDetails);

                case StatusCode.Unauthenticated:
                    return new AuthenticationException(ex.Message, transportDetails);

                case StatusCode.ResourceExhausted:
                    return new LimitExceededException(ex.Message, transportDetails);

                case StatusCode.NotFound:
                    return new NotFoundException(ex.Message, transportDetails);

                case StatusCode.AlreadyExists:
                    return new AlreadyExistsException(ex.Message, transportDetails);

                case StatusCode.DeadlineExceeded:
                    return new TimeoutException(ex.Message, transportDetails);

                case StatusCode.Cancelled:
                    return new CancelledException(ex.Message, transportDetails);

                case StatusCode.Unavailable:
                    return new ServerUnavailableException(ex.Message, transportDetails);

                case StatusCode.Unknown:
                    return new UnknownServiceException(ex.Message, transportDetails);

                case StatusCode.Aborted:
                case StatusCode.DataLoss:
                default: return new InternalServerException(INTERNAL_SERVER_ERROR_MESSAGE, transportDetails, unwrappedException);
            }
        }
        return new UnknownException(SDK_ERROR_MESSAGE, null, unwrappedException);
    }
}
