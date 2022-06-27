using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Grpc.Core;
using Grpc.Proxy.Tools.Extensions;
using Grpc.Proxy.Tools.Generators;
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
            var grpcSourceTextGeneratorContext = new GrpcSourceTextGeneratorContext(baseDeclaration);

            var grpcSourceTextGenerators = new GrpcSourceTextGenerator[]
            {
                new Generators.Impl.GrpcServiceProxyGenerator(grpcSourceTextGeneratorContext)
            };

            foreach (var grpcSourceTextGenerator in grpcSourceTextGenerators)
                grpcSourceTextGenerator.WriteHeader();

            foreach (KeyValuePair<string, GrpcMethodDeclaration> entry in baseDeclaration.ServiceDeclaration.Methods.OrderBy(x => x.Value.Name))
            {
                foreach (var grpcSourceTextGenerator in grpcSourceTextGenerators)
                    grpcSourceTextGenerator.WriteMethod(entry.Key, entry.Value);
            }

            foreach (var grpcSourceTextGenerator in grpcSourceTextGenerators)
            {
                grpcSourceTextGenerator.WriteFooter();
                grpcSourceTextGenerator.Complete(context);
            }
        }
    }
}
