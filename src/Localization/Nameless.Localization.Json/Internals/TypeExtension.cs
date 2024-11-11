namespace Nameless.Localization.Json;
internal static class TypeExtension {
    internal static string GetNameWithoutArity(this Type self)
        => self.IsGenericType
            ? self.Name[..self.Name.IndexOf('`')]
            : self.Name;
}
