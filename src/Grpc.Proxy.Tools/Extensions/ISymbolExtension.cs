using Microsoft.CodeAnalysis;

namespace Grpc.Proxy.Tools.Extensions;

internal static class ISymbolExtension
{
    public static SyntaxNode GetSyntax(this ISymbol symbol)
    {
        if (symbol.DeclaringSyntaxReferences is { Length: 1 } syntaxReferences)
            return syntaxReferences[0].GetSyntax();

        return null;
    }
}
