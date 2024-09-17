using System.Reflection;
using System.Runtime.CompilerServices;

namespace Nameless;

/// <summary>
/// <see cref="object"/> extension methods.
/// </summary>
public static class ObjectExtension {
    /// <summary>
    /// Verifies if the given object (or type) is an anonymous object (or type).
    /// </summary>
    /// <param name="self">The source object.</param>
    /// <returns><c>true</c> if anonymous object (or type), otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static bool IsAnonymous(this object self) {
        Prevent.Argument.Null(self);

        var type = self as Type ?? self.GetType();

        return type.GetCustomAttribute<CompilerGeneratedAttribute>(inherit: true) is not null &&
               type.IsGenericType &&
               type.Name.Contains("AnonymousType") &&
               (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"));
    }

    /// <summary>
    /// Retrieves the specified attribute from the current object.
    /// </summary>
    /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
    /// <param name="self">The current object.</param>
    /// <param name="inherit">Whether it should inspect ancestors or not.</param>
    /// <returns>
    /// <c>true</c> if it has the specified attribute, otherwise; <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static bool HasAttribute<TAttribute>(this object self, bool inherit = false)
        where TAttribute : Attribute
        => Prevent.Argument.Null(self)
                  .GetType()
                  .GetCustomAttribute<TAttribute>(inherit) is not null;
}