using System;
using CacheClient;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using System.Collections.Generic;
using static System.Reflection.Assembly;

namespace MomentoSdk
{
    internal sealed class DataGrpcManager : IDisposable
    {
        private readonly GrpcChannel channel;
        private readonly Scs.ScsClient client;
        private volatile Boolean isUserAgentSent = false;
        private readonly string version = GetAssembly(typeof(MomentoSdk.Responses.CacheGetResponse)).GetName().Version.ToString();

        internal DataGrpcManager(string authToken, string endpoint)
        {
            this.channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions() { Credentials = ChannelCredentials.SecureSsl });
            List<Header> headers = new List<Header> { new Header(name: "Authorization", value: authToken) };
            if (!isUserAgentSent) {
                headers.Add(new Header(name: "Agent", value: version));
                isUserAgentSent = true;
            }
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
