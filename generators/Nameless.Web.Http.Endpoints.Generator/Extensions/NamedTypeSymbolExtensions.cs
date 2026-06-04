using Microsoft.CodeAnalysis;

namespace Nameless.Web.Http.Endpoints.Generator;

public static class NamedTypeSymbolExtensions {

    extension(INamedTypeSymbol self) {
        public ISymbol? GetTypeArgument(int index) {
            return self.TryGetTypeArgument(index, out var output)
                ? output
                : null;
        }

        public bool TryGetTypeArgument(int index, out ISymbol output) {
            output = null!;

            var type = index < self.TypeArguments.Length
                ? self.TypeArguments[index]
                : null;

            output = type!;

            return type is not null;
        }
    }
}