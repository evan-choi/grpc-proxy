using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyNUnit;
using VerifyTests;

namespace Grpc.Proxy.Tests;

public static class SourceGeneratorDriver
{
    static SourceGeneratorDriver()
    {
        VerifySourceGenerators.Enable();
    }

    public static SettingsTask Verify<T>(IEnumerable<string> sources, IEnumerable<Assembly> referenceAssemblies) where T : IIncrementalGenerator, new()
    {
        sources ??= Enumerable.Empty<string>();
        referenceAssemblies ??= Enumerable.Empty<Assembly>();

        IEnumerable<SyntaxTree> syntaxTrees = sources
            .Select(x => CSharpSyntaxTree.ParseText(x));

        IEnumerable<PortableExecutableReference> references = referenceAssemblies
            .Select(x => MetadataReference.CreateFromFile(x.Location))
            .Prepend(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

        var compilation = CSharpCompilation.Create("Grpc.Proxy.Tests", syntaxTrees, references);

        var generatorDriver = CSharpGeneratorDriver
            .Create(new T())
            .RunGenerators(compilation);

        return Verifier.Verify(generatorDriver);
    }
}
