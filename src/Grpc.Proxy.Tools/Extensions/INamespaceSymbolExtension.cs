using System.Text;
using Microsoft.CodeAnalysis;

namespace Grpc.Proxy.Tools.Extensions;

internal static class INamespaceSymbolExtension
{
    public static string GetFullName(this INamespaceSymbol symbol)
    {
        if (symbol is null)
            return null;

        var builder = new StringBuilder();

        do
        {
            if (builder.Length > 0)
                builder.Insert(0, '.');

            builder.Insert(0, symbol.Name);
            symbol = symbol.ContainingNamespace;
        } while (symbol is { Name.Length: > 0 });

        return builder.ToString();
    }
}
