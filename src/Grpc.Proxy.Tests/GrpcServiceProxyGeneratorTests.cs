using System.Reflection;
using System.Threading.Tasks;
using Grpc.Proxy.Tools;
using NUnit.Framework;

namespace Grpc.Proxy.Tests;

public class GrpcServiceProxyGeneratorTests
{
    [Test]
    public Task ProxyClassGenerate()
    {
        string[] sources =
        {
            ResourceManager.GetString("Test.cs"),
            ResourceManager.GetString("TestGrpc.cs"),
            ResourceManager.GetString("Stub.cs")
        };

        Assembly[] referenceAssemblies =
        {
            Assembly.Load(new AssemblyName("netstandard")),
            Assembly.Load(new AssemblyName("System.Runtime")),
            Assembly.Load(new AssemblyName("Google.Protobuf")),
            Assembly.Load(new AssemblyName("Grpc.Core.Api"))
        };

        return SourceGeneratorDriver
            .Verify<Generator>(sources, referenceAssemblies)
            .UseDirectory("Snapshots");
    }
}
