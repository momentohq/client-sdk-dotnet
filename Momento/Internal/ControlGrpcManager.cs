using System;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using ControlClient;
using System.Collections.Generic;
using static System.Reflection.Assembly;

namespace MomentoSdk.Internal;

internal sealed class ControlGrpcManager : IDisposable
{
    private readonly GrpcChannel channel;
    public ScsControl.ScsControlClient Client { get; }
    private readonly string version = "csharp:" + GetAssembly(typeof(MomentoSdk.Responses.CacheGetResponse)).GetName().Version.ToString();

    public ControlGrpcManager(string authToken, string endpoint)
    {
        var uri = $"https://{endpoint}";
        this.channel = GrpcChannel.ForAddress(uri, new GrpcChannelOptions() { Credentials = ChannelCredentials.SecureSsl });
        List<Header> headers = new List<Header> { new Header(name: Header.AuthorizationKey, value: authToken), new Header(name: Header.AgentKey, value: version) };
        CallInvoker invoker = channel.Intercept(new HeaderInterceptor(headers));
        Client = new ScsControl.ScsControlClient(invoker);
    }

    public void Dispose()
    {
        this.channel.Dispose();
        GC.SuppressFinalize(this);
    }
}
