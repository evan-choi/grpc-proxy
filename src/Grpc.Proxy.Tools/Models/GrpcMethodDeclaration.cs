using System;
using Grpc.Core;
using Grpc.Proxy.Tools.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Grpc.Proxy.Tools.Models;

internal readonly struct GrpcMethodDeclaration
{
    public MethodType Type { get; }

    public string Name { get; }

    public ITypeSymbol Request { get; }

    public ITypeSymbol Response { get; }

    public string ReturnType { get; }

    public ParameterInfo[] Parameters { get; }

    public GrpcMethodDeclaration(MethodType type, string name, ITypeSymbol request, ITypeSymbol response)
    {
        Type = type;
        Name = name;
        Request = request;
        Response = response;
        (ReturnType, Parameters) = GetMethodLayout(type, request, response);
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

    private static (string ReturnType, ParameterInfo[] Parameters) GetMethodLayout(MethodType type, ITypeSymbol request, ITypeSymbol response)
    {
        var requestType = $"global::{request.ContainingNamespace.GetFullName()}.{request.Name}";
        var responseType = $"global::{response.ContainingNamespace.GetFullName()}.{response.Name}";

        return type switch
        {
            MethodType.Unary => (
                Task(responseType),
                new[]
                {
                    new ParameterInfo(requestType, "request")
                }
            ),
            MethodType.ClientStreaming => (
                Task(responseType),
                new[]
                {
                    new ParameterInfo(IAsyncStreamReader(requestType), "requestStream")
                }
            ),
            MethodType.ServerStreaming => (
                Task(null),
                new[]
                {
                    new ParameterInfo(requestType, "request"),
                    new ParameterInfo(IServerStreamWriter(responseType), "responseStream")
                }
            ),
            MethodType.DuplexStreaming => (
                Task(null),
                new[]
                {
                    new ParameterInfo(IAsyncStreamReader(requestType), "requestStream"),
                    new ParameterInfo(IServerStreamWriter(responseType), "responseStream")
                }
            ),
            _ => throw new InvalidOperationException()
        };

        static string Task(string genericType)
        {
            if (genericType is null)
                return "global::System.Threading.Tasks.Task";

            return $"global::System.Threading.Tasks.Task<{genericType}>";
        }

        static string IAsyncStreamReader(string genericType)
        {
            return $"grpc::IAsyncStreamReader<{genericType}>";
        }

        static string IServerStreamWriter(string genericType)
        {
            return $"grpc::IServerStreamWriter<{genericType}>";
        }
    }
}
