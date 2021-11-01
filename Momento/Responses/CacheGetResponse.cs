using System;
using System.Text;
using CacheClient;
using Google.Protobuf;
namespace MomentoSdk.Responses
{
    public class CacheGetResponse : BaseCacheResponse
    {
        public MomentoCacheResult Result { get; private set; }
        private readonly ByteString body;

        public CacheGetResponse(GetResponse response)
        {
            body = response.CacheBody;
            Result = this.ResultMapper(response.Result);
        }

        public String String()
        {
            return String(Encoding.UTF8);
        }

        public String String(Encoding encoding)
        {
            if (Result == MomentoCacheResult.Hit)
            {
                return body.ToString(encoding);
            }
            return null;
        }

        public byte[] Bytes()
        {
            if (Result == MomentoCacheResult.Hit)
            {
                return this.body.ToByteArray();
            }
            return null;
        }
    }
}
