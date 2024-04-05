using System;
using System.Text;
using Grpc.Core;
using Grpc.Proxy.Tools.Extensions;
using Grpc.Proxy.Tools.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Grpc.Proxy.Tools.Generators.Impl;

internal sealed class GrpcServiceClientInterfaceGenerator : GrpcSourceTextGenerator
{
    public GrpcServiceClientInterfaceGenerator(GrpcSourceTextGeneratorContext context) : base(context)
    {
    }

    protected override void WriteHeader(StringBuilder gen)
    {
        gen.AppendLine("using grpc = global::Grpc.Core;");
        gen.AppendLine("using grpcProxy = global::Grpc.Proxy;");
        gen.AppendLine();

        if (!string.IsNullOrEmpty(Context.ServiceNamespace))
            gen.AppendLine($"namespace {Context.ServiceNamespace};");

        var modifiers = Context.ServiceTypeSyntax.Modifiers.Any(SyntaxKind.PublicKeyword) ? "public" : "internal";

        gen.Append($@"
[grpcProxy::GrpcServiceClient(typeof({Context.ServiceType.Name}.{Context.ServiceName}Client))]
{modifiers} interface I{Context.ServiceType.Name}Client
{{
");
    }

    protected override void WriteMethod(StringBuilder gen, string methodField, GrpcMethodDeclaration method)
    {
        const string callOptionsFlatten = "grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken)";
        (var syncReturnType, var asyncReturnType, ParameterInfo[] parameters) = GetMethodLayout(method);

        // Sync
        if (syncReturnType != null)
        {
            // Overload 1
            gen.Append($"    {syncReturnType} {method.Name}(");
            WriteParams().Append(callOptionsFlatten).AppendLine(");");

            // Overload 2
            gen.Append($"    {syncReturnType} {method.Name}(");
            WriteParams().AppendLine("grpc::CallOptions options);");
        }

        // Async
        var asyncMethodName = method.Type is MethodType.Unary
            ? $"{method.Name}Async"
            : method.Name;

        // Overload 1
        gen.Append($"    {asyncReturnType} {asyncMethodName}(");
        WriteParams().Append(callOptionsFlatten).AppendLine(");");

        // Overload 2
        gen.Append($"    {asyncReturnType} {asyncMethodName}(");
        WriteParams().AppendLine("grpc::CallOptions options);");

        StringBuilder WriteParams()
        {
            if (parameters.Length > 0)
                gen.Append(string.Join(", ", parameters)).Append(", ");

            return gen;
        }
    }

    protected override void WriteFooter(StringBuilder gen)
    {
        gen.Append(@"
}");
    }

    protected override void OnComplete(SourceProductionContext context, SourceText sourceText)
    {
        var serviceNamespace = Context.ServiceNamespace;

        if (!string.IsNullOrEmpty(serviceNamespace))
            serviceNamespace += '.';

        context.AddSource($"{serviceNamespace}I{Context.ServiceName}Client.gen.cs", sourceText);
    }

    private static (string SyncReturnType, string AsyncReturnType, ParameterInfo[] Parameters) GetMethodLayout(GrpcMethodDeclaration method)
    {
        var requestType = $"global::{method.Request.ContainingNamespace.GetFullName()}.{method.Request.Name}";
        var responseType = $"global::{method.Response.ContainingNamespace.GetFullName()}.{method.Response.Name}";

        return method.Type switch
        {
            MethodType.Unary => (
                responseType,
                AsyncUnaryCall(responseType),
                new[]
                {
                    new ParameterInfo(requestType, "request")
                }
            ),
            MethodType.ClientStreaming => (
                null,
                AsyncClientStreamingCall(requestType, responseType),
                Array.Empty<ParameterInfo>()
            ),
            MethodType.ServerStreaming => (
                null,
                AsyncServerStreamingCall(responseType),
                new[]
                {
                    new ParameterInfo(requestType, "request")
                }
            ),
            MethodType.DuplexStreaming => (
                null,
                AsyncDuplexStreamingCall(requestType, responseType),
                Array.Empty<ParameterInfo>()
            ),
            _ => throw new InvalidOperationException()
        };

        static string AsyncUnaryCall(string genericType)
        {
            return $"grpc::AsyncUnaryCall<{genericType}>";
        }

        static string AsyncClientStreamingCall(string genericType1, string genericType2)
        {
            return $"grpc::AsyncClientStreamingCall<{genericType1}, {genericType2}>";
        }

        static string AsyncServerStreamingCall(string genericType)
        {
            return $"grpc::AsyncServerStreamingCall<{genericType}>";
        }

        static string AsyncDuplexStreamingCall(string genericType1, string genericType2)
        {
            return $"grpc::AsyncDuplexStreamingCall<{genericType1}, {genericType2}>";
        }
    }
}
