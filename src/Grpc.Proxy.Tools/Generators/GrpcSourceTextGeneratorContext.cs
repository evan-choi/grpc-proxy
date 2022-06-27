using System.Linq;
using Grpc.Proxy.Tools.Extensions;
using Grpc.Proxy.Tools.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Grpc.Proxy.Tools.Generators;

internal sealed class GrpcSourceTextGeneratorContext
{
    public ServiceBaseDeclaration BaseDeclaration { get; }

    public string ServiceNamespace { get; }

    public string ServiceName { get; }

    public ITypeSymbol ServiceType { get; }

    public ClassDeclarationSyntax ServiceTypeSyntax { get; }

    public GrpcSourceTextGeneratorContext(ServiceBaseDeclaration baseDeclaration)
    {
        BaseDeclaration = baseDeclaration;
        ServiceName = baseDeclaration.ServiceDeclaration.ServiceName.Split('.').Last();
        ServiceType = baseDeclaration.ServiceDeclaration.TypeSymbol;
        ServiceTypeSyntax = (ClassDeclarationSyntax)ServiceType.GetSyntax();
        ServiceNamespace = ServiceType.ContainingNamespace.GetFullName();
    }
}
