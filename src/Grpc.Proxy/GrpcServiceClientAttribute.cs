using System;

namespace Grpc.Proxy;

[AttributeUsage(AttributeTargets.Interface)]
public sealed class GrpcServiceClientAttribute : Attribute
{
    public Type ServiceType { get; }

    public GrpcServiceClientAttribute(Type serviceType)
    {
        ServiceType = serviceType;
    }
}
