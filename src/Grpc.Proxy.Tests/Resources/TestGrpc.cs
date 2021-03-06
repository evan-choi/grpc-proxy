// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: test.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace Grpc.Proxy.Tests.Proto {
  public static partial class Test
  {
    static readonly string __ServiceName = "a.b.c.d.Test";

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Grpc.Proxy.Tests.Proto.PingRequest> __Marshaller_a_b_c_d_PingRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Grpc.Proxy.Tests.Proto.PingRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Grpc.Proxy.Tests.Proto.PongResponse> __Marshaller_a_b_c_d_PongResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Grpc.Proxy.Tests.Proto.PongResponse.Parser));

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Grpc.Proxy.Tests.Proto.PingRequest, global::Grpc.Proxy.Tests.Proto.PongResponse> __Method_Ping = new grpc::Method<global::Grpc.Proxy.Tests.Proto.PingRequest, global::Grpc.Proxy.Tests.Proto.PongResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Ping",
        __Marshaller_a_b_c_d_PingRequest,
        __Marshaller_a_b_c_d_PongResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Grpc.Proxy.Tests.Proto.PingRequest, global::Grpc.Proxy.Tests.Proto.PongResponse> __Method_PingClientStream = new grpc::Method<global::Grpc.Proxy.Tests.Proto.PingRequest, global::Grpc.Proxy.Tests.Proto.PongResponse>(
        grpc::MethodType.ClientStreaming,
        __ServiceName,
        "PingClientStream",
        __Marshaller_a_b_c_d_PingRequest,
        __Marshaller_a_b_c_d_PongResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Grpc.Proxy.Tests.Proto.PingRequest, global::Grpc.Proxy.Tests.Proto.PongResponse> __Method_PingServerStream = new grpc::Method<global::Grpc.Proxy.Tests.Proto.PingRequest, global::Grpc.Proxy.Tests.Proto.PongResponse>(
        grpc::MethodType.ServerStreaming,
        __ServiceName,
        "PingServerStream",
        __Marshaller_a_b_c_d_PingRequest,
        __Marshaller_a_b_c_d_PongResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Grpc.Proxy.Tests.Proto.PingRequest, global::Grpc.Proxy.Tests.Proto.PongResponse> __Method_PingDuplexStream = new grpc::Method<global::Grpc.Proxy.Tests.Proto.PingRequest, global::Grpc.Proxy.Tests.Proto.PongResponse>(
        grpc::MethodType.DuplexStreaming,
        __ServiceName,
        "PingDuplexStream",
        __Marshaller_a_b_c_d_PingRequest,
        __Marshaller_a_b_c_d_PongResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Grpc.Proxy.Tests.Proto.TestReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of Test</summary>
    [grpc::BindServiceMethod(typeof(Test), "BindService")]
    public abstract partial class TestBase
    {
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::Grpc.Proxy.Tests.Proto.PongResponse> Ping(global::Grpc.Proxy.Tests.Proto.PingRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::Grpc.Proxy.Tests.Proto.PongResponse> PingClientStream(grpc::IAsyncStreamReader<global::Grpc.Proxy.Tests.Proto.PingRequest> requestStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task PingServerStream(global::Grpc.Proxy.Tests.Proto.PingRequest request, grpc::IServerStreamWriter<global::Grpc.Proxy.Tests.Proto.PongResponse> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task PingDuplexStream(grpc::IAsyncStreamReader<global::Grpc.Proxy.Tests.Proto.PingRequest> requestStream, grpc::IServerStreamWriter<global::Grpc.Proxy.Tests.Proto.PongResponse> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for Test</summary>
    public partial class TestClient : grpc::ClientBase<TestClient>
    {
      /// <summary>Creates a new client for Test</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public TestClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for Test that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public TestClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected TestClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected TestClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Grpc.Proxy.Tests.Proto.PongResponse Ping(global::Grpc.Proxy.Tests.Proto.PingRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return Ping(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Grpc.Proxy.Tests.Proto.PongResponse Ping(global::Grpc.Proxy.Tests.Proto.PingRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_Ping, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Grpc.Proxy.Tests.Proto.PongResponse> PingAsync(global::Grpc.Proxy.Tests.Proto.PingRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return PingAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Grpc.Proxy.Tests.Proto.PongResponse> PingAsync(global::Grpc.Proxy.Tests.Proto.PingRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_Ping, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncClientStreamingCall<global::Grpc.Proxy.Tests.Proto.PingRequest, global::Grpc.Proxy.Tests.Proto.PongResponse> PingClientStream(grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return PingClientStream(new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncClientStreamingCall<global::Grpc.Proxy.Tests.Proto.PingRequest, global::Grpc.Proxy.Tests.Proto.PongResponse> PingClientStream(grpc::CallOptions options)
      {
        return CallInvoker.AsyncClientStreamingCall(__Method_PingClientStream, null, options);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncServerStreamingCall<global::Grpc.Proxy.Tests.Proto.PongResponse> PingServerStream(global::Grpc.Proxy.Tests.Proto.PingRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return PingServerStream(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncServerStreamingCall<global::Grpc.Proxy.Tests.Proto.PongResponse> PingServerStream(global::Grpc.Proxy.Tests.Proto.PingRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncServerStreamingCall(__Method_PingServerStream, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncDuplexStreamingCall<global::Grpc.Proxy.Tests.Proto.PingRequest, global::Grpc.Proxy.Tests.Proto.PongResponse> PingDuplexStream(grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return PingDuplexStream(new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncDuplexStreamingCall<global::Grpc.Proxy.Tests.Proto.PingRequest, global::Grpc.Proxy.Tests.Proto.PongResponse> PingDuplexStream(grpc::CallOptions options)
      {
        return CallInvoker.AsyncDuplexStreamingCall(__Method_PingDuplexStream, null, options);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected override TestClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new TestClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static grpc::ServerServiceDefinition BindService(TestBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_Ping, serviceImpl.Ping)
          .AddMethod(__Method_PingClientStream, serviceImpl.PingClientStream)
          .AddMethod(__Method_PingServerStream, serviceImpl.PingServerStream)
          .AddMethod(__Method_PingDuplexStream, serviceImpl.PingDuplexStream).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static void BindService(grpc::ServiceBinderBase serviceBinder, TestBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_Ping, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::Grpc.Proxy.Tests.Proto.PingRequest, global::Grpc.Proxy.Tests.Proto.PongResponse>(serviceImpl.Ping));
      serviceBinder.AddMethod(__Method_PingClientStream, serviceImpl == null ? null : new grpc::ClientStreamingServerMethod<global::Grpc.Proxy.Tests.Proto.PingRequest, global::Grpc.Proxy.Tests.Proto.PongResponse>(serviceImpl.PingClientStream));
      serviceBinder.AddMethod(__Method_PingServerStream, serviceImpl == null ? null : new grpc::ServerStreamingServerMethod<global::Grpc.Proxy.Tests.Proto.PingRequest, global::Grpc.Proxy.Tests.Proto.PongResponse>(serviceImpl.PingServerStream));
      serviceBinder.AddMethod(__Method_PingDuplexStream, serviceImpl == null ? null : new grpc::DuplexStreamingServerMethod<global::Grpc.Proxy.Tests.Proto.PingRequest, global::Grpc.Proxy.Tests.Proto.PongResponse>(serviceImpl.PingDuplexStream));
    }

  }
}
#endregion
