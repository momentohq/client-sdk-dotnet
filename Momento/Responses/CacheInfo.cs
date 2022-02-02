namespace MomentoSdk.Responses
{
    public class CacheInfo
    {
        private readonly string name;
        public CacheInfo(string cachename)
        {
            name = cachename;
        }

        public string Name()
        {
            return name;
        }
    }
}
