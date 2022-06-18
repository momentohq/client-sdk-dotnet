using System.Collections.Generic;

namespace MomentoSdk.Responses
{
    public class CacheMultiSetResponse
    {
        private readonly IDictionary<object, object> items;

        public CacheMultiSetResponse(IDictionary<object, object> items)
        {
            this.items = items;
        }

        public IDictionary<string, string> Strings()
        {
            return (IDictionary<string, string>)items;
        }

        public IDictionary<byte[], byte[]> Bytes()
        {
            return (IDictionary<byte[], byte[]>)items;
        }
    }
}
