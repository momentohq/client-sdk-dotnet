using System;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Momento.Protos.CacheClient;
using static System.Reflection.Assembly;

namespace Momento.Sdk.Internal;

public class DataGrpcManager : IDisposable
{
    private readonly GrpcChannel channel;
    public Scs.ScsClient Client { get; }

    private readonly string version = "csharp:" + GetAssembly(typeof(Momento.Sdk.Responses.CacheGetResponse)).GetName().Version.ToString();

    internal DataGrpcManager(string authToken, string host)
    {
        var url = $"https://{host}";
        this.channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions() { Credentials = ChannelCredentials.SecureSsl });
        List<Header> headers = new List<Header> { new Header(name: Header.AuthorizationKey, value: authToken), new Header(name: Header.AgentKey, value: version) };
        CallInvoker invoker = this.channel.Intercept(new HeaderInterceptor(headers));
        Client = new Scs.ScsClient(invoker);
    }

    public void Dispose()
    {
        this.channel.Dispose();
        GC.SuppressFinalize(this);
    }
}
