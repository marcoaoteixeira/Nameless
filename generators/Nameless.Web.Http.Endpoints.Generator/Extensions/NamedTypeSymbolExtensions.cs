using Microsoft.CodeAnalysis;

namespace Nameless.Web.Http.Endpoints.Generator;

public static class NamedTypeSymbolExtensions {
    extension(INamedTypeSymbol? self) {
        public bool TryGetTypeArgument(int index, out ISymbol? output) {
            output = self is not null && index < self.TypeArguments.Length
                ? self.TypeArguments[index]
                : null;

            return output is not null;
        }

        public ITypeSymbol? GetTypeArgument(int index) {
            return self is not null && index < self.TypeArguments.Length
                ? self.TypeArguments[index]
                : null;
        }
    }
}