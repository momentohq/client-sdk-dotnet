using System;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using ControlClient;

namespace MomentoSdk
{
    public class ControlGrpcManager : IDisposable
    {
        private readonly GrpcChannel channel;
        private readonly ScsControl.ScsControlClient client;
        private bool disposedValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken">Momento jwt</param>
        /// <param name="endpoint">Control client endpint</param>
        public ControlGrpcManager(string authToken, string endpoint)
        {
            this.channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions() { Credentials = ChannelCredentials.SecureSsl });
            Header[] headers = { new Header(name: "Authorization", value: authToken) };
            CallInvoker invoker = channel.Intercept(new HeaderInterceptor(headers));
            this.client = new ScsControl.ScsControlClient(invoker);
        }

        public ScsControl.ScsControlClient Client()
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
