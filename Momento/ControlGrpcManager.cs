using System;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using ControlClient;

namespace MomentoSdk
{
    internal sealed class ControlGrpcManager : IDisposable
    {
        private readonly GrpcChannel channel;
        private readonly ScsControl.ScsControlClient client;

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

        public void Dispose()
        {
            this.channel.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
