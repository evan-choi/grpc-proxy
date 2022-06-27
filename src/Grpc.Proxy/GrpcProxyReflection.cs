using System;
using System.Collections.Concurrent;
using Google.Protobuf.Reflection;

namespace Grpc.Proxy;

public static class GrpcProxyReflection
{
    private static readonly ConcurrentDictionary<Type, ServiceDescriptor> _descriptors = new();

    public static ServiceDescriptor GetDescriptor<T>() where T : IProxyService
    {
        return _descriptors[typeof(T)];
    }

    public static ServiceDescriptor GetDescriptor(Type type)
    {
        return _descriptors[type];
    }

    public static void Add<T>(ServiceDescriptor descriptor) where T : IProxyService
    {
        _descriptors[typeof(T)] = descriptor;
    }
}
