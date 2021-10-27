using System;
using System.Text;
using CacheClient;
using Google.Protobuf;
namespace MomentoSdk.Responses
{
    public class CacheGetResponse : BaseCacheResponse
    {
        public MomentoCacheResult result { get; private set; }
        private byte[] body;
        public CacheGetResponse(GetResponse response)
        {
            body = response.CacheBody.ToByteArray();
            result = this.ResultMapper(response.Result);
        }

        public String Text()
        {
            return Encoding.UTF8.GetString(this.body, 0, this.body.Length);
        }

        public byte[] Bytes()
        {
            return this.body;
        }
    }
}
