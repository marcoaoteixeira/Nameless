using Microsoft.CodeAnalysis;

namespace Nameless.Web.Http.Endpoints.Generator;

internal static class AttributeDataExtensions {
    internal static TypedConstant GetArg(this AttributeData? self, string name) {
        if (self is null) { return default; }

        var arg = self.NamedArguments.FirstOrDefault(item => item.Key == name);

        return arg.Value;
    }

    internal static T? GetCtorArgValue<T>(this AttributeData? self, int index) {
        if (self is null || self.ConstructorArguments.Length <= index) { return default; }

        var arg = self.ConstructorArguments[index];

        return (T?)arg.Value;
    }

    internal static T? GetPropValue<T>(this AttributeData? self, string name) {
        return (T?)self.GetArg(name).Value;
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
