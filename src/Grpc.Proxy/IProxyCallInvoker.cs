using System.Threading.Tasks;
using Grpc.Core;

namespace Grpc.Proxy.Core;

public interface IProxyCallInvoker
{
    Task<TResponse> UnaryCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method,
        TRequest request,
        ServerCallContext context)
        where TRequest : class
        where TResponse : class;

    Task<TResponse> ClientStreamingCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method,
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context)
        where TRequest : class
        where TResponse : class;

    Task ServerStreamingCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method,
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context)
        where TRequest : class
        where TResponse : class;

    Task DuplexStreamingCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method,
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context)
        where TRequest : class
        where TResponse : class;
}
