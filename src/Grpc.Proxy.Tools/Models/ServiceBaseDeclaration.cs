using Microsoft.CodeAnalysis;

namespace Grpc.Proxy.Tools.Models;

internal sealed class ServiceBaseDeclaration
{
    public ITypeSymbol TypeSymbol { get; }

    public GrpcServiceDeclaration ServiceDeclaration { get; }

    public ServiceBaseDeclaration(ITypeSymbol typeSymbol, GrpcServiceDeclaration serviceDeclaration)
    {
        TypeSymbol = typeSymbol;
        ServiceDeclaration = serviceDeclaration;
    }
}
