using Microsoft.CodeAnalysis;

namespace Nameless.Web.Generators;

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

        public ISymbol? GetSymbolValue() {
            return self is { Kind: TypedConstantKind.Type, Value: ISymbol result }
                ? result
                : null;
        }

        public TEnum GetEnumValue<TEnum>(TEnum fallback = default)
            where TEnum : struct, Enum {
            if (self is not { Kind: TypedConstantKind.Enum, Value: int value }) {
                return fallback;
            }

            return Enum.TryParse(value.ToString(), out TEnum output)
                ? output
                : fallback;
        }
    }
}