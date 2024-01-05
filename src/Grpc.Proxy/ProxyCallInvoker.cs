using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Grpc.Core;

namespace Grpc.Proxy;

public sealed class ProxyCallInvoker : IProxyCallInvoker
{
    private readonly CallInvoker _invoker;

    public ProxyCallInvoker(CallInvoker callInvoker)
    {
        _invoker = callInvoker;
    }

    public Task<TResponse> UnaryCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method,
        TRequest request,
        ServerCallContext context)
        where TRequest : class
        where TResponse : class
    {
        return UnaryCallCore(_invoker, method, request, context);
    }

    public Task<TResponse> ClientStreamingCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method,
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context)
        where TRequest : class
        where TResponse : class
    {
        return ClientStreamingCallCore(_invoker, method, requestStream, context);
    }

    public Task ServerStreamingCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method,
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context)
        where TRequest : class
        where TResponse : class
    {
        return ServerStreamingCallCore(_invoker, method, request, responseStream, context);
    }

    public Task DuplexStreamingCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method,
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context)
        where TRequest : class
        where TResponse : class
    {
        return DuplexStreamingCallCore(_invoker, method, requestStream, responseStream, context);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<TResponse> UnaryCallCore<TRequest, TResponse>(
        CallInvoker invoker,
        Method<TRequest, TResponse> method,
        TRequest request,
        ServerCallContext context)
        where TRequest : class
        where TResponse : class
    {
        AsyncUnaryCall<TResponse> call = invoker.AsyncUnaryCall(
            method,
            context.Host,
            CreateCallOptions(context),
            request
        );

        await CopyResponseHeadersToContextAsync(call.ResponseHeadersAsync, context);

        return await call;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task<TResponse> ClientStreamingCallCore<TRequest, TResponse>(
        CallInvoker invoker,
        Method<TRequest, TResponse> method,
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context)
        where TRequest : class
        where TResponse : class
    {
        AsyncClientStreamingCall<TRequest, TResponse> streamingCall = invoker.AsyncClientStreamingCall(
            method,
            context.Host,
            CreateCallOptions(context)
        );

        QueueCopyResponseHeadersToContext(streamingCall.ResponseHeadersAsync, context);

        await CopyStreaming(requestStream, streamingCall.RequestStream, context);

        return await streamingCall.ResponseAsync;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task ServerStreamingCallCore<TRequest, TResponse>(
        CallInvoker invoker,
        Method<TRequest, TResponse> method,
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context)
        where TRequest : class
        where TResponse : class
    {
        AsyncServerStreamingCall<TResponse> streamingCall = invoker.AsyncServerStreamingCall(
            method,
            context.Host,
            CreateCallOptions(context),
            request
        );

        QueueCopyResponseHeadersToContext(streamingCall.ResponseHeadersAsync, context);

        await CopyStreaming(streamingCall.ResponseStream, responseStream, context);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async Task DuplexStreamingCallCore<TRequest, TResponse>(
        CallInvoker invoker,
        Method<TRequest, TResponse> method,
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context)
        where TRequest : class
        where TResponse : class
    {
        AsyncDuplexStreamingCall<TRequest, TResponse> streamingCall = invoker.AsyncDuplexStreamingCall(
            method,
            context.Host,
            CreateCallOptions(context)
        );

        QueueCopyResponseHeadersToContext(streamingCall.ResponseHeadersAsync, context);

        await Task.WhenAll(
            CopyStreaming(requestStream, streamingCall.RequestStream, context),
            CopyStreaming(streamingCall.ResponseStream, responseStream, context)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CallOptions CreateCallOptions(ServerCallContext context)
    {
        return new CallOptions(context.RequestHeaders, context.Deadline, context.CancellationToken, context.WriteOptions);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static async Task CopyStreaming<T>(
        IAsyncStreamReader<T> reader,
        IAsyncStreamWriter<T> writer,
        ServerCallContext context)
        where T : class
    {
        while (await reader.MoveNext(context.CancellationToken))
            await writer.WriteAsync(reader.Current);

        if (writer is IClientStreamWriter<T> clientStreamWriter)
            await clientStreamWriter.CompleteAsync();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static async Task CopyResponseHeadersToContextAsync(Task<Metadata> responseHeadersTask, ServerCallContext context)
    {
        var headers = await responseHeadersTask;

        if (headers.Count > 0)
            await context.WriteResponseHeadersAsync(headers);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void QueueCopyResponseHeadersToContext(Task<Metadata> responseHeadersTask, ServerCallContext context)
    {
        if (context.CancellationToken.IsCancellationRequested)
            return;

        responseHeadersTask.ContinueWith(
            ResponseHeadersCompletedAsync,
            context,
            context.CancellationToken,
            TaskContinuationOptions.OnlyOnRanToCompletion,
            TaskScheduler.Default
        );

        return;

        static async Task ResponseHeadersCompletedAsync(Task<Metadata> responseHeadersTask, object state)
        {
            try
            {
                var context = (ServerCallContext)state;
                var headers = responseHeadersTask.Result; // always completed. (OnlyOnRanToCompletion)

                if (headers.Count > 0)
                    await context.WriteResponseHeadersAsync(headers);
            }
            catch
            {
                // ignored
            }
        }
    }
}
