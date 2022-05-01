using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Grpc.Core;
using Grpc.Proxy.Tools.Extensions;
using Grpc.Proxy.Tools.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Grpc.Proxy.Tools;

[Generator]
public class GrpcServiceProxyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<ServiceBaseDeclaration>> serviceBaseDeclarationsProvier
            = context.SyntaxProvider
                .CreateSyntaxProvider(
                    static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: 1 },
                    static (ctx, _) => GetSemanticTargetForGeneration(ctx))
                .Where(x => x is not null)
                .Collect();

        context.RegisterSourceOutput(
            context.CompilationProvider.Combine(serviceBaseDeclarationsProvier),
            (spc, arg) => Execute(spc, arg.Left, arg.Right)
        );
    }

    private static ServiceBaseDeclaration GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var node = (ClassDeclarationSyntax)context.Node;

        if (!IsBaseService(context, node))
            return null;

        var attributeSyntax = node.AttributeLists[0].Attributes[0];

        if (!BindServiceMethodDataDeclaration.TryResolve(context, attributeSyntax, out var bindServiceMethodData))
            return null;

        if (!GrpcServiceDeclaration.TryResolve(context, bindServiceMethodData.BindType, out var serviceDescriptorDeclaration))
            return null;

        return new ServiceBaseDeclaration(
            context.SemanticModel.GetDeclaredSymbol(node),
            serviceDescriptorDeclaration);
    }

    private static bool IsBaseService(GeneratorSyntaxContext context, ClassDeclarationSyntax node)
    {
        if (!node.Identifier.Text.EndsWith("Base") ||
            !node.Modifiers.Any(SyntaxKind.AbstractKeyword) ||
            node.AttributeLists[0].Attributes.Count != 1)
        {
            return false;
        }

        var attributeSyntax = node.AttributeLists[0].Attributes[0];

        if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
            return false;

        var bindServiceMethodAttributeTypeSymbol = context.SemanticModel.Compilation
            .GetTypeByMetadataName(typeof(BindServiceMethodAttribute).FullName!);

        if (!SymbolEqualityComparer.Default.Equals(attributeSymbol.ContainingType, bindServiceMethodAttributeTypeSymbol))
            return false;

        return true;
    }

    private void Execute(SourceProductionContext context, Compilation compilation, ImmutableArray<ServiceBaseDeclaration> declarations)
    {
        foreach (var baseDeclaration in declarations)
        {
            var gen = new StringBuilder();

            var serviceName = baseDeclaration.ServiceDeclaration.ServiceName.Split('.').Last();
            var serviceType = baseDeclaration.ServiceDeclaration.TypeSymbol;
            var serviceTypeSyntax = (ClassDeclarationSyntax)serviceType.GetSyntax();

            var @namespace = GetFullNamespace(serviceType.ContainingNamespace);

            gen.AppendLine("using System.Threading;");
            gen.AppendLine("using Grpc.Proxy.Core;");
            gen.AppendLine("using grpc = global::Grpc.Core;");
            gen.AppendLine();

            if (!string.IsNullOrEmpty(@namespace))
                gen.AppendLine($"namespace {@namespace};");

            gen.Append($@"
{serviceTypeSyntax.Modifiers} class {serviceType.Name}
{{
    public abstract class {serviceName}Proxy : {baseDeclaration.TypeSymbol.Name}
    {{
        private readonly grpc::ChannelBase _channel;
        private readonly grpc::CallInvoker _invoker;

        protected {serviceName}Proxy(grpc::ChannelBase channel)
        {{
            _channel = channel;
            _invoker = channel.CreateCallInvoker();
        }}

        private static grpc::CallOptions CreateCallOptions(grpc::ServerCallContext context)
        {{
            return new grpc::CallOptions(context.RequestHeaders, context.Deadline, context.CancellationToken, context.WriteOptions);
        }}

        private static async Task CopyStreaming<T>(
            grpc::IAsyncStreamReader<T> reader,
            grpc::IAsyncStreamWriter<T> writer,
            grpc::ServerCallContext context) where T : class
        {{
            while (await reader.MoveNext(context.CancellationToken))
                await writer.WriteAsync(reader.Current, context.CancellationToken);

            if (writer is grpc::IClientStreamWriter<T> clientStreamWriter)
                await clientStreamWriter.CompleteAsync();
        }}
");

            foreach (var method in baseDeclaration.ServiceDeclaration.Methods)
            {
                //                 builder.Append($@"
                //         public override
                // ");
            }

            gen.Append(@"
    }
}");

            context.AddSource($"{serviceName}Proxy.gen.cs", SourceText.From(gen.ToString(), Encoding.UTF8));
        }
    }

    private static string GetFullNamespace(INamespaceSymbol symbol)
    {
        if (symbol is null)
            return null;

        var builder = new StringBuilder();

        do
        {
            if (builder.Length > 0)
                builder.Insert(0, '.');

            builder.Insert(0, symbol.Name);
            symbol = symbol.ContainingNamespace;
        } while (symbol is { Name.Length: > 0 });

        return builder.ToString();
    }
}
