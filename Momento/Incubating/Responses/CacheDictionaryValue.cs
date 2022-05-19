using System.Collections.Generic;
using System.Text;
using Google.Protobuf;

namespace MomentoSdk.Incubating.Responses
{
    public class CacheDictionaryValue
    {
        private readonly ByteString body;

        public CacheDictionaryValue(ByteString body)
        {
            this.body = body;
        }

        public string String(Encoding encoding = null)
        {
            if (encoding is null)
                return body.ToString(Encoding.UTF8);
            return body.ToString(encoding);
        }

        public byte[] Bytes()
        {
            return body.ToByteArray();
        }
    }
}
