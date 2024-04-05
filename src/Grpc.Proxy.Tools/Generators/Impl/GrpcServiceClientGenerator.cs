using System.Text;
using Grpc.Proxy.Tools.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Grpc.Proxy.Tools.Generators.Impl;

internal sealed class GrpcServiceClientGenerator : GrpcSourceTextGenerator
{
    public GrpcServiceClientGenerator(GrpcSourceTextGeneratorContext context) : base(context)
    {
    }

    protected override void WriteHeader(StringBuilder gen)
    {
        if (!string.IsNullOrEmpty(Context.ServiceNamespace))
            gen.AppendLine($"namespace {Context.ServiceNamespace};");

        gen.Append($@"
{Context.ServiceTypeSyntax.Modifiers} class {Context.ServiceType.Name}
{{
    public partial class {Context.ServiceName}Client : I{Context.ServiceType.Name}Client
    {{
    }}
}}");
    }

    protected override void WriteMethod(StringBuilder gen, string methodField, GrpcMethodDeclaration method)
    {
    }

    protected override void WriteFooter(StringBuilder gen)
    {
    }

    protected override void OnComplete(SourceProductionContext context, SourceText sourceText)
    {
        var serviceNamespace = Context.ServiceNamespace;

        if (!string.IsNullOrEmpty(serviceNamespace))
            serviceNamespace += '.';

        context.AddSource($"{serviceNamespace}{Context.ServiceType.Name}.{Context.ServiceName}Client.gen.cs", sourceText);
    }
}
