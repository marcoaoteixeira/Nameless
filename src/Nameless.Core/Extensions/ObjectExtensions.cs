using System.Reflection;
using System.Runtime.CompilerServices;

namespace Nameless;

/// <summary>
///     <see cref="object" /> extension methods.
/// </summary>
public static class ObjectExtensions {
    /// <param name="self">The source object.</param>
    extension(object self) {
        /// <summary>
        ///     Verifies if the given object (or type) is an anonymous object (or type).
        /// </summary>
        /// <returns><see langword="true"/> if anonymous object (or type), otherwise, <see langword="false"/>.</returns>
        public bool IsAnonymous() {
            var type = self as Type ?? self.GetType();

            return type.GetCustomAttribute<CompilerGeneratedAttribute>(inherit: true) is not null &&
                   type.IsGenericType &&
                   type.Name.Contains(value: "AnonymousType") &&
                   (type.Name.StartsWith(value: "<>") || type.Name.StartsWith(value: "VB$"));
        }

        /// <summary>
        ///     Retrieves the specified attribute from the current object.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
        /// <param name="inherit">Whether it should inspect ancestors or not.</param>
        /// <returns>
        ///     <see langword="true"/> if it has the specified attribute, otherwise; <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     if <paramref name="self" /> is <see langword="null"/>.
        /// </exception>
        public bool HasAttribute<TAttribute>(bool inherit = false)
            where TAttribute : Attribute {
            return self.GetType()
                       .GetCustomAttribute<TAttribute>(inherit) is not null;
        }
    }
}