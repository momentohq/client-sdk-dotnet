using System.Text;
using CacheClient;
using Google.Protobuf;
using MomentoSdk.Responses;
using MomentoSdk.Exceptions;


namespace MomentoSdk.Incubating.Responses
{
    public class CacheDictionaryGetResponse
    {
        public CacheGetStatus Status { get; private set; }
        private readonly ByteString body;

        public CacheDictionaryGetResponse()
        {
        }

        public string? String(Encoding encoding = null)
        {
            if (Status == CacheGetStatus.MISS)
                return null;
            if (encoding is null)
                return body.ToString(Encoding.UTF8);
            return body.ToString(encoding);
        }

        public byte[]? Bytes()
        {
            if (Status == CacheGetStatus.MISS)
                return null;
            return body.ToByteArray();
        }
    }
}
