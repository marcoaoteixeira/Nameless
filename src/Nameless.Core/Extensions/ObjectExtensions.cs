using System.Reflection;
using System.Runtime.CompilerServices;

namespace Nameless;

/// <summary>
///     <see cref="object" /> extension methods.
/// </summary>
public static class ObjectExtensions {
    /// <summary>
    ///     Verifies if the given object (or type) is an anonymous object (or type).
    /// </summary>
    /// <param name="self">The source object.</param>
    /// <returns><see langword="true"/> if anonymous object (or type), otherwise, <see langword="false"/>.</returns>
    public static bool IsAnonymous(this object self) {
        Prevent.Argument.Null(self);

        var type = self as Type ?? self.GetType();

        return type.GetCustomAttribute<CompilerGeneratedAttribute>(true) is not null &&
               type.IsGenericType &&
               type.Name.Contains("AnonymousType") &&
               (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"));
    }

    /// <summary>
    ///     Retrieves the specified attribute from the current object.
    /// </summary>
    /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
    /// <param name="self">The current object.</param>
    /// <param name="inherit">Whether it should inspect ancestors or not.</param>
    /// <returns>
    ///     <see langword="true"/> if it has the specified attribute, otherwise; <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="self" /> is <see langword="null"/>.
    /// </exception>
    public static bool HasAttribute<TAttribute>(this object self, bool inherit = false)
        where TAttribute : Attribute {
        return Prevent.Argument
                      .Null(self)
                      .GetType()
                      .GetCustomAttribute<TAttribute>(inherit) is not null;
    }
}