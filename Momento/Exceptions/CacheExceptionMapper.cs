using System;
using Grpc.Core;
namespace MomentoSdk.Exceptions
{
    class CacheExceptionMapper
    {
        public static Exception Convert(Exception e)
        {
            if (e is SdkException)
            {
                return (SdkException)e;
            }
            if(e is RpcException)
            {
                RpcException ex = (RpcException)e;
                if (ex.StatusCode == StatusCode.PermissionDenied)
                {
                    return new PermissionDeniedException(ex.Message);
                }
                if (ex.StatusCode == StatusCode.NotFound)
                {
                    return new CacheNotFoundException(ex.Message);
                }
                if (ex.StatusCode == StatusCode.InvalidArgument)
                {
                    return new IllegalArgumentException(ex.Message);
                }
            }
            return new InternalServerException("unable to process request");
        }
    }
}
