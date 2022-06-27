using System;
using System.Linq;
using System.Text;
using Grpc.Core;
using Grpc.Proxy.Tools.Extensions;
using Grpc.Proxy.Tools.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Grpc.Proxy.Tools.Generators.Impl;

internal sealed class GrpcServiceProxyGenerator : GrpcSourceTextGenerator
{
    public GrpcServiceProxyGenerator(GrpcSourceTextGeneratorContext context) : base(context)
    {
    }

    protected override void WriteHeader(StringBuilder gen)
    {
        gen.AppendLine("using grpc = global::Grpc.Core;");
        gen.AppendLine("using grpcProxy = global::Grpc.Proxy;");
        gen.AppendLine();

        if (!string.IsNullOrEmpty(Context.ServiceNamespace))
            gen.AppendLine($"namespace {Context.ServiceNamespace};");

        gen.Append($@"
{Context.ServiceTypeSyntax.Modifiers} class {Context.ServiceType.Name}
{{
    public class {Context.ServiceName}Proxy : {Context.BaseDeclaration.TypeSymbol.Name}, grpcProxy::IProxyService
    {{
        static {Context.ServiceName}Proxy()
        {{
            grpcProxy::GrpcProxyReflection.Add<{Context.ServiceName}Proxy>(Descriptor);
        }}

        private readonly grpcProxy::IProxyCallInvoker _invoker;

        public {Context.ServiceName}Proxy(grpcProxy::IProxyCallInvoker callInvoker)
        {{
            _invoker = callInvoker;
        }}
");
    }

    protected override void WriteMethod(StringBuilder gen, string methodField, GrpcMethodDeclaration method)
    {
        (var returnType, ParameterInfo[] parameters) = GetMethodLayout(method);
        var invokerMethodName = $"{method.Type.ToString()}Call";

        gen.Append($@"
        public override {returnType} {method.Name}({string.Join(", ", parameters)})
        {{
            return _invoker.{invokerMethodName}({methodField}, {string.Join(", ", parameters.Select(x => x.Name))});
        }}
");
    }

    protected override void WriteFooter(StringBuilder gen)
    {
        gen.Append(@"
    }
}");
    }

    protected override void OnComplete(SourceProductionContext context, SourceText sourceText)
    {
        context.AddSource($"{Context.ServiceType.Name}.{Context.ServiceName}Proxy.gen.cs", sourceText);
    }

    private static (string ReturnType, ParameterInfo[] Parameters) GetMethodLayout(GrpcMethodDeclaration method)
    {
        var requestType = $"global::{method.Request.ContainingNamespace.GetFullName()}.{method.Request.Name}";
        var responseType = $"global::{method.Response.ContainingNamespace.GetFullName()}.{method.Response.Name}";
        var serverCallContext = new ParameterInfo("grpc::ServerCallContext", "context");

        return method.Type switch
        {
            MethodType.Unary => (
                Task(responseType),
                new[]
                {
                    new ParameterInfo(requestType, "request"),
                    serverCallContext
                }
            ),
            MethodType.ClientStreaming => (
                Task(responseType),
                new[]
                {
                    new ParameterInfo(IAsyncStreamReader(requestType), "requestStream"),
                    serverCallContext
                }
            ),
            MethodType.ServerStreaming => (
                Task(null),
                new[]
                {
                    new ParameterInfo(requestType, "request"),
                    new ParameterInfo(IServerStreamWriter(responseType), "responseStream"),
                    serverCallContext
                }
            ),
            MethodType.DuplexStreaming => (
                Task(null),
                new[]
                {
                    new ParameterInfo(IAsyncStreamReader(requestType), "requestStream"),
                    new ParameterInfo(IServerStreamWriter(responseType), "responseStream"),
                    serverCallContext
                }
            ),
            _ => throw new InvalidOperationException()
        };

        static string Task(string genericType)
        {
            if (genericType is null)
                return "global::System.Threading.Tasks.Task";

            return $"global::System.Threading.Tasks.Task<{genericType}>";
        }

        static string IAsyncStreamReader(string genericType)
        {
            return $"grpc::IAsyncStreamReader<{genericType}>";
        }

        static string IServerStreamWriter(string genericType)
        {
            return $"grpc::IServerStreamWriter<{genericType}>";
        }
    }
}
