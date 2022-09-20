﻿using System;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Momento.Protos.ControlClient;

namespace Momento.Sdk.Internal;

internal sealed class ControlGrpcManager : IDisposable
{
    private readonly GrpcChannel channel;
    public ScsControl.ScsControlClient Client { get; }
    private readonly string version = "dotnet:" + System.Environment.Version;

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
