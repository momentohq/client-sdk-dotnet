using System;

namespace MomentoSdk.Responses
{
    public class CacheMultiGetFailureResponse
    {
        public byte[] Key { get; private set; }
        public Exception Failure { get; private set; }

        public CacheMultiGetFailureResponse(byte[] key, Exception failure)
        {
            Key = key;
            Failure = failure;
        }
    }
}
