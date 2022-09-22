﻿namespace Momento.Sdk.Responses;

using Momento.Sdk.Exceptions;

public class DeleteCacheResponse
{

    public class Success : DeleteCacheResponse { }

    public class Error: DeleteCacheResponse
    {
        private readonly SdkException _error;
        public Error(SdkException error)
        {
            _error = error;
        }

        public SdkException Exception
        {
            get => _error;
        }
    }
}
