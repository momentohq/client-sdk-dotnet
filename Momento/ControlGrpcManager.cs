using System;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using ControlClient;
using System.Collections.Generic;
using static System.Reflection.Assembly;

namespace MomentoSdk;

internal sealed class ControlGrpcManager : IDisposable
{
    private readonly GrpcChannel channel;
    private readonly ScsControl.ScsControlClient client;
    private readonly string version = "csharp:" + GetAssembly(typeof(MomentoSdk.Responses.CacheGetResponse)).GetName().Version.ToString();

    public ControlGrpcManager(string authToken, string endpoint)
    {
        this.channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions() { Credentials = ChannelCredentials.SecureSsl });
        List<Header> headers = new List<Header> { new Header(name: Header.AuthorizationKey, value: authToken), new Header(name: Header.AgentKey, value: version) };
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
