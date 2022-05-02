using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Google.Protobuf.Reflection;
using Grpc.Core;

namespace Grpc.Proxy;

public sealed class ProxyCallRouter : IProxyCallInvoker
{
    private readonly Dictionary<string, CallInvoker> _routes = new();

    public void Add(ServiceDescriptor descriptor, CallInvoker callInvoker)
    {
        foreach (var method in descriptor.Methods)
            _routes.Add($"/{method.Service.Name}/{method.Name}", callInvoker);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private CallInvoker GetCallInvoker(IMethod method)
    {
        if (!_routes.TryGetValue(method.FullName, out var invoker))
            throw new RpcException(new Status(StatusCode.Unimplemented, string.Empty));

        return invoker;
    }

    public Task<TResponse> UnaryCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method,
        TRequest request,
        ServerCallContext context)
        where TRequest : class
        where TResponse : class
    {
        return ProxyCallInvoker.UnaryCallCore(GetCallInvoker(method), method, request, context);
    }

    public Task<TResponse> ClientStreamingCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method,
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context)
        where TRequest : class
        where TResponse : class
    {
        return ProxyCallInvoker.ClientStreamingCallCore(GetCallInvoker(method), method, requestStream, context);
    }

    public Task ServerStreamingCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method,
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context)
        where TRequest : class
        where TResponse : class
    {
        return ProxyCallInvoker.ServerStreamingCallCore(GetCallInvoker(method), method, request, responseStream, context);
    }

    public Task DuplexStreamingCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method,
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context)
        where TRequest : class
        where TResponse : class
    {
        return ProxyCallInvoker.DuplexStreamingCallCore(GetCallInvoker(method), method, requestStream, responseStream, context);
    }
}
