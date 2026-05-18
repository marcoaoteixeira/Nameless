using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Nameless.Web.Http.Endpoints.Generator;

internal static class TypedConstantExtensions {
    internal static T? GetValue<T>(this TypedConstant self) {
        return self.GetValue<T?>(fallback: default);
    }

    internal static T GetValue<T>(this TypedConstant self, T fallback) {
        return self is { Kind: TypedConstantKind.Primitive }
            ? self.Value is not null ? (T)self.Value : fallback
            : fallback;
    }

    internal static T[] GetValues<T>(this TypedConstant self) {
        return self is { Kind: TypedConstantKind.Array }
            ? [.. self.Values
                      .Where(static constant => constant.Value is not null)
                      .Select(static constant => (T)constant.Value!)]
            : [];
    }

    internal static string? GetFullyQualifiedName(this TypedConstant self) {
        return self is { Kind: TypedConstantKind.Type, Value: ITypeSymbol symbol }
            ? symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            : null;
    }
}

internal static class AttributeDataExtensions {
    internal static bool TryGetNamedArgument(this AttributeData? self, string name, out TypedConstant output) {
        output = default;

        if (self is null) { return false; }

        var arg = self.NamedArguments.FirstOrDefault(item => item.Key == name);
        var success = !string.IsNullOrWhiteSpace(arg.Key);

        output = success ? arg.Value : default;

        return success;
    }

    internal static TypedConstant GetNamedArgument(this AttributeData? self, string name) {
        if (self is null) { return default; }

        var arg = self.NamedArguments.FirstOrDefault(item => item.Key == name);

        return arg.Value;
    }

    internal static T? GetNamedArgumentValue<T>(this AttributeData? self, string name) {
        return (T?)self.GetNamedArgument(name).Value;
    }

    internal static TypedConstant GetConstructorArguments(this AttributeData? self, int index) {
        return self is not null && index < self.ConstructorArguments.Length
            ? self.ConstructorArguments[index]
            : default;
    }

    internal static bool TryGetConstructorArguments(this AttributeData? self, int index, out TypedConstant output) {
        output = default;

        if (self is null || index >= self.ConstructorArguments.Length) { return false; }

        output = self.ConstructorArguments[index];

        return true;
    }

    internal static T? GetCtorArgValue<T>(this AttributeData? self, int index) {
        if (self is null || self.ConstructorArguments.Length <= index) { return default; }

        var arg = self.ConstructorArguments[index];

        return (T?)arg.Value;
    }

    internal static string? GetTypeArg(this AttributeData? self) {
        if (self is null) { return null; }

        // Generic usage [SomeAttribute<T>]: type is a type argument
        if (self.AttributeClass?.TypeArguments.Length > 0) {
            return self.AttributeClass.TypeArguments[0].ToDisplayString(
                SymbolDisplayFormat.FullyQualifiedFormat
            );
        }

        // Non-generic usage [SomeAttribute(typeof(T))]: type is a ctor arg
        if (self.ConstructorArguments.Length > 0 && self.ConstructorArguments[0].Value is INamedTypeSymbol symbol) {
            return symbol.ToDisplayString(
                SymbolDisplayFormat.FullyQualifiedFormat
            );
        }

        return null;
    }
}
