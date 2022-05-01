using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Grpc.Proxy.Tools.Models;

internal readonly struct BindServiceMethodDataDeclaration
{
    public ITypeSymbol BindType { get; }

    public string BindMethodName { get; }

    public BindServiceMethodDataDeclaration(ITypeSymbol bindType, string bindMethodName)
    {
        BindType = bindType;
        BindMethodName = bindMethodName;
    }

    public static bool TryResolve(
        GeneratorSyntaxContext context,
        AttributeSyntax syntax,
        out BindServiceMethodDataDeclaration declaration)
    {
        declaration = default;

        SeparatedSyntaxList<AttributeArgumentSyntax> arguments = syntax.ArgumentList!.Arguments;

        if (arguments[0].Expression is not TypeOfExpressionSyntax { Type: { } bindType } ||
            context.SemanticModel.GetTypeInfo(bindType) is not { Type: { } bindTypeSymbol })
        {
            return false;
        }

        Optional<object> bindMethodName = context.SemanticModel.GetConstantValue(arguments[1].Expression);

        if (bindMethodName.Value is not string bindMethodNameValue)
            return false;

        declaration = new BindServiceMethodDataDeclaration(bindTypeSymbol, bindMethodNameValue);
        return true;
    }
}
