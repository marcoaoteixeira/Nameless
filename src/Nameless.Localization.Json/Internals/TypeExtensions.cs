namespace Nameless.Localization.Json.Internals;

/// <summary>
/// <see cref="Type"/> extension methods.
/// </summary>
internal static class TypeExtensions {
    /// <summary>
    /// Retrieves the name of the type without its generic arity.
    /// </summary>
    /// <param name="self">The current type.</param>
    /// <returns>
    /// The name of the type without its generic arity.
    /// </returns>
    internal static string GetNameWithoutArity(this Type self) {
        return self.IsGenericType
            ? self.Name[..self.Name.IndexOf('`')]
            : self.Name;
    }
}