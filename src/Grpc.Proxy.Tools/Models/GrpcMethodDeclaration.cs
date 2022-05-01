using System;
using Grpc.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Grpc.Proxy.Tools.Models;

internal readonly struct GrpcMethodDeclaration
{
    public MethodType Type { get; }

    public string Name { get; }

    public ITypeSymbol Request { get; }

    public ITypeSymbol Response { get; }

    public GrpcMethodDeclaration(MethodType type, string name, ITypeSymbol request, ITypeSymbol response)
    {
        Type = type;
        Name = name;
        Request = request;
        Response = response;
    }

    public static bool TryResolve(
        GeneratorSyntaxContext context,
        ObjectCreationExpressionSyntax syntax,
        out GrpcMethodDeclaration declaration)
    {
        declaration = default;

        if (syntax.ArgumentList is not { Arguments.Count: 5 })
            return false;

        if (context.SemanticModel.GetSymbolInfo(syntax.Type).Symbol
            is not INamedTypeSymbol { IsGenericType: true, TypeArguments.Length: 2 } typeSymbol)
        {
            return false;
        }

        var method = context.SemanticModel.Compilation.GetTypeByMetadataName(typeof(Method<,>).FullName!);

        if (!SymbolEqualityComparer.Default.Equals(typeSymbol.OriginalDefinition, method))
            return false;

        var requestType = typeSymbol.TypeArguments[0];
        var responseType = typeSymbol.TypeArguments[1];

        SeparatedSyntaxList<ArgumentSyntax> arguments = syntax.ArgumentList.Arguments;
        Optional<object> methodType = context.SemanticModel.GetConstantValue(arguments[0].Expression);
        Optional<object> methodName = context.SemanticModel.GetConstantValue(arguments[2].Expression);

        if (methodType.Value is not int methodTypeValue)
            return false;

        if (methodName.Value is not string methodNameValue)
            return false;

        declaration = new GrpcMethodDeclaration((MethodType)methodTypeValue, methodNameValue, requestType, responseType);
        return true;
    }
}
