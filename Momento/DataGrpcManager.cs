using System;
using CacheClient;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;

namespace MomentoSdk
{
    internal sealed class DataGrpcManager : IDisposable
    {
        private readonly GrpcChannel channel;
        private readonly Scs.ScsClient client;

        internal DataGrpcManager(string authToken, string endpoint)
        {
            this.channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions() { Credentials = ChannelCredentials.SecureSsl });
            Header[] headers = { new Header(name: "Authorization", value: authToken) };
            CallInvoker invoker = this.channel.Intercept(new HeaderInterceptor(headers));
            this.client = new Scs.ScsClient(invoker);
        }

        internal Scs.ScsClient Client()
        {
            return this.client;
        }

        public void Dispose()
        {
            this.channel.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
