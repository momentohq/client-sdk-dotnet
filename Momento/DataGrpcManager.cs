using System;
using CacheClient;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;

namespace MomentoSdk
{
    public class DataGrpcManager : IDisposable
    {
        private readonly GrpcChannel channel;
        private readonly Scs.ScsClient client;
        private bool disposedValue;

        public DataGrpcManager(string authToken, string endpoint)
        {
            this.channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions() { Credentials = ChannelCredentials.SecureSsl });
            Header[] headers = { new Header(name: "Authorization", value: authToken) };
            CallInvoker invoker = this.channel.Intercept(new HeaderInterceptor(headers));
            this.client = new Scs.ScsClient(invoker);
        }

        public Scs.ScsClient Client()
        {
            return this.client;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.channel.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
