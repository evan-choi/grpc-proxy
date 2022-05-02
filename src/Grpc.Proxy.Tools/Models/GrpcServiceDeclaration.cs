using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Grpc.Proxy.Tools.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Grpc.Proxy.Tools.Models;

internal readonly struct GrpcServiceDeclaration
{
    public ITypeSymbol TypeSymbol { get; }

    public string ServiceName { get; }

    public ImmutableDictionary<string, GrpcMethodDeclaration> Methods { get; }

    public GrpcServiceDeclaration(ITypeSymbol typeSymbol, string serviceName, IDictionary<string, GrpcMethodDeclaration> methods)
    {
        TypeSymbol = typeSymbol;
        ServiceName = serviceName;
        Methods = methods.ToImmutableDictionary();
    }

    public static bool TryResolve(
        GeneratorSyntaxContext context,
        ITypeSymbol typeSymbol,
        out GrpcServiceDeclaration declaration)
    {
        IEnumerable<IFieldSymbol> staticFields = typeSymbol.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(x => x.IsStatic);

        string serviceName = null;
        var grpcMethods = new Dictionary<string, GrpcMethodDeclaration>();

        foreach (var field in staticFields)
        {
            if (field.Name == "__ServiceName")
            {
                var syntax = field.GetSyntax();

                if (syntax is VariableDeclaratorSyntax { Initializer.Value: LiteralExpressionSyntax valueExpression } &&
                    context.SemanticModel.GetConstantValue(valueExpression).Value is string value)
                {
                    serviceName = value;
                }
            }
            else if (field.Name.StartsWith("__Method_"))
            {
                var syntax = field.GetSyntax();

                if (syntax is VariableDeclaratorSyntax { Initializer.Value: ObjectCreationExpressionSyntax value } &&
                    GrpcMethodDeclaration.TryResolve(context, value, out var grpcMethodDeclaration))
                {
                    grpcMethods.Add(field.Name, grpcMethodDeclaration);
                }
            }
        }

        if (serviceName is null)
        {
            declaration = default;
            return false;
        }

        declaration = new GrpcServiceDeclaration(typeSymbol, serviceName, grpcMethods);
        return true;
    }
}
