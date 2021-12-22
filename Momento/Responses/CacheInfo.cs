namespace MomentoSdk.Responses
{
    public class CacheInfo
    {
        private readonly string name;
        public CacheInfo(string cachename)
        {
            this.name = cachename;
        }

        public string Name()
        {
            return this.name;
        }
    }
}
