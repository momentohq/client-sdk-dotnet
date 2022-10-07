using System;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Momento.Sdk.Config.Retry
{
    public interface IRetryEligibilityStrategy
    {
        public ILoggerFactory? LoggerFactory { get; }
        public IRetryEligibilityStrategy WithLoggerFactory(ILoggerFactory loggerFactory);
        public bool IsEligibleForRetry<TRequest>(Status status, TRequest request) where TRequest : class;
    }
}

