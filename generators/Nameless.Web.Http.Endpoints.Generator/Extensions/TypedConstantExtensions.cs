using Microsoft.CodeAnalysis;

namespace Nameless.Web.Http.Endpoints.Generator;

public static class TypedConstantExtensions {
    extension(TypedConstant self) {
        public T? GetPrimitiveValue<T>() {
            return self.GetPrimitiveValue<T?>(fallback: default);
        }

        public T GetPrimitiveValue<T>(T fallback) {
            return self is { Kind: TypedConstantKind.Primitive, Value: T result }
                ? result
                : fallback;
        }

        public T[] GetArrayValue<T>() {
            return self is { Kind: TypedConstantKind.Array }
                ? [.. self.Values
                          .Where(static constant => constant.Value is not null)
                          .Select(static constant => (T)constant.Value!)]
                : [];
        }

        public string? GetFullyQualifiedName() {
            return self is { Kind: TypedConstantKind.Type, Value: ISymbol symbol }
                ? symbol.GetFullyQualifiedName()
                : null;
        }

        public ISymbol? GetSymbolValue() {
            return self is { Kind: TypedConstantKind.Type, Value: ISymbol result }
                ? result
                : null;
        }
    }
}