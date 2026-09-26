using Microsoft.CodeAnalysis;

namespace Nameless.Generators.Shared.Extensions;

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

        public bool DerivesFrom(INamedTypeSymbol? target) {
            if (target is null) { return false; }

            var current = self.BaseType;

            while (current is not null) {
                if (SymbolEqualityComparer.Default.Equals(current.OriginalDefinition, target)) {
                    return true;
                }

                current = current.BaseType;
            }

            return false;
        }
    }
}