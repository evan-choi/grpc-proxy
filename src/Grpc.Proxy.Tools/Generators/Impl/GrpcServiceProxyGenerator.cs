using System.Linq;
using System.Text;
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
    public class {Context.ServiceName}Proxy : {Context.BaseDeclaration.TypeSymbol.Name}
    {{
        private readonly grpcProxy::IProxyCallInvoker _invoker;

        public {Context.ServiceName}Proxy(grpcProxy::IProxyCallInvoker callInvoker)
        {{
            _invoker = callInvoker;
        }}
");
    }

    protected override void WriteMethod(StringBuilder gen, string methodField, GrpcMethodDeclaration method)
    {
        var invokerMethodName = $"{method.Type.ToString()}Call";

        gen.Append($@"
        public override {method.ReturnType} {method.Name}({string.Join(", ", method.Parameters)}, grpc::ServerCallContext context)
        {{
            return _invoker.{invokerMethodName}({methodField}, {string.Join(", ", method.Parameters.Select(x => x.Name))}, context);
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
}
