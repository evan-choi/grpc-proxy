using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Grpc.Proxy.Tools.Tests;

public class GrpcServiceProxyGeneratorTests
{
    [Test]
    public Task ProxyClassGenerate()
    {
        string[] sources =
        {
            ResourceManager.GetString("Test.cs"),
            ResourceManager.GetString("TestGrpc.cs")
        };

        Assembly[] referenceAssemblies =
        {
            Assembly.Load(new AssemblyName("Google.Protobuf")),
            Assembly.Load(new AssemblyName("Grpc.Core")),
            Assembly.Load(new AssemblyName("Grpc.Core.Api")),
            Assembly.Load(new AssemblyName("System.Runtime")),
            Assembly.Load(new AssemblyName("netstandard"))
        };

        return SourceGeneratorDriver
            .Verify<GrpcServiceProxyGenerator>(sources, referenceAssemblies.Concat(AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic)))
            .UseDirectory("Snapshots");
    }
}
