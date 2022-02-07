using System.Text;
using CacheClient;
using Google.Protobuf;
using MomentoSdk.Exceptions;

namespace MomentoSdk.Responses
{
    public class CacheGetResponse
    {
        public CacheGetStatus Status { get; private set; }
        private readonly ByteString body;

        public CacheGetResponse(_GetResponse response)
        {
            body = response.CacheBody;
            Status = From(response.Result);
        }

        public string String()
        {
            return String(Encoding.UTF8);
        }

        public string String(Encoding encoding)
        {
            if (Status == CacheGetStatus.HIT)
            {
                return body.ToString(encoding);
            }
            return null;
        }

        public byte[] Bytes()
        {
            if (Status == CacheGetStatus.HIT)
            {
                return body.ToByteArray();
            }
            return null;
        }


        private static CacheGetStatus From(ECacheResult result)
        {
            switch (result)
            {
                case ECacheResult.Hit: return CacheGetStatus.HIT;
                case ECacheResult.Miss: return CacheGetStatus.MISS;
                default: throw new InternalServerException("Invalid Cache Status.");
            }
        }

    }
}
